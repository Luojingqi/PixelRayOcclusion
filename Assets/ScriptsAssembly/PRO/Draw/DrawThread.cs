using Cysharp.Threading.Tasks;
using PRO.Tool;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
namespace PRO
{
    public static class DrawThread
    {

        /// <summary>
        /// 消费者线程
        /// </summary>
        /// <param name="blocks"></param>
        public static void LoopDraw()
        {
            Debug.Log("消费者线程" + Thread.CurrentThread.ManagedThreadId);
            while (true)
            {
                SceneEntity scene = SceneManager.Inst.NowScene;
                if (scene == null) continue;
                Vector2Int minLightBufferBlockPos = BlockMaterial.CameraCenterBlockPos - BlockMaterial.LightResultBufferBlockSize / 2;
                for (int x = 0; x < BlockMaterial.LightResultBufferBlockSize.x; x++)
                    for (int y = 0; y < BlockMaterial.LightResultBufferBlockSize.y; y++)
                    {
                        Vector2Int nowBlockPos = minLightBufferBlockPos + new Vector2Int(x, y);

                        Block block = scene.GetBlock(nowBlockPos);
                        if (block != null && block.DrawPixelTaskQueue.Count > 0)
                        {
                            lock (block.DrawPixelTaskQueue)
                            {
                                while (block.DrawPixelTaskQueue.Count > 0)
                                {
                                    DrawPixelTask? task = block.DrawPixelTaskQueue.Dequeue();
                                    if (task != null) DrawTool.DrawPixelSync(block, task.Value.pos, task.Value.color);
                                }
                                BlockMaterial.En_Lock_DrawApplyQueue(block);
                            }
                        }

                        //更新背景
                        BackgroundBlock background = scene.GetBackground(nowBlockPos);
                        if (background != null && background.DrawPixelTaskQueue.Count > 0)
                        {
                            lock (background.DrawPixelTaskQueue)
                            {
                                while (background.DrawPixelTaskQueue.Count > 0)
                                {
                                    DrawPixelTask? task = background.DrawPixelTaskQueue.Dequeue();
                                    if (task != null) DrawTool.DrawPixelSync(background, task.Value.pos, task.Value.color);
                                }
                                BlockMaterial.En_Lock_DrawApplyQueue(background);
                            }
                        }
                    }

                Thread.Sleep(50);

                lock (loopDrawLock)
                {
                    for (int i = uniTaskDataList.Count - 1; i >= 0; i--)
                    {
                        var data = uniTaskDataList[i];
                        if (data.endFrame <= Frame)
                        {
                            uniTaskDataList.RemoveAt(i);
                            data.task.TrySetResult();
                        }
                    }
                    Frame++;
                }
            }
        }
        public static int Frame { get; private set; }
        private static readonly object loopDrawLock = new object();

        private static List<WaitLoopDrawUniTask> uniTaskDataList = new List<WaitLoopDrawUniTask>();

        /// <summary>
        /// 主线程等待渲染循环几帧，等待完成后后续的代码会进入渲染循环线程
        /// </summary>
        /// <param name="waitFrame"></param>
        /// <returns></returns>
        public static async UniTask MainThreadWaitLoopDraw(int waitFrame)
        {
            var data = new WaitLoopDrawUniTask()
            {
                task = AutoResetUniTaskCompletionSource.Create(),
            };
            while (true)
            {
                bool isLock = false;
                try
                {
                    isLock = Monitor.TryEnter(loopDrawLock);
                    if (isLock)
                    {
                        data.endFrame = waitFrame + Frame;
                        uniTaskDataList.Add(data);
                        break;
                    }
                    else
                    {
                        await UniTask.Yield();
                    }
                }
                finally
                {
                    if (isLock)
                        Monitor.Exit(loopDrawLock);
                }
            }
            await data.task.Task;
            return;
        }
        private struct WaitLoopDrawUniTask
        {
            public int endFrame;
            public AutoResetUniTaskCompletionSource task;
        }

    }
}
using PRO.Tool;
using System;
using System.Threading;
using UnityEngine;
using static PRO.BlockMaterial;
namespace PRO
{
    public static class DrawThread
    {
        private static int endNum;
        private static int maxTime = 30000;
        public static void Init(SceneEntity scene, Action endAction)
        {
            endNum = BlockBufferLength;
            Vector2Int minLightBufferBlockPos = CameraCenterBlockPos - LightResultBufferBlockSize / 2;
            Vector2Int minBlockBufferPos = minLightBufferBlockPos - EachBlockReceiveLightSize / 2;
            Vector2Int maxBlockBufferPos = minBlockBufferPos + LightResultBufferBlockSize - new Vector2Int(1, 1) + EachBlockReceiveLightSize - new Vector2Int(1, 1);

            for (int y = minBlockBufferPos.y; y <= maxBlockBufferPos.y; y++)
                for (int x = minBlockBufferPos.x; x <= maxBlockBufferPos.x; x++)
                    SceneManager.ThreadLoadOrCreateBlock(scene, new Vector2Int(x, y), null, (b) => { endNum--; });


            while (maxTime >= 0)
            {
                if (endNum <= 0) break;
                Thread.Sleep(100); maxTime -= 100;
            }
            if (maxTime <= 0) Debug.Log("初始化失败");
            Thread thread = new Thread(LoopDraw);
            thread.Start();
            endAction();
        }

        /// <summary>
        /// 消费者线程
        /// </summary>
        /// <param name="blocks"></param>
        private static void LoopDraw(object obj)
        {
            while (true)
            {
                //更新需要绘制图形的任务
                //lock (SceneManager.Inst.DrawGraphTaskQueue)
                //    while (SceneManager.Inst.DrawGraphTaskQueue.Count > 0)
                //    {
                //        DrawGraphTaskData task = SceneManager.Inst.DrawGraphTaskQueue.Dequeue();
                //        DrawGraphTaskInvoke(task);
                //    }
                SceneEntity scene = SceneManager.Inst.NowScene;
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
            }
        }
        #region 绘制图形任务，暂时弃用
        /// <summary>
        /// 绘制任务执行
        /// </summary>
        //private static void DrawGraphTaskInvoke(DrawGraphTaskData task)
        //{
        //    switch (task)
        //    {
        //        case DrawGraph_Line data:
        //            {
        //                var chackBox = GetLine(data.pos_G0, data.pos_G1);
        //                DrawPixelSync(chackBox, data.color);
        //                break;
        //            }
        //        case DrawGraph_Ring data:
        //            {
        //                var chackBox = GetRing(data.pos_G, data.r);
        //                DrawPixelSync(chackBox, data.color);
        //                break;
        //            }
        //        case DrawGraph_Circle data:
        //            {
        //                var chackBox = GetCircle(data.pos_G, data.r);
        //                DrawPixelSync(chackBox, data.color);
        //                break;
        //            }
        //        case DrawGraph_Polygon data:
        //            {
        //                var chackBox = GetPolygon(data.pos_G, data.r, data.n, data.rotate);
        //                DrawPixelSync(chackBox, data.color);
        //                break;
        //            }
        //        case DrawGraph_Octagon data:
        //            {
        //                var chackBox = GetOctagon(data.pos_G, data.r);
        //                DrawPixelSync(chackBox, data.color);
        //                break;
        //            }
        //    }
        //}
        #endregion
    }
}
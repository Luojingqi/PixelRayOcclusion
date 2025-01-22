using PRO.DataStructure;
using PRO.Tool;
using System;
using System.Threading;
using UnityEngine;
using static PRO.Tool.DrawTool;
namespace PRO
{
    public static class DrawThread
    {
        private static int a = 10;
        private static Vector2Int x = new Vector2Int(-a, a);
        private static Vector2Int y = new Vector2Int(-a, a);
        private static int endNum = (a * 2 + 1) * (a * 2 + 1) * 2;
        private static int time = 30000;
        public static void Init(Action endAction)
        {
            InitScene(SceneManager.Inst.NowScene);
            Thread thread = new Thread(LoopDraw);
            thread.Start();
            while (time >= 0) { if (endNum <= 0) break; Thread.Sleep(10); time -= 10; }
            if (time <= 0) Debug.Log("初始化失败");
            endAction();
        }
        public static void InitScene(SceneEntity scene)
        {
            for (int i = x.x; i <= x.y; i++)
            {
                for (int j = y.x; j <= y.y; j++)
                {
                    //创建区块并填充
                    scene.CreateBlock(new Vector2Int(i, j));
                    ThreadPool.QueueUserWorkItem(StartFillBlock, scene.GetBlock(new(i, j)));
                    //创建背景并填充
                    scene.CreateBackground(new Vector2Int(i, j));
                    ThreadPool.QueueUserWorkItem(StartFillBackground, scene.GetBackground(new(i, j)));
                    scene.BlockBaseInRAM.Add(new Vector2Int(i, j));
                }
            }
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
        /// <summary>
        /// 开始填充一个区块
        /// </summary>
        private static void StartFillBlock(object obj)
        {
            try
            {
                var block = (Block)obj;
                RandomFill(block, new System.Random());
                //Iteration(block);
                //var colliderDataList = GreedyCollider.CreateColliderDataList(block, new(0, 0), new(Block.Size.x - 1, Block.Size.y - 1));
                //lock (SceneManager.Inst.mainThreadEventLock)
                //    SceneManager.Inst.mainThreadEvent += () => { GreedyCollider.CreateColliderAction(block, colliderDataList); };
                //SceneManager.Inst.En_Lock_DrawApplyQueue(block);
                Interlocked.Add(ref endNum, -1);
            }
            catch (Exception e)
            {
                Log.Print($"线程报错：{e}", Color.red);
            }
        }
        private static void RandomFill(Block block, System.Random random)
        {
            for (int x = 0; x < Block.Size.x; x++)
                for (int y = 0; y < Block.Size.y; y++)
                {
                    int r = random.Next(0, 4);
                    if (r > 10)//>= 2)
                    {
                        Pixel pixel = Pixel.New("空气", 0, new(x, y));
                        block.SetPixel(pixel, false, false);
                        block.DrawPixelSync(new Vector2Byte(x, y), pixel.colorInfo.color);
                    }
                    else
                    {
                        Pixel pixel = Pixel.New("空气", 0, new(x, y));
                        block.SetPixel(pixel, false, false);
                        block.DrawPixelSync(new Vector2Byte(x, y), pixel.colorInfo.color);
                    }
                }
        }
        private static void Iteration(Block block)
        {
            for (int i = 0; i < 3; i++)
                for (int x = 0; x < Block.Size.x; x++)
                {
                    for (int y = 0; y < Block.Size.y; y++)
                    {
                        //int k = 0;
                        //if (block.GetPixelRelocation(x + 1, y)?.id == 1) k++;
                        //if (block.GetPixelRelocation(x - 1, y)?.id == 1) k++;
                        //if (block.GetPixelRelocation(x, y + 1)?.id == 1) k++;
                        //if (block.GetPixelRelocation(x, y - 1)?.id == 1) k++;
                        //if (block.GetPixelRelocation(x + 1, y + 1)?.id == 1) k++;
                        //if (block.GetPixelRelocation(x - 1, y - 1)?.id == 1) k++;
                        //if (block.GetPixelRelocation(x - 1, y + 1)?.id == 1) k++;
                        //if (block.GetPixelRelocation(x + 1, y - 1)?.id == 1) k++;
                        //if (k > 4)
                        //{
                        //    block.GetPixelRelocation(x, y).id = 1;
                        //    block.DrawPixelSync(new Vector2Byte(x, y), BlockMaterial.GetPixelColorInfo(1).color);
                        //}
                        //else if (k < 4)
                        //{
                        //    block.GetPixelRelocation(x, y).id = 0;
                        //    block.DrawPixelSync(new Vector2Byte(x, y), BlockMaterial.GetPixelColorInfo(0).color);
                        //}
                    }
                }
        }

        /// <summary>
        /// 开始填充背景
        /// </summary>
        private static void StartFillBackground(object obj)
        {
            try
            {
                var background = (BackgroundBlock)obj;
                for (int x = 0; x < Block.Size.x; x++)
                    for (int y = 0; y < Block.Size.y; y++)
                    {
                        Pixel pixel = Pixel.New("背景", 2, new(x, y));
                        background.SetPixel(pixel);
                        background.DrawPixelSync(new Vector2Byte(x, y), pixel.colorInfo.color);
                        //if (x < Block.Size.x / 2 - 10)
                        //{
                        //    Pixel pixel = Pixel.New("背景", 0, new(x, y));
                        //    background.SetPixel(pixel);
                        //    background.DrawPixelSync(new Vector2Byte(x, y), BlockMaterial.GetPixelColorInfo(pixel.colorInfo).color);
                        //}
                        //else
                        //{
                        //    Pixel pixel = Pixel.New("背景", 1, new(x, y));
                        //    background.SetPixel(pixel);
                        //    background.DrawPixelSync(new Vector2Byte(x, y), BlockMaterial.GetPixelColorInfo(pixel.colorInfo).color);
                        //}
                    }
                // SceneManager.Inst.En_Lock_DrawApplyQueue(background);
                Interlocked.Add(ref endNum, -1);
            }
            catch (Exception e)
            {
                Log.Print($"线程报错：{e}", Color.red);
            }
        }
    }
}
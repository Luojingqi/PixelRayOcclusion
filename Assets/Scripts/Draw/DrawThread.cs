using PRO.Data;
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
        private static int a = 3;
        private static Vector2Int x = new Vector2Int(-a, a);
        private static Vector2Int y = new Vector2Int(-a, a);
        private static int endNum = (a * 2 + 1) * (a * 2 + 1) * 2;
        public static void Init(Action endAction)
        {
            for (int i = x.x; i <= x.y; i++)
            {
                for (int j = y.x; j <= y.y; j++)
                {
                    //创建区块并填充
                    BlockManager.Inst.CreateBlock(new Vector2Int(i, j));
                    BlockManager.Inst.AddUpdateBlock(BlockManager.Inst.BlockCrossList[i][j]);
                    ThreadPool.QueueUserWorkItem(StartFillBlock, BlockManager.Inst.BlockCrossList[i][j]);
                    //创建背景并填充
                    BlockManager.Inst.CreateBackground(new Vector2Int(i, j));
                    ThreadPool.QueueUserWorkItem(StartFillBackground, BlockManager.Inst.BackgroundCrossList[i][j]);
                }
            }
            Thread thread = new Thread(LoopDraw);
            thread.Start();
            while (true) if (endNum <= 0) break;

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
                lock (BlockManager.Inst.DrawGraphTaskQueue)
                    while (BlockManager.Inst.DrawGraphTaskQueue.Count > 0)
                    {
                        DrawGraphTaskData task = BlockManager.Inst.DrawGraphTaskQueue.Dequeue();
                        DrawGraphTaskInvoke(task);
                    }

                Vector2Int minLightBufferBlockPos = BlockMaterial.CameraCenterBlockPos - BlockMaterial.LightBufferBlockSize / 2;
                for (int x = 0; x < BlockMaterial.LightBufferBlockSize.x; x++)
                    for (int y = 0; y < BlockMaterial.LightBufferBlockSize.y; y++)
                    {
                        Vector2Int nowBlockPos = minLightBufferBlockPos + new Vector2Int(x, y);

                        Block block = BlockManager.Inst.BlockCrossList[nowBlockPos];
                        if (block.DrawPixelTaskQueue.Count > 0)
                        {
                            lock (block.DrawPixelTaskQueue)
                            {
                                while (block.DrawPixelTaskQueue.Count > 0)
                                {
                                    DrawPixelTask task = block.DrawPixelTaskQueue.Dequeue();
                                    if (task.skip == false)
                                        DrawTool.DrawPixelSync(block, task.pos, task.color);
                                }
                                BlockManager.Inst.En_Lock_DrawApplyQueue(block);
                            }
                        }

                        //更新背景
                        BackgroundBlock background = BlockManager.Inst.BackgroundCrossList[nowBlockPos];
                        if (background.DrawPixelTaskQueue.Count > 0)
                        {
                            lock (background.DrawPixelTaskQueue)
                            {
                                while (background.DrawPixelTaskQueue.Count > 0)
                                {
                                    DrawPixelTask task = background.DrawPixelTaskQueue.Dequeue();
                                    if (task.skip == false)
                                        DrawTool.DrawPixelSync(background, task.pos, task.color);
                                }
                                BlockManager.Inst.En_Lock_DrawApplyQueue(background);
                            }
                        }
                    }

                #region 弃用
                ////遍历所有需要更新的区块，取出其中需要更新的点，更新，最后提交给GPU
                //for (int i = 0; i < BlockManager.Inst.BlockUpdateList.Count; i++)
                //{
                //    Block block = BlockManager.Inst.BlockUpdateList[i];
                //    if (block.DrawPixelTaskQueue.Count > 0)
                //    {
                //        lock (block.DrawPixelTaskQueue)
                //        {
                //            while (block.DrawPixelTaskQueue.Count > 0)
                //            {
                //                DrawPixelTask task = block.DrawPixelTaskQueue.Dequeue();
                //                DrawTool.DrawPixelSync(block, task.pos, task.color);
                //            }
                //            BlockManager.Inst.En_Lock_DrawApplyQueue(block);
                //        }
                //    }

                //    //更新背景
                //    BackgroundBlock background = BlockManager.Inst.BackgroundCrossList[block.BlockPos];
                //    if (background.DrawPixelTaskQueue.Count > 0)
                //    {
                //        lock (background.DrawPixelTaskQueue)
                //        {
                //            while (background.DrawPixelTaskQueue.Count > 0)
                //            {
                //                DrawPixelTask task = background.DrawPixelTaskQueue.Dequeue();
                //                DrawTool.DrawPixelSync(background, task.pos, task.color);
                //            }
                //            BlockManager.Inst.En_Lock_DrawApplyQueue(background);
                //        }
                //    }

                //}
                #endregion
                Thread.Sleep(50);
            }
        }
        /// <summary>
        /// 绘制任务执行
        /// </summary>
        private static void DrawGraphTaskInvoke(DrawGraphTaskData task)
        {
            switch (task)
            {
                case DrawGraph_Line data:
                    {
                        var list = GetLine(data.pos_G0, data.pos_G1);
                        DrawPixelSync(list, data.color);
                        break;
                    }
                case DrawGraph_Ring data:
                    {
                        var list = GetRing(data.pos_G, data.r);
                        DrawPixelSync(list, data.color);
                        break;
                    }
                case DrawGraph_Circle data:
                    {
                        var list = GetCircle(data.pos_G, data.r);
                        DrawPixelSync(list, data.color);
                        break;
                    }
                case DrawGraph_Polygon data:
                    {
                        var list = GetPolygon(data.pos_G, data.r, data.n, data.rotate);
                        DrawPixelSync(list, data.color);
                        break;
                    }
                case DrawGraph_Octagon data:
                    {
                        var list = GetOctagon(data.pos_G, data.r);
                        DrawPixelSync(list, data.color);
                        break;
                    }
            }
        }

        /// <summary>
        /// 开始填充一个区块
        /// </summary>
        private static void StartFillBlock(object obj)
        {

            {
                var block = (Block)obj;
                RandomFill(block, new System.Random());
                //Iteration(block);
                //var colliderDataList = GreedyCollider.CreateColliderDataList(block, new(0, 0), new(Block.Size.x - 1, Block.Size.y - 1));
                //lock (BlockManager.Inst.mainThreadEventLock)
                //    BlockManager.Inst.mainThreadEvent += () => { GreedyCollider.CreateColliderAction(block, colliderDataList); };
                //BlockManager.Inst.En_Lock_DrawApplyQueue(block);
                Interlocked.Add(ref endNum, -1);
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
                        PixelColorInfo info = BlockMaterial.GetPixelColorInfo(1);
                        block.SetPixel(new Pixel() { name = info.name, pos = new(x, y) });
                        block.DrawPixelSync(new Vector2Byte(x, y), info.color);
                    }
                    else
                    {
                        PixelColorInfo info = BlockMaterial.GetPixelColorInfo(0);
                        block.SetPixel(new Pixel() { name = info.name, pos = new(x, y) });
                        block.DrawPixelSync(new Vector2Byte(x, y), info.color);
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
            var background = (BackgroundBlock)obj;
            for (int x = 0; x < Block.Size.x; x++)
                for (int y = 0; y < Block.Size.y; y++)
                {
                    if (x < Block.Size.x / 2 - 10)
                    {
                        PixelColorInfo info = BlockMaterial.GetPixelColorInfo("背景2");
                        background.SetPixel(new Pixel() { name = info.name, pos = new(x, y) });
                        background.DrawPixelSync(new Vector2Byte(x, y), info.color);
                    }
                    else
                    {
                        PixelColorInfo info = BlockMaterial.GetPixelColorInfo("背景2");
                        background.SetPixel(new Pixel() { name = info.name, pos = new(x, y) });
                        background.DrawPixelSync(new Vector2Byte(x, y), info.color);
                    }
                }
            // BlockManager.Inst.En_Lock_DrawApplyQueue(background);
            Interlocked.Add(ref endNum, -1);
        }
    }
}
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
                    //�������鲢���
                    BlockManager.Inst.CreateBlock(new Vector2Int(i, j));
                    BlockManager.Inst.AddUpdateBlock(BlockManager.Inst.BlockCrossList[i][j]);
                    ThreadPool.QueueUserWorkItem(StartFillBlock, BlockManager.Inst.BlockCrossList[i][j]);
                    //�������������
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
        /// �������߳�
        /// </summary>
        /// <param name="blocks"></param>
        private static void LoopDraw(object obj)
        {
            while (true)
            {
                //������Ҫ����ͼ�ε�����
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
                                    DrawPixelTask? task = block.DrawPixelTaskQueue.Dequeue();
                                    if (task != null) DrawTool.DrawPixelSync(block, task.Value.pos, task.Value.color);
                                }
                                BlockManager.Inst.En_Lock_DrawApplyQueue(block);
                            }
                        }

                        //���±���
                        BackgroundBlock background = BlockManager.Inst.BackgroundCrossList[nowBlockPos];
                        if (background.DrawPixelTaskQueue.Count > 0)
                        {
                            lock (background.DrawPixelTaskQueue)
                            {
                                while (background.DrawPixelTaskQueue.Count > 0)
                                {
                                    DrawPixelTask? task = background.DrawPixelTaskQueue.Dequeue();
                                    if (task != null) DrawTool.DrawPixelSync(background, task.Value.pos, task.Value.color);
                                }
                                BlockManager.Inst.En_Lock_DrawApplyQueue(background);
                            }
                        }
                    }
                Thread.Sleep(50);
            }
        }
        /// <summary>
        /// ��������ִ��
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
        /// ��ʼ���һ������
        /// </summary>
        private static void StartFillBlock(object obj)
        {
            try
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
            catch(Exception e)
            {
                Log.Print($"�̱߳���{e}", Color.red);
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
                        Pixel pixel = Pixel.New("����", 0, new(x, y));
                        block.SetPixel(pixel);
                        block.DrawPixelSync(new Vector2Byte(x, y), BlockMaterial.GetPixelColorInfo(pixel.colorName).color);
                    }
                    else
                    {
                        Pixel pixel = Pixel.New("����", 0, new(x, y));
                        block.SetPixel(pixel);
                        block.DrawPixelSync(new Vector2Byte(x, y), BlockMaterial.GetPixelColorInfo(pixel.colorName).color);
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
        /// ��ʼ��䱳��
        /// </summary>
        private static void StartFillBackground(object obj)
        {
            try
            {
                var background = (BackgroundBlock)obj;
                for (int x = 0; x < Block.Size.x; x++)
                    for (int y = 0; y < Block.Size.y; y++)
                    {
                        if (x < Block.Size.x / 2 - 10)
                        {
                            Pixel pixel = Pixel.New("����", 0, new(x, y));
                            background.SetPixel(pixel);
                            background.DrawPixelSync(new Vector2Byte(x, y), BlockMaterial.GetPixelColorInfo(pixel.colorName).color);
                        }
                        else
                        {
                            Pixel pixel = Pixel.New("����", 1, new(x, y));
                            background.SetPixel(pixel);
                            background.DrawPixelSync(new Vector2Byte(x, y), BlockMaterial.GetPixelColorInfo(pixel.colorName).color);
                        }
                    }
                // BlockManager.Inst.En_Lock_DrawApplyQueue(background);
                Interlocked.Add(ref endNum, -1);
            }
            catch (Exception e)
            {
                Log.Print($"�̱߳���{e}", Color.red);
            }
        }
    }
}
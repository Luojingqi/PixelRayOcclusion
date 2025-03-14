using PRO.DataStructure;
using PRO.Disk.Scene;
using PRO.Tool;
using System;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
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
            while (time >= 0) { if (endNum <= 0) break; Thread.Sleep(10); time -= 10; }
            if (time <= 0) Debug.Log("初始化失败");
            Thread thread = new Thread(LoopDraw);
            thread.Start();
            endAction();
        }
        public static void InitScene(SceneEntity scene)
        {
            for (int i = x.x; i <= x.y; i++)
            {
                for (int j = y.x; j <= y.y; j++)
                {
                    #region .
                    //if (i == 0 && j == 0)
                    //{
                    //    var b0 = scene.CreateBlock(new Vector2Int(i, j));
                    //    var b1 = scene.CreateBackground(new Vector2Int(i, j));
                    //    ThreadPool.QueueUserWorkItem((obj) =>
                    //    {
                    //        try
                    //        {
                    //            for (int x = 0; x < Block.Size.x; x++)
                    //                for (int y = 0; y < Block.Size.y; y++)
                    //                {
                    //                    Pixel pixel = Pixel.New("空气", 0, new(x, y));
                    //                    b0.SetPixel(pixel, false, false, false);
                    //                    b0.DrawPixelSync(new Vector2Byte(x, y), pixel.colorInfo.color);
                    //                    Pixel pixel1 = Pixel.New("空气", 0, new(x, y));
                    //                    b1.SetPixel(pixel1, false, false, false);
                    //                    b1.DrawPixelSync(new Vector2Byte(x, y), pixel1.colorInfo.color);
                    //                }
                    //            Interlocked.Add(ref endNum, -2);
                    //            scene.BlockBaseInRAM.Add(new Vector2Int(i, j));
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            SceneManager.Inst.mainThreadEvent += () => Debug.Log(ex);
                    //        }
                    //    });
                    //    continue;
                    //}
                    #endregion
                    var block = scene.CreateBlock(new Vector2Int(i, j));
                    var background = scene.CreateBackground(new Vector2Int(i, j));
                    if (JsonTool.LoadText($@"{scene.sceneCatalog.directoryInfo}\Block\{new Vector2Int(i, j)}\block.txt", out string blockText)
                        && JsonTool.LoadText($@"{scene.sceneCatalog.directoryInfo}\Block\{new Vector2Int(i, j)}\background.txt", out string backgroundText))
                    {
                        ThreadPool.QueueUserWorkItem((obj) =>
                        {
                            try
                            {
                                BlockToDiskEx.ToRAM(blockText, block, scene);
                                var colliderDataList = GreedyCollider.CreateColliderDataList(block, new(0, 0), new(Block.Size.x - 1, Block.Size.y - 1));
                                SceneManager.Inst.mainThreadEvent += () => GreedyCollider.CreateColliderAction(block, colliderDataList);
                            }
                            catch (Exception e) { SceneManager.Inst.mainThreadEvent += () => Log.Print($"线程报错：{e}", Color.red); }
                            Interlocked.Add(ref endNum, -1);
                        });
                        ThreadPool.QueueUserWorkItem((obj) =>
                        {
                            try
                            {
                                BlockToDiskEx.ToRAM(backgroundText, background, scene);
                            }
                            catch (Exception e) { SceneManager.Inst.mainThreadEvent += () => Log.Print($"线程报错：{e}", Color.red); }
                            Interlocked.Add(ref endNum, -1);
                        });
                    }
                    else
                    {
                        ThreadPool.QueueUserWorkItem((obj) =>
                        {
                            BlockBase blockBase = obj as BlockBase;
                            try
                            {
                                for (int x = 0; x < Block.Size.x; x++)
                                    for (int y = 0; y < Block.Size.y; y++)
                                    {
                                        Pixel pixel = Pixel.空气.Clone(new(x, y));
                                        blockBase.SetPixel(pixel, false, false, false);
                                        blockBase.DrawPixelSync(new Vector2Byte(x, y), pixel.colorInfo.color);
                                    }
                                blockBase.DrawPixelAsync();
                            }
                            catch (Exception e) { SceneManager.Inst.mainThreadEvent += () => Log.Print($"线程报错：{e}", Color.red); }
                            Interlocked.Add(ref endNum, -1);
                        }, block);
                        ThreadPool.QueueUserWorkItem((obj) =>
                        {
                            BlockBase blockBase = obj as BlockBase;
                            try
                            {
                                for (int x = 0; x < Block.Size.x; x++)
                                    for (int y = 0; y < Block.Size.y; y++)
                                    {
                                        Pixel pixel = Pixel.New("背景", "背景色2", new(x, y));
                                        blockBase.SetPixel(pixel, false, false, false);
                                        blockBase.DrawPixelSync(new Vector2Byte(x, y), pixel.colorInfo.color);
                                    }
                                blockBase.DrawPixelAsync();
                            }
                            catch (Exception e) { SceneManager.Inst.mainThreadEvent += () => Log.Print($"线程报错：{e}", Color.red); }
                            Interlocked.Add(ref endNum, -1);
                        }, background);
                    }
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
    }
}
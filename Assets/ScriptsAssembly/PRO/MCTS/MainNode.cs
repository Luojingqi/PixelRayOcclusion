using Google.FlatBuffers;
using PRO.Disk.Scene;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace PRO.AI
{
    public partial class MCTS
    {
        internal class MainNode : NodeBase
        {
            public override void 执行()
            {
                throw new System.NotImplementedException();
            }

            public MainNode(MCTS mcts)
            {
                this.mcts = mcts;
#if PRO_MCTS_SERVER
                MCTS.ServerReceive += ServerReceive;
#elif PRO_MCTS_CLIENT
                MCTS.ClientReceive += ClientReceive;
#endif
            }
#if PRO_MCTS_SERVER
            private int max = 100;
            public void 开始模拟()
            {
                TimeManager.enableUpdate = false;
                var round = mcts.round;
                var scene = round.Scene;
                var root = GameSaveCatalog.CreatFile("MCTS_Temp_GameSave");
                var sceneCatalog = SceneCatalog.CreateFile($"MCTS_Temp_Scene_Round_{round.GUID}", root);
                scene.SetTempCatalog(sceneCatalog);
                var countdown = scene.SaveAll();
                扩展();
                ThreadPool.QueueUserWorkItem((obj) =>
                {
                    try
                    {
                        if (countdown.Wait(1000 * 15) == false) return;
                        for (int i = 0; i < max; i++)
                        {
                            Debug.Log($"第{i}次模拟-等待");
                            Socket removeSocket = null;
                            int time = 10000;

                            do
                            {
                                lock (lockObject)
                                {
                                    if (IdleClientSocketQueue.Count > 0)
                                    {
                                        removeSocket = IdleClientSocketQueue.Dequeue();
                                        break;
                                    }
                                }
                                time -= 25;
                                Thread.Sleep(25);
                            } while (time > 0);
                            if (time <= 0)
                            {
                                Debug.Log("没有空闲的模拟器");
                                return;
                            }
                            Debug.Log("等待模拟器成功");
                            访问次数 += 1;
                            var nextNode = chiles.Dequeue();
                            mcts.NodeList.Add(nextNode);
                            nextNode.访问(removeSocket);
                            chiles.Enqueue(nextNode, -nextNode.Get_UCB());
                            mcts.NodeList.Clear();
                        }
                    }
                    catch(Exception e)
                    {
                        Debug.Log(e);
                    }
                    finally
                    {
                        scene.ResetCatalog();
                    }
                });
            }

            private void ServerReceive(Socket removeSocket, FlatBufferBuilder builder, int length)
            {
                var startRstData = Flat.Start_Rst.GetRootAsStart_Rst(builder.DataBuffer);
                lock (lockObject)
                {
                    var workData = WorkClientSocketDic[removeSocket];
                    WorkClientSocketDic.Remove(removeSocket);
                    IdleClientSocketQueue.Enqueue(removeSocket);
                    var node = workData.node;
                    node.Add线程占用(-1);
                    int effectLength = startRstData.EffectsLength;
                    for (int i = (int)EffectAgent.end - 1; i >= 0; i--)
                    {
                        var effectDiskData = startRstData.Effects(i).Value;
                        node.AddEffect((EffectAgent)((int)EffectAgent.end - i - 1), Effect.ToRAM(effectDiskData));
                    }
                    for (int i = startRstData.NodesLength - 1; i >= 0; i--)
                    {
                        var nodeType = startRstData.NodesType(i);
                        NodeBase nextNode = null;
                        switch (nodeType)
                        {
                            case Flat.NodeBase.Node:
                                nextNode = Node.ToRAM(startRstData.Nodes<Flat.Node>(i).Value, mcts.round.Scene);
                                break;
                            case Flat.NodeBase.TimeNode:
                                nextNode = TimeNode.ToRAM(startRstData.Nodes<Flat.TimeNode>(i).Value, mcts.round.Scene);
                                break;
                        }
                        nextNode.parent = node;
                        node.chiles.Enqueue(nextNode, float.MinValue);
                        node.已扩展 = true;
                    }
                }
            }
#elif PRO_MCTS_CLIENT
            private FlatBufferBuilder builder = new FlatBufferBuilder(1024 * 10);
            private void ClientReceive(Socket removeSocket, FlatBufferBuilder builder, int length)
            {
                var startCmdData = Flat.Start_Cmd.GetRootAsStart_Cmd(builder.DataBuffer);
                var scene = SceneEntity.TakeOut(SceneCatalog.LoadSceneInfo(new DirectoryInfo(startCmdData.Path)));
                var countdown = scene.LoadAll();
                countdown.Wait();
                // SceneManager.Inst.SwitchScene(scene);
                SceneManager.Inst.NowScene = scene;
                mcts.round = GamePlayMain.Inst.Round;
                mcts.startingData.Init(mcts.round);
                NodeBase node = mcts.main;
                for (int i = startCmdData.NodesLength - 1; i >= 0; i--)
                {
                    var nodeType = startCmdData.NodesType(i);
                    NodeBase nextNode = null;
                    switch (nodeType)
                    {
                        case Flat.NodeBase.Node:
                            {
                                var diskData = startCmdData.Nodes<Flat.Node>(i).Value;
                                nextNode = Node.ToRAM(diskData, scene);
                                break;
                            }
                        case Flat.NodeBase.TimeNode:
                            {
                                var diskData = startCmdData.Nodes<Flat.TimeNode>(i).Value;
                                nextNode = TimeNode.ToRAM(diskData, scene);
                                break;
                            }
                    }
                    node.chiles.Enqueue(nextNode, 0);
                    node = nextNode;
                }
                builder.Clear();
                Action<FlatBufferBuilder> action = null;
                mcts.main.访问(builder, action);
                Span<int> nodesOffsetArray = stackalloc int[node.chiles.Count];
                Span<Flat.NodeBase> nodeTypesOffsetArray = stackalloc Flat.NodeBase[node.chiles.Count];
                for (int i = 0; i < node.chiles.Count; i++)
                {
                    var data = node.chiles[i].ToDisk(builder);
                    nodeTypesOffsetArray[i] = data.Item1;
                    nodesOffsetArray[i] = data.Item2.Value;
                }
                var nodeTypesOffsetArrayOffset = builder.CreateVector_Data(nodeTypesOffsetArray);
                var nodesOffsetArrayOffset = builder.CreateVector_Offset(nodesOffsetArray);
                Flat.Start_Rst.StartStart_Rst(builder);
                Flat.Start_Rst.AddNodes(builder, nodesOffsetArrayOffset);
                Flat.Start_Rst.AddNodesType(builder, nodeTypesOffsetArrayOffset);
                action?.Invoke(builder);
                builder.Finish(Flat.Start_Rst.EndStart_Rst(builder).Value);
                SceneEntity.PutIn(scene).Wait();
                SceneManager.Inst.NowScene = null;
                mcts.main.PutIn();
                removeSocket.Send(builder.ToSpan());
            }
#endif
            public override (Flat.NodeBase, Offset<int>) ToDisk(FlatBufferBuilder builder)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}

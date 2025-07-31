using Google.FlatBuffers;
using PRO.Disk.Scene;
using PRO.Tool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace PRO.AI
{
    public partial class MCTS
    {
#if PRO_MCTS_SERVER
        private struct ClientWorkData
        {
            public NodeBase node;
        }
        private class MainNode_Server : MainNode
        {
            public override void PutIn()
            {
                base.PutIn();
                workDataDic.Clear();
                nodeList.Clear();
            }
            private List<NodeBase> nodeList = new List<NodeBase>(128);
            private Dictionary<int, ClientWorkData> workDataDic = new Dictionary<int, ClientWorkData>();
            private Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            public MainNode_Server(MCTS mcts) : base(mcts)
            {
                server.Bind(new IPEndPoint(IPAddress.Loopback, UnityEngine.Random.Range(10000, 30000)));
                new Thread(() =>
                {
                    byte[] bytes = new byte[1024 * 10];
                    FlatBufferBuilder builder = new FlatBufferBuilder(new ByteBuffer(bytes));
                    EndPoint remotePoint = new IPEndPoint(IPAddress.Any, 0);
                    while (true)
                    {
                        int length = server.ReceiveFrom(bytes, ref remotePoint);
                        if (length > 0)
                        {
                            Debug.Log("服务器收到消息" + length);
                            lock (lockObject)
                                ServerReceive(remotePoint as IPEndPoint, builder, length);
                        }
                        else break;
                    }
                    Debug.Log("连接断开");
                }).Start();
            }

            private int 单次模拟最大等待时间 = 5000;
            private int max = 300;
            public void 开始模拟()
            {
                var round = mcts.round;
                var scene = round.Scene;
                var root = GameSaveCatalog.LoadGameSaveInfo("MCTS_Temp_GameSave");
                var sceneName = $"MCTS_Temp_Scene_Round_{round.GUID}";
                var sceneCatalog = SceneCatalog.CreateFile(sceneName, root);
                SceneCatalog.Clone(scene.sceneCatalog, sceneCatalog);
                sceneCatalog.name = sceneName;
                var countdown = scene.SaveAll(sceneCatalog);
                扩展();
                ThreadPool.QueueUserWorkItem((obj) =>
                {
                    try
                    {
                        if (countdown.Wait(1000 * 15) == false) return;
                        FlatBufferBuilder builder = FlatBufferBuilder.TakeOut(1024 * 16);
                        IPEndPoint remoteIPEndPoint = new IPEndPoint(IPAddress.Loopback, 0);

                        for (int i = 0; i < max; i++)
                        {
                            #region 等待可用模拟器
                            Debug.Log($"第{i}次模拟-等待模拟器中");
                            int time = 单次模拟最大等待时间;
                            while (time > 0)
                            {
                                lock (lockObject)
                                    if (IdleClientEndPointQueue.TryDequeue(out var removePoint)) { remoteIPEndPoint.Port = removePoint; break; }
                                time -= 25;
                                Thread.Sleep(25);
                            }
                            if (time <= 0)
                            {
                                Debug.Log($"第{i}次模拟-没有空闲的模拟器");
                                return;
                            }
                            Debug.Log($"第{i}次模拟-等待模拟器成功");
                            #endregion
                            #region 单次访问
                            lock (lockObject)
                            {
                                访问次数++;
                                chiles[0].访问(nodeList);
                                线程占用++;
                                for (int n = 0; n < nodeList.Count; n++)
                                    nodeList[n].线程占用++;
                                for (int n = 0; n < nodeList.Count; n++)
                                {
                                    var node = nodeList[n];
                                    node.parent.chiles.Enqueue(node.parent.chiles.Dequeue(), -node.Get_UCB());
                                }
                            }
                            var workData = new ClientWorkData();
                            workData.node = nodeList[nodeList.Count - 1];
                            Span<Flat.NodeBase> nodeTypeListOffsetArray = stackalloc Flat.NodeBase[nodeList.Count];
                            Span<int> nodeListOffsetArray = stackalloc int[nodeList.Count];
                            for (int n = 0; n < nodeList.Count; n++)
                            {
                                var data = nodeList[n].ToDisk(builder);
                                nodeTypeListOffsetArray[n] = data.Item1;
                                nodeListOffsetArray[n] = data.Item2.Value;
                            }
                            lock (lockObject)
                            {
                                workDataDic.Add(remoteIPEndPoint.Port, workData);
                            }
                            var pathOffset = builder.CreateString(sceneCatalog.directoryInfo.FullName);
                            var nodeTypeListOffset = builder.CreateVector_Data(nodeTypeListOffsetArray);
                            var nodeListOffset = builder.CreateVector_Offset(nodeListOffsetArray);
                            Flat.Start_Cmd.StartStart_Cmd(builder);
                            Flat.Start_Cmd.AddPath(builder, pathOffset);
                            Flat.Start_Cmd.AddNodes(builder, nodeListOffset);
                            Flat.Start_Cmd.AddNodesType(builder, nodeTypeListOffset);
                            builder.Finish(Flat.Start_Cmd.EndStart_Cmd(builder).Value);
                            server.SendTo(builder.DataBuffer.GetBytes(), builder.DataBuffer.Position, builder.Offset, SocketFlags.None, remoteIPEndPoint);
                            builder.Clear();
                            nodeList.Clear();
                            #endregion
                        }
                        {
                            int time = 单次模拟最大等待时间;
                            while (time > 0)
                            {
                                if (workDataDic.Count == 0) break;
                                time -= 25;
                                Thread.Sleep(25);
                            }

                            NodeBase node = this;
                            while (true)
                            {
                                if (node.chiles.Count > 0)
                                    node = node.chiles[0];
                                else break;
                                if (node != null)
                                {
                                    switch (node)
                                    {
                                        case Node n: Debug.Log(n.operate.Agent.Name + "|" + n.operate.config.Name); break;
                                        case TimeNode t: Debug.Log(t.timeNum); break;
                                    }
                                }
                                else break;
                            }

                            FlatBufferBuilder.PutIn(builder);
                            mcts.Clear();
                            sceneCatalog.directoryInfo.Delete(true);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                    }
                });
            }

            private void ServerReceive(IPEndPoint removePoint, FlatBufferBuilder builder, int length)
            {
                var startRstData = Flat.Start_Rst.GetRootAsStart_Rst(builder.DataBuffer);
                var workData = workDataDic[removePoint.Port];
                

                var node = workData.node;
                if (startRstData.EffectsLength != 0)
                    for (int i = (int)EffectAgent.end - 1; i >= 0; i--)
                    {
                        var effectDiskData = startRstData.Effects(i).Value;
                        node.AddEffect((EffectAgent)((int)EffectAgent.end - i - 1), Effect.ToRAM(effectDiskData));
                    }
                if (node.已扩展 == false)
                {
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
                        nextNode.mcts = mcts;
                        node.chiles.Enqueue(nextNode, float.MinValue);
                    }
                    node.已扩展 = true;
                }
                node.Add线程占用(-1);
                workDataDic.Remove(removePoint.Port);
                IdleClientEndPointQueue.Enqueue(removePoint.Port);
            }
        }
#endif

#if PRO_MCTS_CLIENT
        private class MainNode_Client : MainNode
        {
            private Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            public MainNode_Client(MCTS mcts) : base(mcts)
            {
                TimeManager.enableUpdate = false;
                client.Bind(new IPEndPoint(IPAddress.Loopback, UnityEngine.Random.Range(10000, 30000)));
                client.SendTo(new byte[0], serverIPEndPoint);
                new Thread(() =>
                {
                    byte[] bytes = new byte[1024 * 10];
                    FlatBufferBuilder builder = new FlatBufferBuilder(new ByteBuffer(bytes));
                    EndPoint remotePoint = new IPEndPoint(IPAddress.Any, 0);
                    while (true)
                    {
                        int length = client.ReceiveFrom(bytes, ref remotePoint);
                        if (length > 0)
                        {
                            //Debug.Log("客户端收到消息" + length);
                            TimeManager.Inst.AddToQueue_MainThreadUpdate_Clear_WaitInvoke(() =>
                                ClientReceive(client, remotePoint as IPEndPoint, builder, length));
                        }
                        else break;
                    }
                    Debug.Log("连接断开");
                }).Start();
            }

            private void ClientReceive(Socket removeSocket, IPEndPoint remotePoint, FlatBufferBuilder builder, int length)
            {
                var startCmdData = Flat.Start_Cmd.GetRootAsStart_Cmd(builder.DataBuffer);
                var scene = SceneEntity.TakeOut(SceneCatalog.LoadSceneInfo(new DirectoryInfo(startCmdData.Path)));
                var countdown = scene.LoadAll(scene.sceneCatalog);
                ThreadPool.QueueUserWorkItem((obj) =>
                {
                    try
                    {
                        countdown.Wait(1000 * 5);
                        TimeManager.Inst.AddToQueue_MainThreadUpdate_Clear_WaitInvoke(() =>
                            SceneManager.Inst.SwitchScene(scene));
                        mcts.round = scene.ActiveRound[scene.sceneCatalog.mainRound];
                        mcts.startingData.Init(mcts.round);
                        NodeBase node = mcts.main;
                        for (int i = startCmdData.NodesLength - 1; i >= 0; i--)
                        {
                            var nodeType = startCmdData.NodesType(i);
                            NodeBase nowNode = null;
                            switch (nodeType)
                            {
                                case Flat.NodeBase.Node:
                                    {
                                        var diskData = startCmdData.Nodes<Flat.Node>(i).Value;
                                        nowNode = Node.ToRAM(diskData, scene);
                                        break;
                                    }
                                case Flat.NodeBase.TimeNode:
                                    {
                                        var diskData = startCmdData.Nodes<Flat.TimeNode>(i).Value;
                                        nowNode = TimeNode.ToRAM(diskData, scene);
                                        break;
                                    }
                            }
                            nowNode.mcts = mcts;
                            node.chiles.Enqueue(nowNode, 0);
                            node = nowNode;
                        }
                        builder.Clear();

                        NodeBase rootNode = mcts.main.chiles.Peek();
                        Action<FlatBufferBuilder> action = null;
                        TimeManager.Inst.AddToQueue_MainThreadUpdate_Clear_WaitInvoke(() =>
                            rootNode.访问(builder, action));
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
                        CountdownEvent countdown_PutIn = null;
                        TimeManager.Inst.AddToQueue_MainThreadUpdate_Clear_WaitInvoke(() =>
                        {
                            SceneManager.Inst.SwitchScene(null);
                            countdown_PutIn = SceneEntity.PutIn(scene);
                        });
                        countdown_PutIn.Wait(1000 * 2);
                        mcts.Clear();
                        removeSocket.SendTo(builder.DataBuffer.GetBytes(), builder.DataBuffer.Position, builder.Offset, SocketFlags.None, remotePoint);
                        builder.Clear();
                    }
                    catch (Exception e)
                    {
                        Log.Print(e.ToString());
                    }
                });
            }
        }
#endif

        internal abstract class MainNode : NodeBase
        {
            public MainNode(MCTS mcts)
            {
                this.mcts = mcts;
            }

            public override void PutIn()
            {
                var mcts = this.mcts;
                base.PutIn();
                this.mcts = mcts;
            }

            public override (Flat.NodeBase, Offset<int>) ToDisk(FlatBufferBuilder builder)
            {
                throw new NotImplementedException();
            }

            public override void 执行()
            {
                throw new NotImplementedException();
            }
        }
    }
}

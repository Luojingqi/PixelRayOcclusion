using Google.FlatBuffers;
using PRO.Disk.Scene;
using PRO.TurnBased;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace PRO.AI
{
    public partial class MCTS
    {
        private MainNode main;
        private RoundFSM round;
        private StartingData startingData = new StartingData();
        private List<NodeBase> NodeList = new List<NodeBase>(128);

        public MCTS()
        {
            main = new MainNode(this);
        }

        public void Clear()
        {
            main.PutIn();
            round = null;
            startingData.Clear();
            NodeList.Clear();
        }

        private struct ClientWorkData
        {
            public NodeBase node;
        }

        /// <summary>
        /// MCTS开始模拟的起始数据
        /// </summary>
        private class StartingData
        {
            public string roleGuid;
            public int roundNum = -1;
            public Dictionary<string, RoleInfo> roleGuidInfoDic = new Dictionary<string, RoleInfo>();

            public void Init(RoundFSM round)
            {
                roleGuid = round.State3_Turn.NowTurn.Agent.GUID;
                roundNum = round.State3_Turn.NowRoundNum;
                foreach (var role in round.RoleHash)
                {
                    var info = RoleInfo.TakeOut();
                    RoleInfo.CloneValue(role.Info, info);
                    roleGuidInfoDic.Add(role.GUID, info);
                }
            }
            public void Clear()
            {
                roleGuid = null;
                roundNum = -1;
                foreach (var info in roleGuidInfoDic.Values)
                    RoleInfo.PutIn(info);
                roleGuidInfoDic.Clear();
            }
        }

        #region Socket
        public static void Init()
        {
#if PRO_MCTS_SERVER
            Server.Bind(new IPEndPoint(IPAddress.Loopback, 17000));
            Server.Listen(100);
            new Thread(AcceptThread).Start();
#endif
#if PRO_MCTS_CLIENT
            Client.Connect(new IPEndPoint(IPAddress.Loopback, 17000));
            Debug.Log("连接成功");
            new Thread(ReceiveThread).Start((Client, ClientReceive));
#endif
        }
#if PRO_MCTS_CLIENT
        private static event Action<Socket, FlatBufferBuilder, int> ClientReceive;
        private static Socket Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
#endif
#if PRO_MCTS_SERVER
        private static object lockObject = new object();
        public void 开始模拟(RoundFSM round)
        {
            this.round = round;
            startingData.Init(round);
            main.开始模拟();
        }
        private static void AcceptThread()
        {
            while (true)
            {
                Socket remoteSocket = Server.Accept();
                new Thread(ReceiveThread).Start((remoteSocket, ServerReceive));
                IdleClientSocketQueue.Enqueue(remoteSocket);
            }
        }
        private static event Action<Socket, FlatBufferBuilder, int> ServerReceive;
        private static Socket Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static Queue<Socket> IdleClientSocketQueue = new Queue<Socket>();
        private static Dictionary<Socket, ClientWorkData> WorkClientSocketDic = new Dictionary<Socket, ClientWorkData>();
#endif
        private static void ReceiveThread(object obj)
        {
            (Socket, Action<Socket, FlatBufferBuilder, int>)? obj_Value = obj as (Socket, Action<Socket, FlatBufferBuilder, int>)?;
            Socket remoteSocket = obj_Value.Value.Item1;
            Action<Socket, FlatBufferBuilder, int> receiveAction = obj_Value.Value.Item2;
            FlatBufferBuilder builder = new FlatBufferBuilder(1024 * 10);
            var buffer = builder.DataBuffer.ToSpan(0, builder.DataBuffer.Length);
            while (true)
            {
                int length = remoteSocket.Receive(buffer);
                if (length > 0)
                {
                    Debug.Log("收到消息");
                    TimeManager.Inst.AddToQueue_MainThreadUpdate_Clear_WaitInvoke(() =>
                    receiveAction.Invoke(remoteSocket, builder, length));
                }
                else if (length == 0)
                {
                    Debug.Log("连接断开");
                    break;
                }
            }
        }

#endregion
    }
}

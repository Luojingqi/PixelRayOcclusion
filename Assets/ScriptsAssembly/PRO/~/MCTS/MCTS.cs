using Google.FlatBuffers;
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
#if PRO_MCTS_SERVER
        private MainNode_Server main_server;
#endif
#if PRO_MCTS_CLIENT
        private MainNode_Client main_client;
#endif
        private MainNode main;
        private RoundFSM round;
        private StartingData startingData = new StartingData();

        public enum Mode
        {
            Server,
            Clinet
        }

        public MCTS(Mode mode)
        {
            switch (mode)
            {
#if PRO_MCTS_SERVER
                case Mode.Server: 
                    main_server = new MainNode_Server(this);
                    main = main_server;
                    break;
#endif
#if PRO_MCTS_CLIENT
                case Mode.Clinet:
                    main_client = new MainNode_Client(this); 
                    main = main_client;
                    break;
#endif
            }
        }

        public void Clear()
        {
            main.PutIn();
            round = null;
            startingData.Clear();
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
        private static IPEndPoint serverIPEndPoint = new IPEndPoint(IPAddress.Loopback, 17000);
#if PRO_MCTS_SERVER
        public static void Init()
        {
            AcceptServer.Bind(serverIPEndPoint);
            new Thread(() =>
            {
                byte[] bytes = new byte[0];
                EndPoint remotePoint = new IPEndPoint(IPAddress.Any, 0);
                while (true)
                {
                    int length = AcceptServer.ReceiveFrom(bytes, ref remotePoint);
                    var remote = remotePoint as IPEndPoint;
                    Debug.Log("客户端连接: " + remote.Port);
                    IdleClientEndPointQueue.Enqueue(remote.Port);
                }
            }).Start();
        }
        public void 开始模拟(RoundFSM round)
        {
            this.round = round;
            startingData.Init(round);
            main_server.开始模拟();
        }
        private static object lockObject = new object();
        private static Socket AcceptServer = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private static Queue<int> IdleClientEndPointQueue = new Queue<int>();
#endif
        #endregion
    }
}

using Cysharp.Threading.Tasks;
using PRO.TurnBased;
using System.Collections.Generic;

namespace PRO.AI
{
    public partial class MCTS
    {
        private MainNode main;
        private RoundFSM round;

        private StartingData startingData = new StartingData();

        public MCTS()
        {
            main = new MainNode(this);
        }

        public void 开始模拟(RoundFSM round)
        {
            TimeManager.Inst.enabled = false;
            this.round = round;
            startingData.Init(round);
            main.开始模拟();
            TimeManager.Inst.enabled = true;
        }

        //public void SendScene(SceneEntity scene, RoundFSM round)
        //{
        //    var root = GameSaveCatalog.CreatFile("MCTS_TEMP");
        //    scene.SetTempCatalog(SceneCatalog.CreateFile(scene.sceneCatalog.name, root));
        //    var countdown = scene.SaveAll();
        //    TimeManager.enableUpdate = false;
        //    ThreadPool.QueueUserWorkItem((obj) =>
        //    {
        //        countdown.Wait(1000 * 15);
        //        TimeManager.enableUpdate = true;

        //    });
        //}

        //public void ResetScene(SceneEntity scene)
        //{
        //   var root = GameSaveCatalog.LoadGameSaveInfo("testSave");
        //    scene.SetTempCatalog()
        //}

        /// <summary>
        /// MCTS开始模拟的起始数据
        /// </summary>
        private class StartingData
        {
            public string roleGuid;
            public int roundNum;
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
        }
    }
}

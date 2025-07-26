using Google.FlatBuffers;
using PRO.AI;
using PRO.Skill;
using PRO.TurnBased;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using PRO.AI.Flat;
namespace PRO
{
    public class GamePlayMain : MonoScriptBase, ITime_Awake, ITime_Update
    {
        public static GamePlayMain Inst { get; private set; }

        public void TimeAwake()
        {
            Inst = this;
            OperateFSMBase.InitOperateType();
            MCTS.Init();
            mcts = new MCTS();
        }


        public RoundFSM Round
        {
            get => round;
            set
            {
                round = value;
#if PRO_RENDER
                if (round == null)
                {
                    GameMainUIC.Inst.CloseTurnUI();
                }
                else
                {
                    GameMainUIC.Inst.OpenTurnUI();
                    GameMainUIC.Inst.SetTurn(round.State3_Turn.TurnFSMList, round.State3_Turn.NowTurn.Index);
                }
                if (SceneManager.Inst.NowScene != null)
                    SceneManager.Inst.NowScene.sceneCatalog.mainRound = round?.GUID;
#endif
            }
        }
        [ShowInInspector]
        private RoundFSM round;

        private MCTS mcts;

        bool p = false;

        public void TimeUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (p) Time.timeScale = 1;
                else Time.timeScale = 0;
                p = !p;
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                var round = new RoundFSM(SceneManager.Inst.NowScene, System.Guid.NewGuid().ToString());
                var round_state0 = round.GetState<RoundState0_DataReady>();
                for (int i = 0; i < 2; i++)
                {
                    var role = RoleManager.Inst.TakeOut("Ĭ��", SceneManager.Inst.NowScene, System.Guid.NewGuid().ToString());
                    role.transform.position = new Vector3(i / 0.5f, 0);
                    round_state0.AddRole(role);

                    role.AddOperate(typeof(Skill_0_0));
                    role.AddOperate(typeof(Skill_3_3));
                }

                round_state0.ReadyOver();
                Round = round; 
            }

#if PRO_MCTS_SERVER
            if (Input.GetKeyDown(KeyCode.M) && m == false)
            {
                MTCS();
                m = true;
            }
        }
        private bool m = false;

        [Button]
        public void MTCS()
        {
            mcts.��ʼģ��(round);
        }
#elif PRO_MCTS_CLIENT
        }
#endif
    }
}
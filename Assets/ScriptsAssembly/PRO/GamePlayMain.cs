using PRO.Skill;
using PRO.TurnBased;
using Sirenix.OdinInspector;
using UnityEngine;
namespace PRO
{
    public class GamePlayMain : MonoScriptBase, ITime_Awake, ITime_Update
    {
        public static GamePlayMain Inst { get; private set; }

        public void TimeAwake()
        {
            Inst = this;
            OperateFSMBase.InitOperateType();
        }
        public RoundFSM Round
        {
            get => round;
            set
            {
                round = value;
#if !PRO_MCTS
                if (round == null)
                {
                    GameMainUIC.Inst.CloseTurnUI();
                }
                else
                {
                    GameMainUIC.Inst.OpenTurnUI();
                    GameMainUIC.Inst.SetTurn(round.State3_Turn.TurnFSMList, round.State3_Turn.NowTurn.Index);
                }
#endif
            }
        }
        [ShowInInspector]
        private RoundFSM round;

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
            Debug.Log("����");
            var mcts = new AI.MCTS();
            mcts.��ʼģ��(round);
        }
    }
}
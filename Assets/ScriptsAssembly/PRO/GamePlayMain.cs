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
        }
        public RoundFSM Round
        {
            get => round;
            set
            {
                round = value;
                if (round == null) return;
                GameMainUIC.Inst.OpenTurnUI();
                GameMainUIC.Inst.SetTurn(round.State3_Turn.TurnFSMList, round.State3_Turn.NowTurn.Index);
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

            if (Input.GetKeyDown(KeyCode.M))
            {
                var round = new RoundFSM(SceneManager.Inst.NowScene, System.Guid.NewGuid().ToString());
                var round_state0 = round.GetState<RoundState0_DataReady>();
                for (int i = 0; i < 3; i++)
                {
                    var role = RoleManager.Inst.TakeOut("д╛хо", SceneManager.Inst.NowScene, System.Guid.NewGuid().ToString());
                    role.transform.position = new Vector3(i / 0.5f, 0);
                    round_state0.AddRole(role);
                }

                round_state0.ReadyOver();
                this.Round = round;
            }
        }
    }
}
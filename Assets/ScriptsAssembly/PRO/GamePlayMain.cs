using PRO.TurnBased;
using UnityEngine;
namespace PRO
{
    public class GamePlayMain : MonoScriptBase, ITime_Start, ITime_Update
    {
        public static GamePlayMain Inst { get; private set; }


        public void TimeStart()
        {
            Inst = this;
            RoleManager.Inst.TakeOut("д╛хо", SceneManager.Inst.NowScene);
            round = new RoundFSM();
        }
        public RoundFSM round;

        bool p = false;

        public void TimeUpdate()
        {
            round.Update();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (p) Time.timeScale = 1;
                else Time.timeScale = 0;
                p = !p;
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                // MCTS.Main();
            }
        }
    }
}
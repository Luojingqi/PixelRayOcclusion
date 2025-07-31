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
        bool p = false;

        public void TimeUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (p) Time.timeScale = 1;
                else Time.timeScale = 0;
                p = !p;
            }
        }
    }
}
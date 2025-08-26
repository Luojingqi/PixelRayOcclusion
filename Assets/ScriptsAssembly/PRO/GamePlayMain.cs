using DG.Tweening;
using PRO.Weapon;
using UnityEngine;
namespace PRO
{
    public class GamePlayMain : MonoScriptBase, ITime_Awake, ITime_Update
    {
        public static GamePlayMain Inst { get; private set; }

        public WeaponConfig WeaponConfig;

        public void TimeAwake()
        {
            Inst = this;
            DOTween.defaultAutoPlay = AutoPlay.None;
        }
        bool p = false;

        public void TimeUpdate()
        {
            DOTween.ManualUpdate(TimeManager.deltaTime, TimeManager.unscaledDeltaTime);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (p) Time.timeScale = 1;
                else Time.timeScale = 0;
                p = !p;
            }
        }
    }
}
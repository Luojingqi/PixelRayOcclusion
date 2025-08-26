using NodeCanvas.Framework;
using PRO.SkillEditor;
using UnityEngine;
namespace PRO.BT.攻击
{
    public class 近战攻击_挥砍_Init : ActionTask
    {
        public BBParameter<SkillPlayData> PlayData_检测;
        private bool isInit = false;
        protected override string OnInit()
        {
            Debug.Log("123");
            if (isInit) return null;


            return null;
        }

        private class 近战攻击_挥砍_检测_Logic : SkillLogicBase
        {

        }
    }
}

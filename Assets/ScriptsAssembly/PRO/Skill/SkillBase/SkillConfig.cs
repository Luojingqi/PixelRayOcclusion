using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace PRO.Skill.Base
{
    [CreateAssetMenu(menuName = "创建一个Skill配置")]
    public class SkillConfig : SerializedScriptableObject
    {
        public string Name;
        public int Radius_G;
        public float Radius_W;

        public 施法触发方式 施法type;
        public List<StartCombatEffectData> StartCombatEffectDataList = new List<StartCombatEffectData>();
    }
}

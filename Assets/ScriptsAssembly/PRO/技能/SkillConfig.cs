using PRO;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace PRO
{
    [CreateAssetMenu(menuName = "创建一个技能配置")]
    public class SkillConfig : SerializedScriptableObject
    {
        public string Name;
        public int 行动点;
        public int Radius_G;
        public float Radius_W;

        public 施法方式 施法type;
        public List<StartCombatEffectData> StartCombatEffectDataList = new List<StartCombatEffectData>();

        public class Viewer
        {
            public string id;
            [ReadOnly]
            public SkillConfig config;
            [ShowInInspector]
            public string name { get => config.Name; set => config.Name = value;  }
            [ShowInInspector]
            public int 行动点 { get => config.行动点; set => config.行动点 = value; }
            [ShowInInspector]
            public int 施法半径 { get => config.Radius_G; set { config.Radius_G = value; config.Radius_W = value * Pixel.Size; } }
            [ShowInInspector]
            public 施法方式 施法方式 { get => config.施法type; set => config.施法type = value; }
            [ShowInInspector]
            [TableList]
            public List<StartCombatEffectData> 伤害 { get => config.StartCombatEffectDataList;set { config.StartCombatEffectDataList = value; } }


            public Viewer(string id, SkillConfig config)
            {
                this.id = id;
                this.config = config;
            }
        }
    }
}

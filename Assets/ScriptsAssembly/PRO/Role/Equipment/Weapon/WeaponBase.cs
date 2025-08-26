using PRO.SkillEditor;
using System;
using UnityEngine;

namespace PRO.Weapon
{
    public abstract class WeaponBase
    {
        public string Name { get; set; }
        public Sprite Icon { get; private set; }
        public int Optional手持方式 { get; private set; }
        public SkillPlayData[] PlayDataArray { get; protected set; }
        public EquipmentPrefix Prefix { get; private set; }
        
        public WeaponBase(EquipmentPrefix prefix)
        {
            Prefix = prefix;
            SetConfigValue(GamePlayMain.Inst.WeaponConfig.DataDic[this.GetType()]);
        }

        public Role Agent { get; private set; }
        public 手持方式 Now手持方式 { get; private set; } = 0;

        [Flags]
        public enum 手持方式
        {
            单手 = 1 << 0,
            双手 = 1 << 1
        }

        public void 穿戴(Role role, 手持方式 method)
        {
            Agent = role;
            Now手持方式 = method;

            //Agent.Info.Add(GamePlayMain.Inst.WeaponConfig.DataDic[this.GetType()].穿戴后属性提升[Math.Log(method)]);

            On穿戴();
        }
        public void 卸下()
        {
            On卸下();
            Now手持方式 = 0;
            Agent = null;
        }
        public virtual bool Can穿戴(手持方式 method) => true;
        protected virtual void On穿戴() { }
        protected virtual void On卸下() { }

        private int index = 0;
        public bool Play攻击(float deltaTime)
        {
            if (PlayDataArray[index].UpdateFrameScript(Agent.SkillPlayAgent, deltaTime))
            {
                index++;
                if (index == PlayDataArray.Length)
                {
                    index = 0;
                    return true;
                }
            }
            return false;
        }
        private void SetConfigValue(WeaponConfig.Data data)
        {
            Name = data.Name;
            Icon = data.Icon;
            Optional手持方式 = data.Optional手持方式;
            PlayDataArray = new SkillPlayData[data.VisualArray.Length];
            for (int i = 0; i < PlayDataArray.Length; i++)
            {
                var playData = new SkillPlayData();
                PlayDataArray[i] = playData;
                playData.SkillVisual = data.VisualArray[i];
            }

            SetAutoConfigValue(data);
        }

        protected virtual void SetAutoConfigValue(WeaponConfig.Data data) { }
    }
}

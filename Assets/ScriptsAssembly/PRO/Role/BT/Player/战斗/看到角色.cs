using NodeCanvas.Framework;
using PRO.Skill;
using PRO.SkillEditor;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace PRO.BT.战斗
{
    public class 看到角色 : ConditionTask
    {
        public BBParameter<Role> Agent;
        public BBParameter<Dictionary<string, Role>> 看到的角色Dic;

        protected override string OnInit()
        {
            SkillVisual_视野_Logic.Node = this;
            PlayData.SkillVisual = SkillVisual_视野;
            PlayData.SkillLogicList.Add(SkillVisual_视野_Logic);
            return null;
        }

        protected override bool OnCheck()
        {
            SkillVisual_视野.UpdateFrame(Agent.value.SkillPlayAgent, PlayData);
            return 看到的角色Dic.value.Count > 0;
        }

        public SkillVisual_Disk SkillVisual_视野;
        private SkillLogic_视野 SkillVisual_视野_Logic = new(null);
        private SkillPlayData PlayData = new();

        public class SkillLogic_视野 : SkillLogicBase
        {
            public SkillLogic_视野(string guid) : base(guid) { }

            public 看到角色 Node;

            public override void Before_AttackTest2D(AttackTestSlice2DBase_Disk slice, FrameData frameData)
            {
                Node.看到的角色Dic.value.Clear();
            }
            public override void Agoing_AttackTest2D(AttackTestSlice2DBase_Disk slice, FrameData frameData, Span<RaycastHit2D> hitSpan)
            {
                var role = Node.Agent.value;
                var set = Node.看到的角色Dic.value;
                for (int i = 0; i < hitSpan.Length; i++)
                {
                    Debug.Log(hitSpan[i].transform.name);
                    var byTransform = hitSpan[i].transform;
                    if (byTransform == role.transform) continue;
                    var byRole = role.Scene.GetRole(byTransform);
                    if (set.ContainsKey(byRole.GUID) == false)
                        set.Add(byRole.GUID, byRole);
                }
            }
        }
    }
}
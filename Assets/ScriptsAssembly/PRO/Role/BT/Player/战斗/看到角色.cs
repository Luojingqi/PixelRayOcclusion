using Google.FlatBuffers;
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
            var skillLogic0 = new SkillLogic_视野(null);
            skillLogic0.Node = this;
            skillLogicArray[0] = skillLogic0;
            return null;
        }

        protected override bool OnCheck()
        {
            视野.UpdateFrame(Agent.value.SkillPlayAgent, skillLogicArray, 0);
            return 看到的角色Dic.value.Count > 0;
        }

        public SkillVisual_Disk 视野;
        private SkillLogicBase[] skillLogicArray = new SkillLogicBase[1];
    }

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
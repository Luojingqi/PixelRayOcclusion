using Google.FlatBuffers;
using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.BehaviorDesigner.Runtime.Tasks.Conditionals;
using Opsive.GraphDesigner.Runtime.Variables;
using PRO.Skill;
using System.Collections.Generic;
namespace PRO.BT
{
    public class 看到角色 : Conditional
    {
        public SharedVariable<Role> Agent;
        public SharedVariable<HashSet<Role>> 视野内角色List;
        public override TaskStatus OnUpdate()
        {
            return base.OnUpdate();
        }
    }

    public class SkillLogic_视野 : SkillLogicBase
    {
        public override void ToDisk(FlatBufferBuilder builder)
        {
            throw new System.NotImplementedException();
        }

        public override void ToRAM(FlatBufferBuilder builder)
        {
            throw new System.NotImplementedException();
        }
    }
}
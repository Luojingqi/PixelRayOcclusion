using PRO.Tool;
using System.Collections.Generic;

namespace PRO.TurnBased
{
    public class RoundFSM : FSMManager<RoundStateEnum>
    {

        public RoundFSM()
        {
            AddState(new RoundState0_DataReady());
            AddState(new RoundState1_BeFrightened());
            AddState(new RoundState2_Initiative());
            AddState(new RoundState3_Turn());
            SwitchState(RoundStateEnum.dataReady);
            State3_Turn = GetState<RoundState3_Turn>();
        }

        public RoundState3_Turn State3_Turn;

        /// <summary>
        /// 轮次管理的角色
        /// </summary>
        public HashSet<Role> RoleHash = new HashSet<Role>();

        public void AddRole(Role role)
        {
            RoleHash.Add(role);
        }

        public override void Update()
        {
            base.Update();
        }
    }
}

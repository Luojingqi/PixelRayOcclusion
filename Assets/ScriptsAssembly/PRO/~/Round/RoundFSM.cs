using Google.FlatBuffers;
using PRO.Tool;
using System;
using System.Collections.Generic;

namespace PRO.TurnBased
{
    public class RoundFSM : FSMManager<RoundStateEnum>, IScene
    {
        public SceneEntity Scene => scene;
        private SceneEntity scene;

        public string GUID => guid;
        private string guid;

        public RoundFSM(SceneEntity scene, string guid)
        {
            this.guid = guid;
            this.scene = scene;
            AddState(new RoundState0_DataReady());
            AddState(new RoundState1_BeFrightened());
            AddState(new RoundState2_Initiative());
            AddState(new RoundState3_Turn());
            AddState(new RoundState4_End());
            SetState(RoundStateEnum.dataReady);
            State3_Turn = GetState<RoundState3_Turn>();
            scene.ActiveRound.Add(guid, this);
        }

        public RoundState3_Turn State3_Turn;

        /// <summary>
        /// 轮次管理的角色
        /// </summary>
        public HashSet<Role> RoleHash = new HashSet<Role>();


        public override void Update()
        {
            base.Update();
            foreach (var role in RoleHash)
            {
                var roleBlockPos = Block.WorldToBlock(role.transform.position);
                for (int y = -1; y <= 1; y++)
                    for (int x = -1; x <= 1; x++)
                    {
                        var blockPos = roleBlockPos + new UnityEngine.Vector2Int(x, y);
                        if (scene.ActiveBlockBase.Contains(blockPos) == false)
                            scene.ThreadLoadOrCreateBlock(scene.sceneCatalog, blockPos);
                        else
                            scene.GetBlock(blockPos).ResetUnLoadCountdown();
                    }
            }
        }

        public void ToDisk(FlatBufferBuilder builder)
        {
            var guidOffset = builder.CreateString(guid);
            Span<int> roleHashOffsetArray = stackalloc int[RoleHash.Count];
            int index = 0;
            foreach (var role in RoleHash)
                roleHashOffsetArray[index++] = builder.CreateString(role.GUID).Value;
            var roleHashOffset = builder.CreateVector_Offset(roleHashOffsetArray);
            var state3Offset = State3_Turn.ToDisk(builder);
            Flat.RoundFSMData.StartRoundFSMData(builder);
            Flat.RoundFSMData.AddGuid(builder, guidOffset);
            Flat.RoundFSMData.AddNowState(builder, (int)NowState.EnumName);
            Flat.RoundFSMData.AddRoleHash(builder, roleHashOffset);
            Flat.RoundFSMData.AddState3Turn(builder, state3Offset);
            builder.Finish(Flat.RoundFSMData.EndRoundFSMData(builder).Value);
        }
        public void ToRAM(Flat.RoundFSMData diskData)
        {
            SetState((RoundStateEnum)diskData.NowState);
            for (int i = diskData.RoleHashLength - 1; i >= 0; i--)
            {
                var roleGuid = diskData.RoleHash(i);
                var role = scene.GetRole(roleGuid);
                RoleHash.Add(role);
            }
            if (NowState.EnumName == RoundStateEnum.turn)
                State3_Turn.ToRAM(diskData.State3Turn.Value);
        }
    }
}

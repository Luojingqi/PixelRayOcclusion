using Google.FlatBuffers;
using PRO.Flat.Ex;
using PRO.Tool;
using UnityEngine;
using static PRO.Skill.OperateFSMBase;

namespace PRO.Skill
{
    public interface ISkillPointer_射线选择 : ISkillPointer
    {
        public SkillPointer_范围内射线类 SkillPointer { get; }

        public static TriggerState Trigger<T>(T operate, FlatBufferBuilder record) where T : OperateFSMBase, ISkillPointer_射线选择
        {
            operate.Agent.LookAt(MousePoint.worldPos);

            Vector2 d = (MousePoint.worldPos - (Vector2)operate.SkillPointer.transform.position).normalized;

            var hit = Physics2D.Raycast(operate.SkillPointer.transform.position, d, operate.config.Radius_W,
               1 << (int)GameLayer.Role | 1 << (int)GameLayer.Block);

            Role byRole = null;
            if (hit.collider != null)
            {
                byRole = operate.Agent.Scene.GetRole(hit.transform);
                operate.SkillPointer.EndPos = hit.point;
            }
            else
            {
                operate.SkillPointer.EndPos = operate.SkillPointer.StartPos + d.normalized * operate.config.Radius_W;
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
                return TriggerState.toT0;
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                var byRoleGuidOffset = record.CreateString(byRole?.GUID);
                Flat.OperateRecord_RaySelect.StartOperateRecord_RaySelect(record);
                Flat.OperateRecord_RaySelect.AddStartPos(record, operate.SkillPointer.StartPos.ToDisk(record));
                Flat.OperateRecord_RaySelect.AddEndPos(record, operate.SkillPointer.EndPos.ToDisk(record));
                Flat.OperateRecord_RaySelect.AddByRoleGuid(record, byRoleGuidOffset);
                record.Finish(Flat.OperateRecord_RaySelect.EndOperateRecord_RaySelect(record).Value);
                return TriggerState.toT2;
            }

            return TriggerState.update;
        }

        public static void 扩展节点<T>(T operate, ref ReusableList<FlatBufferBuilder> recordList) where T : OperateFSMBase, ISkillPointer_射线选择
        {
            foreach (var byRole in operate.Turn.RoundFSM.RoleHash)
            {
                if (byRole == operate.Agent) continue;
                Vector2 startPos = operate.Agent.CenterPos.GlobalToWorld();
                Vector2 endPos = byRole.CenterPos.GlobalToWorld();
                var hit = Physics2D.Raycast(startPos, endPos - startPos, operate.config.Radius_W,
                1 << (int)GameLayer.Role | 1 << (int)GameLayer.Block);
                if (hit.collider != null && hit.transform != byRole.transform)
                {
                    var record = FlatBufferBuilder.TakeOut(1024);
                    var byRoleGuidOffset = record.CreateString(byRole.GUID);
                    Flat.OperateRecord_RaySelect.StartOperateRecord_RaySelect(record);
                    Flat.OperateRecord_RaySelect.AddStartPos(record, operate.SkillPointer.StartPos.ToDisk(record));
                    Flat.OperateRecord_RaySelect.AddEndPos(record, operate.SkillPointer.EndPos.ToDisk(record));
                    Flat.OperateRecord_RaySelect.AddByRoleGuid(record, byRoleGuidOffset);
                    record.Finish(Flat.OperateRecord_RaySelect.EndOperateRecord_RaySelect(record).Value);
                    recordList.Add(record);
                }
            }
        }

        public static void 节点执行<T>(T operate, FlatBufferBuilder record, Operator form) where T : OperateFSMBase, ISkillPointer_射线选择
        {
            var operateData = Flat.OperateRecord_RaySelect.GetRootAsOperateRecord_RaySelect(record.DataBuffer);
            operate.Agent.LookAt(operateData.EndPos.Value.ToRAM());
            string byRoleGuid = operateData.ByRoleGuid;
            if (byRoleGuid != null)
                operate.context.AddByAgent(operate.context.Agent.Scene.GetRole(byRoleGuid), true);
        }
    }
}

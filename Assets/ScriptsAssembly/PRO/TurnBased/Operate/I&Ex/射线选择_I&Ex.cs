using PRO.Proto.Ex;
using PRO.Skill.Proto;
using PRO.Tool;
using UnityEngine;
using static PRO.OperateFSMBase;

namespace PRO.Skill
{
    public interface IOperate_射线选择
    {
        public SkillPointer_范围内射线类 SkillPointer { get; set; }

        public static TriggerState Trigger<T>(T operate, out IOperateRecord operateRecord) where T : OperateFSMBase, IOperate_射线选择
        {
            var record = PRO.Proto.ProtoPool.TakeOut<OperateRecordRaySelect>();
            operateRecord = record;
            operate.Turn.Agent.LookAt(MousePoint.worldPos);

            Vector2 d = (MousePoint.worldPos - (Vector2)operate.SkillPointer.transform.position).normalized;

            var hit = Physics2D.Raycast(operate.SkillPointer.transform.position, d, operate.config.Radius_W,
               1 << (int)GameLayer.Role | 1 << (int)GameLayer.Block);

            if (hit.collider != null)
            {
                var byRole = operate.Turn.Agent.Scene.GetRole(hit.transform);
                if (byRole != null)
                {
                    record.ByRole = byRole.Guid;
                }
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
                record.StartPos = operate.SkillPointer.StartPos.ToDisk();
                record.EndPos = operate.SkillPointer.EndPos.ToDisk();
                return TriggerState.toT2;
            }

            return TriggerState.update;
        }

        public static void 扩展节点<T>(T operate, ref ReusableList<IOperateRecord> operateRecordList) where T : OperateFSMBase, IOperate_射线选择
        {
            foreach (var byRole in operate.Turn.RoundFSM.RoleHash)
            {
                if (byRole == operate.Turn.Agent) continue;
                Vector2 startPos = operate.Turn.Agent.CenterPos.GlobalToWorld();
                Vector2 endPos = byRole.CenterPos.GlobalToWorld();
                var hit = Physics2D.Raycast(startPos, endPos - startPos, operate.config.Radius_W,
                1 << (int)GameLayer.Role | 1 << (int)GameLayer.Block);
                if (hit.collider != null && hit.transform != byRole.transform)
                {
                    var record = PRO.Proto.ProtoPool.TakeOut<OperateRecordRaySelect>();
                    record.ByRole = byRole.Guid;
                    record.StartPos = startPos.ToDisk();
                    record.EndPos = endPos.ToDisk();
                    operateRecordList.Add(record);
                }
            }
        }

        public static void 节点执行<T>(T operate, IOperateRecord operateRecord) where T : OperateFSMBase, IOperate_射线选择
        {
            var record = operateRecord as OperateRecordRaySelect;
            operate.Turn.Agent.LookAt(record.EndPos.ToRAM());
            operate.context.AddByAgent(operate.context.Agent.Scene.GetRole(record.ByRole), true);
        }
    }
}
namespace PRO.Skill.Proto
{
    public partial class OperateRecordRaySelect : IOperateRecord
    {

    }
}

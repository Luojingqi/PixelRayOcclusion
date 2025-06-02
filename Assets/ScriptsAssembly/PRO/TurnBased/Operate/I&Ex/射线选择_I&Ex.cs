using PRO.Proto.Ex;
using UnityEngine;
using static PRO.OperateFSMBase;

namespace PRO
{
    public interface IOperate_射线选择
    {
        public SkillPointer_范围内射线类 SkillPointer { get; set; }
    }
    //public static class OperateEx_射线选择
    //{
    //    public static TriggerState Trigger<T>(T operate, out OperateRecord operateRecord) where T : OperateFSMBase, IOperate_射线选择
    //    {
    //        var record = OperateRecord_射线选择.TakeOut();
    //        operateRecord = record;

    //    }

    //    public static void 扩展节点_仅目标<T>(T operate, ref List<OperateRecord> operateRecordList) where T : OperateFSMBase, IOperate_射线选择
    //    {
    //        foreach (var byRole in operate.Turn.RoundFSM.RoleHash)
    //        {
    //            if (byRole == operate.Turn.Agent) continue;
    //            Vector2 startPos = operate.Turn.Agent.CenterPos.GlobalToWorld();
    //            Vector2 endPos = byRole.CenterPos.GlobalToWorld();
    //            var hit = Physics2D.Raycast(startPos, endPos - startPos, operate.config.Radius_W,
    //            1 << (int)GameLayer.Role | 1 << (int)GameLayer.Block);
    //            if (hit.collider != null && hit.transform != byRole.transform)
    //            {
    //                var record = OperateRecord_射线选择.TakeOut();
    //                record.byRolelist.Add(byRole);
    //                record.startPos = startPos;
    //                record.endPos = endPos;
    //                operateRecordList.Add(record);
    //            }
    //        }
    //    }

    //    public static void 节点执行<T>(T operate, OperateRecord operateRecord) where T : OperateFSMBase, IOperate_射线选择
    //    {
    //        var record = operateRecord as OperateRecord_射线选择;
    //        operate.Turn.Agent.LookAt(record.endPos);
    //        foreach (var byRole in record.byRolelist)
    //            operate.context.AddByAgent(byRole, true);
    //    }
    //}
    //public class OperateRecord_射线选择 : OperateRecord
    //{
    //    public Vector2 startPos;
    //    public Vector2 endPos;
    //    public List<Role> byRolelist = new List<Role>();

    //    private static ObjectPool<OperateRecord_射线选择> pool = new ObjectPool<OperateRecord_射线选择>();
    //    public override void PutIn()
    //    {
    //        byRolelist.Clear();
    //        pool.PutIn(this);
    //    }
    //    public static OperateRecord_射线选择 TakeOut() => pool.TakeOut();
    //}
}
namespace PRO.Skill.Proto
{
    public partial class OperateRecordRaySelect : IOperateRecord
    {
        public static TriggerState Trigger<T>(T operate, out IOperateRecord operateRecord) where T : OperateFSMBase, IOperate_射线选择
        {
            var record = PRO.Proto.ProtoPool.TakeOut<OperateRecordRaySelect>();
            operateRecord = record;
            operate.Turn.Agent.LookAt(MousePoint.worldPos);
            operate.context.ClearByAgentData();
            Vector2 d = (MousePoint.worldPos - (Vector2)operate.SkillPointer.transform.position).normalized;
            var hit = Physics2D.Raycast(operate.SkillPointer.transform.position, d,
               operate.config.Radius_W,
               1 << (int)GameLayer.Role | 1 << (int)GameLayer.Block);
            if (hit.collider != null)
            {
                if (PROMain.Inst.GetRole(hit.transform, out Role byRole))
                {
                    //record.byRolelist.Add(byRole);
                    operate.context.AddByAgent(byRole, true);
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
    }
}

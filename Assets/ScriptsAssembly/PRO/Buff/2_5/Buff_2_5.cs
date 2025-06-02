using PRO.Buff.Base;
using PRO.Buff.Base.IBuff;
using PRO;
using UnityEngine;

namespace PRO.Buff
{
    /// <summary>
    /// 导体
    /// </summary>
    public class Buff_2_5 : BuffBase<Buff_2_5>, IBuff_独有
    {
        public override BuffTriggerType TriggerType => BuffTriggerType.受到战斗效果前;

        public override string Name => "导体";

        public override RoleBuffUpdateCheckBase<Buff_2_5> UpdateCheck => null;

        public override void ApplyEffect(CombatContext context, int index)
        {
            var byAgentData = context.ByAgentDataList[index];

            for (int i = 0; i < byAgentData.StartCombatEffectDataList.Count; i++)
            {
                var startData = byAgentData.StartCombatEffectDataList[i];
                if (startData.type == 属性.电)
                {
                    SetActive(false);
                    RaycastHit2D[] hits = Physics2D.CircleCastAll((Vector2)byAgentData.Agent.transform.position + (Vector2)byAgentData.Agent.nav.AgentMould.center * Pixel.Size, 30 * Pixel.Size, Vector2.zero, 0, 1 << (int)GameLayer.Role | 1 << (int)GameLayer.UnRole);
                    int num = 0;
                    foreach (var hit in hits)
                    {
                        var nextRole = PROMain.Inst.GetRole(hit.transform);
                        var nextRole_buff = nextRole.GetBuff<Buff_2_5>();

                        if (nextRole_buff != null && nextRole_buff.GetActive())
                        {
                            BuffEx.Set导电Path(byAgentData.Agent.Scene, byAgentData.Agent.GlobalPos, nextRole.GlobalPos, false, true, () =>
                            {
                                num++;
                                var c = CombatContext.TakeOut();
                                c.SetAgent(nextRole, context.Round, context.Turn);
                                c.LogBuilder.Insert(0, "(");
                                c.LogBuilder.Append("电流传导。");
                                c.Calculate_战斗技能初始化_直接对发起人结算(stackalloc StartCombatEffectData[]
                                {
                                    new(属性.电,startData.value),
                                });
                                c.LogBuilder.Append(")");
                                byAgentData.LogBuilder.Append(c.LogBuilder);
                                CombatContext.PutIn(c);
                            });
                        }
                    }
                    while (num++ < 3)
                    {
                        BuffEx.Set导电Path(byAgentData.Agent.Scene, byAgentData.Agent.GlobalPos, byAgentData.Agent.GlobalPos + new Vector2Int(Random.Range(-1, 2), Random.Range(-1, 2)) * Random.Range(10, 30), false, false);
                    }
                    SetActive(true);
                }
            }
        }

        public Buff_2_5()
        {
            active = true;
        }
    }
}

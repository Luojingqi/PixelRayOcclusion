using PRO;
using PRO.Tool;
using UnityEngine;
namespace PRO
{
    public class InjuryEstimationPanelC : UIChildControllerBase
    {
        public override UIChildViewBase View => view;
        private InjuryEstimationPanelV view = new InjuryEstimationPanelV();

        public override void Init()
        {
            base.Init();
        }

        public void Open(CombatContext context, int index)
        {
            var data = context.ByAgentDataList[index].EndCombatEffectData;
            view.Info_命中率.text = $"{data.命中率 * 100:F1}%";
            view.Info_暴击率.text = $"{data.暴击率 * 100:F1}%";

            var byAgent = context.ByAgentDataList[index].Agent;
            transform.position = Camera.main.WorldToScreenPoint(byAgent.transform.position + new Vector3(0, (byAgent.nav.AgentMould.size.y + 2) * Pixel.Size));
        }


        public static GameObjectPool<InjuryEstimationPanelC> pool { get; private set; }

        public static void InitPool(InjuryEstimationPanelC panel)
        {
            pool = new GameObjectPool<InjuryEstimationPanelC>(panel.gameObject, GameMainUIC.Inst.transform);
            pool.CreateEventT += (g, t) => t.Init();
        }
    }
}
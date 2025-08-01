using Cysharp.Threading.Tasks;
using System;
using static PRO.LogPanelM;
namespace PRO
{
    public class LogPanelC : UIControllerBase, ITime_Awake
    {
        public static LogPanelC Inst { get; private set; }
        public override UIViewBase View => view;
        private LogPanelV view = new LogPanelV();

        public override void Init(string uiName)
        {
            Inst = this;
            base.Init(uiName);
            OneLog.InitPool(view.LogPrefab.gameObject, transform);
        }
        public void TimeAwake()
        {
            Init("");
        }

        public async UniTask AddLog(string logText, bool useTime = false)
        {
            var log = OneLog.pool.TakeOut();
            var dataTime = DateTime.Now;
            string time = useTime ? $"{dataTime.Hour:D2}:{dataTime.Minute:D2}:{dataTime.Second:D2}:  " : null;
            log.value.text = $"{time}{logText}";//.Replace(' ','\u00A0');//<nobr><space=float>
            await UniTask.Yield();
            view.ScrollRect.verticalNormalizedPosition = 0;
        }
        public void AddLog(CombatContext context, bool useTime = false)
        {
            var log = OneLog.pool.TakeOut();
            AddLog(context.LogBuilder.ToString(), useTime);
            foreach (var data in context.ByAgentDataList)
                AddLog(data.LogBuilder.ToString(), useTime);
        }

        public void ClearLog()
        {
            while (OneLog.oneLogQueue.Count > 0)
            {
                OneLog.pool.PutIn(OneLog.oneLogQueue.Dequeue());
            }
        }
    }
}

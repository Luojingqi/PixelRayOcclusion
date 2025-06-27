using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using static PRO.Console.ConsolePanelV;
namespace PRO.Console
{
    public class ConsolePanelC : UIControllerBase, ITime_Start
    {
        public override UIViewBase View => view;
        private ConsolePanelV view = new ConsolePanelV();

        public bool useTime = true;
        public override void Init(string name)
        {
            base.Init(name);

            OneLog.InitPool(view.OneLogPrefab, this);
            view.InputField.onEndEdit.AddListener(InputFieldOnEndEdit);

            view.ClearButton.onClick.AddListener(ClearLog);
            view.CloseButton.onClick.AddListener(() => { gameObject.SetActive(false); });
            view.TimeButton.onClick.AddListener(() => { useTime = !useTime; Refresh(); });
            GameMainUIC.Inst.UpdateAction += () => { if (Input.GetKeyDown(KeyCode.Slash)) gameObject.SetActive(true); };

            Console.Init(this);
        }

        public void InputFieldOnEndEdit(string value)
        {
            if (value.Length <= 0 || value[0] != '/') return;
            string[] instructions = value.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (var instruction in instructions)
            {
                if (instruction.Length <= 0 || instruction[0] != '/') continue;
                string[] datas = instruction.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (Console.MethodInfoDic.TryGetValue(datas[0], out Func<string[], string> action))
                {
                    string retLog = action.Invoke(datas);
                    if (retLog != null) AddLog(retLog);
                }
                else
                {
                    AddLog($"Œ¥’“µΩ¥À√¸¡Ó£∫{datas[0]}");
                }
            }
        }

        private async UniTask AddLogAsync(string logText)
        {
            var log = OneLog.pool.TakeOut();
            var dataTime = DateTime.Now;
            log.time = $"{dataTime.Hour:D2}:{dataTime.Minute:D2}:{dataTime.Second:D2}:  ";
            log.content = logText;
            log.value.text = $"{(useTime ? log.time : null)}{log.content}";//.Replace(' ','\u00A0');//<nobr><space=float>
            await UniTask.Yield();
            view.ScrollRect.verticalNormalizedPosition = 0;
        }
        public void AddLog(string logText) => AddLogAsync(logText);
        public void ClearLog()
        {
            while (OneLog.queue.Count > 0)
            {
                OneLog.pool.PutIn(OneLog.queue.Dequeue());
            }
        }

        public void Refresh()
        {
            foreach (var oneLog in OneLog.queue)
            {
                oneLog.value.text = $"{(useTime ? oneLog.time : null)}{oneLog.content}";
            }
        }
        public void TimeStart()
        {
            Init("");
        }
    }
}

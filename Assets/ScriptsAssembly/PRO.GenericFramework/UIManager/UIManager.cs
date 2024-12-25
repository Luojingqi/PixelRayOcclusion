using Cysharp.Threading.Tasks;
using PRO.Disk;
using PRO.Tool;
using System.Collections.Generic;
using UnityEngine;
namespace PRO
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Inst { get; private set; }
        public void Awake() => Inst = this;

        /// <summary>
        /// 从UIConfigs.Json文件加载到的UI配置字典
        /// </summary>
        private Dictionary<string, UIConfig> UIConfigsDic = new Dictionary<string, UIConfig>();

        /// <summary>
        /// 当前在场景中的UI
        /// </summary>
        private Dictionary<string, UIControllerBase> SceneUI = new Dictionary<string, UIControllerBase>();
        private UIControllerBase nowUI;
        /// <summary>
        /// UI反切栈
        /// </summary>
        private Stack<UIControllerBase> UIStack = new Stack<UIControllerBase>();

        /// <summary>
        /// UI放此物体下边
        /// </summary>
        private Transform UIParent;

        /// <summary>
        /// 每打开一个面板，数值加一，并设置UICanvas的SortOrder
        /// </summary>
        private int SortOrder;
        public void Init()
        {
            LoadUIConfig();
        }

        private void LoadUIConfig()
        {
            UIConfigsDic.Clear();
            JsonTool.LoadText(Application.streamingAssetsPath + @"\Json\UIConfig.json", out string uiConfigTaxt);
            var array = JsonTool.ToObject<UIConfig[]>(uiConfigTaxt);
            foreach (var item in array)
                UIConfigsDic.Add(item.Name, item);
            if (UIParent == null)
                new GameObject().name = "UIPool";
            UIParent = GameObject.Find("UIPool").transform;
            SortOrder = 0;
        }
        private UIConfig GetUIConfig(string uiName)
        {
            UIConfigsDic.TryGetValue(uiName, out UIConfig uiConfig);
            return uiConfig;
        }


        /// <summary>
        /// 打开一个UI并执行UI开启事件
        /// </summary>
        /// <param name="uiName"></param>
        /// <param name="nowUI">当前的界面 用于记录下一个界面是否反切回当前界面</param>
        /// <returns></returns>
        public async UniTask<UIControllerBase> OpenUI(string uiName)
        {
            UIConfig uiConfig = GetUIConfig(uiName);
            if (SceneUI.TryGetValue(uiName, out UIControllerBase openUI) == false)
            {
                openUI = await LoadUI(uiConfig);
                openUI.transform.SetParent(UIParent);
                openUI.transform.localPosition = Vector3.zero;
            }
            openUI.Open();
            SortOrder++;
            openUI.View.canvas.sortingOrder = SortOrder;

            //if (openUI.NowUseLanguage != LanguageManager.Inst.NowUseLanguage)
            //{
            //    LanguageManager.Inst.SwitchLanguage(openUI.transform, LanguageManager.Inst.NowUseLanguage);
            //    openUI.NowUseLanguage = LanguageManager.Inst.NowUseLanguage;
            //}
            if (uiConfig.ShowMode && nowUI != null)
                UIStack.Push(nowUI);
            nowUI = openUI;
            return openUI;
        }


        /// <summary>
        /// 关闭/销毁UI
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="IsOpenShowMode">是否判断是否需要反切</param>
        public void CloseUI(UIControllerBase ui)
        {
            ui.Close();
            UIConfig uiConfig = GetUIConfig(ui.UIName);

            SceneUI.Remove(ui.UIName);
            Log.Print("销毁" + ui.name);
            GameObject.Destroy(ui.gameObject);

            if (uiConfig.ShowMode)
                UIStack.Pop().Open();
        }

        /// <summary>
        /// 从当前UI切换到下一个UI
        /// </summary>
        /// <param name="nowUI"></param>
        /// <param name="uiType"></param>
        /// <param name="mode"></param>
        public async UniTask<UIControllerBase> SwitchUI(string nextUI, SwitchUIMode mode)
        {
            UIControllerBase ret = null;
            switch (mode)
            {
                case SwitchUIMode.HideOldUI:
                    nowUI.Hide();
                    ret = await OpenUI(nextUI);
                    break;
                case SwitchUIMode.PauseOldUI:
                    nowUI.Pause();
                    ret = await OpenUI(nextUI);
                    break;
                case SwitchUIMode.CloseOldUI:
                    nowUI.Close();
                    ret = await OpenUI(nextUI);
                    break;
            }
            return ret;
        }


        /// <summary>
        /// UI工厂，通过配置文件初始化
        /// </summary>
        /// <param name="uiName"></param>
        /// <returns></returns>
        private async UniTask<UIControllerBase> LoadUI(UIConfig uiConfig)
        {
            string[] strs = uiConfig.Path.Split('|');
            GameObject uiClone = GameObject.Instantiate(await AssetManager.LoadAsync_A<GameObject>(strs[0], strs[1]));
            UIControllerBase ui = uiClone.GetComponent<UIControllerBase>();
            ui.Init(uiConfig.Name);
            return ui;
        }
    }
}
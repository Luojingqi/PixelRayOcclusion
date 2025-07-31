using PRO;
using System;
using UnityEngine;
namespace PRO
{
    public class RoleInfoPanelC : UIChildControllerBase
    {
        public static RoleInfoPanelC Inst;
        public override UIChildViewBase View => view;
        private RoleInfoPanelV view = new RoleInfoPanelV();

        public override void Init()
        {
            base.Init();
            //InitAction();
            Inst = this;
            AddChildUI(view.BuffInfoListC);
        }
        #region 值改变委托（改变ui）
        //private Action<RoleInfo> 血量Changed;
        //private Action<RoleInfo> 护甲Changed;
        //private Action<RoleInfo> 闪避Changed;
        //private Action<RoleInfo> 命中Changed;
        //private Action<RoleInfo> 暴击Changed;
        //private Action<RoleInfo> 行动点Changed;
        //private void InitAction()
        //{
        //    血量Changed = (info) => view.Info_血.Value.text = $"{info.血量.Value}/{info.最大血量.Value}";
        //    护甲Changed = (info) => view.Info_护甲.Value.text = $"{info.临时护甲.Value}";
        //    闪避Changed = (info) => view.Info_闪避.Value.text = $"{info.闪避率.Value * 100:F1}%";
        //    命中Changed = (info) => view.Info_命中.Value.text = $"{info.命中率.Value * 100:F1}%";
        //    暴击Changed = (info) => view.Info_暴击.Value.text = $"{info.暴击率.Value * 100:F1}%";
        //    行动点Changed = (info) => Set行动点(info.行动点.Value, info.行动点上限.Value);
        //}
        #endregion
        public void SetRole(Role role)
        {
            //#region 一大堆属性的数值绑定
            //role.Info.最大血量.ValueChange = 血量Changed;
            //role.Info.血量.ValueChange = 血量Changed;
            //role.Info.临时护甲.ValueChange = 护甲Changed;
            //role.Info.闪避率.ValueChange = 闪避Changed;
            //role.Info.命中率.ValueChange = 命中Changed;
            //role.Info.暴击率.ValueChange = 暴击Changed;
            //role.Info.行动点.ValueChange = 行动点Changed;
            //role.Info.行动点上限.ValueChange = 行动点Changed;
            ////挨个触发事件
            //RoleInfo.CloneValue(role.Info, role.Info);
            //#endregion
            //view.Icon.sprite = role.Icon;
            //view.BuffInfoListC.SetRole(role);
        }
        public BuffInfoListC BuffInfoListC => view.BuffInfoListC;
        public void Set行动点(int 可用行动点, int 全部行动点) => view.行动点Panel.SetValue(可用行动点, 全部行动点);

    }
}
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
        #region ֵ�ı�ί�У��ı�ui��
        //private Action<RoleInfo> Ѫ��Changed;
        //private Action<RoleInfo> ����Changed;
        //private Action<RoleInfo> ����Changed;
        //private Action<RoleInfo> ����Changed;
        //private Action<RoleInfo> ����Changed;
        //private Action<RoleInfo> �ж���Changed;
        //private void InitAction()
        //{
        //    Ѫ��Changed = (info) => view.Info_Ѫ.Value.text = $"{info.Ѫ��.Value}/{info.���Ѫ��.Value}";
        //    ����Changed = (info) => view.Info_����.Value.text = $"{info.��ʱ����.Value}";
        //    ����Changed = (info) => view.Info_����.Value.text = $"{info.������.Value * 100:F1}%";
        //    ����Changed = (info) => view.Info_����.Value.text = $"{info.������.Value * 100:F1}%";
        //    ����Changed = (info) => view.Info_����.Value.text = $"{info.������.Value * 100:F1}%";
        //    �ж���Changed = (info) => Set�ж���(info.�ж���.Value, info.�ж�������.Value);
        //}
        #endregion
        public void SetRole(Role role)
        {
            //#region һ������Ե���ֵ��
            //role.Info.���Ѫ��.ValueChange = Ѫ��Changed;
            //role.Info.Ѫ��.ValueChange = Ѫ��Changed;
            //role.Info.��ʱ����.ValueChange = ����Changed;
            //role.Info.������.ValueChange = ����Changed;
            //role.Info.������.ValueChange = ����Changed;
            //role.Info.������.ValueChange = ����Changed;
            //role.Info.�ж���.ValueChange = �ж���Changed;
            //role.Info.�ж�������.ValueChange = �ж���Changed;
            ////���������¼�
            //RoleInfo.CloneValue(role.Info, role.Info);
            //#endregion
            //view.Icon.sprite = role.Icon;
            //view.BuffInfoListC.SetRole(role);
        }
        public BuffInfoListC BuffInfoListC => view.BuffInfoListC;
        public void Set�ж���(int �����ж���, int ȫ���ж���) => view.�ж���Panel.SetValue(�����ж���, ȫ���ж���);

    }
}
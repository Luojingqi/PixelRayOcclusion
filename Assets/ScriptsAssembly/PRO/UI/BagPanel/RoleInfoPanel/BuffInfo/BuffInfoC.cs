using PRO.Buff.Base;
using PRO.Buff.Base.IBuff;
using PRO;
using UnityEngine.EventSystems;

namespace PRO
{
    /// <summary>
    /// buff信息展示ui
    /// </summary>
    public class BuffInfoC : UIChildControllerBase, IPointerEnterHandler, IPointerExitHandler
    {
        public override UIChildViewBase View => view;
        private BuffInfoV view = new BuffInfoV();

        private IBuffBase buff;
        public void SetBuff(IBuffBase buff)
        {
            this.buff = buff;
            BuffUpdate();
        }
        public void BuffUpdate()
        {
            switch (buff)
            {
                case IBuff_叠加 i:
                    {
                        view.NumText.text = $"{i.StackNumber}";
                        break;
                    }
                case IBuff_回合 i:
                    {
                        view.NumText.text = $"{i.Round}";
                        break;
                    }
                case IBuff_比例 i:
                    {
                        view.NumText.text = $"{i.Proportion * 100:F0}";
                        break;
                    }
            }
        }
        public void Clear()
        {
            buff = null;
            view.NumText.text = null;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            BuffInfoPanelC.Inst.Open();
            BuffInfoPanelC.Inst.Name.text = buff.Name;
            BuffInfoPanelC.Inst.Info.text = (buff as IBuff_UI).Info;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            BuffInfoPanelC.Inst.Close();
        }
    }
}

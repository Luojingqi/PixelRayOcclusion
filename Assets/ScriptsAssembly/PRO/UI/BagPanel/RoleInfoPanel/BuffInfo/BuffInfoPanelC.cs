using PRO;
using TMPro;

namespace PRO
{
    public class BuffInfoPanelC : UIChildControllerBase
    {
        public override UIChildViewBase View => view;
        private BuffInfoPanelV view = new BuffInfoPanelV();

        public static BuffInfoPanelC Inst { get; private set; }
        public override void Init()
        {
            base.Init();
            Inst = this;
        }

        public void Open()
        {
            gameObject.SetActive(true);
        }
        public void Close()
        {
            gameObject.SetActive(false);
            view.Name.text = null;
            view.Info.text = null;
        }
        public TMP_Text Name => view.Name;
        public TMP_Text Info => view.Info;
    }
}

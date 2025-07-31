using PRO;
using UnityEngine;
namespace PRO
{
    public class RoundPanelV : UIChildViewBase
    {

        public TurnImage afoot { get; private set; }
        public TurnImage wait { get; private set; }
        public Transform ImageNode { get; private set; }

        public override void Init(UIChildControllerBase controller)
        {
            base.Init(controller);
            afoot = transform.Find("TurnList/afoot").GetComponent<TurnImage>();
            wait = transform.Find("TurnList/wait").GetComponent<TurnImage>();
            ImageNode = transform.Find("TurnList");
        }
    }
}

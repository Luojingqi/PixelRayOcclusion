using PRO.TurnBased;
using UnityEngine;
using UnityEngine.UI;

namespace PRO
{
    public class TurnImage : MonoBehaviour
    {
        public Image Icon { get; private set; }
        public Button Button { get; private set; }
        public void Init()
        {
            Icon = GetComponent<Image>();
            Button = GetComponent<Button>();
        }
        private TurnFSM turn;
        public void SetTurn(TurnFSM turn)
        {
            this.turn = turn;
        }

        public void Clear()
        {
            turn = null;
            Button.onClick.RemoveAllListeners();
        }
    }
}

using PRO.Tool;
using PRO.TurnBased;
using System.Collections.Generic;
namespace PRO
{
    public class RoundPanelC : UIChildControllerBase
    {
        #region TurnImageµÄ¶ÔÏó³Ø
        private GameObjectPool<TurnImage> AfootPool;
        private GameObjectPool<TurnImage> WaitPool;
        #endregion

        public override UIChildViewBase View => view;

        private RoundPanelV view = new RoundPanelV();

        public override void Init()
        {
            base.Init();

            AfootPool = new GameObjectPool<TurnImage>(view.afoot, view.ImageNode.parent);
            WaitPool = new GameObjectPool<TurnImage>(view.wait, view.ImageNode.parent);
            AfootPool.CreateEvent += t => t.Init();
            WaitPool.CreateEvent += t => t.Init();
        }
        public List<TurnImage> TurnImageList = new List<TurnImage>();
        public void SetTurn(List<TurnFSM> list, int nowTurn)
        {
            Clear();
            for (int i = 0; i < list.Count; i++)
            {
                var turn = list[i];
                TurnImage ti = null;
                if (i == nowTurn)
                    ti = AfootPool.TakeOut();
                else

                    ti = WaitPool.TakeOut();
                ti.transform.SetParent(view.ImageNode);
                ti.Icon.sprite = turn.Agent.Icon;
                ti.SetTurn(turn);
                TurnImageList.Add(ti);
            }
        }

        public void Clear()
        {
            for (int i = 0; i < TurnImageList.Count; i++)
            {
                var ti = TurnImageList[i];
                ti.Clear();
                if (ti.name[0] == 'a') AfootPool.PutIn(ti);
                else if (ti.name[0] == 'w') WaitPool.PutIn(ti);
            }
            TurnImageList.Clear();
        }
    }
}

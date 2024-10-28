using PRO.Tool;
using System.Collections.Generic;
using System.IO;

namespace PRO.SceneEditor
{
    public class ElementViewPanelC : UIChildControllerBase
    {
        public override UIChildViewBase View => view;
        private ElementViewPanelV view = new ElementViewPanelV();

        public override UIChildModelBase Model => model;
        private ElementViewPanelM model = new ElementViewPanelM();

        public static ElementViewPanelC Inst { get; private set; }

        private GameObjectPool<ElementC> ElementPool;
        public override void Init()
        {
            base.Init();
            Inst = this;

            ElementPool = new GameObjectPool<ElementC>(
                view.Element.gameObject,
                view.Content,
                50, true);
            ElementPool.CreateEventT += (g, t) => t.Init();
        }
        private List<ElementC> showElementList = new List<ElementC>();
        public void ShowFiles(FileInfo[] infos)
        {
            Clear();
            for (int i = 0; i < infos.Length; i++)
            {
                ElementC element = TakeOut();
                element.SetFileInfo(infos[i]);
                showElementList.Add(element);
            }
        }

        private void Clear()
        {
            for (int i = 0; i < showElementList.Count; i++)
                PutIn(showElementList[i]);
            showElementList.Clear();
        }

        private ElementC TakeOut()
        {
            return ElementPool.TakeOutT();
        }

        private void PutIn(ElementC element)
        {
            element.Clear();
            ElementPool.PutIn(element.gameObject);
        }

    }
}

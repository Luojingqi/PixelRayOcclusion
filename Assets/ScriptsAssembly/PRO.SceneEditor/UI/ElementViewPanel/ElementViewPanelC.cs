using PRO.Tool;
using PRO.Tool.Serialize.IO;
using PRO.Tool.Serialize.Json;
using System.Collections.Generic;
using System.IO;

namespace PRO.SceneEditor
{
    public class ElementViewPanelC : UIChildControllerBase
    {
        public override UIChildViewBase View => view;
        private ElementViewPanelV view = new ElementViewPanelV();


        public static ElementViewPanelC Inst { get; private set; }

        private GameObjectPool<ElementC> ElementPool;
        public override void Init()
        {
            base.Init();
            Inst = this;

            ElementPool = new GameObjectPool<ElementC>(view.Element.gameObject, view.Content);
            ElementPool.CreateEventT += (g, t) => t.Init();
        }
        private List<ElementC> showElementList = new List<ElementC>();
        public void ShowFiles(FileInfo[] infos)
        {
            Clear();
            for (int i = 0; i < infos.Length; i++)
            {
                Element_Disk entity = TryGetEntity(infos[i]);
                if (entity == null) continue;

                ElementC element = TakeOut();
                element.SetEntity(infos[i], entity);
                showElementList.Add(element);
            }
        }

        private Element_Disk TryGetEntity(FileInfo info)
        {
            if (info.Extension != ".json") return null;
            if (IOTool.LoadText(info.FullName, out string text) == false) return null;
            else
            {
                Element_Disk entity = JsonTool.ToObject<Element_Disk>(text);
                return entity;
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

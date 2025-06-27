using PRO;
using UnityEngine;
using UnityEngine.UI;
using static PRO.BottomBagM;

namespace PRO
{
    public class BottomBagV : UIChildViewBase
    {
        public RectTransform BagScrollViewRect { get; private set; }
        public GridLayoutGroup GridLayout { get; private set; }

        public Button EnlargeButton { get; private set; }
        public Button NarrowButton { get; private set; }

        public GridObject GridObject { get; private set; }

        public override void Init(UIChildControllerBase controller)
        {
            base.Init(controller);
            BagScrollViewRect = transform.Find("BagScrollView").GetComponent<RectTransform>();
            GridLayout = transform.Find("BagScrollView/Viewport/Content").GetComponent<GridLayoutGroup>();
            EnlargeButton = transform.Find("TopInfo/EnlargeButton").GetComponent<Button>();
            NarrowButton = transform.Find("TopInfo/NarrowButton").GetComponent<Button>();
            GridObject = transform.Find("BagScrollView/Viewport/Content/GridObject").gameObject.AddComponent<GridObject>();
        }
        /// <summary>
        /// 可以容纳的最大列
        /// </summary>
        public int MaxColumn
        {
            get
            {
                return (int)((rectTransform.rect.size.x - GridLayout.padding.left - GridLayout.padding.right + GridLayout.spacing.x)
                    / (GridLayout.cellSize.x + GridLayout.spacing.x));
            }
        }
        public int Row
        {
            get
            {
                return (int)((BagScrollViewRect.rect.size.y - GridLayout.padding.top - GridLayout.padding.bottom + GridLayout.spacing.y)
                    / (GridLayout.cellSize.y + GridLayout.spacing.y));
            }
            set
            {
                int change = value - Row;
                rectTransform.sizeDelta = new Vector2(rectTransform.rect.size.x,
                    rectTransform.rect.size.y + change * (GridLayout.cellSize.y + GridLayout.spacing.y));
            }
        }
    }
}

using PRO;
using UnityEngine.EventSystems;
using UnityEngine;
using PRO.Tool;
using System;
using TMPro;
using UnityEngine.UI;

namespace PRO
{
    public class BottomBagM
    {
        public class GridObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
        {
            /// 父物体
            /// </summary>
            private Transform Content { get; set; }
            private RectTransform rectTransform { get; set; }
            public Image Icon { get; private set; }
            public TMP_Text Name { get; private set; }
            public TMP_Text Number { get; private set; }
            public Button Button { get; private set; }

            /// <summary>
            /// 自己被点击的事件
            /// </summary>
            public event Action<GridObject> ImageOnClick;

            public void Init()
            {
                Content = transform.parent;
                rectTransform = transform.GetComponent<RectTransform>();
                Icon = transform.Find("Icon").GetComponent<Image>();
                Name = transform.Find("Name").GetComponent<TMP_Text>();
                Number = transform.Find("Number").GetComponent<TMP_Text>();
                Button = transform.GetComponent<Button>();
                Button.onClick.AddListener(() => { ImageOnClick?.Invoke(this); });
            }


            public void OnReset()
            {
                Icon.sprite = null;
                Name.text = null;
                Number.text = null;
                ImageOnClick = null;
            }


            public bool CanDrag = false;
            #region 拖拽控制
            private Vector3 dirV3;
            public void OnBeginDrag(PointerEventData eventData)
            {
                if (!CanDrag) return;
                transform.GetComponent<CanvasGroup>().blocksRaycasts = false;
                //解除父物体
                transform.SetParent(Content.parent.parent);
                if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, eventData.enterEventCamera, out Vector3 globalMousePos))
                {
                    //计算UI和指针之间的位置偏移量
                    dirV3 = rectTransform.position - globalMousePos;
                }

            }
            public void OnDrag(PointerEventData eventData)
            {
                if (!CanDrag) return;
                //跟随鼠标移动
                if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, eventData.enterEventCamera, out Vector3 pos))
                {
                    transform.position = pos + dirV3;
                }
            }
            public void OnEndDrag(PointerEventData eventData)
            {
                if (!CanDrag) return;
                transform.SetParent(Content);
                transform.GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
            #endregion

            public void OnPointerEnter(PointerEventData eventData)
            {

            }

            public void OnPointerExit(PointerEventData eventData)
            {

            }

            #region 静态对象池
            public static GameObjectPool<GridObject> Pool;
            public static void InitPool(GridObject grid)
            {
                Pool = new GameObjectPool<GridObject>(grid, grid.transform.parent);
                Pool.CreateEvent += t => { t.Init(); };
                Pool.PutInEvent += t => { t.OnReset(); };
            }
            #endregion
        }

        public class GridObjectGroup
        {
            public int Index;
            public GridObject[] Grids;

            public GridObjectGroup(int index, int valueNum)
            {
                Index = index;
                Grids = new GridObject[valueNum];
                for (int i = 0; i < valueNum; i++)
                {
                    Grids[i] = GridObject.Pool.TakeOut();
                }
            }
        }
    }
}

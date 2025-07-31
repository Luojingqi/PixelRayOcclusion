//using PRO.Tool;
//using System;
//using TMPro;
//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.GridUI;

//namespace PRO
//{
//    public abstract class BagGridBase : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,IPointerEnterHandler,IPointerExitHandler
//    {
//        /// <summary>
//        /// 父物体
//        /// </summary>
//        private Transform Content { get; set; }
//        private RectTransform rectTransform { get; set; }
//        public Image PropImage { get; private set; }
//        public TMP_Text PropName { get; private set; }
//        public TMP_Text PropNumber { get; private set; }
//        public Button Button { get; private set; }

//        /// <summary>
//        /// 自己被点击的事件
//        /// </summary>
//        public event Action<BagGridBase> ImageOnClick;

//        public void Init()
//        {
//            Content = transform.parent;
//            rectTransform = transform.GetComponent<RectTransform>();
//            PropImage = transform.Find("PropImage").GetComponent<Image>();
//            PropName = transform.Find("PropName").GetComponent<TMP_Text>();
//            PropNumber = transform.Find("PropNumber").GetComponent<TMP_Text>();
//            Button = transform.GetComponent<Button>();
//            Button.onClick.AddListener(() => { ImageOnClick?.Invoke(this); });
//        }


//        public void OnReset()
//        {
//            PropImage.sprite = null;
//            PropName.content = null;
//            PropNumber.content = null;
//            ImageOnClick = null;
//        }

//        public void PropNumberChanges(int number)
//        {
//            this.PropNumber.content = number.ToString();
//        }


//        public bool CanDrag = false;
//        #region 拖拽控制
//        private Vector3 dirV3;
//        public void OnBeginDrag(PointerEventData eventData)
//        {
//            if (!CanDrag) return;
//            transform.GetComponent<CanvasGroup>().blocksRaycasts = false;
//            //解除父物体
//            transform.SetParent(Content.parent.parent);
//            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, eventData.enterEventCamera, out Vector3 globalMousePos))
//            {
//                //计算UI和指针之间的位置偏移量
//                dirV3 = rectTransform.position - globalMousePos;
//            }

//        }
//        public void OnDrag(PointerEventData eventData)
//        {
//            if (!CanDrag) return;
//            //跟随鼠标移动
//            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, eventData.enterEventCamera, out Vector3 pos))
//            {
//                transform.position = pos + dirV3;
//            }
//        }
//        public void OnEndDrag(PointerEventData eventData)
//        {
//            if (!CanDrag) return;
//            transform.SetParent(Content);
//            transform.GetComponent<CanvasGroup>().blocksRaycasts = true;
//        }
//        #endregion

//        #region 静态对象池
//        /// <summary>
//        /// 道具格子的对象池
//        /// </summary>
//        private static GameObjectPool<PropGrid> PropGridPool;
//        public static PropGrid TakeOut()
//        {
//            return PropGridPool.TakeOutT();
//        }
//        public static void PutIn(PropGrid propGrid)
//        {
//            propGrid.OnReset();
//            PropGridPool.PutIn(propGrid.gameObject);
//        }
//        public static void InitPool(PropGrid propGrid)
//        {
//            PropGridPool = new GameObjectPool<PropGrid>(propGrid.gameObject, propGrid.transform.parent);
//            PropGridPool.CreateEventT += (g, t) => { t.Init(); };
//        }

//        public void OnPointerEnter(PointerEventData eventData)
//        {
//            throw new NotImplementedException();
//        }

//        #endregion
//    }
//}
//using PRO.Tool;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.GridUI;

//namespace PRO
//{
//    public class PropBagWindow : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
//    {
//        private RectTransform rectTransform;
//        /// <summary>
//        /// 关闭按钮
//        /// </summary>
//        private Button CloseButton;
//        /// <summary>
//        /// 一个道具信息方框
//        /// </summary>
//        private Transform APropInfoImage;
//        /// <summary>
//        /// 表格父物体
//        /// </summary>
//        private Transform Content;
//        private TMP_Dropdown BagTypeDropdown;

//        GameObjectPool<PropGrid> pool;
//        public void Init()
//        {
//            rectTransform = transform.GetComponent<RectTransform>();
//            CloseButton = transform.Find("TopPanel/CloseButton").GetComponent<Button>();
//            APropInfoImage = transform.Find("Viewport/Content/PropGrid");
//            Content = transform.Find("Viewport/Content");
//            BagTypeDropdown = transform.Find("TopPanel/BagTypeDropdown").GetComponent<TMP_Dropdown>();
//            BagTypeDropdown.onValueChanged.AddListener((index) => { SwitchBagType((PropType)index); });
//            CloseButton.onClick.AddListener(Close);

//            pool = new GameObjectPool<PropGrid>(APropInfoImage.gameObject, Content);
//            pool.CreateEventT += (go, t) => { t.Init(); };
//            pool.PutInEventT += (go, t) => { t.OnReset(); };
//            //pool.TakeOutEventT += (go, t) => { PropInfoImageList.Add(t); t.ImageOnClick = this.ImageOnClick; };
//        }


//        public Action<PropGrid> ImageOnClick;
//        public void Open(Action<PropGrid> ImageOnClick)
//        {
//            gameObject.SetActive(true);
//            this.ImageOnClick = ImageOnClick;
//        }
//        public void Close()
//        {
//            gameObject.SetActive(false);
//            foreach (var item in PropInfoImageList)
//                pool.PutIn(item.gameObject);
//            PropInfoImageList.Clear();
//        }



//        /// <summary>
//        /// 道具方块图片的集合
//        /// </summary>
//        public HashSet<PropGrid> PropInfoImageList = new HashSet<PropGrid>();


//        ///// <summary>
//        ///// 设置每个格子显示的信息
//        ///// </summary>
//        ///// <param Name="typeInfo"></param>
//        //public void UpdateImageInfo(PropGrid propGo)
//        //{
//        //    switch (propGo.Prop_Disk.propType)
//        //    {
//        //        case Bag_Disk.PropType.BuildCube: BuildCubeSetInfo(propGo); break;
//        //        case Bag_Disk.PropType.BuildPrefab: BuildPrefabSetInfo(propGo); break;
//        //    }
//        //}
//        //private void BuildCubeSetInfo(PropGrid propGo)
//        //{
//        //    Bag_Disk.BuildCubeInfo Prop_Disk = (Bag_Disk.BuildCubeInfo)propGo.Prop_Disk;
//        //    // propGo.PropName.content = CubeTypeInfoManager.cubeMaterialInfos[Prop_Disk.MaterialType].MaterialType + CubeTypeInfoManager.cubeShapeInfos[Prop_Disk.ShapeType].ShapeType;
//        //    propGo.PropNumber.content = Prop_Disk.PropNumber.ToString();
//        //}

//        //private void BuildPrefabSetInfo(PropGrid propGo)
//        //{
//        //    Bag_Disk.BuildPrefabInfo Prop_Disk = (Bag_Disk.BuildPrefabInfo)propGo.Prop_Disk;
//        //    propGo.PropName.content = Prop_Disk.PrefabType;
//        //    propGo.PropNumber.content = Prop_Disk.PropNumber.ToString();
//        //}




//        private void SwitchBagType(PropType type)
//        {
//            //this.type = type;
//            //Close();
//            //Open(ImageOnClick);
//            //switch (type)
//            //{
//            //    case PropType.Null: break;
//            //    case PropType.All:
//            //        foreach (var typeInfo in GameMainData.Inst.AllPropInfo.infos)
//            //        {
//            //            PropGrid propGo = pool.TakeOut().GetComponent<PropGrid>();
//            //            propGo.Prop_Disk = typeInfo;
//            //            UpdateImageInfo(propGo);
//            //        }
//            //        break;
//            //    case PropType.BuildCube:
//            //        break;
//            //}

//        }

//        public PropType type;
//        public enum PropType
//        {
//            /// <summary>
//            /// 空背包
//            /// </summary>
//            Null,
//            /// <summary>
//            /// 所有道具
//            /// </summary>
//            All,
//            /// <summary>
//            /// 建造方块
//            /// </summary>
//            BuildCube,
//        }


//        private Vector3 dirV3;
//        public void OnBeginDrag(PointerEventData eventData)
//        {
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
//            //跟随鼠标移动
//            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, eventData.enterEventCamera, out Vector3 pos))
//            {
//                transform.position = pos + dirV3;
//            }
//        }
//        public void OnEndDrag(PointerEventData eventData)
//        {
//            transform.SetParent(Content);
//            transform.GetComponent<CanvasGroup>().blocksRaycasts = true;
//        }

//    }
//}
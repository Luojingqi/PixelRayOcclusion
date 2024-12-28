using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace PRO.SkillEditor
{
    /// <summary>
    /// 一个轨道的基类
    /// </summary>
    internal abstract class TrackBase
    {
        public int trackIndex = -1;
        public VisualElement View { get; private set; }
        public OneTrackHeading Heading { get; private set; }
        public TrackBase(Track_Disk track_Disk)
        {
            View = new VisualElement();
            View.name = "OneTrackBase";
            Heading = new OneTrackHeading(this);

            #region 样式
            #region 轨道外边框颜色
            var borderColor = new StyleColor(Color.black);
            View.style.borderLeftColor = borderColor;
            View.style.borderBottomColor = borderColor;
            View.style.borderTopColor = borderColor;
            View.style.borderRightColor = borderColor;
            #endregion
            #region 轨道外边框宽度
            var borderLength = new StyleFloat(1);
            View.style.borderBottomWidth = borderLength;
            View.style.borderTopWidth = borderLength;
            #endregion
            View.style.flexDirection = FlexDirection.Row;   //子物体排序方向
            View.style.backgroundColor = new StyleColor(Color.gray / 3 * 2);    //背景色
            View.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            #endregion
            #region 拖拽资源事件绑定
            View.RegisterCallback<DragUpdatedEvent>(DragAssetUpdate);
            View.RegisterCallback<DragExitedEvent>((evt) =>
            {
                DragAssetExit(evt, DragAndDrop.objectReferences);
            });
            #endregion
            #region 拖拽切片事件绑定
            View.RegisterCallback<PointerDownEvent>(DragSliceMoveEnter);
            View.RegisterCallback<PointerMoveEvent>(DragSliceMoveUpdate);
            View.RegisterCallback<PointerUpEvent>(DragSliceMoveExit);
            #endregion
            #region 右键事件
            View.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                evt.menu.AppendAction("添加空切片", _ => AddSlice(new NullSlice(new NullSlice_Disk())));
            }));
            #endregion

            SetHeight(60);
            this.track_Disk = track_Disk;
            HashSet<SliceBase_Disk> hashSet = new HashSet<SliceBase_Disk>();
            foreach (var sliceDisk in track_Disk.SlickList)
            {
                if (hashSet.Contains(sliceDisk) == false)
                {
                    hashSet.Add(sliceDisk);
                    if (sliceDisk is NullSlice_Disk disk)
                    {
                        AddSlice(new NullSlice(disk));
                        continue;
                    }
                    ForeachSliceDiskToSlice(sliceDisk);
                }
            }
        }
        /// <summary>
        /// 请在此函数中检查切片数据的类型并添加对应类型的切片到轨道
        /// </summary>
        /// <param name="sliceDisk"></param>
        protected abstract void ForeachSliceDiskToSlice(SliceBase_Disk sliceDisk);
        #region 拖拽资源进入
        /// <summary>
        /// 检查拖拽进入的资源类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected abstract bool DragAssetTypeCheck(System.Type type);

        protected abstract void DragAssetExit(DragExitedEvent evt, object[] objects);


        private void DragAssetUpdate(DragUpdatedEvent evt)
        {
            if (DragAssetTypeCheck(DragAndDrop.objectReferences[0].GetType()))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            }
        }
        #endregion

        public List<SliceBase> sliceList = new List<SliceBase>();
        public Track_Disk track_Disk;

        #region 设置轨道高与切片宽度
        /// <summary>
        /// 设置整个轨道的高，会同步更新所有的切片的高度
        /// </summary>
        public virtual void SetHeight(int pixel)
        {
            View.style.height = new StyleLength(new Length(pixel, LengthUnit.Pixel));
            Heading.View.style.height = new StyleLength(new Length(pixel, LengthUnit.Pixel));
            foreach (var slice in sliceList)
            {
                slice.SetHeight(pixel);
            }
        }
        /// <summary>
        /// 设置每个切片的宽度，请不要使用此函数，设置时间轴的间距会同步更新
        /// </summary>
        public virtual void SetWidth(int pixel)
        {
            foreach (var slice in sliceList)
            {
                slice.SetWidth(pixel);
            }
        }
        #endregion

        #region 切片位置切换
        /// <summary>
        /// 添加一个新切片
        /// </summary>
        public void AddSlice(SliceBase slice)
        {
            slice.Track = this;
            if (slice.StartFrame == -1)
            {
                //说明此数据是新添加的
                slice.StartFrame = sliceList.Count;
                slice.FrameLength = 1;
                track_Disk.SlickList.Add(slice.DiskData);
            }
            else
            {
                slice.StartFrame = slice.StartFrame;
                slice.FrameLength = slice.FrameLength;
            }
            slice.SetHeight(View.style.height.value.value);
            View.Add(slice.View);
            for (int i = 0; i < slice.FrameLength; i++)
                sliceList.Add(slice);
        }

        public void RemoveSlice(int index)
        {
            if (index < 0 || index >= sliceList.Count) return;
            var slice = sliceList[index];
            for (int i = slice.StartFrame; i < slice.StartFrame + slice.FrameLength; i++)
            {
                sliceList.RemoveAt(slice.StartFrame);
            }
            Reset();
        }
        /// <summary>
        /// 交换两个切片
        /// </summary>
        public void SwapSlice(int index0, int index1)
        {
            if (index0 == index1) return;
            if (index0 < 0 || index0 >= sliceList.Count || index1 < 0 || index1 >= sliceList.Count) return;
            if (index0 > index1) { int temp = index0; index0 = index1; index1 = temp; }

            var slice0 = sliceList[index0];
            var slice1 = sliceList[index1];


            var tempSlice = sliceList[slice0.StartFrame];
            sliceList[slice0.StartFrame] = sliceList[slice1.StartFrame];
            sliceList[slice1.StartFrame] = tempSlice;

            Reset();
        }

        private void Reset()
        {
            HashSet<SliceBase> hash = new HashSet<SliceBase>();
            List<SliceBase> list = new List<SliceBase>();
            for (int i = 0; i < sliceList.Count; i++)
            {
                if (hash.Contains(sliceList[i]) == false)
                {
                    hash.Add(sliceList[i]);
                    list.Add(sliceList[i]);
                }
            }

            View.Clear();
            sliceList.Clear();
            track_Disk.SlickList.Clear();
            int startFrame = 0;

            for (int i = 0; i < list.Count; i++)
            {
                var slice = list[i];
                View.Add(slice.View);
                for (int f = 0; f < slice.FrameLength; f++)
                {
                    sliceList.Add(slice);
                    track_Disk.SlickList.Add(slice.DiskData);
                    if (f == 0 && slice.StartFrame != startFrame) slice.StartFrame = startFrame;
                    startFrame++;
                }
            }
        }

        public void SetSliceFrameLength(int index, int newLength)
        {
            if (index < 0 || index >= sliceList.Count) return;
            if (newLength <= 0) return;
            if (newLength > sliceList[index].FrameLength && sliceList.Count >= SkillEditorWindow.Inst.TimeScaleAxis.MaxFrame) return;

            var slice = sliceList[index];
            int oldLength = slice.FrameLength;
            if (slice.FrameLength != newLength)
                slice.FrameLength = newLength;
            if (oldLength == newLength) return;
            Reset();
        }
        #endregion

        #region 切片拖拽移动

        private SliceBase DragSelectSlice;
        private float DragXOffset;
        private SliceBase LocalPointToSlice(float x)
        {
            int index = (int)(x / SkillEditorWindow.Inst.TimeScaleAxis.Spacing);
            if (index < 0 || index >= sliceList.Count) return null;
            return sliceList[index];
        }

        private void DragSliceMoveEnter(PointerDownEvent evt)
        {
            DragSelectSlice = LocalPointToSlice(evt.localPosition.x);
            DragXOffset = evt.localPosition.x;
        }

        private void DragSliceMoveUpdate(PointerMoveEvent evt)
        {
            if (DragSelectSlice != null)
            {
                if (Mathf.Abs(evt.localPosition.x - DragXOffset) > SkillEditorWindow.Inst.TimeScaleAxis.Spacing)
                {
                    if (evt.localPosition.x > DragXOffset)
                        SwapSlice(DragSelectSlice.StartFrame, DragSelectSlice.StartFrame + DragSelectSlice.FrameLength);
                    else
                        SwapSlice(DragSelectSlice.StartFrame, DragSelectSlice.StartFrame - 1);
                    DragXOffset = evt.localPosition.x;

                }
            }
        }
        private void DragSliceMoveExit(PointerUpEvent evt)
        {
            DragSelectSlice = null;
        }

        #endregion
    }
}

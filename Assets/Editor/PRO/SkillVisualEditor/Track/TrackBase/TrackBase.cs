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
            View.style.position = Position.Relative;
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

            SetHeight(60);
            this.track_Disk = track_Disk;
            HashSet<Slice_DiskBase> hashSet = new HashSet<Slice_DiskBase>();
            sliceArray = new SliceBase[track_Disk.SlickArray.Length];
            for (int i = 0; i < track_Disk.SlickArray.Length; i++)
            {
                var sliceDisk = track_Disk.SlickArray[i];
                if (sliceDisk != null && hashSet.Contains(sliceDisk) == false)
                {
                    hashSet.Add(sliceDisk);
                    if (ForeachSliceDiskToSlice(sliceDisk) == false)
                    {
                        //  AddSlice(new NullSlice(new NullSlice_Disk() { name = $"未识别的切片类型", startFrame = sliceDisk.startFrame, frameLength = sliceDisk.frameLength, enable = false }));
                        Debug.LogError($"轨道有未识别的切片类型");
                    }
                }
            }
        }
        #region 创建切片
        public T CreateSlice<T>(Slice_DiskBase diskData) where T : SliceBase
        {
            var slice = ScriptableObject.CreateInstance<T>();
            slice.Init(diskData);
            return slice;
        }
        public SliceBase CreateSlice(Type type, Slice_DiskBase diskData)
        {
            var slice = ScriptableObject.CreateInstance(type) as SliceBase;
            slice.Init(diskData);
            return slice;
        }
        #endregion
        /// <summary>
        /// 请在此函数中检查切片数据的类型并添加对应类型的切片到轨道
        /// </summary>
        /// <param name="sliceDisk"></param>
        protected abstract bool ForeachSliceDiskToSlice(Slice_DiskBase sliceDisk);
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

        public SliceBase[] sliceArray;
        public Track_Disk track_Disk;

        #region 设置轨道高与切片宽度
        /// <summary>
        /// 设置整个轨道的高，会同步更新所有的切片的高度
        /// </summary>
        public virtual void SetHeight(int pixel)
        {
            View.style.height = new StyleLength(new Length(pixel, LengthUnit.Pixel));
            Heading.View.style.height = new StyleLength(new Length(pixel, LengthUnit.Pixel));
            if (sliceArray != null)
                foreach (var slice in sliceArray)
                    slice?.SetHeight(pixel);
        }
        /// <summary>
        /// 设置每个切片的宽度，请不要使用此函数，设置时间轴的间距会同步更新
        /// </summary>
        public virtual void SetWidth(int pixel)
        {
            if (sliceArray != null)
                foreach (var slice in sliceArray)
                    slice?.SetWidth(pixel);
        }
        #endregion

        #region 切片位置切换
        /// <summary>
        /// 添加一个新切片
        /// </summary>
        public void AddSlice(SliceBase slice, int startFrame)
        {
            slice.Track = this;
            if (startFrame != -1)     //说明此数据是新添加的
            {
                if (sliceArray[startFrame] != null)
                {
                    Debug.Log($"索引{startFrame}已有切片");
                    return;
                }
                slice.StartFrame = startFrame;
                slice.FrameLength = 1;

                track_Disk.SlickArray[startFrame] = slice.DiskData;
            }
            else
            {
                slice.StartFrame = slice.StartFrame;
                slice.FrameLength = slice.FrameLength;
            }

            slice.SetHeight(View.style.height.value.value);
            View.Add(slice.View);
            for (int i = 0; i < slice.FrameLength; i++)
                sliceArray[slice.StartFrame + i] = slice;
        }

        public void RemoveSlice(int index)
        {
            if (index < 0 || index >= sliceArray.Length) return;
            var slice = sliceArray[index];
            for (int i = slice.StartFrame; i < slice.StartFrame + slice.FrameLength; i++)
            {
                sliceArray[i] = null;
            }
            Reset();
        }
        /// <summary>
        /// 交换两个切片
        /// </summary>
        public void SwapSlice(int index0, int index1)
        {
            if (index0 == index1) return;
            if (index0 > index1) { int temp = index0; index0 = index1; index1 = temp; }
            if (index0 < 0) index0 = 0;
            if (index1 < 0) index1 = 0;
            if (index1 >= track_Disk.SlickArray.Length) return;
            var slice0 = sliceArray[index0];
            var slice1 = sliceArray[index1];
            int slice0_start = slice0 != null ? slice0.StartFrame : index0;
            int slice1_start = slice1 != null ? slice1.StartFrame : index1;
            var tempSlice = sliceArray[slice0_start];
            sliceArray[slice0_start] = sliceArray[slice1_start];
            sliceArray[slice1_start] = tempSlice;

            Reset();
        }
        /// <summary>
        /// 根据当前的SliceArray，重新排列
        /// </summary>
        public void Reset()
        {
            HashSet<SliceBase> hash = new HashSet<SliceBase>();
            List<SliceBase> list = new List<SliceBase>();
            for (int i = 0; i < sliceArray.Length; i++)
            {
                SliceBase slice = sliceArray[i];
                if (slice == null)
                    list.Add(null);
                else if (hash.Contains(slice) == false)
                {
                    hash.Add(slice);
                    list.Add(slice);
                }
            }

            View.Clear();
            Array.Clear(sliceArray, 0, sliceArray.Length);
            Array.Clear(track_Disk.SlickArray, 0, track_Disk.SlickArray.Length);
            int startFrame = 0;

            for (int i = 0; i < list.Count; i++)
            {
                var slice = list[i];
                if (slice != null)
                {
                    View.Add(slice.View);
                    slice.StartFrame = startFrame;
                    for (int f = 0; f < slice.FrameLength; f++)
                    {
                        sliceArray[startFrame] = slice;
                        track_Disk.SlickArray[startFrame] = slice.DiskData;
                        startFrame++;
                    }
                }
                else
                {
                    startFrame++;
                }
            }
        }

        public void SetSliceFrameLength(int index, int newLength)
        {
            if (index < 0 || index >= sliceArray.Length) return;
            if (newLength <= 0) return;
            var slice = sliceArray[index];
            if (slice == null) return;
            if (slice.DiskData.frameLength == newLength) return;

            int d = newLength - slice.DiskData.frameLength;


            if (d > 0)
            {
                List<int> nullIndexList = new List<int>();
                for (int i = slice.StartFrame + slice.FrameLength; i < sliceArray.Length; i++)
                {
                    SliceBase sliceBase = sliceArray[i];
                    if (sliceBase == null)
                        nullIndexList.Add(i);
                }
                d = Mathf.Min(d, nullIndexList.Count);
                for (int i = 0; i < d; i++)
                    sliceArray[nullIndexList[i]] = slice;
                slice.FrameLength += d;
            }
            else
            {
                var newFrameLength = Mathf.Max(1, slice.FrameLength + d);
                for (int i = newFrameLength; i < slice.FrameLength; i++)
                    sliceArray[i + slice.StartFrame] = null;
                slice.FrameLength = newFrameLength;
            }

            Reset();
        }
        #endregion

        #region 切片拖拽移动

        private SliceBase DragSelectSlice;
        private float DragXOffset;
        private SliceBase LocalPointToSlice(float x)
        {
            int index = (int)(x / SkillVisualEditorWindow.Inst.TimeScaleAxis.Spacing);
            if (index < 0 || index >= sliceArray.Length) return null;
            return sliceArray[index];
        }
        public int LocalPointToSliceIndex(float x)
        {
            int index = (int)(x / SkillVisualEditorWindow.Inst.TimeScaleAxis.Spacing);
            return Mathf.Clamp(index, 0, sliceArray.Length - 1);
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
                if (Mathf.Abs(evt.localPosition.x - DragXOffset) > SkillVisualEditorWindow.Inst.TimeScaleAxis.Spacing)
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

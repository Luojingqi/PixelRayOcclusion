using Sirenix.OdinInspector;
using System.Collections.Generic;
using static PRO.SkillEditor.SceneCreate_Disk;

namespace PRO.SkillEditor
{
    internal class SceneCreateSlice : SliceBase
    {
        public SceneCreateSlice(SceneCreate_Disk sliceDisk) : base(sliceDisk)
        {
        }

        public override void DrawGizmo(SkillPlayAgent agent)
        {

        }

        private SceneCreate_Disk diskData => DiskData as SceneCreate_Disk;

        [LabelText("放置的层")]
        [ShowInInspector]
        public BlockBase.BlockType dlockType { get => diskData.blockType; set => diskData.blockType = value; }

        [LabelText("按照坚硬度先破坏再放置")]
        [ShowInInspector]
        public bool tryRuin { get => diskData.tryRuin; set => diskData.tryRuin = value; }

        [LabelText("创建的点集")]
        [ShowInInspector]
        public List<PixelData> pixelList { get => diskData.pixelList; set => diskData.pixelList = value; }



        [Button("剔除重复点")]
        [ShowInInspector]
        private void 剔除重复点Action()
        {
            List<SceneCreateSlice> sliceList = new List<SceneCreateSlice>();

            int index = 0;
            foreach (var selectSlice in SkillEditorWindow.Inst.SelectSliceHash)
            {
                if (index++ == 0 && this != selectSlice) return;
                if (selectSlice is SceneCreateSlice slice) sliceList.Add(slice);
            }

            QuickSort(sliceList, 0, sliceList.Count - 1);

            HashSet<PixelData> hash = new HashSet<PixelData>();
            foreach (var slice in sliceList)
            {
                List<PixelData> newSet = new List<PixelData>();
                foreach (var data in slice.pixelList)
                {
                    if (hash.Contains(data) == false)
                    {
                        newSet.Add(data);
                        hash.Add(data);
                    }
                }
                slice.pixelList = newSet;
                slice.Name = $"{slice.Name.Split('\n')[0]}\n点:{slice.pixelList.Count}";
            }

        }
        #region 快速排序
        private static void QuickSort(List<SceneCreateSlice> list, int begin, int end)
        {
            if (begin >= end) return; // 基线条件：子数组长度为0或1时停止递归

            int pivotIndex = Partition(list, begin, end); // 获取基准元素的正确位置
            QuickSort(list, begin, pivotIndex - 1);       // 递归处理左半区
            QuickSort(list, pivotIndex + 1, end);          // 递归处理右半区
        }
        // 分区函数（核心逻辑）
        private static int Partition(List<SceneCreateSlice> list, int begin, int end)
        {
            var pivot = list[begin]; // 选择首元素作为基准（可优化为随机选择）
            int i = begin;          // 左扫描指针
            int j = end;            // 右扫描指针

            while (i < j)
            {
                // 从右向左找第一个小于基准的元素
                while (i < j && list[j].pixelList.Count >= pivot.pixelList.Count) j--;
                list[i] = list[j];    // 将该元素移到左侧空位

                // 从左向右找第一个大于基准的元素
                while (i < j && list[i].pixelList.Count <= pivot.pixelList.Count) i++;
                list[j] = list[i];    // 将该元素移到右侧空位
            }

            list[i] = pivot;         // 基准元素归位
            return i;               // 返回基准的最终索引
        }

        public override void DrawHandle(SkillPlayAgent agent)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}

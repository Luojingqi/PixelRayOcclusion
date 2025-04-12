using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PRO.SkillEditor
{
    internal class SceneRuinSlice : SliceBase
    {
        public SceneRuinSlice(SceneRuin_Disk sliceDisk) : base(sliceDisk)
        {
            if (diskData.sprite != null)
                Background = new StyleBackground(diskData.sprite);
        }

        public override void DrawGizmo(SkillPlayAgent agent)
        {

        }

        public override void DrawHandle(SkillPlayAgent agent)
        {
        }

        public override void Select()
        {
            base.Select();
            if (SkillEditorWindow.Inst.Config.Agent == null) return;
            diskData.EditorShow(SkillEditorWindow.Inst.Config.Agent, Track.trackIndex);
            SkillEditorWindow.Inst.Config.Agent.transform.Find($"场景破坏轨道{Track.trackIndex}").gameObject.SetActive(true);
        }
        public override void UnSelect()
        {
            base.UnSelect();
            if (SkillEditorWindow.Inst.Config.Agent == null) return;
            SkillEditorWindow.Inst.Config.Agent.transform.Find($"场景破坏轨道{Track.trackIndex}").gameObject.SetActive(false);
        }

        private SceneRuin_Disk diskData => DiskData as SceneRuin_Disk;


        [LabelText("破坏的层")]
        [ShowInInspector]
        public BlockBase.BlockType BlockType { get => diskData.BlockType; set => diskData.BlockType = value; }
        [LabelText("破坏的坚硬度")]
        [ShowInInspector]
        public int hardness { get => diskData.hardness; set => diskData.hardness = value; }
        [LabelText("破坏的耐久度")]
        [ShowInInspector]
        public int durability { get => diskData.durability; set => diskData.durability = value; }
        [LabelText("破坏的点集")]
        [ShowInInspector]
        public List<Vector2Int> pixelList { get => diskData.pixelList; set => diskData.pixelList = value; }




        [LabelText("颜色精灵图")]
        [ShowInInspector]
        public Sprite sprite
        {
            get { return diskData.sprite; }
            set
            {
                diskData.pixelList.Clear();
                diskData.sprite = value;
                Background = new StyleBackground(diskData.sprite);
                for (int y = 0; y < diskData.sprite.rect.height; y++)
                    for (int x = 0; x < diskData.sprite.rect.width; x++)
                    {
                        Color32 color = (Color32)diskData.sprite.texture.GetPixel(x + (int)diskData.sprite.rect.x, y + (int)diskData.sprite.rect.y);
                        if (color.a != 255) continue;
                        diskData.pixelList.Add(new Vector2Int(x, y));
                    }
            }
        }

        [Button("剔除重复点")]
        [ShowInInspector]
        private void 剔除重复点Action()
        {
            List<SceneRuinSlice> sliceList = new List<SceneRuinSlice>();
            int index = 0;
            foreach (var selectSlice in SkillEditorWindow.Inst.SelectSliceHash)
            {
                if (index++ == 0 && this != selectSlice) return;
                if (selectSlice is SceneRuinSlice slice) sliceList.Add(slice);
            }

            QuickSort(sliceList, 0, sliceList.Count - 1);

            HashSet<Vector2Int> hash = new HashSet<Vector2Int>();
            foreach (var slice in sliceList)
            {
                List<Vector2Int> newList = new List<Vector2Int>();
                foreach (var data in slice.pixelList)
                {
                    if (hash.Contains(data) == false)
                    {
                        newList.Add(data);
                        hash.Add(data);
                    }
                }
                slice.pixelList = newList;
            }

        }
        #region 快速排序
        private static void QuickSort(List<SceneRuinSlice> list, int begin, int end)
        {
            if (begin >= end) return; // 基线条件：子数组长度为0或1时停止递归

            int pivotIndex = Partition(list, begin, end); // 获取基准元素的正确位置
            QuickSort(list, begin, pivotIndex - 1);       // 递归处理左半区
            QuickSort(list, pivotIndex + 1, end);          // 递归处理右半区
        }
        // 分区函数（核心逻辑）
        private static int Partition(List<SceneRuinSlice> list, int begin, int end)
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
        #endregion
    }
}

using PRO.Tool;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PRO.SkillEditor
{
    internal class SceneRuinTrack : TrackBase
    {
        public SceneRuinTrack(Track_Disk track_Disk) : base(track_Disk)
        {
            Heading.NameText.text = "场景破坏轨道";


            View.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                evt.menu.AppendAction("剔除重复点", _ => 剔除重复点Action());
            }));
        }

        private void 剔除重复点Action()
        {
            List<SceneRuinSlice> sliceList = new List<SceneRuinSlice>();

            foreach (var selectSlice in SkillEditorWindow.Inst.SelectSliceHash)
                if (selectSlice is SceneRuinSlice slice) sliceList.Add(slice);

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
                //slice.Name = $"{slice.Name.Split('\n')[0]}\n点:{slice.pixelList.Count}";
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
        protected override bool ForeachSliceDiskToSlice(Slice_DiskBase sliceDisk)
        {
            switch (sliceDisk)
            {
                case SceneRuin_Disk disk: { AddSlice(new SceneRuinSlice(disk)); return true; }
            }
            return false;
        }

        protected override bool DragAssetTypeCheck(Type type)
        {
            return type == typeof(DefaultAsset) || type == typeof(Sprite);
        }

        protected override void DragAssetExit(DragExitedEvent evt, object[] objects)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i] is DefaultAsset da)
                {
                    string path = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length) + AssetDatabase.GetAssetPath(da);
                    FileInfo fileInfo = new FileInfo(path);
                    if (fileInfo.Extension == ".png")
                    {
                        var byteArray = File.ReadAllBytes(path);
                        Texture2D texture = new Texture2D(0, 0);
                        texture.LoadImage(byteArray);
                        texture.filterMode = FilterMode.Point;
                        var disk = new SceneRuin_Disk();
                        var slice = new SceneRuinSlice(disk);
                        slice.sprite = Texture2DPool.CreateSprite(texture);
                        AddSlice(slice);
                    }
                }
                else if (objects[i] is Sprite sprite)
                {
                    var disk = new SceneRuin_Disk();
                    var slice = new SceneRuinSlice(disk);
                    slice.sprite = sprite;
                    AddSlice(slice);
                }
            }
        }
    }
}

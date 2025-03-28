using PRO.Tool;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static PRO.SkillEditor.SceneCreate_Disk;
namespace PRO.SkillEditor
{
    internal class SceneCreateTrack : TrackBase
    {
        public SceneCreateTrack(Track_Disk track_Disk) : base(track_Disk)
        {
            Heading.NameText.text = "�����������";

            View.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                evt.menu.AppendAction("�޳��ظ���", _ => �޳��ظ���Action());
            }));
        }

        private void �޳��ظ���Action()
        {
            List<SceneCreateSlice> sliceList = new List<SceneCreateSlice>();

            foreach (var selectSlice in SkillEditorWindow.Inst.SelectSliceHash)
                if (selectSlice is SceneCreateSlice slice) sliceList.Add(slice);

            QuickSort(sliceList, 0, sliceList.Count - 1);

            HashSet<PixelData> hash = new HashSet<PixelData>();
            foreach (var slice in sliceList)
            {
                List<PixelData> newList = new List<PixelData>();
                foreach (var data in slice.diskData.CreatePixelList)
                {
                    if (hash.Contains(data) == false)
                    {
                        newList.Add(data);
                        hash.Add(data);
                    }
                }
                slice.diskData.CreatePixelList = newList;
                slice.Name = $"{slice.diskData.name.Split('\n')[0]}\n��:{slice.diskData.CreatePixelList.Count}";
            }

        }
        #region ��������
        private static void QuickSort(List<SceneCreateSlice> list, int begin, int end)
        {
            if (begin >= end) return; // ���������������鳤��Ϊ0��1ʱֹͣ�ݹ�

            int pivotIndex = Partition(list, begin, end); // ��ȡ��׼Ԫ�ص���ȷλ��
            QuickSort(list, begin, pivotIndex - 1);       // �ݹ鴦�������
            QuickSort(list, pivotIndex + 1, end);          // �ݹ鴦���Ұ���
        }
        // ���������������߼���
        private static int Partition(List<SceneCreateSlice> list, int begin, int end)
        {
            var pivot = list[begin]; // ѡ����Ԫ����Ϊ��׼�����Ż�Ϊ���ѡ��
            int i = begin;          // ��ɨ��ָ��
            int j = end;            // ��ɨ��ָ��

            while (i < j)
            {
                // ���������ҵ�һ��С�ڻ�׼��Ԫ��
                while (i < j && list[j].diskData.CreatePixelList.Count >= pivot.diskData.CreatePixelList.Count) j--;
                list[i] = list[j];    // ����Ԫ���Ƶ�����λ

                // ���������ҵ�һ�����ڻ�׼��Ԫ��
                while (i < j && list[i].diskData.CreatePixelList.Count <= pivot.diskData.CreatePixelList.Count) i++;
                list[j] = list[i];    // ����Ԫ���Ƶ��Ҳ��λ
            }

            list[i] = pivot;         // ��׼Ԫ�ع�λ
            return i;               // ���ػ�׼����������
        }
        #endregion
        protected override void ForeachSliceDiskToSlice(SliceBase_Disk sliceDisk)
        {
            switch (sliceDisk)
            {
                case SceneCreate_Disk disk: { AddSlice(new SceneCreateSlice(disk)); break; }
            }
        }

        protected override bool DragAssetTypeCheck(Type type)
        {
            return type == typeof(TextAsset);
        }

        protected override void DragAssetExit(DragExitedEvent evt, object[] objects)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i] is TextAsset da)
                {
                    string path = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length) + AssetDatabase.GetAssetPath(da);
                    FileInfo fileInfo = new FileInfo(path);
                    if (fileInfo.Extension == ".json" || fileInfo.Extension == ".txt")
                    {
                        Element_Disk element = JsonTool.ToObject<Element_Disk>(File.ReadAllText(path));
                        if (element == null) continue;
                        var disk = new SceneCreate_Disk();
                        for (int y = 0; y < element.height; y++)
                            for (int x = 0; x < element.width; x++)
                            {
                                string typeName = element.pixels[y * element.width + x].typeName;
                                string colorName = element.pixels[y * element.width + x].colorName;
                                if (typeName == "����") continue;
                                disk.CreatePixelList.Add(new PixelData()
                                {
                                    typeName = typeName,
                                    colorName = colorName,
                                    pos = new Vector2Int(x, y)
                                });
                            }

                        if (da.name.Contains("temp_out"))
                            disk.name = da.name.Substring("temp_out".Length, da.name.Length - "temp_out".Length);
                        else
                            disk.name = da.name;
                        disk.name += $"\n��:{disk.CreatePixelList.Count}";
                        var slice = new SceneCreateSlice(disk);
                        AddSlice(slice);
                    }
                }
            }
        }
        internal class Element_Disk
        {
            public int height;
            public int width;
            public string name;
            public Pixel[] pixels;
            internal class Pixel
            {
                public string typeName;
                public string colorName;
            }
        }
    }
}
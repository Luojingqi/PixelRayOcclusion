using PRO.Tool;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace PRO.SkillEditor
{
    internal class SceneCreateTrack : TrackBase
    {
        public SceneCreateTrack(Track_Disk track_Disk) : base(track_Disk)
        {
            Heading.NameText.text = "�����������";
        }

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
                                disk.CreatePixelList.Add(new SceneCreate_Disk.PixelData()
                                {
                                    typeName = typeName,
                                    colorName = colorName,
                                    pos = new Vector2Int(x, y)
                                });
                            }

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
using System;
using System.Collections;
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
        }
        protected override void ForeachSliceDiskToSlice(SliceBase_Disk sliceDisk)
        {
            switch (sliceDisk)
            {
                case SceneRuin_Disk disk: { AddSlice(new SceneRuinSlice(disk)); break; }
                case NullSlice_Disk disk: { AddSlice(new NullSlice(disk)); break; }
            }
        }

        protected override bool DragAssetCheck(Type type)
        {
            return type == typeof(DefaultAsset) || type == typeof(Texture2D);
        }

        protected override void DragAssetExit(DragExitedEvent evt, object[] objects)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i] is DefaultAsset)
                {
                    DefaultAsset da = objects[i] as DefaultAsset;
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
                        slice.Texture = texture;
                        AddSlice(slice);
                    }
                }
                else if (objects[i] is Texture2D)
                {
                    Texture2D texture = objects[i] as Texture2D;
                    texture.filterMode = FilterMode.Point;
                    var disk = new SceneRuin_Disk();
                    var slice = new SceneRuinSlice(disk);
                    slice.Texture = texture;
                    AddSlice(slice);
                }
            }
        }


    }
}

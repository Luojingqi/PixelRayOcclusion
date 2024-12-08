using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace PRO.SkillEditor
{
    /// <summary>
    /// 2D的动画轨道
    /// </summary>
    internal class AnimationTrack2D : TrackBase
    {
        public AnimationTrack2D(Track_Disk track_Disk) : base(track_Disk)
        {
            Heading.NameText.text = "2D动画轨道";
        }
        protected override void ForeachSliceDiskToSlice(SliceBase_Disk sliceDisk)
        {
            switch (sliceDisk)
            {
                case AnimationSlice2D_Disk disk: { AddSlice(new AnimationSlice2D(disk)); break; }
            }
        }


        protected override bool DragAssetCheck(Type type)
        {
            if (type == typeof(Sprite)) return true;
            else return false;
        }


        protected override void DragAssetExit(DragExitedEvent evt, object[] objects)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                Sprite sprite = objects[i] as Sprite;
                if (sprite == null) continue;
                AnimationSlice2D slice = new AnimationSlice2D(new AnimationSlice2D_Disk());
                slice.Sprite = sprite;
                AddSlice(slice);
            }
        }


    }
}

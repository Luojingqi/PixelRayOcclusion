using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace PRO.SkillEditor
{
    internal class SpecialEffectTrack2D : TrackBase
    {
        public SpecialEffectTrack2D(Track_Disk track_Disk) : base(track_Disk)
        {
            Heading.NameText.text = "2D特效轨道";
        }
        protected override void ForeachSliceDiskToSlice(SliceBase_Disk sliceDisk)
        {
            switch (sliceDisk)
            {
                case SpecialEffectSlice2D_Disk disk: { AddSlice(new SpecialEffectSlice2D(disk)); break; }
            }
        }

        protected override bool DragAssetCheck(Type type)
        {
            return type == typeof(Sprite);
        }

        protected override void DragAssetExit(DragExitedEvent evt, object[] objects)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i] is Sprite)
                {
                    Sprite sprite = (Sprite)objects[i];
                    var disk = new SpecialEffectSlice2D_Disk();
                    var slice = new SpecialEffectSlice2D(disk);
                    slice.sprite = sprite;
                    AddSlice(slice);
                }
            }
        }


    }
}

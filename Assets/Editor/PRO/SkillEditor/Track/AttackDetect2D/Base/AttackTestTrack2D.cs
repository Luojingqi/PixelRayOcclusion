using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace PRO.SkillEditor
{
    internal class AttackTestTrack2D : TrackBase
    {
        public AttackTestTrack2D(Track_Disk track_Disk) : base(track_Disk)
        {
            Heading.NameText.text = "2D攻击检测轨道";

            View.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                evt.menu.AppendAction("添加射线切片", _ => AddSlice(new AttackTestSlice2D_Ray(new AttackTestSlice2D_Ray_Disk())));

                evt.menu.AppendAction("添加矩形切片", _ => AddSlice(new AttackTestSlice2D_Rect(new AttackTestSlice2D_Rect_Disk())));
            }));
        }
        protected override void ForeachSliceDiskToSlice(SliceBase_Disk sliceDisk)
        {
            switch (sliceDisk)
            {
                case AttackTestSlice2D_Ray_Disk disk: { AddSlice(new AttackTestSlice2D_Ray(disk)); break; }
                case AttackTestSlice2D_Rect_Disk disk: { AddSlice(new AttackTestSlice2D_Rect(disk)); break; }
            }
        }

        protected override bool DragAssetTypeCheck(Type type)
        {
            return false;
        }

        protected override void DragAssetExit(DragExitedEvent evt, object[] objects)
        {
            return;
        }
    }
}

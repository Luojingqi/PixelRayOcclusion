using System;
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
                int index = LocalPointToSliceIndex(evt.localMousePosition.x);
                evt.menu.AppendAction("添加射线切片", _ => AddSlice(CreateSlice<AttackTestSlice2D_Ray>(new AttackTestSlice2D_Ray_Disk()), index));

                evt.menu.AppendAction("添加矩形切片", _ => AddSlice(CreateSlice<AttackTestSlice2D_Rect>(new AttackTestSlice2D_Rect_Disk()), index));

                evt.menu.AppendAction("添加圆形切片", _ => AddSlice(CreateSlice<AttackTestSlice2D_Circle>(new AttackTestSlice2D_Circle_Disk()), index));

                evt.menu.AppendAction("添加胶囊切片", _ => AddSlice(CreateSlice<AttackTestSlice2D_Capsule>(new AttackTestSlice2D_Capsule_Disk()), index));

                evt.menu.AppendAction("添加扇形切片", _ => AddSlice(CreateSlice<AttackTestSlice2D_FanShaped>(new AttackTestSlice2D_FanShaped_Disk()), index));
            }));
        }
        protected override bool ForeachSliceDiskToSlice(Slice_DiskBase sliceDisk)
        {
            switch (sliceDisk)
            {
                case AttackTestSlice2D_Ray_Disk disk: { AddSlice(CreateSlice<AttackTestSlice2D_Ray>(disk), -1); return true; }
                case AttackTestSlice2D_Rect_Disk disk: { AddSlice(CreateSlice<AttackTestSlice2D_Rect>(disk), -1); return true; }
                case AttackTestSlice2D_Circle_Disk disk: { AddSlice(CreateSlice<AttackTestSlice2D_Circle>(disk), -1); return true; }
                case AttackTestSlice2D_Capsule_Disk disk: { AddSlice(CreateSlice<AttackTestSlice2D_Capsule>(disk), -1); return true; }
                case AttackTestSlice2D_FanShaped_Disk disk: { AddSlice(CreateSlice<AttackTestSlice2D_FanShaped>(disk), -1);return true; }
            }
            return false;
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

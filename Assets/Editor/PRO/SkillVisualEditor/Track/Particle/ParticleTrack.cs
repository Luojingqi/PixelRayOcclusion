using System;
using UnityEngine.UIElements;
namespace PRO.SkillEditor
{
    internal class ParticleTrack : TrackBase
    {
        public ParticleTrack(Track_Disk track_Disk) : base(track_Disk)
        {
            Heading.NameText.text = "粒子特效轨道";
            View.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                int index = LocalPointToSliceIndex(evt.localMousePosition.x);
                evt.menu.AppendAction("添加粒子切片", _ => AddSlice(CreateSlice<ParticleSlice>(new ParticleSlice_Disk()), index));
                evt.menu.AppendAction("添加单像素粒子切片", _ => AddSlice(CreateSlice<ParticleSlice>(new ParticleSlice_Disk() { loadPath = "单像素" }), index));
            }));
        }

        protected override bool DragAssetTypeCheck(Type type)
        {
            return false;
        }
        protected override bool ForeachSliceDiskToSlice(Slice_DiskBase sliceDisk)
        {
            switch (sliceDisk)
            {
                case ParticleSlice_Disk disk: { AddSlice(CreateSlice<ParticleSlice>(disk), -1); return true; }
            }
            return false;
        }
        protected override void DragAssetExit(DragExitedEvent evt, object[] objects)
        {
        }


    }
}
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace PRO.SkillEditor
{
    internal class ParticleTrack : TrackBase
    {
        public ParticleTrack(Track_Disk track_Disk) : base(track_Disk)
        {
            Heading.NameText.text = "������Ч���";
            View.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                evt.menu.AppendAction("���������Ƭ", _ => AddSlice(new ParticleSlice(new ParticleSlice_Disk())));
                evt.menu.AppendAction("��ӵ�����������Ƭ", _ => AddSlice(new ParticleSlice(new ParticleSlice_Disk() { loadPath = "������" })));
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
                case ParticleSlice_Disk disk: { AddSlice(new ParticleSlice(disk)); return true; }
            }
            return false;
        }
        protected override void DragAssetExit(DragExitedEvent evt, object[] objects)
        {
        }


    }
}
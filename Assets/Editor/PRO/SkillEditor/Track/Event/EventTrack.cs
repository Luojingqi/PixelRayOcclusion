using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace PRO.SkillEditor
{
    internal class EventTrack : TrackBase
    {
        public EventTrack(Track_Disk track_Disk) : base(track_Disk)
        {
            Heading.NameText.text = "ÊÂ¼þ¹ìµÀ";
        }
        protected override void ForeachSliceDiskToSlice(SliceBase_Disk sliceDisk)
        {
            switch (sliceDisk)
            {
                case EventSlice_Disk disk: { AddSlice(new EventSlice(disk)); break; }
                case NullSlice_Disk disk: { AddSlice(new NullSlice(disk)); break; }
            }
        }

        protected override bool DragAssetCheck(Type type)
        {
            return type == typeof(MonoScript);
        }

        protected override void DragAssetExit(DragExitedEvent evt, object[] objects)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                MonoScript monoScript = objects[i] as MonoScript;
                if (monoScript == null) continue;
                Type type = monoScript.GetClass();
                if (type.IsSubclassOf(typeof(EventSlice_Disk)) == false) continue;
                EventSlice_Disk disk = Activator.CreateInstance(type) as EventSlice_Disk;
                AddSlice(new EventSlice(disk));
            }
        }
    }
}
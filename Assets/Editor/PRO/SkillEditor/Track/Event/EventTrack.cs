using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace PRO.SkillEditor
{
    internal class EventTrack : TrackBase
    {
        public EventTrack(Track_Disk track_Disk) : base(track_Disk)
        {
            Heading.NameText.text = "�¼����";
            View.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                evt.menu.AppendAction("����ز���Ƭ", _ => AddSlice(new EventSlice_��ת����(new EventDisk_��ת����())));
            }));
        }
        protected override void ForeachSliceDiskToSlice(SliceBase_Disk sliceDisk)
        {
            switch (sliceDisk)
            {
                case EventDisk_��ת���� disk: { AddSlice(new EventSlice_��ת����(disk)); break; }
                case EventSlice_Disk disk: { AddSlice(new EventSlice(disk)); break; }
            }
        }

        protected override bool DragAssetTypeCheck(Type type)
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
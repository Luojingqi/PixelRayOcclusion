using PROTool;
using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace PRO.SkillEditor
{
    internal class EventTrack : TrackBase
    {
        public EventTrack(Track_Disk track_Disk) : base(track_Disk)
        {
            Heading.NameText.text = "事件轨道";
            var list = ReflectionTool.GetDerivedClasses(typeof(EventSlice_DiskBase));
            foreach (var item in list)
            {
                View.AddManipulator(new ContextualMenuManipulator(evt =>
                {
                    evt.menu.AppendAction(item.Name.Split('_', 2)[1], _ =>
                    {
                        AddSlice(DiskToSlice(Activator.CreateInstance(item) as Slice_DiskBase));
                    });
                }));
            }
        }
        protected override bool ForeachSliceDiskToSlice(Slice_DiskBase sliceDisk)
        {
            var slice = DiskToSlice(sliceDisk);
            if (slice == null) return false;
            else
            {
                AddSlice(slice);
                return true;
            }
        }

        private EventSlice DiskToSlice(Slice_DiskBase sliceDisk)
        {
            switch (sliceDisk)
            {
                case EventDisk_跳转播放 disk: return new EventSlice_跳转播放(disk);
                case EventDisk_创建Building disk: return new EventSlice_创建Building(disk);
                case EventSlice_DiskBase disk: return new EventSlice(disk);
            }
            return null;
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
                if (type.IsSubclassOf(typeof(EventSlice_DiskBase)) == false) continue;
                EventSlice_DiskBase disk = Activator.CreateInstance(type) as EventSlice_DiskBase;
                AddSlice(new EventSlice(disk));
            }
        }
    }
}
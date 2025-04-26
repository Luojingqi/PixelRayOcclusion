using PROTool;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace PRO.SkillEditor
{
    internal class EventTrack : TrackBase
    {
        private List<Type> EventSlice_List;
        public EventTrack(Track_Disk track_Disk) : base(track_Disk)
        {
            Heading.NameText.text = "ÊÂ¼þ¹ìµÀ";
            List<Type> EventDisk_Base_List = ReflectionTool.GetDerivedClasses(typeof(EventDisk_Base));
            foreach (var item in EventDisk_Base_List)
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
            if (sliceDisk is EventDisk_Base == false) return null;
            if(EventSlice_List == null) EventSlice_List = ReflectionTool.GetDerivedClasses(typeof(EventSlice));
            foreach (var item in EventSlice_List)
            {
                var strings = item.Name.Split('_');
                string name = "EventDisk";
                for (int i = 1; i < strings.Length; i++)
                    name += "_" + strings[i];
                if (name == sliceDisk.GetType().Name)
                {
                    return Activator.CreateInstance(item, sliceDisk) as EventSlice;
                }
            }
            return new EventSlice(sliceDisk as EventDisk_Base);
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
                if (type.IsSubclassOf(typeof(EventDisk_Base)) == false) continue;
                EventDisk_Base disk = Activator.CreateInstance(type) as EventDisk_Base;
                DiskToSlice(disk);
            }
        }
    }
}
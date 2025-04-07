using PROTool;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PRO.SkillEditor
{
    internal class EventSlice_创建Building : EventSlice
    {
        public EventSlice_创建Building(EventSlice_DiskBase sliceDisk) : base(sliceDisk)
        {

        }

        private EventDisk_创建Building diskData => (EventDisk_创建Building)DiskData;

        [ShowInInspector]
        [ValueDropdown(nameof(GetTypes))]
        [InlineButton("ClearType","清除")]
        public Type Type
        {
            get { return diskData.type; }
            set { diskData.type = value; }
        }

        
        private void ClearType()
        {
            diskData.type = null;
        }

        private List<Type> GetTypes()
        {
            var list = ReflectionTool.GetDerivedClasses(typeof(BuildingBase));
            return list;
        }
    }
}

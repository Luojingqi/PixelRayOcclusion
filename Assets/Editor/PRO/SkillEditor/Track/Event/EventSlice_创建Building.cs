using PRO.SceneEditor;
using PRO.Tool.Serialize.Json;
using PROTool;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static PRO.SkillEditor.EventDisk_创建Building;

namespace PRO.SkillEditor
{
    internal class EventSlice_创建Building : EventSlice
    {
        public EventSlice_创建Building(EventDisk_创建Building sliceDisk) : base(sliceDisk)
        {

        }

        private EventDisk_创建Building diskData => (EventDisk_创建Building)DiskData;

        [ShowInInspector]
        [ValueDropdown(nameof(GetTypes))]
        [InlineButton("ClearType", "清除")]
        #region 类型
        public Type type
        {
            get { return diskData.type; }
            set { diskData.type = value; }
        }
        private void ClearType() => diskData.type = null;
        private List<Type> GetTypes() => ReflectionTool.GetDerivedClasses(typeof(BuildingBase));
        #endregion

        [ShowInInspector]
        public Vector2Int size { get => diskData.size; set => diskData.size = value; }

        [Button("添加数据")]
        public void AddBuilding(TextAsset json数据, BlockBase.BlockType blockType)
        {

            string path = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length) + AssetDatabase.GetAssetPath(json数据);
            FileInfo fileInfo = new FileInfo(path);
            if (fileInfo.Extension == ".json" || fileInfo.Extension == ".txt")
            {
                string[] strings = fileInfo.Name.Split('_');
                if (strings.Length == 3 && strings[2][0] == 'f')
                {
                    Element_Disk element = JsonTool.ToObject<Element_Disk>(File.ReadAllText(path));
                    diskData.size = new Vector2Int(element.width, element.height);
                    for (int y = 0; y < element.height; y++)
                        for (int x = 0; x < element.width; x++)
                        {
                            var data = element.pixels[y * element.width + x];
                            string typeName = data.typeName;
                            string colorName = data.colorName;
                            if (typeName == "空气") continue;
                            diskData.pixelList.Add(new PixelData()
                            {
                                typeName = typeName,
                                colorName = colorName,
                                pos = new Vector2Int(x, y),
                                blockType = blockType,
                            });
                        }
                    Name += "\n" + json数据.name;
                }
            }
        }

        [Button("清空数据")]
        public void ClearBuilding()
        {
            Name = Name.Split('\n')[0];
            diskData.pixelList.Clear();
        }

        public override void DrawGizmo(SkillPlayAgent agent)
        {

        }

        public override void DrawHandle(SkillPlayAgent agent)
        {

        }
    }
}

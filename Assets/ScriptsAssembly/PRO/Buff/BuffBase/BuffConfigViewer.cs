#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace PRO.Buff.Base
{
    //[CreateAssetMenu(menuName = "创建")]
    internal class BuffConfigViewer : SerializedScriptableObject
    {
        [PropertyOrder(-1)]
        [Button("保存")]
        public void Save()
        {
            foreach (var viewer in ViewerList)
                EditorUtility.SetDirty(viewer.config);
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        [TableList]
        public List<Viewer> ViewerList = new List<Viewer>();

        [Button("更新")]
        public void Update()
        {
            ViewerList.Clear();
            var infos = new DirectoryInfo(Application.dataPath + @"\ScriptsAssembly\PRO\Buff\").GetDirectories();
            foreach (var info in infos)
            {
                var files = info.GetFiles("*_Config.asset");
                foreach (var file in files)
                {
                    string path = file.FullName.Substring(Application.dataPath.Length - @"Assets".Length);
                    var config = AssetDatabase.LoadAssetAtPath<BuffConfig>(path);
                    if (config != null)
                    {
                        ViewerList.Add(new Viewer(file.Name.Substring("Buff_".Length, file.Name.Length - "Buff__Config.asset".Length), config));
                    }
                }
            }
        }



        public class Viewer
        {
            [ReadOnly]
            public string id;
            [ReadOnly]
            [ShowInInspector]
            public BuffConfig config;
            [ShowInInspector]
            public string name { get => config.Name; set => config.Name = value; }

            [ShowInInspector]
            public int 优先级 { get => config.Priority; set { config.Priority = value; } }
            [ShowInInspector]
            public bool 独有 { get => config.独有; set { config.独有 = value; } }
            [ShowInInspector]
            public bool 启用ui { get => config.ui; set { config.ui = value; } }

            [ShowInInspector]
            public string 说明文本 { get => config.说明文本; set { config.说明文本 = value; } }


            public Viewer(string id, BuffConfig config)
            {
                this.id = id;
                this.config = config;
            }
        }
    }
}
#endif
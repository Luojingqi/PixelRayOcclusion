using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static PRO.SkillConfig;

namespace PRO
{
    //[CreateAssetMenu(menuName = "创建")]
    internal class SkillConfigViewer : SerializedScriptableObject
    {
        [PropertyOrder(-1)]
        [Button("保存")]
        public void Save()
        {
            foreach (var viewer in ViewerList)
                EditorUtility.SetDirty(viewer.config);
            AssetDatabase.SaveAssets();
        }

        [ShowInInspector]
        [TableList]
        private List<Viewer> ViewerList = new List<Viewer>();

        [Button("更新")]
        public void Update()
        {
            ViewerList.Clear();
            var infos = new DirectoryInfo(Application.dataPath + @"\ScriptsAssembly\PRO\技能\").GetDirectories();
            foreach (var info in infos)
            {
                var config = AssetDatabase.LoadAssetAtPath<SkillConfig>($@"Assets\ScriptsAssembly\PRO\技能\{info.Name}\Skill_{info.Name}_Config.asset");
                if (config != null)
                {
                    ViewerList.Add(new Viewer(info.Name, config));
                }
            }
        }
    }
}

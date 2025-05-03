using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static GamePlay.SkillConfig;

namespace GamePlay
{
    //[CreateAssetMenu(menuName = "创建")]
    internal class SkillConfigViewer : SerializedScriptableObject
    {
        [ShowInInspector]
        [TableList]
        private List<Viewer> ViewerList = new List<Viewer>();

        [Button("更新")]
        public void Update()
        {
            ViewerList.Clear();
            var infos = new DirectoryInfo(Application.dataPath + @"\ScriptsAssembly\GamePlay\技能\").GetDirectories();
            foreach (var info in infos)
            {
                var config = AssetDatabase.LoadAssetAtPath<SkillConfig>($@"Assets\ScriptsAssembly\GamePlay\技能\{info.Name}\Skill_{info.Name}_Config.asset");
                if (config != null)
                {
                    ViewerList.Add(new Viewer(info.Name, config));
                }
            }
        }
    }
}

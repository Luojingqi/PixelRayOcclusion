#if UNITY_EDITOR
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace PRO.Skill.Base
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
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        [TableList]
        public List<Viewer> ViewerList = new List<Viewer>();

        [Button("更新")]
        public void Update()
        {
            ViewerList.Clear();
            var infos = new DirectoryInfo(Application.dataPath + @"\ScriptsAssembly\PRO\Skill\").GetDirectories();
            foreach (var info in infos)
            {
                var config = AssetDatabase.LoadAssetAtPath<SkillConfig>($@"Assets\ScriptsAssembly\PRO\Skill\{info.Name}\Skill_{info.Name}_Config.asset");
                if (config != null)
                    ViewerList.Add(new Viewer(info.Name, config));
            }
        }



        public class Viewer
        {
            [ReadOnly]
            [TableColumnWidth(0)]
            public SkillConfig config;
            [TableColumnWidth(0)]
            [ReadOnly]
            public string id;
            [ShowInInspector]
            public string name { get => config.Name; set => config.Name = value; }
            [TableColumnWidth(0)]
            [ShowInInspector]
            public int 行动点 { get => config.行动点; set => config.行动点 = value; }
            [TableColumnWidth(0)]
            [ShowInInspector]
            public int 施法半径 { get => config.Radius_G; set { config.Radius_G = value; config.Radius_W = value * Pixel.Size; } }
            [TableColumnWidth(0)]
            [ShowInInspector]
            public 施法方式 施法方式 { get => config.施法type; set => config.施法type = value; }
            [TableColumnWidth(150)]
            [ShowInInspector]
            [TableList]
            public List<StartCombatEffectData> 伤害 { get => config.StartCombatEffectDataList; set { config.StartCombatEffectDataList = value; } }

            [TableColumnWidth(200)]
            [ValueDropdown(nameof(Dropdown指示器加载路径))]
            [ShowInInspector]
            public string 指示器加载路径 { get => config.SkillPointerLoadPath; set => config.SkillPointerLoadPath = value; }
            private List<string> Dropdown指示器加载路径()
            {
                List<string> list = new List<string>(10);
                var monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>($@"Assets\ScriptsAssembly\PRO\Skill\{id}\Skill_{id}.cs");
                if (typeof(ISkillPointer).IsAssignableFrom(monoScript.GetClass()))
                {
                    string path = Application.dataPath + @"\ScriptsAssembly\PRO\技能指示器\";
                    string fileName = null;
                    if (typeof(ISkillPointer_射线选择).IsAssignableFrom(monoScript.GetClass())) fileName = "范围内射线类";
                    if (typeof(ISkillPointer_范围选择).IsAssignableFrom(monoScript.GetClass())) fileName = "范围内选择类";
                    var infos = new DirectoryInfo(@$"{path}\{fileName}").GetFiles(@$"*.prefab", SearchOption.AllDirectories);

                    foreach (var info in infos)
                    {
                        list.Add(info.FullName.Substring(path.Length, info.FullName.Length - path.Length - ".prefab".Length));
                    }
                }
                list.Add("");
                return list;
            }

            public Viewer(string id, SkillConfig config)
            {
                this.id = id;
                this.config = config;
            }
        }
    }
}
#endif
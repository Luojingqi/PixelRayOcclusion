using PRO.Buff.Base;
using PRO.Skill;
using PRO.Skill.Base;
using PRO.SkillEditor;
using PRO.Tool;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PRO
{
    /// <summary>
    /// 资源加载扩展，用于路径相对固定，只存在
    /// </summary>
    public static class AssetManagerEX
    {
        /// <summary>
        /// 技能文件夹的目录
        /// </summary>
        public static string SkillDirectoryPath = @"ScriptsAssembly\PRO\Skill";
        /// <summary>
        /// 加载技能轨道数据
        /// 
        /// Asset\ScriptsAssembly\PRO\技能\
        /// </summary>
        public static SkillVisual_Disk LoadSkillVisualDisk(string path,bool path补全 = true)
        {
            if (path补全)
                return AssetManager.Load_A<SkillVisual_Disk>("skill.ab", @$"{SkillDirectoryPath}\{path}.asset");
            else
                return AssetManager.Load_A<SkillVisual_Disk>("skill.ab", $@"{path}.asset");
        }
        private static StringBuilder stringBuilder = new StringBuilder(64);
        //public static Skill_Disk LoadSkillDisk(OperateFSMBase SkillOperate)
        //{
        //    string name = SkillOperate.GetType().Name;
        //    int i = 0;
        //    while (i < name.Length)
        //        if (name[i++] == '_') break;
        //    while (i < name.Length)
        //        stringBuilder.Append(name[i++]);
        //    stringBuilder.Append('\\');
        //    stringBuilder.Append(name);
        //    string path = stringBuilder.ToString();
        //    stringBuilder.Clear();
        //    return AssetManager.Load_A<Skill_Disk>("skill.ab", @$"{SkillDirectoryPath}\{path}.asset");
        //}
        //public static SkillConfig LoadSkillConfig(OperateFSMBase SkillOperate)
        //{
        //    string name = SkillOperate.GetType().Name;
        //    int i = 0;
        //    while (i < name.Length)
        //        if (name[i++] == '_') break;
        //    while (i < name.Length)
        //        stringBuilder.Append(name[i++]);
        //    stringBuilder.Append('\\');
        //    stringBuilder.Append($"{name}_Config");
        //    string path = stringBuilder.ToString();
        //    stringBuilder.Clear();
        //    return AssetManager.Load_A<SkillConfig>("skill.ab", @$"{SkillDirectoryPath}\{path}.asset");
        //}
        /// <summary>
        /// 加载技能指示器
        /// 
        /// Asset\ScriptsAssembly\PRO\技能指示器\
        /// 
        /// Asset\ScriptsAssembly\PRO\技能\
        /// </summary>
        public static T LoadSkillPointer<T>(string path) where T : SkillPointerBase
        {
            if (skillPointerInScene.TryGetValue(path, out SkillPointerBase ret) == false)
            {
                var assetInRAW = AssetManager.Load_A<GameObject>("skillpointer.ab", @$"ScriptsAssembly\PRO\技能指示器\{path}");
                if (assetInRAW == null) assetInRAW = AssetManager.Load_A<GameObject>("skillpointer.ab", @$"{SkillDirectoryPath}\{path}.asset");
                if (assetInRAW == null) return null;
                ret = GameObject.Instantiate(assetInRAW).GetComponent<T>();
                skillPointerInScene.Add(path, ret);
                ret.Init();
            }
            return ret as T;
        }
        /// <summary>
        /// 已经实例化到场景中的技能指示器
        /// </summary>
        private static Dictionary<string, SkillPointerBase> skillPointerInScene = new Dictionary<string, SkillPointerBase>();

        /// <summary>
        /// 技能文件夹的目录
        /// </summary>
        public static string BuffDirectoryPath = @"ScriptsAssembly\PRO\Buff";
        public static BuffConfig LoadBuffConfig(BuffBase buff)
        {
            string name = buff.GetType().Name;
            int i = 0;
            while (i < name.Length)
                if (name[i++] == '_') break;
            int end = i;
            while (end < name.Length)
                if (name[end++] == '_') break;
            while (end < name.Length)
                if (name[end++] == '_') break;

            while (i < end)
                stringBuilder.Append(name[i++]);
            stringBuilder.Append('\\');
            stringBuilder.Append($"{name}_Config");
            string path = stringBuilder.ToString();
            stringBuilder.Clear();
            return AssetManager.Load_A<BuffConfig>("skill.ab", @$"{BuffDirectoryPath}\{path}.asset");
        }
    }
}
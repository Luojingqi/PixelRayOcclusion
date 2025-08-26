using PRO.SkillEditor;
using PROTool;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;
using static PRO.Weapon.WeaponBase;
namespace PRO.Weapon
{
    [CreateAssetMenu(menuName = "创建")]
    public class WeaponConfig : SerializedScriptableObject
    {
        public Dictionary<Type, Data> DataDic = new();

        [HideReferenceObjectPicker]
        public class Data
        {
            public string Name;
            public Sprite Icon;
            [HideInInspector]
            public int Optional手持方式;

#if UNITY_EDITOR
            [ShowInInspector, EnumToggleButtons]
            private 手持方式 Optional手持方式Set
            {
                get => (手持方式)Optional手持方式;
                set => Optional手持方式 = (int)value;
            }
            [ShowIf(nameof(ShowIf_单手))]
            private RoleInfo 穿戴后属性提升_单手 => 穿戴后属性提升[手持方式.单手];
            private bool ShowIf_单手() => (Optional手持方式Set & 手持方式.单手) == 手持方式.单手;
            [ShowIf(nameof(ShowIf_双手))]
            private RoleInfo 穿戴后属性提升_双手 => 穿戴后属性提升[手持方式.双手];
            private bool ShowIf_双手() => (Optional手持方式Set & 手持方式.双手) == 手持方式.双手;
#endif
            public SkillVisual_Disk[] VisualArray;
            [HideInInspector]
            public Dictionary<手持方式, RoleInfo> 穿戴后属性提升;

            public Dictionary<string, ConfigValue> ValueDic = new();

            public Data()
            {
                int length = Enum.GetValues(typeof(手持方式)).Length;
                穿戴后属性提升 = new(length);
                foreach (var enumValue in Enum.GetValues(typeof(手持方式)))
                {
                    穿戴后属性提升.Add((手持方式)enumValue, new RoleInfo());
                }
            }
        }

        [Button]
        public void Update()
        {
            var list = ReflectionTool.GetDerivedClasses(typeof(WeaponBase));
            StringBuilder sb = new(1024);
            List<string> removeList = new();
            HashSet<string> valueNameSet = new();

            string path = Application.dataPath + @"\ScriptsAssembly\PRO\Role\Equipment\Weapon\WeaponValueAutoRWClass.cs";
            File.CreateText(path).Close();

            foreach (var type in list)
            {
                Data data = null;
                if (DataDic.TryGetValue(type, out data) == false)
                {
                    data = new();
                    DataDic.Add(type, data);
                }
                data.GetType();

                foreach (PropertyInfo prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {

                    if (prop.Name.StartsWith("Auto_") && prop.CanWrite && prop.GetSetMethod(true) != null)
                    {
                        valueNameSet.Add(prop.Name);
                        if (data.ValueDic.ContainsKey(prop.Name) == false)
                            data.ValueDic.Add(prop.Name, (ConfigValue)Activator.CreateInstance(typeof(ConfigValue<>).MakeGenericType(prop.PropertyType)));
                        sb.AppendLine($"this.{prop.Name} = (data.ValueDic[\"{prop.Name}\"] as ConfigValue<{prop.PropertyType.FullName}>).Value;");
                    }
                }
                foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (field.Name.StartsWith("Auto_"))
                    {
                        valueNameSet.Add(field.Name);
                        if (data.ValueDic.ContainsKey(field.Name) == false)
                            data.ValueDic.Add(field.Name, (ConfigValue)Activator.CreateInstance(typeof(ConfigValue<>).MakeGenericType(field.FieldType)));
                        sb.AppendLine($"this.{field.Name} = (data.ValueDic[\"{field.Name}\"] as ConfigValue<{field.FieldType.FullName}>).Value;");
                    }
                }

                foreach (var valueName in data.ValueDic.Keys)
                    if (valueNameSet.Contains(valueName) == false)
                        removeList.Add(valueName);
                foreach (var valueName in removeList)
                    data.ValueDic.Remove(valueName);


                File.AppendAllText(path,
$@"namespace PRO.Weapon
{{   
    public partial class {type.Name}
    {{
        protected override void SetAutoConfigValue({typeof(Data).FullName.Replace('+', '.')} data)
        {{
            {sb}
        }}
    }}
}}");
                sb.Clear();
                removeList.Clear();
                valueNameSet.Clear();
            }

        }
    }
}
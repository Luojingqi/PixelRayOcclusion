//using PRO.SkillEditor;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using Unity.Properties;
//using UnityEngine;

//public class TestOdin : Sirenix.OdinInspector.Editor.OdinAttributeDrawer<FiledAttribute, Skill_Disk>
//{
//    public object e;
//    protected override void Initialize()
//    {
//        base.Initialize();
//        var stringTips = new List<string>();
//        stringTips.Add("a");
//        ChangeList(stringTips);
//        stringTips = Save(stringTips);
//    }

//    public void ChangeList(List<string> stringTips)
//    {
//        stringTips.Add("abc");
//    }

//    public List<string> Save(List<string> stringTips)
//    {

//    }


//}

//[AttributeUsage(AttributeTargets.Field)]
//public class FiledAttribute : Attribute { }

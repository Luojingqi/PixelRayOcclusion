using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace PRO.SkillEditor
{
    public class AnimationSlice2D_Disk : SliceBase_Disk
    {
        public Sprite sprite;

        public override void UpdateFrame(SkillPlayAgent agent, int frame, int index)
        {
            agent.AgentSprice.sprite = sprite;
        }


        //[DictionaryDrawerSettings(KeyLabel = "Item", ValueLabel = "Color")]
        //public Dictionary<string, string> itemColors = new Dictionary<string, string>();

        //// 在Inspector中显示并为字典元素添加颜色标记
        //[Button]
        //private void OnValidate()
        //{
        //    itemColors.Clear();
        //    itemColors.Add("Item1", "Red");
        //    itemColors.Add("Item2", "Green");
        //    itemColors.Add("Item3", "Blue");
        //}

        //// 使用GUIColor为每个字典项设置不同的颜色
        //[Button]
        //private void DisplayColoredDictionary()
        //{
        //    foreach (var item in itemColors)
        //    {
        //        string color = item.Value;

        //        switch (color)
        //        {
        //            case "Red":
        //                GUI.color = Color.red;
        //                break;
        //            case "Green":
        //                GUI.color = Color.green;
        //                break;
        //            case "Blue":
        //                GUI.color = Color.blue;
        //                break;
        //            default:
        //                GUI.color = Color.white;
        //                break;
        //        }

        //        Debug.Log($"Item: {item.Key} - Color: {color}");

        //        // 恢复GUI颜色
        //        GUI.color = Color.white;
        //    }
        //}
    }
}

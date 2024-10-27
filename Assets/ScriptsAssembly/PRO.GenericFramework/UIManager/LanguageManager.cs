//using Data;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using TMPro;
//using UnityEngine;

//public class LanguageManager : IManager
//{
//    public static LanguageManager Inst { get; set; }

//    private Dictionary<string, Language> LanguageDic = new Dictionary<string, Language>();

//    public LanguageEnum NowUseLanguage { get; set; }

//    public void Init()
//    {
//        Inst= this;
//        LanguageDic.Clear();

//        NowUseLanguage = (LanguageEnum)ConfigManager.Inst.ConstConfig.NowUseLanguage;
//    }

//    public void SwitchLanguage(Transform panel, LanguageEnum languageEnum)
//    {
//        if (LanguageDic == null || LanguageDic.Count == 0)
//            LanguageDic = ((List<Language>)ConfigManager.Inst.ConfigFactory(ConfigManager.ConfigEnum.Language)).ToDictionary(key => key.ID, value => value);

//        UILanguageConfig[] uIs = panel.GetComponentsInChildren<UILanguageConfig>();
//        foreach (var u in uIs)
//        {
//            if (LanguageDic.TryGetValue(u.ID, out Language l))
//            {
//                if (l.UIComponentType == 0)
//                {
//                    u.GetComponent<TMP_Text>().text = LanguageEnumToText(l, languageEnum);
//                }
//                else if (l.UIComponentType == 1)
//                {

//                }
//            }
//            else
//            {
//                Debug.Log("UI的Id输入错误，来自LanguageManager");
//            }
//        }
//    }

//    /// <summary>
//    /// 由语言枚举获取文本 
//    /// </summary>
//    /// <returns></returns>
//    private string LanguageEnumToText(Language language, LanguageEnum languageEnum)
//    {
//        switch (languageEnum)
//        {
//            case LanguageEnum.Chinese: return language.Chinese;
//            case LanguageEnum.English: return language.English;
//            case LanguageEnum.Japanese: return language.Japanese;
//            default: return language.Chinese;
//        }
//    }

//    public enum LanguageEnum
//    {
//        /// <summary>
//        /// 中文
//        /// </summary>
//        Chinese,
//        /// <summary>
//        /// 英语
//        /// </summary>
//        English,
//        /// <summary>
//        /// 日语
//        /// </summary>
//        Japanese,
//    }
//}

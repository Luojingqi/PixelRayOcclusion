using LitJson;
using System;
using System.IO;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;
namespace PRO.Tool
{
    public class JsonTool
    {
        public static string ToJson(object obj)
        {
            //if (!JsonToolCustom.isInit) JsonToolCustom.Init();
            //string jsonText = JsonMapper.ToJson(obj);
            //return Regex.Unescape(jsonText);
            string jsonText = JsonConvert.SerializeObject(obj, Formatting.Indented);
            return jsonText;

        }

        public static T ToObject<T>(string jsonText)
        {
            //if (!JsonToolCustom.isInit) JsonToolCustom.Init();
            //return JsonMapper.ToObject<T>(jsonText);
            return JsonConvert.DeserializeObject<T>(jsonText);
        }

        public static bool ToObject<T>(string jsonText, out T obj)
        {
            if (!JsonToolCustom.isInit) JsonToolCustom.Init();
            obj = JsonMapper.ToObject<T>(jsonText);
            return true;
        }


        /// <summary>
        /// 加载磁盘中文本数据
        /// </summary>
        /// <param name="path"></param>
        /// <param name="jsonText"></param>
        /// <returns></returns>
        public static bool LoadText(string path, out string text)
        {
            try
            {
                //读取文件
                using (StreamReader sr = File.OpenText(path))
                {
                    //数据保存
                    text = sr.ReadToEnd();
                    sr.Close();
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.Log("加载磁盘中文本数据失败" + e);
                text = null;
                return false;
            }
        }


        /// <summary>
        /// 保存文本到磁盘,需要带后缀
        /// </summary>
        /// <param name="path"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool StoreText(string path, string text)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(path, false))
                {

                    sw.Write(text);
                    sw.Close();
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.Log("保存文本到磁盘失败" + e);
                return false;
            }
        }
    }
}
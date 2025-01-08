using System;
using System.IO;
using System.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
namespace PRO.Tool
{
    public static class JsonTool
    {
        private static JsonSerializerSettings writeSetting;
        private static JsonSerializerSettings readSetting;
        private static void Init()
        {
            writeSetting = new JsonSerializerSettings();
            writeSetting.Converters.Add(new JsonToolWriteEx());
            readSetting = new JsonSerializerSettings();
            readSetting.Converters.Add(new JsonToolReadEx());

        }

        public static string ToJson(object obj, bool indented = true)
        {
            if (writeSetting == null) Init();
            string jsonText = JsonConvert.SerializeObject(obj, indented ? Formatting.Indented : Formatting.None, writeSetting);
            return jsonText;
        }

        public static byte[] ToByteArray(object obj)
        {
            byte[] jsonByteArray = Encoding.UTF8.GetBytes(ToJson(obj));
            return jsonByteArray;
        }

        public static T ToObject<T>(string jsonText)
        {
            if (writeSetting == null) Init();
            return JsonConvert.DeserializeObject<T>(jsonText, readSetting);
        }

        public static bool ToObject<T>(string jsonText, out T obj)
        {
            if (writeSetting == null) Init();
            obj = JsonConvert.DeserializeObject<T>(jsonText, readSetting);
            return true;
        }

        public static void AddWriteJsonConverter(JsonConverter converter)
        {
            if (writeSetting == null) Init();
            writeSetting.Converters.Add(converter);
        }
        public static void AddReadJsonConverter(JsonConverter converter)
        {
            if (readSetting == null) Init();
            readSetting.Converters.Add(converter);
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
        public static async UniTask<string> LoadTextAsync(string path)
        {
            string text = null;
            try
            {
                using (StreamReader sr = File.OpenText(path))
                {
                    text = await sr.ReadToEndAsync();
                    sr.Close();
                }
            }
            catch (Exception e)
            {
                Debug.Log("加载磁盘中文本数据失败" + e);
            }
            return text;
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
                using (StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8))
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

        public static bool StoreObject(string path, object obj)
        {
            File.WriteAllBytes(path, Encoding.UTF8.GetBytes(ToJson(obj, false)));
            return true;
        }
    }
}
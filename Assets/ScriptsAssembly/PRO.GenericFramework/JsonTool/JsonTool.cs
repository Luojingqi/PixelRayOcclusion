using System;
using System.Collections.Generic;
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
                //Debug.Log("加载磁盘中文本数据失败" + e);
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
                //Debug.Log("加载磁盘中文本数据失败" + e);
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

        public static int StackToInt(Stack<char> stack)
        {
            int ret = 0;
            int symbol = 1;
            if (stack.Peek() == '-') { symbol = -1; stack.Pop(); }
            while (stack.Count > 0)
                ret += (stack.Pop() - '0') * (int)Mathf.Pow(10, stack.Count);
            return ret * symbol;
        }
        public static string StackToString(Stack<char> stack, ref StringBuilder sb)
        {
            sb.Clear();
            while (stack.Count > 0)
                sb.Append(stack.Pop());
            return sb.ToString();
        }
        /// <summary>
        /// 通用反序列化数据
        /// 从lastDelimiter-1开始向前找
        /// 遇到‘:’代表可以读取一个字段了，数据被存入stack中
        /// 遇到‘,’代表可以将读到的字典写入对象了，开始读取下一个对象
        /// 遇到‘|’代表当前类型的对象全部读取完毕
        /// </summary>
        /// <param name="text"></param>
        /// <param name="readDataAction"></param>
        /// <param name="writeDataAction"></param>
        /// <param name="lastDelimiter"></param>
        /// <param name="stack"></param>
        public static void Deserialize_Data(string text, Action<int> readDataAction, Action writeDataAction, ref int lastDelimiter, ref Stack<char> stack)
        {
            stack.Clear();
            int valueNum = 0;
            for (int i = lastDelimiter - 1; i >= 0; i--)
            {
                char c = text[i];
                if (c == '|' || c == ',' || c == ':')
                {
                    readDataAction?.Invoke(valueNum++);
                    if (c == '|' || c == ',')
                    {
                        writeDataAction?.Invoke();
                        stack.Clear();
                        valueNum = 0;
                        if (c == '|')
                        {
                            lastDelimiter = i; return;
                        }
                    }
                }
                else stack.Push(c);
            }
        }
    }
}
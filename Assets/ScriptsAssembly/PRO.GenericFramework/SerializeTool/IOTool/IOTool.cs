using Cysharp.Threading.Tasks;
using Google.Protobuf;
using PRO.Proto;
using System;
using System.IO;
using System.Text;
using UnityEngine;
using static PRO.Proto.ProtoPool;

namespace PRO.Tool.Serialize.IO
{
    public static class IOTool
    {
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

        //public static bool LoadByte(string path)
        //{
        //    var ssstream = File.OpenRead(path);
        //    Span<byte> bytes = stackalloc byte[(int)ssstream.Length];
        //    ssstream.Read(new byte[10]);
        //}

        public static string protoExtension = ".protobit";

        public static bool LoadProto<T>(string path, MessageParser<T> parser, out T protoData) where T : IMessage<T>
        {
            string protoPath = path + protoExtension;
            try
            {
                using (var stream = File.OpenRead(protoPath))
                {
                    Span<byte> bytes = stackalloc byte[(int)stream.Length];
                    stream.Read(bytes);
                    protoData = parser.ParseFrom(bytes);
                    return true;
                }
            }
            catch (Exception e)
            {
                protoData = default;
                //  Debug.Log("加载protobit数据失败");
                return false;
            }
        }

        public static bool SaveProto<T>(string path, T protoData) where T : IMessage<T>
        {
            Span<byte> bytes = stackalloc byte[protoData.CalculateSize()];
            protoData.WriteTo(bytes);
            try
            {
                using (var stream = File.Create(path + protoExtension))
                {
                    stream.Write(bytes);
                    stream.Close();
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.Log("保存Proto到磁盘失败" + e);
                return false;
            }
        }

        /// <summary>
        /// 保存文本到磁盘，路径需要带后缀
        /// </summary>
        /// <param name="path"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool SaveText(string path, string text)
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
    }
}

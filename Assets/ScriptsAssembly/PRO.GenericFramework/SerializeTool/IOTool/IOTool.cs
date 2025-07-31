using Cysharp.Threading.Tasks;
using Google.FlatBuffers;
using Google.Protobuf;
using Sirenix.Serialization;
using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace PRO.Tool.Serialize.IO
{
    public static class IOTool
    {
        #region Text
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
                if (File.Exists(path))
                {
                    using (StreamReader sr = File.OpenText(path))
                    {
                        text = sr.ReadToEnd();
                        sr.Close();
                    }
                    return true;
                }
                else
                {
                    text = null;
                    return false;
                }
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
                //Debug.Log("加载磁盘中文本数据失败" + e);
            }
            return text;
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
                Debug.Log($"保存Text到磁盘失败: {path}\n{e}");
                return false;
            }
        }
        #endregion
        //public static bool LoadByte(string path)
        //{
        //    var ssstream = File.OpenRead(path);
        //    Span<byte> bytes = stackalloc byte[(int)ssstream.Length];
        //    ssstream.Read(new byte[10]);
        //}
        #region ProtoBuffer
        public static string protoExtension = ".protobit";

        public static bool LoadProto<T>(string path, MessageParser<T> parser, out T protoData) where T : IMessage<T>
        {
            string protoPath = path + protoExtension;
            try
            {
                if (File.Exists(protoPath))
                {
                    using (var stream = File.OpenRead(protoPath))
                    {
                        Span<byte> bytes = stackalloc byte[(int)stream.Length];
                        stream.Read(bytes);
                        protoData = parser.ParseFrom(bytes);
                        return true;
                    }
                }
                else
                {
                    protoData = default;
                    return false;
                }
            }
            catch (Exception e)
            {
                protoData = default;
                Debug.Log("加载protobit数据失败");
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
                Debug.Log($"保存Proto到磁盘失败: {path}\n{e}");
                return false;
            }
        }
        #endregion


        public static string flatExtension = ".flatbit";
        public static bool LoadFlat(string path, out FlatBufferBuilder builder)
        {
            try
            {
                using (FileStream fileStream = new FileStream($"{path}{flatExtension}", FileMode.Open, FileAccess.Read))
                {
                    builder = FlatBufferBuilder.TakeOut((int)fileStream.Length);
                    fileStream.Read(builder.DataBuffer.ToSpan(0, (int)fileStream.Length));
                    return true;
                }
            }
            catch (Exception e)
            {
                builder = null;
                //Debug.Log($"加载Flat到内存失败: {path}\n{e}");
                return false;
            }
        }
        public static bool SaveFlat(string path, FlatBufferBuilder builder)
        {
            try
            {
                using (FileStream fileStream = new FileStream($"{path}{flatExtension}", FileMode.Create, FileAccess.Write))
                {
                    fileStream.Write(builder.DataBuffer.ToSpan(builder.DataBuffer.Position, builder.Offset));
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.Log($"保存Flat到磁盘失败: {path}\n{e}");
                return false;
            }
        }
    }
}

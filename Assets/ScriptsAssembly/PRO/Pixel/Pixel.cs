using PRO.Disk;
using PRO.DataStructure;
using PRO.Tool;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using PRO.Disk.Scene;
namespace PRO
{
    public class Pixel
    {
        /// <summary>
        /// 在块内坐标
        /// </summary>
        public Vector2Byte pos;
        /// <summary>
        /// 类型名称
        /// </summary>
        public PixelTypeInfo info;
        /// <summary>
        /// 使用的颜色名称
        /// </summary>
        public string colorName;

        /// <summary>
        /// 请不要使用构造函数，使用Pixel.New()方法与重载，因为要对构造合法性进行检查
        /// </summary>
        public Pixel() { }

        private static void InitPixel(Pixel pixel, PixelTypeInfo info, string colorName, Vector2Byte pixelPos)
        {
            pixel.pos = pixelPos;
            pixel.info = info;
            pixel.colorName = colorName;
        }


        #region 静态对象池等数据

        /// <summary>
        /// 每个像素点占世界空间的大小
        /// </summary>
        public static float Size = 0.05f;

        private static List<PixelTypeInfo> pixelTypeInfoList = new List<PixelTypeInfo>();
        private static Dictionary<string, PixelTypeInfo> pixelTypeInfoDic = new Dictionary<string, PixelTypeInfo>();
        #region 对象池
        private static ObjectPool<Pixel> pixelPool = new ObjectPool<Pixel>(Block.Size.x * Block.Size.y * 50, true);
        public static void PutIn(Pixel pixel)
        {
            pixel.info = null;
            pixel.colorName = null;
            pixel.pos = Vector2Byte.max;
            pixelPool.PutIn(pixel);
        }

        public static Pixel TakeOut(PixelToDisk pixelToDisk, Vector2Byte pixelPos) => TakeOut(pixelToDisk.typeName, pixelToDisk.colorName, pixelPos);

        public static Pixel TakeOut(string typeName, string colorName, Vector2Byte pixelPos)
        {
            if (CheckNew(typeName, colorName, pixelPos, out PixelTypeInfo info))
            {
                Pixel pixel = pixelPool.TakeOut();
                InitPixel(pixel, info, colorName, pixelPos);
                return pixel;
            }
            else return null;
        }
        public static Pixel TakeOut(string typeName, int colorIndex, Vector2Byte pixelPos)
        {
            if (CheckNew(typeName, colorIndex, pixelPos, out PixelTypeInfo info, out string colorName))
            {
                Pixel pixel = pixelPool.TakeOut();
                InitPixel(pixel, info, colorName, pixelPos);
                return pixel;
            }
            else return null;
        }
        #endregion
        public static List<string> GetPixelAvailableColors(string typeName)
        {
            if (pixelTypeInfoDic.TryGetValue(typeName, out PixelTypeInfo info)) return info.availableColors;
            else return null;
        }

        public static PixelTypeInfo GetPixelTypeInfo(string typeName)
        {
            if (pixelTypeInfoDic.TryGetValue(typeName, out PixelTypeInfo info)) return info;
            else return null;
        }

        #region 构造函数
        /// <summary>
        /// 请不要使用构造函数，使用此方法与重载，因为要对构造合法性进行检查
        /// </summary>
        /// <returns></returns>
        public static Pixel New(string typeName, string colorName, Vector2Byte pixelPos)
        {
            if (CheckNew(typeName, colorName, pixelPos, out PixelTypeInfo info))
            {
                Pixel pixel = new Pixel();
                InitPixel(pixel, info, colorName, pixelPos);
                return pixel;
            }
            else return null;
        }
        public static Pixel New(string typeName, int colorIndex, Vector2Byte pixelPos)
        {
            if (CheckNew(typeName, colorIndex, pixelPos, out PixelTypeInfo info, out string colorName))
            {
                Pixel pixel = new Pixel();
                InitPixel(pixel, info, colorName, pixelPos);
                return pixel;
            }
            else return null;
        }
        public static Pixel New(PixelToDisk pixelToDisk, Vector2Byte pixelPos) => New(pixelToDisk.typeName, pixelToDisk.colorName, pixelPos);

        private static bool CheckNew(string typeName, string colorName, Vector2Byte pixelPos, out PixelTypeInfo info)
        {
            info = null;
            if (Block.Check(pixelPos) == false) return false;
            if (pixelTypeInfoDic.TryGetValue(typeName, out info))
            {
                if (BlockMaterial.GetPixelColorInfo(colorName) != null) return true;
                else return false;
            }
            else
            {
                Log.Print($"没有名称为 {typeName} 的像素类型", Color.red);
                return false;
            }
        }
        private static bool CheckNew(string typeName, int colorIndex, Vector2Byte pixelPos, out PixelTypeInfo info, out string colorName)
        {
            info = null;
            colorName = null;
            if (Block.Check(pixelPos) == false) return false;
            if (pixelTypeInfoDic.TryGetValue(typeName, out info))
            {
                colorName = info.availableColors[Mathf.Min(colorIndex, info.availableColors.Count - 1)];
                if (BlockMaterial.GetPixelColorInfo(colorName) != null) return true;
                else return false;
            }
            else
            {
                Log.Print($"没有名称为 {typeName} 的像素类型", Color.red);
                return false;
            }
        }
        #endregion
        public static void LoadPixelTypeInfo()
        {
            string rootPath = AssetManager.ExcelToolSaveJsonPath;
            DirectoryInfo root = new DirectoryInfo(rootPath);
            foreach (var fileInfo in root.GetFiles())
            {
                if (fileInfo.Extension != ".json") continue;
                string[] strArray = fileInfo.Name.Split('^');
                if (strArray.Length <= 1 || strArray[0] != "PixelTypeInfo") continue;
                JsonTool.LoadText(fileInfo.FullName, out string infoText);
                Log.Print(fileInfo.FullName, Color.green);
                //加载到的像素数组
                var InfoArray = JsonTool.ToObject<PixelTypeInfo[]>(infoText);
                for (int i = 0; i < InfoArray.Length; i++)
                    if (pixelTypeInfoDic.ContainsKey(InfoArray[i].typeName) == false)
                    {
                        pixelTypeInfoDic.Add(InfoArray[i].typeName, InfoArray[i]);
                        pixelTypeInfoList.Add(InfoArray[i]);
                    }
            }
        }
        #endregion
    }
}
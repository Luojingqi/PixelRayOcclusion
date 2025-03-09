using PRO.DataStructure;
using PRO.Disk;
using PRO.Tool;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace PRO
{
    public class Pixel
    {
        /// <summary>
        /// 在块内坐标
        /// </summary>
        public Vector2Byte pos;
        /// <summary>
        /// 全局坐标
        /// </summary>
        public Vector2Int posG;
        /// <summary>
        /// 类型名称
        /// </summary>
        public PixelTypeInfo typeInfo;
        /// <summary>
        /// 使用的颜色名称
        /// </summary>
        public PixelColorInfo colorInfo;
        /// <summary>
        /// 所在的区块
        /// </summary>
        public BlockBase block;
        /// <summary>
        /// 所属的建筑
        /// </summary>
        public BuildingBase building;
        /// <summary>
        /// 耐久度
        /// </summary>
        public int durability = -1;
        /// <summary>
        /// 透明度影响系数
        /// </summary>
        public float affectsTransparency = 1;

        /// <summary>
        /// 请不要使用构造函数，使用Pixel.New()方法与重载，因为要对构造合法性进行检查
        /// </summary>
        public Pixel() { }

        public Pixel Clone()
        {
            Pixel pixel = pixelPool.TakeOut();
            InitPixel(pixel, typeInfo, colorInfo, pos, durability);
            pixel.affectsTransparency = affectsTransparency;
            return pixel;
        }
        public Pixel Clone(Vector2Byte pos)
        {
            Pixel pixel = pixelPool.TakeOut();
            InitPixel(pixel, typeInfo, colorInfo, pos, durability);
            pixel.affectsTransparency = affectsTransparency;
            return pixel;
        }

        private static void InitPixel(Pixel pixel, PixelTypeInfo typeInfo, PixelColorInfo colorInfo, Vector2Byte pixelPos, int durability)
        {
            pixel.pos = pixelPos;
            pixel.typeInfo = typeInfo;
            pixel.colorInfo = colorInfo;
            if (durability == 0)
                pixel.durability = typeInfo.durability;
            else
                pixel.durability = durability;
        }


        #region 静态对象池等数据

        /// <summary>
        /// 每个像素点占世界空间的大小
        /// </summary>
        public static readonly float Size = 0.05f;

        private static List<PixelTypeInfo> pixelTypeInfoList = new List<PixelTypeInfo>();
        private static Dictionary<string, PixelTypeInfo> pixelTypeInfoDic = new Dictionary<string, PixelTypeInfo>();
        #region 对象池
        public static ObjectPool<Pixel> pixelPool = new ObjectPool<Pixel>();
        public static void PutIn(Pixel pixel, Action<BuildingBase> byUnloadAllPixelAction = null)
        {
            pixel.typeInfo = null;
            pixel.colorInfo = null;
            pixel.pos = Vector2Byte.max;
            pixel.posG = new Vector2Int(int.MaxValue, int.MaxValue);
            pixel.block = null;
            pixel.durability = 0;
            pixel.affectsTransparency = 1;
            if (pixel.building != null)
            {
                pixel.building.UnloadPixel(pixel, byUnloadAllPixelAction);
                pixel.building = null;
            }

            pixelPool.PutIn(pixel);
        }
        public static Pixel TakeOut(PixelTypeInfo typeInfo, PixelColorInfo colorInfo, Vector2Byte pixelPos, int durability = 0)
        {
            if (Block.Check(pixelPos))
            {
                Pixel pixel = pixelPool.TakeOut();
                InitPixel(pixel, typeInfo, colorInfo, pixelPos, durability);
                return pixel;
            }
            else return null;
        }
        public static Pixel TakeOut(string typeName, string colorName, Vector2Byte pixelPos, int durability = 0)
        {
            if (CheckNew(typeName, colorName, pixelPos, out PixelTypeInfo typeInfo, out PixelColorInfo colorInfo))
            {
                Pixel pixel = pixelPool.TakeOut();
                InitPixel(pixel, typeInfo, colorInfo, pixelPos, durability);
                return pixel;
            }
            else return null;
        }
        public static Pixel TakeOut(string typeName, int colorIndex, Vector2Byte pixelPos, int durability = 0)
        {
            if (CheckNew(typeName, colorIndex, pixelPos, out PixelTypeInfo typeInfo, out PixelColorInfo colorInfo))
            {
                Pixel pixel = pixelPool.TakeOut();
                InitPixel(pixel, typeInfo, colorInfo, pixelPos, durability);
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
        public static Pixel 空气;

        #region 构造函数
        /// <summary>
        /// 请不要使用构造函数，使用此方法与重载，因为要对构造合法性进行检查
        /// </summary>
        /// <returns></returns>
        public static Pixel New(string typeName, string colorName, Vector2Byte pixelPos, int durability = -1)
        {
            if (CheckNew(typeName, colorName, pixelPos, out PixelTypeInfo typeInfo, out PixelColorInfo colorInfo))
            {
                Pixel pixel = new Pixel();
                InitPixel(pixel, typeInfo, colorInfo, pixelPos, durability);
                return pixel;
            }
            else return null;
        }
        public static Pixel New(string typeName, int colorIndex, Vector2Byte pixelPos, int durability = -1)
        {
            if (CheckNew(typeName, colorIndex, pixelPos, out PixelTypeInfo typeInfo, out PixelColorInfo colorInfo))
            {
                Pixel pixel = new Pixel();
                InitPixel(pixel, typeInfo, colorInfo, pixelPos, durability);
                return pixel;
            }
            else return null;
        }

        private static bool CheckNew(string typeName, string colorName, Vector2Byte pixelPos, out PixelTypeInfo typeInfo, out PixelColorInfo colorInfo)
        {
            typeInfo = null;
            colorInfo = null;
            if (Block.Check(pixelPos) == false) return false;
            if (pixelTypeInfoDic.TryGetValue(typeName, out typeInfo))
            {
                colorInfo = BlockMaterial.GetPixelColorInfo(colorName);
                if (colorInfo != null) return true;
                else return false;
            }
            else
            {
                Log.Print($"没有名称为 {typeName} 的像素类型", Color.red);
                return false;
            }
        }
        private static bool CheckNew(string typeName, int colorIndex, Vector2Byte pixelPos, out PixelTypeInfo typeInfo, out PixelColorInfo colorInfo)
        {
            typeInfo = null;
            colorInfo = null;
            if (Block.Check(pixelPos) == false) return false;
            if (pixelTypeInfoDic.TryGetValue(typeName, out typeInfo))
            {
                string colorName = typeInfo.availableColors[Mathf.Min(colorIndex, typeInfo.availableColors.Count - 1)];
                colorInfo = BlockMaterial.GetPixelColorInfo(colorName);
                if (colorInfo != null) return true;
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
                {
                    PixelTypeInfo info = InfoArray[i];
                    if (pixelTypeInfoDic.ContainsKey(info.typeName) == false)
                    {
                        if (info.durability == 0)
                        {
                            info.durability = int.MaxValue;
                        }
                        pixelTypeInfoDic.Add(info.typeName, info);
                        pixelTypeInfoList.Add(info);
                    }
                }
            }
        }
        #endregion
    }
}
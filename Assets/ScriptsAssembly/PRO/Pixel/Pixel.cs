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
        /// �ڿ�������
        /// </summary>
        public Vector2Byte pos;
        /// <summary>
        /// ȫ������
        /// </summary>
        public Vector2Int posG;
        /// <summary>
        /// ��������
        /// </summary>
        public PixelTypeInfo typeInfo;
        /// <summary>
        /// ʹ�õ���ɫ����
        /// </summary>
        public PixelColorInfo colorInfo;
        /// <summary>
        /// ���ڵ�����
        /// </summary>
        public BlockBase block;
        /// <summary>
        /// �����Ľ���
        /// </summary>
        public BuildingBase building;
        /// <summary>
        /// �;ö�
        /// </summary>
        public int durability = -1;
        /// <summary>
        /// ͸����Ӱ��ϵ��
        /// </summary>
        public float affectsTransparency = 1;

        /// <summary>
        /// �벻Ҫʹ�ù��캯����ʹ��Pixel.New()���������أ���ΪҪ�Թ���Ϸ��Խ��м��
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


        #region ��̬����ص�����

        /// <summary>
        /// ÿ�����ص�ռ����ռ�Ĵ�С
        /// </summary>
        public static readonly float Size = 0.05f;

        private static List<PixelTypeInfo> pixelTypeInfoList = new List<PixelTypeInfo>();
        private static Dictionary<string, PixelTypeInfo> pixelTypeInfoDic = new Dictionary<string, PixelTypeInfo>();
        #region �����
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
        public static Pixel ����;

        #region ���캯��
        /// <summary>
        /// �벻Ҫʹ�ù��캯����ʹ�ô˷��������أ���ΪҪ�Թ���Ϸ��Խ��м��
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
                Log.Print($"û������Ϊ {typeName} ����������", Color.red);
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
                Log.Print($"û������Ϊ {typeName} ����������", Color.red);
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
                //���ص�����������
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
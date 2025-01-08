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
        /// �����Ľ���
        /// </summary>
        public Building building;

        /// <summary>
        /// �벻Ҫʹ�ù��캯����ʹ��Pixel.New()���������أ���ΪҪ�Թ���Ϸ��Խ��м��
        /// </summary>
        public Pixel() { }

        public Pixel Clone()
        {
            Pixel pixel = pixelPool.TakeOut();
            InitPixel(pixel, typeInfo, colorInfo, pos);
            return pixel;
        }

        private static void InitPixel(Pixel pixel, PixelTypeInfo typeInfo, PixelColorInfo colorInfo, Vector2Byte pixelPos)
        {
            pixel.pos = pixelPos;
            pixel.typeInfo = typeInfo;
            pixel.colorInfo = colorInfo;
        }


        #region ��̬����ص�����

        /// <summary>
        /// ÿ�����ص�ռ����ռ�Ĵ�С
        /// </summary>
        public static float Size = 0.05f;

        private static List<PixelTypeInfo> pixelTypeInfoList = new List<PixelTypeInfo>();
        private static Dictionary<string, PixelTypeInfo> pixelTypeInfoDic = new Dictionary<string, PixelTypeInfo>();
        #region �����
        public static ObjectPool<Pixel> pixelPool = new ObjectPool<Pixel>();
        public static void PutIn(Pixel pixel)
        {
            pixel.typeInfo = null;
            pixel.colorInfo = null;
            pixel.pos = Vector2Byte.max;
            pixel.posG = new Vector2Int(int.MaxValue, int.MaxValue);
            pixelPool.PutIn(pixel);
        }

        public static Pixel TakeOut(Pixel_Disk_Name pixelToDisk, Vector2Byte pixelPos) => TakeOut(pixelToDisk.typeName, pixelToDisk.colorName, pixelPos);

        public static Pixel TakeOut(string typeName, string colorName, Vector2Byte pixelPos)
        {
            if (CheckNew(typeName, colorName, pixelPos, out PixelTypeInfo typeInfo, out PixelColorInfo colorInfo))
            {
                Pixel pixel = pixelPool.TakeOut();
                InitPixel(pixel, typeInfo, colorInfo, pixelPos);
                return pixel;
            }
            else return null;
        }
        public static Pixel TakeOut(string typeName, int colorIndex, Vector2Byte pixelPos)
        {
            if (CheckNew(typeName, colorIndex, pixelPos, out PixelTypeInfo typeInfo, out PixelColorInfo colorInfo))
            {
                Pixel pixel = pixelPool.TakeOut();
                InitPixel(pixel, typeInfo, colorInfo, pixelPos);
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

        #region ���캯��
        /// <summary>
        /// �벻Ҫʹ�ù��캯����ʹ�ô˷��������أ���ΪҪ�Թ���Ϸ��Խ��м��
        /// </summary>
        /// <returns></returns>
        public static Pixel New(string typeName, string colorName, Vector2Byte pixelPos)
        {
            if (CheckNew(typeName, colorName, pixelPos, out PixelTypeInfo typeInfo, out PixelColorInfo colorInfo))
            {
                Pixel pixel = new Pixel();
                InitPixel(pixel, typeInfo, colorInfo, pixelPos);
                return pixel;
            }
            else return null;
        }
        public static Pixel New(string typeName, int colorIndex, Vector2Byte pixelPos)
        {
            if (CheckNew(typeName, colorIndex, pixelPos, out PixelTypeInfo typeInfo, out PixelColorInfo colorInfo))
            {
                Pixel pixel = new Pixel();
                InitPixel(pixel, typeInfo, colorInfo, pixelPos);
                return pixel;
            }
            else return null;
        }
        public static Pixel New(Pixel_Disk_Name pixelToDisk, Vector2Byte pixelPos) => New(pixelToDisk.typeName, pixelToDisk.colorName, pixelPos);

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
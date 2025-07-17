using PRO.DataStructure;
using PRO.Disk;
using PRO.Tool;
using PRO.Tool.Serialize.IO;
using PRO.Tool.Serialize.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace PRO
{
    public class Pixel
    {
        public override string ToString() => $"{pos} {posG} {typeInfo.typeName} {colorInfo.colorName} {blockBase?.BlockPos} {buildingSet.Count}";
        /// <summary>
        /// �ڿ������꣬��������
        /// </summary>
        public Vector2Byte pos;
        /// <summary>
        /// ȫ�����꣬��������
        /// </summary>
        public Vector2Int posG;
        /// <summary>
        /// �������ͣ���������
        /// </summary>
        public PixelTypeInfo typeInfo;
        /// <summary>
        /// ������ɫ����������
        /// </summary>
        public PixelColorInfo colorInfo;
        /// <summary>
        /// ���ڵ����飬��������
        /// </summary>
        public BlockBase blockBase;
        /// <summary>
        /// �����Ľ�������������
        /// </summary>
        public List<BuildingBase> buildingSet = new List<BuildingBase>();
        /// <summary>
        /// �;ö�
        /// </summary>
        public int durability { get => _durability; set { _durability = value; if (blockBase != null) { blockBase.SetPixelInfoToShader(pos); blockBase.DrawPixelAsync(pos, colorInfo.color); } } }
        public int _durability = -1;
        /// <summary>
        /// ͸����Ӱ��ϵ��
        /// </summary>
        public float affectsTransparency { get => _affectsTransparency; set { _affectsTransparency = value; if (blockBase != null) { blockBase.SetPixelInfoToShader(pos); blockBase.DrawPixelAsync(pos, colorInfo.color); } } }
        private float _affectsTransparency = 1;
        /// <summary>
        /// �벻Ҫʹ�ù��캯����ʹ��Pixel.New()���������أ���ΪҪ�Թ���Ϸ��Խ��м��
        /// </summary>
        public Pixel() { }
        /// <summary>
        /// ��¡
        /// ��{ �������ͣ���ɫ���ͣ����꣬�;ã�͸����Ӱ��ϵ�� }��ͬ
        /// </summary>
        public Pixel Clone()
        {
            Pixel pixel = pixelPool.TakeOut();
            InitPixel(pixel, typeInfo, colorInfo, pos, durability);
            pixel._affectsTransparency = _affectsTransparency;
            pixel.posG = posG;
            return pixel;
        }
        /// <summary>
        /// ��¡���������þֲ�����
        /// ��{ �������ͣ���ɫ���ͣ��ֲ����꣬�;ã�͸����Ӱ��ϵ�� }��ͬ
        /// </summary>
        public Pixel Clone(Vector2Byte pos)
        {
            Pixel pixel = pixelPool.TakeOut();
            InitPixel(pixel, typeInfo, colorInfo, pos, durability);
            pixel._affectsTransparency = _affectsTransparency;
            return pixel;
        }
        /// <summary>
        /// ��¡���������þֲ�����
        /// ��{ �������ͣ���ɫ���ͣ��ֲ����꣬�;ã�͸����Ӱ��ϵ�� }��ͬ
        /// </summary>
        public Pixel CloneTo(Pixel pixel, Vector2Byte pos)
        {
            InitPixel(pixel, typeInfo, colorInfo, pos, durability);
            pixel._affectsTransparency = _affectsTransparency;
            return pixel;
        }

        public static void InitPixel(Pixel pixel, PixelTypeInfo typeInfo, PixelColorInfo colorInfo, Vector2Byte pixelPos, int durability)
        {
            pixel.pos = pixelPos;
            pixel.typeInfo = typeInfo;
            pixel.colorInfo = colorInfo;
            if (durability == 0)
                pixel._durability = typeInfo.durability;
            else
                pixel._durability = durability;
        }


        #region ��̬����ص�����

        /// <summary>
        /// ÿ�����ص�ռ����ռ�Ĵ�С
        /// </summary>
        public static readonly float Size = 0.05f;

        private static List<PixelTypeInfo> pixelTypeInfoList = new List<PixelTypeInfo>();
        private static Dictionary<string, PixelTypeInfo> name_pixelTypeInfo_Dic = new Dictionary<string, PixelTypeInfo>();
        private static Dictionary<string, List<PixelColorInfo>> tag_pixelTypeInfoList_Dic = new Dictionary<string, List<PixelColorInfo>>();
        #region �����
        public readonly static ObjectPool<Pixel> pixelPool = new ObjectPool<Pixel>();
        public static void PutIn(Pixel pixel)
        {
            if (pixel == null) return;
            pixel.typeInfo = null;
            pixel.colorInfo = null;
            pixel.pos = Vector2Byte.max;
            pixel.posG = new Vector2Int(int.MaxValue, int.MaxValue);
            pixel.blockBase = null;
            pixel._durability = 0;
            pixel._affectsTransparency = 1;
            foreach (var building in pixel.buildingSet)
                building.UnloadPixel(pixel);
            pixel.buildingSet.Clear();
            lock (pixelPool)
                pixelPool.PutIn(pixel);
        }
        /// <summary>
        /// �̲߳���ȫ
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <param name="colorInfo"></param>
        /// <param name="pixelPos"></param>
        /// <param name="durability"></param>
        /// <returns></returns>
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
        public static string[] GetPixelAvailableColors(string typeName)
        {
            if (name_pixelTypeInfo_Dic.TryGetValue(typeName, out PixelTypeInfo info)) return info.availableColors;
            else return null;
        }

        public static PixelTypeInfo GetPixelTypeInfo(string typeName)
        {
            if (name_pixelTypeInfo_Dic.TryGetValue(typeName, out PixelTypeInfo info)) return info;
            else return null;
        }
        public static Pixel ����;

        #region ���캯��
        /// <summary>
        /// �벻Ҫʹ�ù��캯����ʹ�ô˷��������أ���ΪҪ�Թ���Ϸ��Խ��м��
        /// </summary>
        /// <returns></returns>
        public static Pixel New(string typeName, string colorName, Vector2Byte pixelPos, int durability = 0)
        {
            if (CheckNew(typeName, colorName, pixelPos, out PixelTypeInfo typeInfo, out PixelColorInfo colorInfo))
            {
                Pixel pixel = new Pixel();
                InitPixel(pixel, typeInfo, colorInfo, pixelPos, durability);
                return pixel;
            }
            else return null;
        }
        public static Pixel New(string typeName, int colorIndex, Vector2Byte pixelPos, int durability = 0)
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
            if (name_pixelTypeInfo_Dic.TryGetValue(typeName, out typeInfo))
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
            if (name_pixelTypeInfo_Dic.TryGetValue(typeName, out typeInfo))
            {
                string colorName = typeInfo.availableColors[Mathf.Min(colorIndex, typeInfo.availableColors.Length - 1)];
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
        /// <summary>
        /// ��������������Ϣ
        /// </summary>
        public static void LoadPixelTypeInfo()
        {
            string rootPath = AssetManager.ExcelToolSaveJsonPath;
            DirectoryInfo root = new DirectoryInfo(rootPath);
            foreach (var fileInfo in root.GetFiles())
            {
                if (fileInfo.Extension != ".json") continue;
                string[] strArray = fileInfo.Name.Split('^');
                if (strArray.Length <= 1 || strArray[0] != "PixelTypeInfo") continue;
                IOTool.LoadText(fileInfo.FullName, out string infoText);
                Log.Print(fileInfo.FullName, Color.green);
                //���ص�����������
                var InfoArray = JsonTool.ToObject<PixelTypeInfo[]>(infoText);
                for (int i = 0; i < InfoArray.Length; i++)
                {
                    PixelTypeInfo info = InfoArray[i];
                    if (name_pixelTypeInfo_Dic.ContainsKey(info.typeName) == false)
                    {
                        if (info.durability == 0)
                        {
                            info.durability = int.MaxValue;
                        }
                        if (info.tags == null) info.tags = new HashSet<string>();

                        name_pixelTypeInfo_Dic.Add(info.typeName, info);
                        pixelTypeInfoList.Add(info);

                        //if(tag_pixelTypeInfoList_Dic)
                    }
                }
            }
            Pixel.���� = Pixel.TakeOut("����", 0, new());
        }
        #endregion
    }
}
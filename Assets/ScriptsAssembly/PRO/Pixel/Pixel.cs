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
        /// 在块内坐标，不可设置
        /// </summary>
        public Vector2Byte pos;
        /// <summary>
        /// 全局坐标，不可设置
        /// </summary>
        public Vector2Int posG => Block.PixelToGlobal(blockBase.BlockPos, pos);
        /// <summary>
        /// 像素类型，不可设置
        /// </summary>
        public PixelTypeInfo typeInfo;
        /// <summary>
        /// 像素颜色，不可设置
        /// </summary>
        public PixelColorInfo colorInfo;
        /// <summary>
        /// 所在的区块，不可设置
        /// </summary>
        public BlockBase blockBase;
        /// <summary>
        /// 所属的建筑，不可设置
        /// </summary>
        public HashSet<BuildingBase> buildingSet = new HashSet<BuildingBase>();
        /// <summary>
        /// 耐久度
        /// </summary>
        public int durability
        {
            get => _durability;
            set
            {
                _durability = value;
#if PRO_RENDER
                blockBase.SetPixelInfoToShader(pos);
                blockBase.DrawPixelAsync(pos, colorInfo.color);
#endif
            }
        }
        private int _durability;
        /// <summary>
        /// 透明度影响系数
        /// </summary>
        public float affectsTransparency
        {
            get => _affectsTransparency;
            set
            {
                _affectsTransparency = value;
#if PRO_RENDER
                blockBase.SetPixelInfoToShader(pos);
                blockBase.DrawPixelAsync(pos, colorInfo.color);
#endif
            }
        }
        private float _affectsTransparency;

        public Pixel(PixelTypeInfo typeInfo, PixelColorInfo colorInfo, BlockBase blockBase, Vector2Byte pos)
        {
            this.typeInfo = typeInfo;
            this.colorInfo = colorInfo;
            this.pos = pos;
            this.blockBase = blockBase;
            this._durability = typeInfo.durability;
            this._affectsTransparency = 1;
        }
        public void Replace(string typeInfoName, int colorIndex)
        {
            var typeInfo = Pixel.GetPixelTypeInfo(typeInfoName);
            if (typeInfo == null || typeInfo.availableColors.Length == 0) return;
            if (colorIndex > typeInfo.availableColors.Length) colorIndex = typeInfo.availableColors.Length - 1;
            var colorInfo = Pixel.GetPixelColorInfo(typeInfo.availableColors[colorIndex]);
            if (colorInfo == null) return;
            Replace(typeInfo, colorInfo);
        }
        public void Replace(string typeInfoName, string colorInfoName)
        {
            var typeInfo = Pixel.GetPixelTypeInfo(typeInfoName);
            var colorInfo = Pixel.GetPixelColorInfo(colorInfoName);
            if (typeInfo != null && colorInfo != null)
                Replace(typeInfo, colorInfo);
        }
        public void Replace(Pixel pixel)
        {
            Replace(pixel.typeInfo, pixel.colorInfo);
            _durability = pixel.durability;
            _affectsTransparency = pixel.affectsTransparency;
        }
        public void Replace(PixelTypeInfo newTypeInfo, PixelColorInfo newColorInfo)
        {
            var oldTypeInfo = typeInfo;
            var oldColorInfo = colorInfo;

            typeInfo = newTypeInfo;
            colorInfo = newColorInfo;
            _durability = newTypeInfo.durability;
            _affectsTransparency = 1;

            //切换建筑这个点的状态（死亡||存活
            foreach (var building in buildingSet)
                building.PixelSwitch(building.GetBuilding_Pixel(posG, blockBase.blockType), oldTypeInfo, oldColorInfo);

#if PRO_RENDER
            if (newColorInfo.luminousRadius > 0 && newColorInfo.luminousRadius <= BlockMaterial.LightRadiusMax)
                blockBase.AddLightSource(pos, newColorInfo);
            else if (oldColorInfo.luminousRadius > 0)
                blockBase.RemoveLightSource(pos);
            blockBase.textureData.SetPixelInfoToShader(this);
            blockBase.DrawPixelAsync(pos, newColorInfo.color);
#endif

            if (blockBase is Block block)
            {
                block.ChangeCollider(oldTypeInfo, newTypeInfo, pos);
                for (int y = -2; y <= 2; y++)
                    for (int x = -2; x <= 2; x++)
                        Block.AddFluidUpdateHash(posG + new Vector2Int(x, y));
            }

            if (newTypeInfo.typeName == "火焰") blockBase.queue_火焰.Enqueue(pos);
        }

        public void ToRAM(Flat.PixelData pixelDiskData, PixelTypeInfo[] typeInfoArray, PixelColorInfo[] colorInfoArray, BuildingBase[] buildingArray)
        {
            typeInfo = typeInfoArray[pixelDiskData.TypeIndex];
            colorInfo = colorInfoArray[pixelDiskData.ColorIndex];
            _durability = pixelDiskData.Durability;
            _affectsTransparency = pixelDiskData.AffectsTransparency;
            for (int i = pixelDiskData.BuildingListLength - 1; i >= 0; i--)
            {
                var building = buildingArray[pixelDiskData.BuildingList(i)];
                building.ToRAM_PixelSwitch(building.GetBuilding_Pixel(posG, blockBase.blockType), this);
            }
#if PRO_RENDER
            if (colorInfo.luminousRadius > 0 && colorInfo.luminousRadius <= BlockMaterial.LightRadiusMax)
                blockBase.AddLightSource(pos, colorInfo);
            blockBase.textureData.SetPixelInfoToShader(this);
#endif
        }
        #region 静态数据

        /// <summary>
        /// 每个像素点占世界空间的大小
        /// </summary>
        public static readonly float Size = 0.05f;
        public static readonly float Size_Half = Size / 2f;

        private static List<PixelTypeInfo> pixelTypeInfoList = new List<PixelTypeInfo>();
        private static Dictionary<string, PixelTypeInfo> name_pixelTypeInfo_Dic = new Dictionary<string, PixelTypeInfo>();
        private static Dictionary<string, List<PixelColorInfo>> tag_pixelTypeInfoList_Dic = new Dictionary<string, List<PixelColorInfo>>();
        public void Clear()
        {
            typeInfo = null;
            colorInfo = null;
            _durability = 0;
            _affectsTransparency = 1;
            foreach (var building in buildingSet)
                building.UnloadPixel(building.GetBuilding_Pixel(posG, blockBase.blockType));
            buildingSet.Clear();
        }

        public static PixelTypeInfo GetPixelTypeInfo(string typeName)
        {
            if (name_pixelTypeInfo_Dic.TryGetValue(typeName, out PixelTypeInfo info)) return info;
            else return null;
        }
        public static PixelColorInfo GetPixelColorInfo(string colorName)
        {
            if (BlockMaterial.pixelColorInfoDic.TryGetValue(colorName, out PixelColorInfo value)) return value;
            else Debug.Log($"没有像素颜色名称为{colorName}");
            return null;
        }
        public static Pixel 空气;

        /// <summary>
        /// 加载像素类型信息
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
                //加载到的像素数组
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
            空气 = new Pixel(GetPixelTypeInfo("空气"), GetPixelColorInfo("空气色"), null, new());
        }
        #endregion
    }
}
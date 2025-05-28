using PRO.DataStructure;
using PRO.Disk;
using PRO.Proto.Block;
using System;
using System.Collections.Generic;

namespace PRO
{
    /// <summary>
    /// 块数据存储到磁盘与从磁盘中加载的类
    /// </summary>
    public abstract partial class BlockBase
    {
        public abstract void ToDisk(ref BlockBaseData data);

        public abstract void ToRAM(BlockBaseData data);

        public BlockBaseData ToDisk()
        {
            var diskData = new BlockBaseData();
            for (int y = 0; y < Block.Size.y; y++)
                for (int x = 0; x < Block.Size.x; x++)
                {
                    Pixel pixel = GetPixel(new Vector2Byte(x, y));
                    if (diskData.TypeNameIndexDic.TryGetValue(pixel.typeInfo.typeName, out int typeNameIndex) == false)
                    {
                        typeNameIndex = diskData.TypeNameIndexDic.Count;
                        diskData.TypeNameIndexDic.Add(pixel.typeInfo.typeName, typeNameIndex);
                    }
                    if (diskData.ColorNameIndexDic.TryGetValue(pixel.colorInfo.colorName, out int colorNameIndex) == false)
                    {
                        colorNameIndex = diskData.ColorNameIndexDic.Count;
                        diskData.ColorNameIndexDic.Add(pixel.colorInfo.colorName, colorNameIndex);
                    }
                    var pixelData = new BlockBaseData.Types.PixelData();
                    pixelData.TypeIndex = typeNameIndex;
                    pixelData.ColorIndex = colorNameIndex;
                    pixelData.Durability = pixel.durability;
                    pixelData.AffectsTransparency = pixel.affectsTransparency;

                    foreach (var building in pixel.buildingSet)
                    {
                        if (diskData.BuildingGuidIndexDic.TryGetValue(building.GUID, out int buildingIndex) == false)
                        {
                            buildingIndex = diskData.BuildingGuidIndexDic.Count + 1;
                            diskData.BuildingGuidIndexDic.Add(building.GUID, buildingIndex);
                        }
                        pixelData.BuildingList.Add(buildingIndex);
                    }
                    diskData.AllPixel.Add(pixelData);
                }
            ToDisk(ref diskData);
            return diskData;
        }
        public void ToRAM(BlockBaseData diskData, SceneEntity sceneEntity)
        {
            Dictionary<int, PixelTypeInfo> typeNameDic = new Dictionary<int, PixelTypeInfo>();
            Dictionary<int, PixelColorInfo> colorNameDic = new Dictionary<int, PixelColorInfo>();
            Dictionary<int, BuildingBase> buildingDic = new Dictionary<int, BuildingBase>();

            foreach (var kv in diskData.TypeNameIndexDic)
                typeNameDic.Add(kv.Value, Pixel.GetPixelTypeInfo(kv.Key));
            foreach (var kv in diskData.ColorNameIndexDic)
                colorNameDic.Add(kv.Value, BlockMaterial.GetPixelColorInfo(kv.Key));
            foreach (var kv in diskData.BuildingGuidIndexDic)
            {
                var building = sceneEntity.GetBuilding(kv.Key);
                if (building == null)
                    SceneManager.Inst.AddMainThreadEvent_Clear_WaitInvoke_Lock(() => sceneEntity.LoadBuilding(kv.Key));
                buildingDic.Add(kv.Value, sceneEntity.GetBuilding(kv.Key));
            }

            for (int i = 0; i < diskData.AllPixel.Count; i++)
            {
                var pixelData = diskData.AllPixel[i];
                Pixel pixel = null;
                lock (Pixel.pixelPool)
                    pixel = Pixel.pixelPool.TakeOut();
                Pixel.InitPixel(pixel, typeNameDic[pixelData.TypeIndex], colorNameDic[pixelData.ColorIndex], new(i % Block.Size.y, i / Block.Size.y), pixelData.Durability);
                pixel.affectsTransparency = pixelData.AffectsTransparency;
                foreach (var buildingIndex in pixelData.BuildingList)
                {
                    var building = buildingDic[buildingIndex];
                    pixel.buildingSet.Add(building);
                    building.Deserialize_PixelSwitch(building.GetBuilding_Pixel(pixel.posG, pixel.blockBase.blockType), pixel);
                }
                SetPixel(pixel, false, false, false);
            }
            ToRAM(diskData);
        }
    }
}

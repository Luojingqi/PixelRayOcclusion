using PRO.DataStructure;
using PRO.Disk;
using System.Collections.Generic;

namespace PRO
{
    /// <summary>
    /// 块数据存储到磁盘与从磁盘中加载的类
    /// </summary>
    public abstract partial class BlockBase
    {
        public virtual void ToDisk(ref Proto.BlockBaseData diskData)
        {
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
                    var pixelData = Proto.ProtoPool.TakeOut<Proto.BlockBaseData.Types.PixelData>();
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
                    switch (blockType)
                    {
                        case BlockType.Block: diskData.BlockPixelArray.Add(pixelData); break;
                        case BlockType.BackgroundBlock: diskData.BackgroundPixelArray.Add(pixelData); break;
                    }
                }
        }
        public virtual void ToRAM(Proto.BlockBaseData diskData, SceneEntity sceneEntity)
        {
            Dictionary<int, PixelTypeInfo> typeNameDic = new Dictionary<int, PixelTypeInfo>(diskData.TypeNameIndexDic.Count);
            Dictionary<int, PixelColorInfo> colorNameDic = new Dictionary<int, PixelColorInfo>(diskData.ColorNameIndexDic.Count);
            Dictionary<int, BuildingBase> buildingDic = new Dictionary<int, BuildingBase>(diskData.BuildingGuidIndexDic.Count);

            foreach (var kv in diskData.TypeNameIndexDic)
                typeNameDic.Add(kv.Value, Pixel.GetPixelTypeInfo(kv.Key));
            foreach (var kv in diskData.ColorNameIndexDic)
                colorNameDic.Add(kv.Value, BlockMaterial.GetPixelColorInfo(kv.Key));
            foreach (var kv in diskData.BuildingGuidIndexDic)
            {
                var building = sceneEntity.GetBuilding(kv.Key);
                if (building == null)
                    TimeManager.Inst.AddToQueue_MainThreadUpdate_Clear_WaitInvoke(() => sceneEntity.LoadBuilding(kv.Key));
                buildingDic.Add(kv.Value, sceneEntity.GetBuilding(kv.Key));
            }
            Google.Protobuf.Collections.RepeatedField<Proto.BlockBaseData.Types.PixelData> pixelArray = null;
            switch (blockType)
            {
                case BlockType.Block: pixelArray = diskData.BlockPixelArray; break;
                case BlockType.BackgroundBlock:pixelArray = diskData.BackgroundPixelArray; break;
            }
            for (int i = 0; i < pixelArray.Count; i++)
            {
                var pixelData = pixelArray[i];
                Pixel pixel = null;
                lock (Pixel.pixelPool)
                    pixel = Pixel.pixelPool.TakeOut();
                Pixel.InitPixel(pixel, typeNameDic[pixelData.TypeIndex], colorNameDic[pixelData.ColorIndex], new(i % Block.Size.y, i / Block.Size.y), pixelData.Durability);
                pixel.affectsTransparency = pixelData.AffectsTransparency;
                foreach (var buildingIndex in pixelData.BuildingList)
                {
                    var building = buildingDic[buildingIndex];
                    pixel.buildingSet.Add(building);
                    building.ToRAM_PixelSwitch(building.GetBuilding_Pixel(pixel.posG, pixel.blockBase.blockType), pixel);
                }
                SetPixel(pixel, false, false, false);
            }
        }
    }
}

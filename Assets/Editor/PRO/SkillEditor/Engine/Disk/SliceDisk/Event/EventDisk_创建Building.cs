using PRO.Tool;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PRO.SkillEditor
{
    public class EventDisk_创建Building : EventSlice_DiskBase
    {
        public Type type;

        public Vector2Int size;


        public List<PixelData> pixelList = new List<PixelData>();

        public override void UpdateFrame(SkillPlayAgent agent, int frame, int frameIndex, int trackIndex)
        {
            if (frameIndex == frameLength - 1)
            {
                var nor = PixelPosRotate.New(agent.transform.rotation.eulerAngles);
                Vector2Int agentGlobalPos = Block.WorldToGlobal(agent.transform.position);

                var building = BuildingBase.New(type, Guid.NewGuid().ToString(), agent.Scene);
                building.TriggerCollider.size = (Vector2)size * Pixel.Size;
                building.TriggerCollider.offset = building.TriggerCollider.size / 2f;
                building.transform.position = Block.GlobalToWorld(agentGlobalPos);
                building.transform.rotation = agent.transform.rotation;
                building.global = agentGlobalPos;
                building.Size = size;

                foreach (var pixelData in pixelList)
                {
                    Building_Pixel building_Pixel = Building_Pixel.TakeOut().Init(Pixel.GetPixelTypeInfo(pixelData.typeName), BlockMaterial.GetPixelColorInfo(pixelData.colorName), pixelData.pos, pixelData.blockType);
                    building.Deserialize_AddBuilding_Pixel(building_Pixel);
                }


                building.Scene.BuildingInRAM.Add(building.GUID, building);
                building.Scene.sceneCatalog.buildingTypeDic.Add(building.GUID, building.GetType());

                foreach (var pixelData in pixelList)
                {
                    Vector2Int globalPos = agentGlobalPos + nor.RotatePos(pixelData.pos);
                    BlockBase blockBase = building.Scene.GetBlockBase(pixelData.blockType, Block.GlobalToBlock(globalPos));
                    if (blockBase == null) continue;
                    Pixel pixel = blockBase.GetPixel(Block.GlobalToPixel(globalPos));
                    building.Deserialize_PixelSwitch(building.GetBuilding_Pixel(pixel.posG, pixelData.blockType), pixel);
                }
                building.Init();
            }
        }

        public struct PixelData
        {
            public string typeName;
            public string colorName;
            public Vector2Int pos;
            public BlockBase.BlockType blockType;
        }

        public class BufferData : ISliceBufferData
        {
            public BuildingBase value;
            public void PutIn()
            {
                value = null;
                pool.PutIn(this);
            }
            public static BufferData TakeOut()
            {
                return pool.TakeOut();
            }

            private static ObjectPool<BufferData> pool = new ObjectPool<BufferData>();
        }
    }
}

using PRO.DataStructure;
using PRO.Disk;
using PRO.Tool;
using System.Collections.Generic;
using UnityEngine;

namespace PRO.SkillEditor
{
    public class SceneCreate_Disk : Slice_DiskBase
    {
        public BlockBase.BlockType blockType;
        public List<PixelData> pixelList = new List<PixelData>();
        /// <summary>
        /// 尝试破坏旧像素点还是强制破坏旧像素点
        /// </summary>
        public bool tryRuinOrForceRuin = true;

        public override void UpdateFrame(SkillPlayAgent agent, int frame, int frameIndex, int trackIndex)
        {
            try
            {
                var nor = PixelPosRotate.New(agent.transform.rotation.eulerAngles);
                Vector2Int agentPos = Block.WorldToGlobal(agent.transform.position);
                if (tryRuinOrForceRuin)
                {
                    foreach (var data in pixelList)
                    {
                        Vector2Int gloabPos = agentPos + nor.RotatePos(data.pos);
                        BlockBase blockBase = agent.Scene.GetBlockBase(blockType, Block.GlobalToBlock(gloabPos));
                        if (blockBase == null) continue;
                        Vector2Byte pixelPos = Block.GlobalToPixel(gloabPos);
                        Pixel oldPixel = blockBase.GetPixel(pixelPos);
                        PixelTypeInfo pixelTypeInfo = Pixel.GetPixelTypeInfo(data.typeName);
                        PixelColorInfo pixelColorInfo = BlockMaterial.GetPixelColorInfo(data.colorName);


                        if (oldPixel.typeInfo.typeName == data.lastTypeName && oldPixel.colorInfo.colorName == data.colorName)
                        {
                            Pixel pixel = Pixel.TakeOut(pixelTypeInfo, pixelColorInfo, pixelPos);
                            blockBase.SetPixel(pixel);
                        }
                        else
                        {
                            blockBase.TryDestroyPixel(pixelPos, pixelTypeInfo.hardness);
                            if (blockBase.GetPixel(pixelPos).typeInfo.typeName == "空气")
                            {
                                Pixel pixel = Pixel.TakeOut(pixelTypeInfo, pixelColorInfo, pixelPos);
                                blockBase.SetPixel(pixel);
                            }
                        }
                    }
                }
                else
                {
                    foreach (var data in pixelList)
                    {
                        Vector2Int gloabPos = agentPos + nor.RotatePos(data.pos);
                        BlockBase blockBase = agent.Scene.GetBlockBase(blockType, Block.GlobalToBlock(gloabPos));
                        if (blockBase == null) continue;
                        Vector2Byte pixelPos = Block.GlobalToPixel(gloabPos);
                        Pixel oldPixel = blockBase.GetPixel(pixelPos);
                        PixelTypeInfo pixelTypeInfo = Pixel.GetPixelTypeInfo(data.typeName);
                        PixelColorInfo pixelColorInfo = BlockMaterial.GetPixelColorInfo(data.colorName);

                        Pixel pixel = Pixel.TakeOut(pixelTypeInfo, pixelColorInfo, pixelPos);
                        blockBase.SetPixel(pixel);
                    }
                }
            }
            catch
            {
                Debug.Log("请在运行模式下查看效果：场景创建轨道");
            }
        }

        public struct PixelData
        {
            public string typeName;
            public string colorName;
            public Vector2Int pos;
            public string lastTypeName;
            public string lastColorName;
        }
    }
}
namespace PRO
{
    public struct PixelPosRotate
    {
        public int z90Num;
        public int y180Num;
        public int x180Num;
        private Vector2Int xyCoefficient;

        public bool activeZ;
        public bool activeY;
        public bool activeX;
        public static PixelPosRotate New(int z90Num, int y180Num, int x180Num)
        {
            var nor = new PixelPosRotate();
            nor.z90Num = z90Num % 4; if (nor.z90Num < 0) nor.z90Num += 4;
            nor.y180Num = y180Num % 2; if (nor.y180Num < 0) nor.y180Num += 2;
            nor.x180Num = x180Num % 2; if (nor.x180Num < 0) nor.x180Num += 2;
            nor.xyCoefficient = Vector2Int.one;
            if (nor.y180Num == 1)
                nor.xyCoefficient.x *= -1;
            if (nor.x180Num == 1)
                nor.xyCoefficient.y *= -1;
            nor.activeZ = true;
            nor.activeY = true;
            nor.activeX = true;
            return nor;
        }
        public static PixelPosRotate New(Vector3 angle)
        {
            var nor = new PixelPosRotate();
            for (int i = 0; i < 3; i++)
                angle[i] %= 360f;
            return New((int)(angle.z / 90f), (int)(angle.y / 180f), (int)(angle.x / 180f));
        }


        public Vector2Int RotatePos(Vector2Int pos)
        {
            if (activeZ)
                switch (z90Num)
                {
                    case 1: pos = new Vector2Int(-pos.y, pos.x); break;
                    case 2: pos = new Vector2Int(-pos.x, -pos.y); break;
                    case 3: pos = new Vector2Int(pos.y, -pos.x); break;
                }
            if (activeY) pos.x *= xyCoefficient.x;
            if (activeX) pos.y *= xyCoefficient.y;
            return pos;
        }
        public Vector2Int RotatePosInverse(Vector2Int pos)
        {
            if (activeX) pos.y *= xyCoefficient.y;
            if (activeY) pos.x *= xyCoefficient.x;
            if (activeZ)
                switch (4 - z90Num)
                {
                    case 1: pos = new Vector2Int(-pos.y, pos.x); break;
                    case 2: pos = new Vector2Int(-pos.x, -pos.y); break;
                    case 3: pos = new Vector2Int(pos.y, -pos.x); break;
                }
            return pos;
        }


        private static double pixelOffset = Pixel.Size * 0.99999;
        public void RotateTransform(Transform transform, Vector2Int globalOffset)
        {
            transform.rotation = GetQuaternion();
            globalOffset = RotatePos(globalOffset);
            Vector3 position = Block.GlobalToWorld(Block.WorldToGlobal(transform.position) - globalOffset);
            Vector2Int oneRotate = RotatePos(new Vector2Int(1, 1));
            if (oneRotate.x < 0) position.x = (float)(position.x + pixelOffset);
            if (oneRotate.y < 0) position.y = (float)(position.y + pixelOffset);
            transform.position = position;
        }
        public Vector3 GetAngle() => new Vector3(x180Num * 180f, y180Num * 180f, z90Num * 90f);
        public Quaternion GetQuaternion() => Quaternion.Euler(GetAngle());
    }
}

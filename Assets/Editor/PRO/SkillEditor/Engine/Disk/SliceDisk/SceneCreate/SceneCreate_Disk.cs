using PRO.DataStructure;
using System.Collections.Generic;
using UnityEngine;

namespace PRO.SkillEditor
{
    public class SceneCreate_Disk : Slice_DiskBase
    {
        public List<PixelData> CreatePixelList = new List<PixelData>();
        public Vector2Int offset;

        public override void UpdateFrame(SkillPlayAgent agent, int frame, int frameIndex, int trackIndex)
        {
            try
            {
                var nor = PixelPosRotate.New(agent.transform.rotation.eulerAngles);
                Vector2Int agentPos = Block.WorldToGlobal(agent.transform.position);
                foreach (var data in CreatePixelList)
                {
                    Vector2Int gloabPos = agentPos + nor.RotatePos(data.pos + offset);
                    Block block = SceneManager.Inst.NowScene.GetBlock(Block.GlobalToBlock(gloabPos));
                    Vector2Byte pixelPos = Block.GlobalToPixel(gloabPos);
                    Pixel pixel = Pixel.TakeOut(data.typeName, data.colorName, pixelPos);
                    block.SetPixel(pixel);
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
        }
    }

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
        private static float pixelOffset = Pixel.Size * 0.99999f;
        public void RotateTransform(Transform transform, Vector2Int globalOffset)
        {
            transform.rotation = GetQuaternion();
            globalOffset = RotatePos(globalOffset);
            Vector3 position = Block.GlobalToWorld(MousePoint.globalPos - globalOffset);
            Vector2Int oneRotate = RotatePos(new Vector2Int(1, 1));
            if (oneRotate.x < 0) position.x += pixelOffset;
            if (oneRotate.y < 0) position.y += pixelOffset;
            transform.position = position;
        }
        public Vector3 GetAngle() => new Vector3(x180Num * 180f, y180Num * 180f, z90Num * 90f);
        public Quaternion GetQuaternion() => Quaternion.Euler(GetAngle());
    }
}

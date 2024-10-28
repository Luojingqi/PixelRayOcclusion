using PRO.DataStructure;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace PRO
{
    /// <summary>
    /// 贪心算法生成碰撞箱
    /// </summary>
    public static class GreedyCollider
    {
        public struct ColliderData
        {
            /// <summary>
            /// colider的size
            /// </summary>
            public Vector2 size;
            /// <summary>
            /// 世界坐标
            /// </summary>
            public Vector2 position;
            /// <summary>
            /// 在块内的点
            /// </summary>
            public Vector2Byte pos;
            /// <summary>
            /// 在块内的大小==size/Pixel.Size
            /// </summary>
            public Vector2Byte length;
        }
        /// <summary>
        /// 返回用于创建碰撞箱的数据集合
        /// </summary>
        public static List<ColliderData> CreateColliderDataList(Block block, Vector2Byte min, Vector2Byte max)
        {
            HashSet<Vector2Int> hash = new HashSet<Vector2Int>();
            List<ColliderData> colliderDataList = new List<ColliderData>();
            for (int y = max.y; y >= min.y; y--)
                for (int x = min.x; x <= max.x; x++)
                    if (!hash.Contains(new Vector2Int(x, y)) && Pixel.GetPixelTypeInfo(block.GetPixelRelocation(x, y).typeName).collider)
                    {
                        hash.Add(new Vector2Int(x, y));

                        ColliderData colliderData = new ColliderData();
                        colliderData.size = new Vector2(Pixel.Size, Pixel.Size);
                        colliderData.length = new Vector2Byte(1, 1);
                        colliderData.position = block.PixelToWorld(new Vector2Byte(x, y));
                        colliderData.pos = new Vector2Byte(x, y);
                        int xShifting = x + 1;
                        while (xShifting <= max.x && !hash.Contains(new Vector2Int(xShifting, y)) && Pixel.GetPixelTypeInfo(block.GetPixelRelocation(x, y).typeName).collider)
                            hash.Add(new Vector2Int(xShifting++, y));
                        xShifting = xShifting - x;
                        colliderData.size.x = Pixel.Size * xShifting;
                        colliderData.length.x = (byte)xShifting;
                        int yShifting = y - 1;
                        bool ok = true;
                        while (ok && yShifting >= min.y)
                        {
                            for (int i = 0; i < xShifting; i++)
                            {
                                if (!hash.Contains(new Vector2Int(x + i, yShifting)) && Pixel.GetPixelTypeInfo(block.GetPixelRelocation(x, y).typeName).collider)
                                {
                                    ok = false;
                                    break;
                                }
                            }
                            if (ok)
                            {
                                for (int i = 0; i < xShifting; i++)
                                    hash.Add(new Vector2Int(i + x, yShifting));
                                yShifting--;
                            }
                        }
                        yShifting++;
                        colliderData.size.y = Pixel.Size * (y - yShifting + 1);
                        colliderData.length.y = (byte)(y - yShifting + 1);
                        colliderData.position = block.PixelToWorld(new Vector2Byte(x, yShifting));
                        colliderData.pos = new Vector2Byte(x, yShifting);
                        colliderDataList.Add(colliderData);
                    }
            return colliderDataList;
        }

        /// <summary>
        /// 根据提供的碰撞箱数据列表在区块内对应位置创建碰撞箱
        /// </summary>
        public static void CreateColliderAction(Block block, List<ColliderData> colliderDataList, bool bb = false)
        {
            for (int i = 0; i < colliderDataList.Count; i++)
            {
                Color32 color = new Color32((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255), 255);
                var data = colliderDataList[i];
                var box = BlockManager.Inst.BoxCollider2DPool.TakeOutT();
                box.size = data.size;
                box.transform.position = data.position;

                box.offset = box.size / 2;
                box.transform.parent = block.colliderNode;
                if (bb) box.transform.parent = null;
                for (byte x = data.pos.x; x < data.pos.x + data.length.x; x++)
                    for (byte y = data.pos.y; y < data.pos.y + data.length.y; y++)
                    {
                        block.allCollider[x, y] = box;
                        block.DrawPixelAsync(new Vector2Byte(x, y), color);
                    }
            }
        }
        /// <summary>
        /// 尝试在一点内创建新碰撞箱，自动与周围可合成的碰撞箱合成
        /// </summary>
        public static void TryExpandCollider(Block block, Vector2Byte pos)
        {
            if (Block.Check(pos) == false || block.allCollider[pos.x, pos.y] != null) return;
            Span<Vector2Byte> span = stackalloc Vector2Byte[] { pos + new Vector2Byte(1, 0), pos + new Vector2Byte(-1, 0), pos + new Vector2Byte(0, 1), pos + new Vector2Byte(0, -1) };
            for (int i = 0; i < 4; i++)
            {
                if (Block.Check(span[i]) == false) continue;
                var box = block.allCollider[span[i].x, span[i].y];
                if (box != null)
                {
                    if (i < 2 && box.size.y == Pixel.Size)
                    {
                        box.size += new Vector2(Pixel.Size, 0);
                        box.offset += new Vector2(Pixel.Size / 2 * Math.Sign(span[i].x), 0);
                        block.allCollider[pos.x, pos.y] = box;
                        break;
                    }
                    else if (i >= 2 && box.size.x == Pixel.Size)
                    {
                        box.size += new Vector2(0, Pixel.Size);
                        box.offset += new Vector2(0, Pixel.Size / 2 * Math.Sign(span[i].x));
                        block.allCollider[pos.x, pos.y] = box;
                        break;
                    }
                }
            }
        }
        static System.Random random = new System.Random();
        /// <summary>
        /// 尝试删除一点的碰撞箱，并将此处原本的碰撞箱拆分为合适大小
        /// </summary>
        public static void TryShrinkCollider(Block block, Vector2Byte pos)
        {
            if (Block.Check(pos) == false || block.allCollider[pos.x, pos.y] == null) return;
            var box = block.allCollider[pos.x, pos.y];
            Vector2Byte colliderPos = new Vector2Byte((byte)Mathf.RoundToInt(box.transform.localPosition.x / Pixel.Size), (byte)Mathf.RoundToInt(box.transform.localPosition.y / Pixel.Size));
            Vector2Byte colliderSize = new Vector2Byte((byte)Mathf.RoundToInt(box.size.x / Pixel.Size), (byte)Mathf.RoundToInt(box.size.y / Pixel.Size));
            Debug.Log(colliderPos + "|" + colliderSize);
            Color32 color = new Color32((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255), 255);
            for (byte x = colliderPos.x; x < colliderPos.x + colliderSize.x; x++)
                for (byte y = colliderPos.y; y < colliderPos.y + colliderSize.y; y++)
                {
                    block.allCollider[x, y] = null;
                    block.DrawPixelAsync(new Vector2Byte(x, y), color);
                }
            GameObject.Destroy(box.gameObject);
            var list = CreateColliderDataList(block, colliderPos, colliderPos + colliderSize - new Vector2Byte(1, 1));
            CreateColliderAction(block, list, true);
        }
    }
}
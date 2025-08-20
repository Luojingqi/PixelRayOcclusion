using PRO.DataStructure;
using PRO.Tool;
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
        #region 碰撞箱对象池
        private static GameObjectPool<BoxCollider2D> BoxCollider2DPool;
        public static void InitBoxCollider2DPool()
        {
            GameObject boxCollider2DPoolGo = new GameObject("BoxCollider2DPool");
            boxCollider2DPoolGo.transform.SetParent(SceneManager.Inst.PoolNode);
            BoxCollider2D boxCollider = new GameObject("boxCollider").AddComponent<BoxCollider2D>();
            boxCollider.gameObject.layer = GameLayer.Block.ToUnityLayer();
            BoxCollider2DPool = new GameObjectPool<BoxCollider2D>(boxCollider, boxCollider2DPoolGo.transform);

        }
        public static BoxCollider2D TakeOut()
        {
            BoxCollider2D box = BoxCollider2DPool.TakeOut();
            return box;
        }
        public static void PutIn(BoxCollider2D box)
        {
            box.isTrigger = false;
            BoxCollider2DPool.PutIn(box);
        }
        #endregion
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

        private static ObjectPoolArbitrary<HashSet<Vector2Int>> pool_set = new ObjectPoolArbitrary<HashSet<Vector2Int>>(() => new HashSet<Vector2Int>(Block.Size.x * Block.Size.y));
        private static ObjectPoolArbitrary<List<ColliderData>> pool_list = new ObjectPoolArbitrary<List<ColliderData>>(() => new List<ColliderData>(Block.Size.x));
        /// <summary>
        /// 返回用于创建碰撞箱的数据集合
        /// </summary>
        public static List<ColliderData> CreateColliderDataList(this Block block, Vector2Byte min, Vector2Byte max)
        {
            HashSet<Vector2Int> hash;
            List<ColliderData> colliderDataList;
            lock (pool_list)
            {
                hash = pool_set.TakeOut();
                colliderDataList = pool_list.TakeOut();
            }

            for (int y = max.y; y >= min.y; y--)
                for (int x = min.x; x <= max.x; x++)
                    if (!hash.Contains(new Vector2Int(x, y)) && block.GetPixelRelocation(x, y).typeInfo.collider)
                    {
                        hash.Add(new Vector2Int(x, y));

                        ColliderData colliderData = new ColliderData();
                        colliderData.size = new Vector2(Pixel.Size, Pixel.Size);
                        colliderData.length = new Vector2Byte(1, 1);
                        colliderData.position = block.PixelToWorld(new Vector2Byte(x, y));
                        colliderData.pos = new Vector2Byte(x, y);
                        int xShifting = x + 1;
                        while (xShifting <= max.x && !hash.Contains(new Vector2Int(xShifting, y)) && block.GetPixelRelocation(xShifting, y).typeInfo.collider)
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
                                if (!hash.Contains(new Vector2Int(x + i, yShifting)) && !block.GetPixelRelocation(x + i, yShifting).typeInfo.collider)
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
            lock (pool_list)
            {
                hash.Clear();
                pool_set.PutIn(hash);
            }
            return colliderDataList;
        }

        /// <summary>
        /// 根据提供的碰撞箱数据列表在区块内对应位置创建碰撞箱
        /// </summary>
        public static void CreateColliderAction(this Block block, List<ColliderData> colliderDataList)
        {
            for (int i = 0; i < colliderDataList.Count; i++)
            {
                //Color32 color = new Color32((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255), 255);
                var data = colliderDataList[i];
                var box = TakeOut();
                box.size = data.size;
                box.transform.position = data.position;

                box.offset = box.size / 2f;
                box.transform.SetParent(block.colliderNode);
                for (byte x = data.pos.x; x < data.pos.x + data.length.x; x++)
                    for (byte y = data.pos.y; y < data.pos.y + data.length.y; y++)
                        block.allCollider[x, y] = box;
            }
            lock (pool_list)
            {
                colliderDataList.Clear();
                pool_list.PutIn(colliderDataList);
            }
        }
        /// <summary>
        /// 尝试在一点内创建新碰撞箱，自动与周围可合成的碰撞箱合成
        /// </summary>
        public static void TryExpandCollider(this Block block, Vector2Byte pos)
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
                        box.offset = box.size / 2f;
                        block.allCollider[pos.x, pos.y] = box;
                        if (i == 0)
                            box.transform.position += new Vector3(-Pixel.Size, 0);
                        break;
                    }
                    else if (i >= 2 && box.size.x == Pixel.Size)
                    {
                        box.size += new Vector2(0, Pixel.Size);
                        box.offset = box.size / 2f;
                        block.allCollider[pos.x, pos.y] = box;
                        if (i == 2)
                            box.transform.position += new Vector3(0, -Pixel.Size);
                        break;
                    }
                }
            }
            if (block.allCollider[pos.x, pos.y] == null)
            {
                var box = TakeOut();
                box.size = new Vector2(Pixel.Size, Pixel.Size);
                box.offset = box.size / 2;
                box.transform.position = block.PixelToWorld(pos);
                box.transform.SetParent(block.colliderNode);
                block.allCollider[pos.x, pos.y] = box;
            }
        }
        static System.Random random = new System.Random();
        /// <summary>
        /// 尝试删除一点的碰撞箱，并将此处原本的碰撞箱拆分为合适大小
        /// </summary>
        public static void TryShrinkCollider(this Block block, Vector2Byte pos)
        {
            if (Block.Check(pos) == false || block.allCollider[pos.x, pos.y] == null) return;
            var box = block.allCollider[pos.x, pos.y];
            Vector2Byte colliderPos = new Vector2Byte((byte)Mathf.RoundToInt(box.transform.localPosition.x / Pixel.Size), (byte)Mathf.RoundToInt(box.transform.localPosition.y / Pixel.Size));
            Vector2Byte colliderSize = new Vector2Byte((byte)Mathf.RoundToInt(box.size.x / Pixel.Size), (byte)Mathf.RoundToInt(box.size.y / Pixel.Size));
            Color32 color = new Color32((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255), 255);
            for (byte x = colliderPos.x; x < colliderPos.x + colliderSize.x; x++)
                for (byte y = colliderPos.y; y < colliderPos.y + colliderSize.y; y++)
                    block.allCollider[x, y] = null;
            PutIn(box);
            var list = CreateColliderDataList(block, colliderPos, colliderPos + colliderSize - new Vector2Byte(1, 1));
            CreateColliderAction(block, list);
        }
    }
}
using PRO.DataStructure;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PRO
{
    public class Nav
    {
        public Block block;

        public Vector2Int AgentSize = new Vector2Int(2, 7);
        public Vector2Int AgentOffset = new Vector2Int(1, 0);
        public Vector2Int[] chackBox;
        public Vector2Int[] walkRing;
        public Nav()
        {
            chackBox = new Vector2Int[AgentSize.x * 2 + AgentSize.y * 2 - 4];
            int index = 0;
            for (int x = 0; x < AgentSize.x; x++)
            {
                chackBox[index++] = new Vector2Int(x, 0);
                chackBox[index++] = new Vector2Int(x, AgentSize.y - 1);
            }
            for (int y = 1; y < AgentSize.y - 1; y++)
            {
                chackBox[index++] = new Vector2Int(0, y);
                chackBox[index++] = new Vector2Int(AgentSize.x - 1, y);
            }
            index = 0;
            walkRing = new Vector2Int[10];
            for (int x = -1; x <= 1; x++)
                for (int y = -2; y <= 2; y++)
                    if (x != 0)
                        walkRing[index++] = new Vector2Int(x, y);

        }

        public HashSet<Vector2Int> CanNavPointHash = new HashSet<Vector2Int>();

        public void Build(Vector2Int globalPosMin, Vector2Int globalPosMax)
        {
            for (int y_global = globalPosMin.y; y_global <= globalPosMax.y; y_global++)
                for (int x_global = globalPosMin.x; x_global <= globalPosMax.x; x_global++)
                {
                    Vector2Int navDot = new Vector2Int(x_global, y_global);
                    Vector2Int start = new Vector2Int(navDot.x - AgentOffset.x, navDot.y - AgentOffset.y);
                    Vector2Int end = new Vector2Int(start.x + AgentSize.x, start.y + AgentSize.y);
                    bool canNav = false;
                    //脚下最少有一个像素点才可站立
                    for (int x = start.x; x < end.x; x++)
                        if (ChackCollider(new Vector2Int(x, navDot.y - 1)) == true)
                        {
                            canNav = true;
                            break;
                        }
                    for (int y = start.y; y < end.y; y++)
                        for (int x = start.x; x < end.x; x++)
                            if (ChackCollider(new Vector2Int(x, y)) == true)
                            {
                                canNav = false;
                                goto to;
                            }

                        to:
                    if (canNav)
                    {
                        CanNavPointHash.Add(navDot);
                    }
                }
            foreach (var n in CanNavPointHash)
            {
                block.SetPixel(Pixel.TakeOut("测试", 0, Block.GlobalToPixel(n)));
            }
            block.DrawPixelAsync();
        }
        /// <summary>
        /// 检查一个点是否有碰撞箱
        /// </summary>
        private bool ChackCollider(Vector2Int globalPos)
        {
            Block block = SceneManager.Inst.NowScene.GetBlock(Block.GlobalToBlock(globalPos));
            if (block == null) return true;
            Pixel pixel = block.GetPixel(Block.GlobalToPixel(globalPos));
            return pixel.typeInfo.collider;
        }



        public List<Vector2Int> TryNav(Vector2Int start_G, Vector2Int end_G, PriorityQueue<Vector2Int> queue = null, Dictionary<Vector2Int, Vector2Int> dic = null, List<Vector2Int> navList = null)
        {
            if (queue == null) queue = new PriorityQueue<Vector2Int>(); else queue.Clear();
            if (dic == null) dic = new Dictionary<Vector2Int, Vector2Int>(); else dic.Clear();
            if (navList == null) navList = new List<Vector2Int>(); else navList.Clear();
            queue.Enqueue(start_G, 0);
            dic.Add(start_G, start_G);
            bool success = false;
            int depth = 0;
            while (queue.Count > 0 && depth++ < 50)
            {
                Vector2Int now = queue.Dequeue();
                if (now == end_G)
                {
                    success = true;
                    break;
                }
                for(int i =0;i<walkRing.Length;i++)
                {
                    Vector2Int r = walkRing[i] + now;
                    if (dic.ContainsKey(r) == false && ChackCanNav(r))
                    {
                        queue.Enqueue(r, FastDistance(r, end_G));
                        dic.Add(r, now);
                    }
                }
            }
            navList.Clear();
            if (success)
            {
                Vector2Int last = end_G;
                while (dic.TryGetValue(last, out Vector2Int pos))
                {
                    navList.Add(last);
                    if (pos == start_G)
                        break;
                    last = pos;
                }
                navList.Reverse();
            }

            return navList;
        }

        public bool ChackCanNav(Vector2Int globalPos)
        {
            Block block = null;
            for (int i = 0; i < chackBox.Length; i++)
            {
                var pos = globalPos + chackBox[i] - AgentOffset;
                var blockPos = Block.GlobalToBlock(pos);
                if (block == null || block.BlockPos != blockPos)
                {
                    block = SceneManager.Inst.NowScene.GetBlock(blockPos);
                    if (block == null) return false;
                }
                Pixel pixel = block.GetPixel(Block.GlobalToPixel(pos));
                if (pixel.typeInfo.collider) return false;
            }
            for (int x = 0; x < AgentSize.x; x++)
            {
                var pos = globalPos + new Vector2Int(x, -1) - AgentOffset;
                var blockPos = Block.GlobalToBlock(pos);
                if (block == null || block.BlockPos != blockPos)
                {
                    block = SceneManager.Inst.NowScene.GetBlock(blockPos);
                    if (block == null) return false;
                }
                Pixel pixel = block.GetPixel(Block.GlobalToPixel(pos));
                if (pixel.typeInfo.collider) return true;
            }
            return false;
        }


        /// <summary>
        /// 快速估算距离代价
        /// </summary>
        private static int FastDistance(Vector2Int start, Vector2Int end) => Mathf.Abs(end.x - start.x + end.y - start.y);


    }
}

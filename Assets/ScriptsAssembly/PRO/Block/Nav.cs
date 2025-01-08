using PRO.DataStructure;
using System.Collections.Generic;
using UnityEngine;

namespace PRO
{
    public class Nav
    {
        public Block block;

        public Vector2Int AgentSize = new Vector2Int(1, 1);
        public Vector2Int AgentOffset = new Vector2Int(0, 0);

        public HashSet<Vector2Int> CanNavPointHash = new HashSet<Vector2Int>();

        public void Build(Vector2Int globalPosMin, Vector2Int globalPosMax)
        {
            for (int y_global = globalPosMin.y; y_global <= globalPosMax.y; y_global++)
                for (int x_global = globalPosMin.x; x_global <= globalPosMax.x; x_global++)
                {
                    Vector2Int navDot = new Vector2Int(x_global, y_global);
                    Vector2Int end = new Vector2Int(navDot.x - AgentOffset.x + AgentSize.x, navDot.y - AgentOffset.y + AgentSize.y);
                    bool canNav = true;
                    //脚下最少有一个像素点才可站立
                    if (ChackCollider(navDot + Vector2Int.down) == false)
                    {
                        canNav = false;
                        goto to;
                    }
                    for (int y = navDot.y - AgentOffset.y; y < end.y; y++)
                        for (int x = navDot.x - AgentOffset.y; x < end.x; x++)
                        {
                            Vector2Int globalPos = new Vector2Int(x, y);
                            if (ChackCollider(globalPos) == true)
                            {
                                canNav = false;
                                goto to;
                            }
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



        public static List<Vector2Int> TryNav(Vector2Int start_G, Vector2Int end_G)
        {
            PriorityQueue<Vector2Int> queue = new PriorityQueue<Vector2Int>();
            Dictionary<Vector2Int, Vector2Int> dic = new Dictionary<Vector2Int, Vector2Int>();
            queue.Enqueue(start_G, 0);
            bool success = false;
            int depth = 0;
            while (queue.Count > 0 && depth++ < 64 * 64)
            {
                Vector2Int now = queue.Dequeue();
                if (now == end_G)
                {
                    success = true;
                    break;
                }

                List<Vector2Int> ring = new List<Vector2Int>();
                #region
                ring.Add(now + new Vector2Int(1, 0));
                ring.Add(now + new Vector2Int(1, 1));
                ring.Add(now + new Vector2Int(1, 2));
                ring.Add(now + new Vector2Int(1, -1));
                ring.Add(now + new Vector2Int(1, -2));
                ring.Add(now + new Vector2Int(-1, 0));
                ring.Add(now + new Vector2Int(-1, 1));
                ring.Add(now + new Vector2Int(-1, 2));
                ring.Add(now + new Vector2Int(-1, -1));
                ring.Add(now + new Vector2Int(-1, -2));
                ring.Add(now + new Vector2Int(0, 1));
                ring.Add(now + new Vector2Int(0, -1));
                #endregion
                foreach (Vector2Int r in ring)
                {
                    Block block = SceneManager.Inst.NowScene.GetBlock(Block.GlobalToBlock(r));
                    if (block != null)
                    {
                        if (dic.ContainsKey(r) == false && block.nav.CanNavPointHash.Contains(r))
                        {
                            queue.Enqueue(r, FastDistance(r, end_G));
                            dic.Add(r, now);
                        }
                    }
                }
            }

            List<Vector2Int> ret = new List<Vector2Int>();
            if (success)
            {
                Vector2Int last = end_G;
                while (dic.TryGetValue(last, out Vector2Int pos))
                {
                    ret.Add(last);
                    last = pos;
                }
                ret.Reverse();
            }

            return ret;
        }



        /// <summary>
        /// 快速估算距离代价
        /// </summary>
        private static int FastDistance(Vector2Int start, Vector2Int end) => Mathf.Abs(end.x - start.x + end.y - start.y);


    }

    //public struct NavDot
    //{

    //}
}

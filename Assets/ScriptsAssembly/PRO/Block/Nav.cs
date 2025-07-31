using PRO.DataStructure;
using System.Collections.Generic;
using UnityEngine;

namespace PRO
{
    public static class Nav
    {
        public static List<Vector2Int> TryNav(SceneEntity scene, AgentNavMould navMould, Vector2Int start_G, Vector2Int end_G, PriorityQueue<Vector2Int> queue = null, Dictionary<Vector2Int, Vector2Int> dic = null, List<Vector2Int> navList = null)
        {
            if (queue == null) queue = new PriorityQueue<Vector2Int>(); else queue.Clear();
            if (dic == null) dic = new Dictionary<Vector2Int, Vector2Int>(); else dic.Clear();
            if (navList == null) navList = new List<Vector2Int>(); else navList.Clear();
            queue.Enqueue(start_G, 0);
            dic.Add(start_G, start_G);
            bool success = false;
            int depth = 0;
            while (queue.Count > 0 && depth++ < 200)
            {
                Vector2Int now = queue.Dequeue();
                if (now == end_G)
                {
                    success = true;
                    break;
                }
                for (int i = 0; i < navMould.walkRing.Length; i++)
                {
                    Vector2Int r = navMould.walkRing[i] + now;
                    if (dic.ContainsKey(r) == false && ChackCanNav(scene, navMould, r))
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

        public static bool ChackCanNav(SceneEntity scene, AgentNavMould navMould, Vector2Int globalPos)
        {
            Block block = null;
            //检查角色的内边框是否有遮挡
            for (int i = 0; i < navMould.chackBox.Length; i++)
            {
                var pos = globalPos + navMould.chackBox[i] - navMould.mould.offset;
                var blockPos = Block.GlobalToBlock(pos);
                if (block == null || block.BlockPos != blockPos)
                {
                    block = scene.GetBlock(blockPos);
                    if (block == null) return false;
                }
                Pixel pixel = block.GetPixel(Block.GlobalToPixel(pos));
                if (pixel.typeInfo.collider) return false;
            }
            //检查角色的脚下是否有至少一个方块
            for (int x = 0; x < navMould.mould.size.x; x++)
            {
                var pos = globalPos + new Vector2Int(x, -1) - navMould.mould.offset;
                var blockPos = Block.GlobalToBlock(pos);
                if (block == null || block.BlockPos != blockPos)
                {
                    block = scene.GetBlock(blockPos);
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
        private static int FastDistance(Vector2Int start, Vector2Int end) => Mathf.Abs(end.x - start.x) + Mathf.Abs(end.y - start.y);


    }
}

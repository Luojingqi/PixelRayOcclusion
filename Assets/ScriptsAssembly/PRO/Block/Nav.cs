using PRO.DataStructure;
using System.Collections.Generic;
using UnityEngine;

namespace PRO
{
    public class Nav
    {
        /// <summary>
        /// 角色模型大小与偏移
        /// </summary>
        public NavAgentMould AgentMould { get; private set; }
        /// <summary>
        /// 检查盒子，角色模型的边框
        /// </summary>
        public Vector2Int[] chackBox { get; private set; }
        /// <summary>
        /// 角色每次可以移动的下一个点的集合
        /// </summary>
        public Vector2Int[] walkRing { get; private set; }
        public Nav(NavAgentMould NavAgentMould)
        {
            AgentMould = NavAgentMould;
            chackBox = new Vector2Int[AgentMould.size.x * 2 + AgentMould.size.y * 2 - 4];
            int index = 0;
            for (int x = 0; x < AgentMould.size.x; x++)
            {
                chackBox[index++] = new Vector2Int(x, 0);
                chackBox[index++] = new Vector2Int(x, AgentMould.size.y - 1);
            }
            for (int y = 1; y < AgentMould.size.y - 1; y++)
            {
                chackBox[index++] = new Vector2Int(0, y);
                chackBox[index++] = new Vector2Int(AgentMould.size.x - 1, y);
            }
            index = 0;
            walkRing = new Vector2Int[10];
            for (int x = -1; x <= 1; x++)
                for (int y = -2; y <= 2; y++)
                    if (x != 0)
                        walkRing[index++] = new Vector2Int(x, y);

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
            while (queue.Count > 0 && depth++ < 200)
            {
                Vector2Int now = queue.Dequeue();
                if (now == end_G)
                {
                    success = true;
                    break;
                }
                for (int i = 0; i < walkRing.Length; i++)
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
                var pos = globalPos + chackBox[i] - AgentMould.offset;
                var blockPos = Block.GlobalToBlock(pos);
                if (block == null || block.BlockPos != blockPos)
                {
                    block = SceneManager.Inst.NowScene.GetBlock(blockPos);
                    if (block == null) return false;
                }
                Pixel pixel = block.GetPixel(Block.GlobalToPixel(pos));
                if (pixel.typeInfo.collider) return false;
            }
            for (int x = 0; x < AgentMould.size.x; x++)
            {
                var pos = globalPos + new Vector2Int(x, -1) - AgentMould.offset;
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

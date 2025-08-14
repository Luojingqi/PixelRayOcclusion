using PRO.DataStructure;
using System.Collections.Generic;
using UnityEngine;

namespace PRO
{
    public static class Nav
    {
        public struct Node
        {
            public Vector2Int globalPos;
            public int jumpValue;

            public Node(Vector2Int globalPos, int jumpValue)
            {
                this.globalPos = globalPos;
                this.jumpValue = jumpValue;
            }
        }
        public static List<Node> TryNav(
            SceneEntity scene, AgentNavMould navMould, double 移动速度, int 跳跃高度,
            Vector2Int start_G, Vector2Int end_G,
            PriorityQueue<Node> queue = null, Dictionary<Node, Node> dic = null, List<Node> navList = null)
        {
            if (queue == null) queue = new(); else queue.Clear();
            if (dic == null) dic = new(); else dic.Clear();
            if (navList == null) navList = new(); else navList.Clear();
            var start_Node = new Node(start_G, 0);
            queue.Enqueue(start_Node, 0);
            dic.Add(start_Node, start_Node);
            Node? successNode = null;
            int depth = 0;
            while (queue.Count > 0 && depth++ < 200)
            {
                var now = queue.Dequeue();
                if (now.globalPos == end_G)
                {
                    successNode = now;
                    break;
                }
            生成下一节点:
                if (now.jumpValue == 0)
                {
                    {
                        //向上
                        var nextNode = new Node(now.globalPos + new Vector2Int(0, 1), 2);
                        if (dic.ContainsKey(nextNode) == false && Chack_没有阻拦(scene, navMould, nextNode.globalPos))
                            AddNavNode(now, nextNode, end_G, queue, dic);
                    }
                    {
                        //向左
                        var nextNode = new Node(now.globalPos + new Vector2Int(-1, 0), 0);
                        if (dic.ContainsKey(nextNode) == false && Chack_没有阻拦(scene, navMould, nextNode.globalPos) && Chack_可以站立(scene, navMould, nextNode.globalPos))
                            AddNavNode(now, nextNode, end_G, queue, dic);
                    }
                    {
                        //向右
                        var nextNode = new Node(now.globalPos + new Vector2Int(1, 0), 0);
                        if (dic.ContainsKey(nextNode) == false && Chack_没有阻拦(scene, navMould, nextNode.globalPos) && Chack_可以站立(scene, navMould, nextNode.globalPos))
                            AddNavNode(now, nextNode, end_G, queue, dic);
                    }
                }
                else if (now.jumpValue == -1)
                {
                    if (Chack_可以站立(scene, navMould, now.globalPos))
                    {
                        now.jumpValue = 0;
                        goto 生成下一节点;
                    }
                    else
                    {
                        {
                            //向下
                            var nextNode = new Node(now.globalPos + new Vector2Int(0, -1), -1);
                            if (dic.ContainsKey(nextNode) == false && Chack_没有阻拦(scene, navMould, nextNode.globalPos))
                                AddNavNode(now, nextNode, end_G, queue, dic);
                        }
                        {
                            //向右下
                            var nextNode = new Node(now.globalPos + new Vector2Int(1, -1), -1);
                            if (dic.ContainsKey(nextNode) == false && Chack_没有阻拦(scene, navMould, nextNode.globalPos))
                                AddNavNode(now, nextNode, end_G, queue, dic);
                        }
                        {
                            //向左下
                            var nextNode = new Node(now.globalPos + new Vector2Int(-1, -1), -1);
                            if (dic.ContainsKey(nextNode) == false && Chack_没有阻拦(scene, navMould, nextNode.globalPos))
                                AddNavNode(now, nextNode, end_G, queue, dic);
                        }
                    }
                }
                else
                {
                    if (now.jumpValue >= 跳跃高度 * 2)
                    {
                        now.jumpValue = -1;
                        goto 生成下一节点;
                    }
                    else
                    {
                        int 余 = now.jumpValue % 2;
                        if (余 == 1)
                        {
                            //奇数节点，上下移动
                            {
                                //向上
                                var nextNode = new Node(now.globalPos + new Vector2Int(0, 1), now.jumpValue + 1);
                                if (dic.ContainsKey(nextNode) == false && Chack_没有阻拦(scene, navMould, nextNode.globalPos))
                                    AddNavNode(now, nextNode, end_G, queue, dic);
                            }
                            {
                                //向下
                                var nextNode = new Node(now.globalPos + new Vector2Int(0, -1), -1);
                                if (dic.ContainsKey(nextNode) == false && Chack_没有阻拦(scene, navMould, nextNode.globalPos))
                                    AddNavNode(now, nextNode, end_G, queue, dic);
                            }
                        }
                        else
                        {
                            //偶数节点，上下左右移动
                            {
                                //向上
                                var nextNode = new Node(now.globalPos + new Vector2Int(0, 1), now.jumpValue + 2);
                                if (dic.ContainsKey(nextNode) == false && Chack_没有阻拦(scene, navMould, nextNode.globalPos))
                                    AddNavNode(now, nextNode, end_G, queue, dic);
                            }
                            {
                                //向下
                                var nextNode = new Node(now.globalPos + new Vector2Int(0, -1), -1);
                                if (dic.ContainsKey(nextNode) == false && Chack_没有阻拦(scene, navMould, nextNode.globalPos))
                                    AddNavNode(now, nextNode, end_G, queue, dic);
                            }
                            {
                                //向左
                                var nextNode = new Node(now.globalPos + new Vector2Int(-1, 0), now.jumpValue + 1);
                                if (dic.ContainsKey(nextNode) == false && Chack_没有阻拦(scene, navMould, nextNode.globalPos) && Chack_可以站立(scene, navMould, nextNode.globalPos))
                                    AddNavNode(now, nextNode, end_G, queue, dic);
                            }
                            {
                                //向右
                                var nextNode = new Node(now.globalPos + new Vector2Int(1, 0), now.jumpValue + 1);
                                if (dic.ContainsKey(nextNode) == false && Chack_没有阻拦(scene, navMould, nextNode.globalPos) && Chack_可以站立(scene, navMould, nextNode.globalPos))
                                    AddNavNode(now, nextNode, end_G, queue, dic);
                            }
                        }
                    }
                }
            }

            if (successNode != null)
            {
                var last = successNode.Value;
                while (dic.TryGetValue(last, out var node))
                {
                    navList.Add(last);
                    if (node.globalPos == start_G)
                        break;
                    last = node;
                }
                navList.Add(start_Node);
                navList.Reverse();
            }
            queue.Clear();
            dic.Clear();
            return navList;
        }
        private static void AddNavNode(Node now, Node next, Vector2Int end_G, PriorityQueue<Node> queue, Dictionary<Node, Node> dic)
        {
            queue.Enqueue(next, FastDistance(next.globalPos, end_G) + Mathf.Sign(next.jumpValue));
            dic.Add(next, now);
        }

        public static List<Vector2Int> TryNav(Role Agent, Vector2Int start_G, Vector2Int end_G, PriorityQueue<Vector2Int> queue = null, Dictionary<Vector2Int, Vector2Int> dic = null, List<Vector2Int> navList = null)
        {
            return null;
            //return TryNav(Agent.Scene, Agent.Info.NavMould, start_G, end_G, queue, dic, navList);
        }
        //public static List<Vector2Int> TryNav(SceneEntity scene, AgentNavMould navMould, Vector2Int start_G, Vector2Int end_G, PriorityQueue<Vector2Int> queue = null, Dictionary<Vector2Int, Vector2Int> dic = null, List<Vector2Int> navList = null)
        //{
        //    if (queue == null) queue = new PriorityQueue<Vector2Int>(); else queue.Clear();
        //    if (dic == null) dic = new Dictionary<Vector2Int, Vector2Int>(); else dic.Clear();
        //    if (navList == null) navList = new List<Vector2Int>(); else navList.Clear();
        //    queue.Enqueue(start_G, 0);
        //    dic.Add(start_G, start_G);
        //    bool success = false;
        //    int depth = 0;
        //    while (queue.Count > 0 && depth++ < 200)
        //    {
        //        Vector2Int now = queue.Dequeue();
        //        if (now == end_G)
        //        {
        //            success = true;
        //            break;
        //        }
        //        for (int i = 0; i < navMould.walkRing.Length; i++)
        //        {
        //            Vector2Int r = navMould.walkRing[i] + now;
        //            if (dic.ContainsKey(r) == false && ChackCanNav(scene, navMould, r))
        //            {
        //                queue.Enqueue(r, FastDistance(r, end_G));
        //                dic.Add(r, now);
        //            }
        //        }
        //    }
        //    navList.Clear();
        //    if (success)
        //    {
        //        Vector2Int last = end_G;
        //        while (dic.TryGetValue(last, out Vector2Int pos))
        //        {
        //            navList.Add(last);
        //            if (pos == start_G)
        //                break;
        //            last = pos;
        //        }
        //        navList.Add(start_G);
        //        navList.Reverse();
        //    }
        //    return navList;
        //}
        public static bool Chack_可以站立(SceneEntity scene, AgentNavMould navMould, Vector2Int globalPos)
        {
            Block block = null;
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
        public static bool Chack_没有阻拦(SceneEntity scene, AgentNavMould navMould, Vector2Int globalPos)
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
            return false;
        }


        /// <summary>
        /// 快速估算距离代价
        /// </summary>
        private static int FastDistance(Vector2Int start, Vector2Int end) => Mathf.Abs(end.x - start.x) + Mathf.Abs(end.y - start.y);


    }
}

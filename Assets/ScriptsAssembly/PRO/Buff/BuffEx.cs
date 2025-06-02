using Cysharp.Threading.Tasks;
using PRO;
using PRO.DataStructure;
using PRO.Tool;
using System.Collections.Generic;
using UnityEngine;

namespace PRO.Buff
{
    /// <summary>
    /// Buff冲突
    /// </summary>
    public static class BuffEx
    {
        #region buff_2_0 潮湿
        public static void Conflic(Buff_2_0 buff_潮湿, Buff_2_1 buff_燃烧)
        {
            if (buff_潮湿 == null || buff_燃烧 == null || !buff_潮湿.GetActive() || !buff_燃烧.GetActive()) return;
            buff_燃烧.stackNumber = (int)((1 - buff_潮湿.Proportion) * buff_燃烧.StackNumber);
            if (buff_燃烧.stackNumber <= 0) buff_燃烧.SetActive(false);
            CombatContext context = CombatContext.TakeOut();
            context.SetAgent(buff_潮湿.Agent, null, null);
            context.LogBuilder.Append($"{buff_潮湿.Name}与{buff_燃烧.Name}冲突，{buff_潮湿.Name}消失，{buff_燃烧.Name}下降{buff_潮湿.Proportion * 100:F0}%，现有{buff_燃烧.StackNumber}层燃烧。");
            LogPanelC.Inst.AddLog(context, true);
            CombatContext.PutIn(context);
            buff_潮湿.Vaporize(buff_燃烧.Agent);
            buff_潮湿.SetActive(false);
            buff_潮湿.Agent.UpdateBuffUI();
        }

        #endregion
        //public static void Conflic(Buff_2_3 buff_沾毒, Buff_2_1 buff_燃烧)
        //{
        //    if (buff_沾毒 == null || buff_燃烧 == null || !buff_沾毒.GetActive() || !buff_燃烧.GetActive()) return;
        //    buff_燃烧.stackNumber = (int)((1 - buff_沾毒.Proportion) * buff_燃烧.StackNumber);
        //    if (buff_燃烧.stackNumber <= 0) buff_燃烧.SetActive(false);
        //    CombatContext context = CombatContext.TakeOut();
        //    context.SetAgent(buff_沾毒.Agent, null, null);
        //    context.LogBuilder.Append($"{buff_沾毒.Name}与{buff_燃烧.Name}冲突，{buff_沾毒.Name}消失，{buff_燃烧.Name}下降{buff_沾毒.Proportion * 100:F0}%，现有{buff_燃烧.StackNumber}层燃烧。");
        //    LogPanelC.Inst.AddLog(context, true);
        //    CombatContext.PutIn(context);
        //    buff_沾毒.Vaporize(buff_燃烧.Agent);
        //    buff_沾毒.SetActive(false);
        //    buff_沾毒.Agent.UpdateBuffUI();
        //}

        #region Buff_2_1 燃烧  火焰
        public static void Conflic(Buff_2_1 buff_燃烧, Buff_2_0 buff_潮湿) => Conflic(buff_潮湿, buff_燃烧);
        public static void Conflic(Buff_2_1 buff_燃烧, Buff_2_9 buff_浸油)
        {
            if (buff_燃烧 == null || buff_浸油 == null || !buff_燃烧.GetActive() || !buff_浸油.GetActive()) return;
            buff_燃烧.stackNumber = (int)((1 + buff_浸油.Proportion) * buff_燃烧.StackNumber);
            if (buff_燃烧.stackNumber <= 0) buff_燃烧.SetActive(false);
            CombatContext context = CombatContext.TakeOut();
            context.SetAgent(buff_浸油.Agent, null, null);
            context.LogBuilder.Append($"{buff_浸油.Name}与{buff_燃烧.Name}冲突，{buff_浸油.Name}消失，{buff_燃烧.Name}提升{buff_浸油.Proportion * 100:F0}%，现有{buff_燃烧.StackNumber}层燃烧。");
            LogPanelC.Inst.AddLog(context, true);
            CombatContext.PutIn(context);
            buff_浸油.SetActive(false);
            buff_浸油.Agent.UpdateBuffUI();
        }

        public static void 火焰(SceneEntity scene, Vector2Int global)
        {
            Pixel pixel = scene.GetPixel(BlockBase.BlockType.Block, global);
            if (pixel == null) return;

            if (pixel.typeInfo.typeName == "水")
            {
                pixel.blockBase.SetPixel(Pixel.TakeOut("水蒸气", "水蒸气色0", pixel.pos));
            }
            else if (pixel.typeInfo.typeName == "冰")
            {
                pixel.blockBase.SetPixel(Pixel.TakeOut("水", "水色0", pixel.pos));
            }
            else if (pixel.typeInfo.burnOdds > Random.Range(0, 100))
            {
                pixel.blockBase.SetPixel(Pixel.TakeOut("火焰", 0, pixel.pos));
            }
        }
        #endregion

        public static void Conflic(Buff_2_9 buff_浸油, Buff_2_1 buff_燃烧) => Conflic(buff_燃烧, buff_浸油);


        #region 导电
        /// <summary>
        /// 模拟电流
        /// </summary>
        /// <param name="scene">场景</param>
        /// <param name="start_G">起始坐标</param>
        /// <param name="end_G">终止坐标</param>
        /// <param name="electricalBreakdown">电击穿空气</param>
        /// <param name="mustConduction">必须导通，为否如果无法导通将导通至最近点</param>
        public static void Set导电Path(SceneEntity scene, Vector2Int start_G, Vector2Int end_G, bool electricalBreakdown, bool mustConduction, System.Action conductionAction = null) => 导电.Set导电Path(scene, start_G, end_G, electricalBreakdown, mustConduction, conductionAction);
        private static class 导电
        {
            public static async UniTask Set导电Path(SceneEntity scene, Vector2Int start_G, Vector2Int end_G, bool electricalBreakdown, bool mustConduction,System.Action conductionAction)
            {
                var list = pool.TakeOut();
                if (ChackCanElectrified(scene, start_G, electricalBreakdown, out GlobalPosAndBlockType start))
                {
                    list.Add(start);
                    Get导电Path(scene, start, end_G, 200, 0, electricalBreakdown, mustConduction, ref list);
                    for (int i = 0; i < list.Count; i++)
                    {
                        var data = list[i];
                        if (UnityEngine.Random.Range(0, 100) < 2 / (1 + data.rank) && i < list.Count - 1)
                        {
                            Vector2Int d = PixelPosRotate.New(UnityEngine.Random.Range(0, 2) - 1, 0, 0).RotatePos(list[i].globalPos - list[i + 1].globalPos);
                            Get导电Path(scene, new GlobalPosAndBlockType() { globalPos = data.globalPos, blockType = data.blockType }, data.globalPos + d * UnityEngine.Random.Range(3, FastDistance(start_G, end_G) / 10 / (1 + data.rank)), 30, data.rank + 1, electricalBreakdown, mustConduction, ref list);
                        }
                    }
                    if (list.Count > 1) conductionAction?.Invoke();
                }
                else return;
                await DrawThread.MainThreadWaitLoopDraw(1);
                TimeManager.Inst.AddToQueue_MainThreadUpdate_Clear(() =>
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        var data = list[i];
                        var blockBase = scene.GetBlockBase(data.blockType, Block.GlobalToBlock(data.globalPos));
                        var pixel = blockBase.GetPixel(Block.GlobalToPixel(data.globalPos));
                        if (pixel.typeInfo.tags.Contains("带电") == false)
                            blockBase.SetPixel(Pixel.TakeOut(pixel.typeInfo.electricityPixle[0], pixel.typeInfo.electricityPixle[1], pixel.pos));
                    }
                });
                await DrawThread.MainThreadWaitLoopDraw(3);
                TimeManager.Inst.AddToQueue_MainThreadUpdate_Clear(() =>
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        var data = list[i];
                        var blockBase = scene.GetBlockBase(data.blockType, Block.GlobalToBlock(data.globalPos));
                        if (blockBase == null) continue;
                        var pixel = blockBase.GetPixel(Block.GlobalToPixel(data.globalPos));
                        if (pixel.typeInfo.tags.Contains("带电"))
                        {
                            blockBase.SetPixel(Pixel.TakeOut(pixel.typeInfo.electricityPixle[0], pixel.typeInfo.electricityPixle[1], pixel.pos));
                        }
                    }
                    list.Clear();
                    pool.PutIn(list);
                });

            }
            private static ObjectPool<List<GlobalPosAndBlockType>> pool = new ObjectPool<List<GlobalPosAndBlockType>>();
            private static PriorityQueue<GlobalPosAndBlockType> queue = new PriorityQueue<GlobalPosAndBlockType>();
            private static Dictionary<GlobalPosAndBlockType, GlobalPosAndBlockType> dic = new Dictionary<GlobalPosAndBlockType, GlobalPosAndBlockType>();
            private static Vector2Int[] ring = new Vector2Int[] { new(0, 1), new(0, -1), new(1, 0), new(-1, 0), new(1, 1), new(1, -1), new(-1, 1), new(-1, -1) };
            private static void Get导电Path(SceneEntity scene, GlobalPosAndBlockType start, Vector2Int end, int maxSeekDepth, int rank, bool electricalBreakdown, bool mustConduction, ref List<GlobalPosAndBlockType> list)
            {
                queue.Enqueue(start, 0);
                dic.Add(start, start);
                int depth = 0;
                bool success = false;
                Vector2Int recentVector2 = start.globalPos;
                int recentDistance = int.MaxValue;
                while (queue.Count > 0 && depth++ < maxSeekDepth)
                {
                    GlobalPosAndBlockType now = queue.Dequeue();
                    if (now.globalPos == end)
                    {
                        success = true;
                        break;
                    }
                    for (int i = 0; i < ring.Length; i++)
                    {
                        Vector2Int next = ring[i] + now.globalPos;
                        if (ChackCanElectrified(scene, next, electricalBreakdown, out GlobalPosAndBlockType nextData) && dic.ContainsKey(nextData) == false)
                        {
                            int d = FastDistance(next, end);
                            if (d < recentDistance)
                            {
                                recentDistance = d;
                                recentVector2 = next;
                            }
                            queue.Enqueue(nextData, d + UnityEngine.Random.Range(0, 4));
                            dic.Add(nextData, now);
                        }
                    }
                }

                ChackCanElectrified(scene, success || mustConduction ? end : recentVector2, electricalBreakdown, out GlobalPosAndBlockType last);
                while (dic.TryGetValue(last, out GlobalPosAndBlockType data))
                {

                    if (data.globalPos == start.globalPos) break;
                    var blockBase = scene.GetBlockBase(data.blockType, Block.GlobalToBlock(data.globalPos));
                    if (blockBase == null) break;
                    Pixel pixel = blockBase.GetPixel(Block.GlobalToPixel(data.globalPos));
                    list.Add(new GlobalPosAndBlockType()
                    {
                        blockType = data.blockType,
                        globalPos = pixel.posG,
                        rank = rank,
                    });
                    last = data;
                }
                queue.Clear();
                dic.Clear();
            }
            private struct GlobalPosAndBlockType
            {
                public Vector2Int globalPos;
                public BlockBase.BlockType blockType;
                public int rank;
            }
            private static bool ChackCanElectrified(SceneEntity scene, Vector2Int globalPos, bool electricalBreakdown, out GlobalPosAndBlockType data)
            {
                data = new GlobalPosAndBlockType() { globalPos = globalPos };
                var blockPos = Block.GlobalToBlock(globalPos);
                var pixelPos = Block.GlobalToPixel(globalPos);
                var block = scene.GetBlock(blockPos);
                if (block != null)
                {
                    Pixel pixel = block.GetPixel(pixelPos);
                    data.blockType = BlockBase.BlockType.Block;
                    if (pixel.typeInfo.tags.Contains("导电") || pixel.typeInfo.tags.Contains("带电")) return true;
                    else if (electricalBreakdown && pixel.typeInfo.typeName == "空气") return true;
                }
                else
                {
                    var back = scene.GetBackground(blockPos);
                    if (back != null)
                    {
                        Pixel pixel = back.GetPixel(pixelPos);
                        data.blockType = BlockBase.BlockType.BackgroundBlock;
                        if (pixel.typeInfo.tags.Contains("导电") || pixel.typeInfo.tags.Contains("带电")) return true;
                        else if (electricalBreakdown && pixel.typeInfo.typeName == "空气") return true;
                    }
                }
                return false;
            }
            private static int FastDistance(Vector2Int start, Vector2Int end) => Mathf.Abs(end.x - start.x) + Mathf.Abs(end.y - start.y);
        }
        #endregion


    }
}
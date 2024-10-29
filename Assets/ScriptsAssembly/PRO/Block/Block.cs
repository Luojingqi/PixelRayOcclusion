using Cysharp.Threading.Tasks;
using PRO.DataStructure;
using PRO.Renderer;
using PRO.Tool;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
namespace PRO
{
    public class Block : BlockBase
    {
        /// <summary>
        /// 每区块所占像素点
        /// </summary>
        public static readonly Vector2Byte Size = new Vector2Byte(64, 64);
        #region 坐标转换
        /// <summary>
        /// 世界坐标点to块坐标
        /// </summary>
        public static Vector2Int WorldToBlock(Vector2 worldPos) => new Vector2Int((int)Math.Round(worldPos.x / Block.Size.x / Pixel.Size - 0.5f), (int)Math.Round(worldPos.y / Block.Size.y / Pixel.Size - 0.5f));
        public static Vector2Int WorldToGloab(Vector2 worldPos) => new Vector2Int((int)(worldPos.x / Pixel.Size), (int)(worldPos.y / Pixel.Size));
        /// <summary>
        /// 块坐标to世界坐标点
        /// </summary>
        public static Vector3 BlockToWorld(Vector2Int blockPos) => new Vector3(blockPos.x * Block.Size.x * Pixel.Size, blockPos.y * Block.Size.y * Pixel.Size);
        /// <summary>
        /// 世界坐标点to点坐标（局部）
        /// </summary>
        public static Vector2Byte WorldToPixel(Vector3 worldPos)
        {
            int x = (int)(worldPos.x / Pixel.Size) % Block.Size.x;
            int y = (int)(worldPos.y / Pixel.Size) % Block.Size.y;
            if (x < 0) x += Block.Size.x;
            if (y < 0) y += Block.Size.y;
            return new Vector2Byte(x, y);
        }
        /// <summary>
        /// 点坐标（局部）to世界坐标点
        /// </summary>
        public Vector3 PixelToWorld(Vector2Byte pixelPos) => new Vector3((BlockPos.x * Block.Size.x + pixelPos.x) * Pixel.Size, (BlockPos.y * Block.Size.y + pixelPos.y) * Pixel.Size);
        public Vector2Int PixelToGloab(Vector2Byte pixelPos) => new Vector2Int(BlockPos.x * Block.Size.x + pixelPos.x, BlockPos.y * Block.Size.y + pixelPos.y);
        public static Vector2Int PixelToGloab(Vector2Int blockPos, Vector2Byte pixelPos) => new Vector2Int(blockPos.x * Block.Size.x + pixelPos.x, blockPos.y * Block.Size.y + pixelPos.y);
        public Vector2Byte GloabToPixel(Vector2Int gloabPos) => new Vector2Byte(gloabPos.x - BlockPos.x * Block.Size.x, gloabPos.y - BlockPos.y * Block.Size.y);
        public static Vector2Int GloabToBlock(Vector2Int gloabPos)
        {
            if (gloabPos.x < 0) gloabPos.x -= Block.Size.x - 1;
            if (gloabPos.y < 0) gloabPos.y -= Block.Size.y - 1;
            return new Vector2Int(gloabPos.x / Block.Size.x, gloabPos.y / Block.Size.y);
        }
        public static Vector3 GloabToWorld(Vector2Int gloabPos) => new Vector3(gloabPos.x * Pixel.Size, gloabPos.y * Pixel.Size);
        /// <summary>
        /// 检查坐标是否非法
        /// </summary>
        public static bool Check(Vector2Byte pos)
        {
            if (pos.x < 0 || pos.x >= Block.Size.x || pos.y < 0 || pos.y >= Block.Size.y) return false;
            else return true;
        }

        /// <summary>
        /// 修正，如果当前点不在区块内，返回修正的区块和点
        /// <returns></returns>
        public static bool Relocation(BlockBase block, Vector2Int pos, out Vector2Int rightBlock, out Vector2Byte rightPos)
        {
            rightBlock = block.BlockPos;
            rightPos = (Vector2Byte)pos;
            bool ret = true;
            if (pos.x < 0)
            {
                rightBlock.x = block.BlockPos.x - 1 - pos.x / Block.Size.x;
                rightPos.x = (byte)(pos.x % Block.Size.x + Block.Size.x);
                ret = false;
            }
            else if (pos.x >= Block.Size.x)
            {
                rightBlock.x = block.BlockPos.x + pos.x / Block.Size.x;
                rightPos.x = (byte)(pos.x % Block.Size.x);
                ret = false;
            }
            if (pos.y < 0)
            {
                rightBlock.y = block.BlockPos.y - 1 - pos.y / Block.Size.y;
                rightPos.y = (byte)(pos.y % Block.Size.y + Block.Size.y);
                ret = false;
            }
            else if (pos.y >= Block.Size.y)
            {
                rightBlock.y = block.BlockPos.y + pos.y / Block.Size.y;
                rightPos.y = (byte)(pos.y % Block.Size.y);
                ret = false;
            }
            return ret;
        }
        #endregion
        #region 静态对象池
        private static Transform BlockNode;
        private static GameObjectPool<Block> BlockPool;
        public static void InitBlockPool()
        {
            #region 加载Block初始GameObject
            GameObject go = new GameObject("Block");
            SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
            renderer.sharedMaterial = BlockShareMaterialManager.ShareMaterial;
            go.gameObject.SetActive(false);
            go.AddComponent<Block>();
            #endregion
            GameObject blockPoolGo = new GameObject("BlockPool");
            blockPoolGo.transform.parent = SceneManager.Inst.PoolNode;
            BlockPool = new GameObjectPool<Block>(go, blockPoolGo.transform, 20, true);
            BlockPool.CreateEventT += (g, t) =>
            {
                t.Init();
            };
            go.transform.parent = blockPoolGo.transform;
        }

        public static Block TakeOut() => BlockPool.TakeOutT();

        public static async void PutIn(Block block)
        {
            block.gameObject.SetActive(false);
            for (int y = 0; y < Block.Size.y; y++)
            {
                for (int x = 0; x < Block.Size.x; x++)
                {
                    Pixel pixel = block.allPixel[x, y];
                    block.allPixel[x, y] = null;
                    Pixel.PutIn(pixel);

                    BoxCollider2D box = block.allCollider[x, y];
                    block.allCollider[x, y] = null;
                    GreedyCollider.PutIn(box);
                }
                await UniTask.Yield();
            }
            BlockPool.PutIn(block.gameObject);
        }
        #endregion 
        public override void Init()
        {
            base.Init();
            for (int i = 0; i < AllLiquid.Length; i++)
                AllLiquid[i] = new HashSet<Liquid>();
            colliderNode = new GameObject("ColliderNode").transform;
            colliderNode.parent = transform;
        }

        public static Pixel GetPixel(Vector2Int worldPos)
        {
            var block = SceneManager.Inst.NowScene.GetBlock(Block.GloabToBlock(worldPos));
            if (block == null) return null;
            return block.GetPixel(block.GloabToPixel(worldPos));
        }
        /// <summary>
        /// 获取点（如果点不在此区块会被修正到对应区块）
        /// </summary>
        public Pixel GetPixelRelocation(int x, int y)
        {
            if (Block.Relocation(this, new Vector2Int(x, y), out Vector2Int rightBlock, out Vector2Byte rightPos))
            {
                return allPixel[x, y];
            }
            else
            {
                Block block = SceneManager.Inst.NowScene.GetBlock(rightBlock);
                if (block == null)
                    return null;
                else
                    return block.GetPixel(rightPos);
            }
        }


        #region 流体
        /// <summary>
        /// 液体压强更新队列
        /// </summary>
        //  public Queue<Vector2Byte> LiquidPressureUpdateQueue = new Queue<Vector2Byte>();
        private HashSet<Liquid>[] AllLiquid = new HashSet<Liquid>[Block.Size.y];

        public void AddLiquid(Liquid liquid)
        {
            HashSet<Liquid> hash = AllLiquid[liquid.pos.y];
            if (hash.Contains(liquid) == false)
            {
                hash.Add(liquid);
                Vector2Int posG = this.PixelToGloab(liquid.pos);
                var block = SceneManager.Inst.NowScene.GetBlock(Block.GloabToBlock(posG + Vector2Int.up));
                if (block == null) UpdateLiquidPressure(posG, 0);
                else
                {
                    var pixel = block.GetPixel(block.GloabToPixel(posG + Vector2Int.up));
                    if (pixel is Liquid == false) UpdateLiquidPressure(posG, 0);
                    else UpdateLiquidPressure(posG, (pixel as Liquid).pressure + 1);
                }
            }
        }
        public void RemoveLiquid(Liquid liquid)
        {
            AllLiquid[liquid.pos.y].Remove(liquid);
            UpdateLiquidPressure(this.PixelToGloab(liquid.pos) + Vector2Int.down, 0);
        }

        private static void UpdateLiquidPressure(Vector2Int posG, int pressure)
        {
            try
            {
                var block = SceneManager.Inst.NowScene.GetBlock(Block.GloabToBlock(posG));
                while (true)
                {
                    var blockPos = Block.GloabToBlock(posG);
                    if (blockPos != block.BlockPos)
                    {
                        block = SceneManager.Inst.NowScene.GetBlock(blockPos);
                        if (block == null) return;
                    }
                    Pixel pixel = block.GetPixel(block.GloabToPixel(posG));
                    if (pixel is Liquid)
                    {
                        Liquid liquid_Next = (Liquid)pixel;
                        liquid_Next.pressure = pressure;
                        pressure++;
                        posG += Vector2Int.down;
                    }
                    else break;
                }
            }
            catch
            {
                Debug.Log("错误" + Block.GloabToBlock(posG));
            }
        }

        private Liquid[] liquids = new Liquid[Block.Size.x];
        public void UpdateLiquid()
        {
            for (int i = 0; i < AllLiquid.Length; i++)
            {
                AllLiquid[i].CopyTo(liquids);
                foreach (Liquid liquid in liquids)
                {
                    if (liquid == null) break;
                    TryLiquidFlow(liquid);
                }
                for (int j = 0; j < liquids.Length; j++)
                    if (liquids[j] == null) break;
                    else liquids[j] = null;
            }
        }
        Queue<Vector2Int> _queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> _hash = new HashSet<Vector2Int>();
        private void AddQueueHash(Vector2Int posG)
        {
            if (_hash.Contains(posG)) return;
            _queue.Enqueue(posG); _hash.Add(posG);
        }
        private bool TryLiquidFlow(Liquid liquid)
        {
            _queue.Clear();
            _hash.Clear();
            Vector2Int g = PixelToGloab(liquid.pos);
            AddQueueHash(g + Vector2Int.down);
            Vector2Int right = g + Vector2Int.right;
            var rightBlock = SceneManager.Inst.NowScene.GetBlock(Block.GloabToBlock(right));
            if (rightBlock != null && rightBlock.GetPixel(rightBlock.GloabToPixel(right)) is Liquid) AddQueueHash(right);
            Vector2Int left = g + Vector2Int.left;
            var leftBlock = SceneManager.Inst.NowScene.GetBlock(Block.GloabToBlock(left));
            if (leftBlock != null && leftBlock.GetPixel(leftBlock.GloabToPixel(left)) is Liquid) AddQueueHash(left);

            AddQueueHash(g + Vector2Int.right + Vector2Int.down);
            AddQueueHash(g + Vector2Int.left + Vector2Int.down);
            while (_queue.Count > 0)
            {
                Vector2Int posG = _queue.Dequeue();
                Block block = SceneManager.Inst.NowScene.GetBlock(Block.GloabToBlock(posG));
                if (block == null) return false;
                Vector2Byte pos = block.GloabToPixel(posG);

                Pixel pixel = block.GetPixel(pos);
                //switch (pixel.id)
                //{
                //    case 0://空气
                //        if (block.BlockPos == this.BlockPos)
                //        {
                //            RemoveLiquid(liquid);
                //            Swap(liquid, pixel);
                //            AddLiquid(liquid);
                //        }
                //        else
                //        {
                //            this.RemoveLiquid(liquid);
                //            Block.Swap(this.PixelToGloab(liquid.pos), posG);
                //            block.AddLiquid(liquid);
                //        }
                //        return true;
                //    case 1://墙

                //        break;
                //    case 2://液体
                //        Liquid liquid_Now = (Liquid)pixel;
                //        if (liquid.pressure - liquid_Now.pressure > 1)
                //        {
                //            //Vector2Int _down = posG + Vector2Int.down;
                //            //if (_hash.Check(_down) == false) { _queue.Enqueue(_down); _hash.Add(_down); }
                //            Vector2Int _right = posG + Vector2Int.right;
                //            if (_hash.Contains(_right) == false) { _queue.Enqueue(_right); _hash.Add(_right); }
                //            Vector2Int _left = posG + Vector2Int.left;
                //            if (_hash.Contains(_left) == false) { _queue.Enqueue(_left); _hash.Add(_left); }
                //            Vector2Int _up = posG + Vector2Int.up;
                //            if (_hash.Contains(_up) == false) { _queue.Enqueue(_up); _hash.Add(_up); }
                //        }
                //        else if (liquid.pressure == 1 && liquid_Now.pressure == 0)
                //        {
                //            AddQueueHash(posG + Vector2Int.right);
                //            AddQueueHash(posG + Vector2Int.left);
                //        }
                //        break;
                //}
            }
            return false;
        }
        #endregion

        public Transform colliderNode;
        public readonly BoxCollider2D[,] allCollider = new BoxCollider2D[Block.Size.x, Block.Size.y];

        /// <summary>
        /// 交换块内两个点，并提交绘制请求
        /// </summary>
        public void Swap(Pixel p0, Pixel p1)
        {
            Vector2Byte t = p0.pos;
            p0.pos = p1.pos;
            p1.pos = t;
            SetPixel(p0);
            SetPixel(p1);
            DrawPixelAsync(p0.pos, BlockMaterial.GetPixelColorInfo(p0.colorName).color);
            DrawPixelAsync(p1.pos, BlockMaterial.GetPixelColorInfo(p1.colorName).color);
        }

        /// <summary>
        /// 交换块内两个点，并提交绘制请求
        /// </summary>
        public static void Swap(Vector2Int p0_G, Vector2Int p1_G)
        {
            var block0 = SceneManager.Inst.NowScene.GetBlock(Block.GloabToBlock(p0_G));
            var block1 = SceneManager.Inst.NowScene.GetBlock(Block.GloabToBlock(p1_G));
            var p0 = block0.GetPixel(block0.GloabToPixel(p0_G));
            var p1 = block1.GetPixel(block1.GloabToPixel(p1_G));
            Vector2Byte t = p0.pos;
            p0.pos = p1.pos;
            p1.pos = t;
            block1.SetPixel(p0);
            block0.SetPixel(p1);
            block1.DrawPixelAsync(p0.pos, BlockMaterial.GetPixelColorInfo(p0.colorName).color);
            block0.DrawPixelAsync(p1.pos, BlockMaterial.GetPixelColorInfo(p1.colorName).color);
        }
    }
}
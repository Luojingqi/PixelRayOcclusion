using Google.FlatBuffers;
using PRO.DataStructure;
using PRO.Disk;
using PRO.Flat.Ex;
using PRO.Renderer;
using PRO.Tool;
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
        public static Vector2Int WorldToBlock(Vector2 worldPos) => GlobalToBlock(WorldToGlobal(worldPos));
        public static Vector2Int WorldToGlobal(Vector2 worldPos)
        {
            // float absX = Mathf.Abs(worldPos.x);
            // float absY = Mathf.Abs(worldPos.y);
            int x = (int)Mathf.Floor((int)(worldPos.x * 100) / 100f / Pixel.Size);
            int y = (int)Mathf.Floor((int)(worldPos.y * 100) / 100f / Pixel.Size);
            //  if (worldPos.x < 0 && absX % Pixel.Size > 0) x = -x - 1;
            //  if (worldPos.y < 0 && absY % Pixel.Size > 0) y = -y - 1;
            return new Vector2Int(x, y);
        }
        /// <summary>
        /// 块坐标to世界坐标点
        /// </summary>
        public static Vector3 BlockToWorld(Vector2Int blockPos) => new Vector3((blockPos.x * Block.Size.x * Pixel.Size), (blockPos.y * Block.Size.y * Pixel.Size));
        /// <summary>
        /// 世界坐标点to点坐标（局部）
        /// </summary>
        public static Vector2Byte WorldToPixel(Vector3 worldPos) => GlobalToPixel(WorldToGlobal(worldPos));
        public static Vector2Int PixelToGlobal(Vector2Int blockPos, Vector2Byte pixelPos) => new Vector2Int(blockPos.x * Block.Size.x + pixelPos.x, blockPos.y * Block.Size.y + pixelPos.y);
        public static Vector2Byte GlobalToPixel(Vector2Int globalPos)
        {
            globalPos.x %= Block.Size.x;
            globalPos.y %= Block.Size.y;
            if (globalPos.x < 0) globalPos.x += Block.Size.x;
            if (globalPos.y < 0) globalPos.y += Block.Size.y;
            return (Vector2Byte)globalPos;
        }
        public static Vector2Int GlobalToBlock(Vector2Int globalPos)
        {
            if (globalPos.x < 0) globalPos.x -= Block.Size.x - 1;
            if (globalPos.y < 0) globalPos.y -= Block.Size.y - 1;
            return new Vector2Int(globalPos.x / Block.Size.x, globalPos.y / Block.Size.y);
        }
        public static Vector3 GlobalToWorld(Vector2Int globalPos) => new Vector3((globalPos.x * Pixel.Size), (globalPos.y * Pixel.Size));
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
        private static GameObjectPool<Block> BlockPool;
        public static void InitPool()
        {
            #region 加载Block初始GameObject
            GameObject go = new GameObject("Block");
            SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
            renderer.sharedMaterial = BlockShareMaterialManager.ShareMaterial;
            Block block = go.AddComponent<Block>();
            #endregion
            GameObject blockPoolGo = new GameObject("BlockPool");
            blockPoolGo.transform.SetParent(SceneManager.Inst.PoolNode);
            BlockPool = new GameObjectPool<Block>(block, blockPoolGo.transform);
            BlockPool.CreateEvent += t => t.Init();
        }

        public static Block TakeOut(SceneEntity scene, Vector2Int blockPos)
        {
            Block block = BlockPool.TakeOut();
            block._screen = scene;
            block.name = $"Block{blockPos}";
            block.transform.position = Block.BlockToWorld(blockPos);
            block.BlockPos = blockPos;
            block.transform.SetParent(scene.BlockNode);
            block.background.TakeOut(scene, blockPos);

            return block;
        }

        public static void PutIn(Block block, CountdownEvent countdown)
        {
            countdown.AddCount();
            block.PutIn();
            HashSet<BoxCollider2D> boxHash = new HashSet<BoxCollider2D>(10);
            for (int y = 0; y < Block.Size.y; y++)
                for (int x = 0; x < Block.Size.x; x++)
                {
                    block.GetPixel(new(x, y)).Clear();

                    BoxCollider2D box = block.allCollider[x, y];
                    if (box != null)
                    {
                        boxHash.Add(box);
                        block.allCollider[x, y] = null;
                    }
                }

            for (int i = 0; i < Block.Size.y; i++)
            {
                block.fluidUpdateHash1[i].Clear();
                block.fluidUpdateHash2[i].Clear();
                block.fluidUpdateHash3[i].Clear();
            }
            block.FreelyLightSourceHash.Clear();
            block.background.PutIn(countdown);
            TimeManager.Inst.AddToQueue_MainThreadUpdate_Clear(() =>
            {
#if PRO_RENDER
                block.name = "Block(Clone)";
                block.spriteRenderer.SetPropertyBlock(BlockMaterial.NullMaterialPropertyBlock);
#endif
                BlockPool.PutIn(block);
                foreach (var box in boxHash)
                    GreedyCollider.PutIn(box);
                countdown.Signal();
            });
        }
        #endregion

        /// <summary>
        /// 卸载倒计时
        /// </summary>
        public float UnLoadCountdown;
        public void ResetUnLoadCountdown() => UnLoadCountdown = BlockMaterial.proConfig.AutoUnLoadBlockCountdownTime;

        public override void Init()
        {
            base.Init();
            for (int i = 0; i < Block.Size.y; i++)
            {
                fluidUpdateHash1[i] = new HashSet<Vector2Byte>(16);
                fluidUpdateHash2[i] = new HashSet<Vector2Byte>(16);
                fluidUpdateHash3[i] = new HashSet<Vector2Byte>(16);
            }
            colliderNode = new GameObject("ColliderNode").transform;
            colliderNode.SetParent(transform);

            spriteRenderer.sortingOrder = 10;

            _blockType = BlockType.Block;
            background = BackgroundBlock.Create();
            background.transform.SetParent(transform);
        }
        [HideInInspector]
        public BackgroundBlock background;

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
                Block block = Scene.GetBlock(rightBlock);
                if (block == null)
                    return null;
                else
                    return block.GetPixel(rightPos);
            }
        }


        #region 流体
        /// <summary>
        /// 流体1的更新概率衰减
        /// </summary>
        private static readonly int updateProbabilityDecay1 = 3;
        /// <summary>
        /// 流体2的更新概率衰减
        /// </summary>
        private static readonly int updateProbabilityDecay2 = 3;

        //流体更新队列
        //液体
        private HashSet<Vector2Byte>[] fluidUpdateHash1 = new HashSet<Vector2Byte>[Block.Size.y];
        //气体
        private HashSet<Vector2Byte>[] fluidUpdateHash2 = new HashSet<Vector2Byte>[Block.Size.y];
        //固体
        private HashSet<Vector2Byte>[] fluidUpdateHash3 = new HashSet<Vector2Byte>[Block.Size.y];
        public static void AddFluidUpdateHash(Vector2Int pos_G)
        {
            var block = SceneManager.Inst.NowScene.GetBlock(Block.GlobalToBlock(pos_G));
            if (block != null)
            {
                Pixel pixel = block.GetPixel(Block.GlobalToPixel(pos_G));
                switch (pixel.typeInfo.fluidType)
                {
                    case 1: AddHashSet(block.fluidUpdateHash1[pixel.pos.y], pixel.pos); break;
                    case 2: AddHashSet(block.fluidUpdateHash2[pixel.pos.y], pixel.pos); break;
                    case 3: AddHashSet(block.fluidUpdateHash3[pixel.pos.y], pixel.pos); break;
                }
            }
        }
        public static void AddFluidUpdateHash(Pixel pixel)
        {
            if (pixel == null) return;
            Block block = pixel.blockBase as Block;
            switch (pixel.typeInfo.fluidType)
            {
                case 1: AddHashSet(block.fluidUpdateHash1[pixel.pos.y], pixel.pos); break;
                case 2: AddHashSet(block.fluidUpdateHash2[pixel.pos.y], pixel.pos); break;
                case 3: AddHashSet(block.fluidUpdateHash3[pixel.pos.y], pixel.pos); break;
            }
        }
        private static void AddHashSet(HashSet<Vector2Byte> hash, Vector2Byte pos)
        {
            if (hash.Contains(pos) == false)
                hash.Add(pos);
        }
        public void UpdateFluid1()
        {
            SceneEntity scene = SceneManager.Inst.NowScene;
            Random.InitState((int)(TimeManager.deltaTime * 1000000));
            for (int i = 0; i < fluidUpdateHash1.Length; i++)
            {
                foreach (var pos in fluidUpdateHash1[i])
                    _posList.Add(pos);
                foreach (var pos in _posList)
                {
                    _queue.Clear();
                    _hash.Clear();
                    Pixel pixel = GetPixel(pos);
                    //标记位为false时，代表此点无法流动，移除更新队列
                    bool stopUpdate = true;
                    if (pixel.typeInfo.fluidType != 1)
                        goto end;

                    #region 添加遍历队列
                    System.Span<Vector2Int> nextPosSpan = stackalloc Vector2Int[] { new(0, -1), new(1, 0), new(-1, 0), new(1, -1), new(-1, -1) };
                    //根据概率来看优先向哪个方向移动
                    if (Random.Range(0, 100) < 50)
                    {
                        Vector2Int temp = nextPosSpan[1];
                        nextPosSpan[1] = nextPosSpan[2];
                        nextPosSpan[2] = temp;
                    }
                    if (Random.Range(0, 100) < 50)
                    {
                        Vector2Int temp = nextPosSpan[3];
                        nextPosSpan[3] = nextPosSpan[4];
                        nextPosSpan[4] = temp;
                    }
                    foreach (var spanPos in nextPosSpan)
                        AddQueueHash(pixel.posG + spanPos);
                    #endregion

                    int updateProbability = 100;
                    while (_queue.Count > 0)
                    {
                        Vector2Int nextPosG = _queue.Dequeue();
                        Block nextBlock = scene.GetBlock(Block.GlobalToBlock(nextPosG));
                        if (nextBlock == null)
                        {
                            updateProbability -= updateProbabilityDecay1;
                            continue;
                        }

                        Pixel nextPixel = nextBlock.GetPixel(Block.GlobalToPixel(nextPosG));

                        if ((nextPixel.typeInfo.typeName == "空气") ||    //下一个点为空气
                            (nextPixel.typeInfo.fluidType == 1 && pixel.typeInfo.fluidDensity > nextPixel.typeInfo.fluidDensity) ||  //下个点为液体，且密度高于他
                            (nextPixel.typeInfo.fluidType == 2))  //下个点为气体
                        {
                            if (Random.Range(0, 100) < updateProbability)
                            {
                                SwapFluid(nextPosG, pixel.posG);
                                stopUpdate = false;

                                #region 处理特殊像素
                                {
                                    //Pixel pixel_next = nextBlock.GetPixel(Block.GlobalToPixel(nextPosG));
                                    //if (pixel_next.typeInfo.typeName == "毒液")
                                    //{
                                    //    pixel_next.affectsTransparency -= Random.Range(0, 0.05f);
                                    //    if (pixel_next.affectsTransparency < 0.07f)
                                    //    {
                                    //        SetPixel(Pixel.空气.Clone(pixel_next.pos));
                                    //        stopUpdate = true;
                                    //        if (Random.Range(0, 100) < 25)
                                    //        {
                                    //            Particle particle = ParticleManager.Inst.GetPool("特殊粒子/水").TakeOutT();
                                    //            particle.SetGlobal(nextPosG);
                                    //        }
                                    //    }
                                    //}
                                }
                                #endregion
                                break;
                            }
                        }
                        else
                        {
                            updateProbability -= updateProbabilityDecay1;
                            continue;
                        }
                    }
                end:
                    if (stopUpdate == true)
                    {
                        RemoveFluidUpdateHash(pos);
                    }
                }
                _posList.Clear();
            }
        }
        public void UpdateFluid2()
        {
            SceneEntity scene = SceneManager.Inst.NowScene;
            Random.InitState((int)(TimeManager.deltaTime * 1000000));
            for (int i = fluidUpdateHash2.Length - 1; i >= 0; i--)
            {
                foreach (var pos in fluidUpdateHash2[i])
                    _posList.Add(pos);
                foreach (var pos in _posList)
                {
                    _queue.Clear();
                    _hash.Clear();
                    Pixel pixel = GetPixel(pos);
                    //标记位为false时，代表此点无法流动，移除更新队列
                    bool stopUpdate = true;
                    if (pixel.typeInfo.fluidType != 2)
                        goto end;

                    #region 添加遍历队列
                    int sign = (int)Mathf.Sign(pixel.typeInfo.fluidDensity);
                    System.Span<Vector2Int> nextPosSpan = stackalloc Vector2Int[] { new(0, sign), new(1, 0), new(-1, 0), new(1, sign), new(-1, sign) };
                    //根据概率来看优先向哪个方向移动
                    for (int s = 0; s < nextPosSpan.Length; s++)
                        if (s < 3)
                        {
                            int tempIndex = Random.Range(0, 3);
                            Vector2Int temp = nextPosSpan[tempIndex];
                            nextPosSpan[tempIndex] = nextPosSpan[s];
                            nextPosSpan[s] = temp;
                        }
                        //百分之10的概率优先斜向扩散
                        else if (Random.Range(0, 100) < 10)
                        {
                            int tempIndex = Random.Range(0, nextPosSpan.Length);
                            Vector2Int temp = nextPosSpan[tempIndex];
                            nextPosSpan[tempIndex] = nextPosSpan[s];
                            nextPosSpan[s] = temp;
                        }
                    foreach (var spanPos in nextPosSpan)
                        AddQueueHash(pixel.posG + spanPos);
                    #endregion

                    int updateProbability = 100;
                    while (_queue.Count > 0)
                    {
                        Vector2Int nextPosG = _queue.Dequeue();
                        Block nextBlock = scene.GetBlock(Block.GlobalToBlock(nextPosG));
                        if (nextBlock == null) { updateProbability -= updateProbabilityDecay2; continue; }

                        Pixel nextPixel = nextBlock.GetPixel(Block.GlobalToPixel(nextPosG));

                        if ((nextPixel.typeInfo.typeName == "空气") ||    //下一个点为空气
                            (nextPixel.typeInfo.fluidType == 2 &&
                            sign == (int)Mathf.Sign(nextPixel.typeInfo.fluidDensity) &&
                            Mathf.Abs(pixel.typeInfo.fluidDensity) > Mathf.Abs(nextPixel.typeInfo.fluidDensity) ||//下个点为气体，且密度高于他
                            (nextPixel.typeInfo.fluidType == 1 && sign == 1))   //下个点为液体，且自身是向上飘的
                            )
                        {
                            if (Random.Range(0, 100) < updateProbability)
                            {
                                SwapFluid(nextPosG, pixel.posG);
                                stopUpdate = false;

                                #region 处理特殊像素
                                {
                                    Pixel pixel_next = nextBlock.GetPixel(Block.GlobalToPixel(nextPosG));
                                    if (pixel_next.typeInfo.typeName == "水蒸气")
                                    {
                                        pixel_next.affectsTransparency -= Random.Range(0, 0.05f);
                                        if (pixel_next.affectsTransparency < 0.07f)
                                        {
                                            pixel_next.Replace(Pixel.空气);
                                            stopUpdate = true;
                                            if (Random.Range(0, 100) < 25)
                                            {
                                                Particle particle = ParticleManager.Inst.GetPool("特殊粒子/水").TakeOut(scene);
                                                particle.SetGlobal(nextPosG);
                                            }
                                        }
                                    }
                                }
                                #endregion

                                break;
                            }
                        }
                        else { updateProbability -= updateProbabilityDecay2; continue; }
                    }
                end:
                    if (stopUpdate == true)
                    {
                        RemoveFluidUpdateHash(pos);
                    }
                }
                _posList.Clear();
            }
        }
        public void UpdateFluid3()
        {
            SceneEntity scene = SceneManager.Inst.NowScene;
            for (int i = 0; i < fluidUpdateHash3.Length; i++)
            {
                foreach (var pos in fluidUpdateHash3[i])
                    _posList.Add(pos);
                foreach (var pos in _posList)
                {
                    _queue.Clear();
                    _hash.Clear();
                    Pixel pixel = GetPixel(pos);
                    //标记位为false时，代表此点无法流动，移除更新队列
                    bool stopUpdate = true;
                    if (pixel.typeInfo.fluidType != 3)
                        goto end;
                    //根据概率来看优先向哪个方向移动
                    AddQueueHash(pixel.posG + Vector2Int.down);
                    Random.InitState((int)(TimeManager.deltaTime * 1000000));
                    //if (Random.Range(0, 100) >= 50)
                    //{
                    //    AddQueueHash(g + Vector2Int.right);
                    //    AddQueueHash(g + Vector2Int.left);
                    //}
                    //else
                    //{
                    //    AddQueueHash(g + Vector2Int.left);
                    //    AddQueueHash(g + Vector2Int.right);
                    //}

                    if (Random.Range(0, 100) >= 50)
                    {
                        AddQueueHash(pixel.posG + Vector2Int.right + Vector2Int.down);
                        AddQueueHash(pixel.posG + Vector2Int.left + Vector2Int.down);
                    }
                    else
                    {
                        AddQueueHash(pixel.posG + Vector2Int.left + Vector2Int.down);
                        AddQueueHash(pixel.posG + Vector2Int.right + Vector2Int.down);
                    }


                    while (_queue.Count > 0)
                    {
                        Vector2Int nextPosG = _queue.Dequeue();
                        Block nextBlock = scene.GetBlock(Block.GlobalToBlock(nextPosG));
                        if (nextBlock == null) continue;

                        Pixel nextPixel = nextBlock.GetPixel(Block.GlobalToPixel(nextPosG));

                        if ((nextPixel.typeInfo.typeName == "空气") ||    //下一个点为空气
                            (nextPixel.typeInfo.fluidType == 1) ||  //下个点为液体
                            (nextPixel.typeInfo.fluidType == 2) ||  //下个点为气体
                            (nextPixel.typeInfo.fluidType == 3 && pixel.typeInfo.fluidDensity > nextPixel.typeInfo.fluidDensity && Random.Range(0, 100) >= 50))//下个点为固体，当密度比他大的时候概率会沉入
                        {
                            SwapFluid(nextPosG, pixel.posG);
                            stopUpdate = false;

                            break;
                        }
                    }
                end:
                    if (stopUpdate == true)
                    {
                        RemoveFluidUpdateHash(pos);
                    }
                }
                _posList.Clear();
            }
        }

        private void RemoveFluidUpdateHash(Vector2Byte pos)
        {
            fluidUpdateHash1[pos.y].Remove(pos);
            fluidUpdateHash2[pos.y].Remove(pos);
            fluidUpdateHash3[pos.y].Remove(pos);
        }
        private void SwapFluid(Vector2Int p0_G, Vector2Int p1_G)
        {
            var block0 = _screen.GetBlock(GlobalToBlock(p0_G));
            var block1 = _screen.GetBlock(GlobalToBlock(p1_G));
            var p0 = block0.GetPixel(Block.GlobalToPixel(p0_G));
            var p1 = block1.GetPixel(Block.GlobalToPixel(p1_G));
            var tempTypeInfo = p0.typeInfo;
            var tempColorInfo = p0.colorInfo;
            var tempDurability = p0.durability;
            var tempAffectsTransparency = p0.affectsTransparency;
            p0.Replace(p1);
            p1.Replace(tempTypeInfo, tempColorInfo);
            p1.durability = tempDurability;
            p1.affectsTransparency = tempAffectsTransparency;
        }

        #region 更新流体的临时变量
        private Queue<Vector2Int> _queue = new Queue<Vector2Int>();
        private HashSet<Vector2Int> _hash = new HashSet<Vector2Int>();
        private List<Vector2Byte> _posList = new List<Vector2Byte>();
        private void AddQueueHash(Vector2Int posG)
        {
            if (_hash.Contains(posG)) return;
            _queue.Enqueue(posG); _hash.Add(posG);
        }
        #endregion
        #endregion

        [HideInInspector]
        public Transform colliderNode;
        [HideInInspector]
        public readonly BoxCollider2D[,] allCollider = new BoxCollider2D[Block.Size.x, Block.Size.y];
        public void ChangeCollider(PixelTypeInfo old, PixelTypeInfo now, Vector2Byte pos)
        {
            bool oldCollider = (old == null || !old.collider) ? false : true;
            //原本无碰撞箱，现在有就创建，反之删除
            if (oldCollider == false && now.collider) GreedyCollider.TryExpandCollider(this, pos);
            else if (oldCollider && now.collider == false) GreedyCollider.TryShrinkCollider(this, pos);
        }

        /// <summary>
        /// 自由光源
        /// </summary>
        public HashSet<FreelyLightSource> FreelyLightSourceHash = new HashSet<FreelyLightSource>();

        public override System.Action ToDisk(FlatBufferBuilder builder)
        {
            int count = queue_火焰.Count;
            Flat.BlockBaseData.StartBlockFlameQueueVector(builder, count);
            for (int i = 0; i < count; i++)
            {
                var pos = queue_火焰.Dequeue();
                pos.ToDisk(builder);
                queue_火焰.Enqueue(pos);
            }
            var blockFlameQueueOffset = builder.EndVector();


            var offset1 = AddFluidUpdateHash(builder, fluidUpdateHash1);
            var offset2 = AddFluidUpdateHash(builder, fluidUpdateHash2);
            var offset3 = AddFluidUpdateHash(builder, fluidUpdateHash3);
            return () =>
            {
                Flat.BlockBaseData.AddBlockFlameQueue(builder, blockFlameQueueOffset);
                Flat.BlockBaseData.AddFluidUpdateHash1(builder, offset1);
                Flat.BlockBaseData.AddFluidUpdateHash2(builder, offset2);
                Flat.BlockBaseData.AddFluidUpdateHash3(builder, offset3);
            };
        }
        private VectorOffset AddFluidUpdateHash(FlatBufferBuilder builder, HashSet<Vector2Byte>[] fluidUpdateHash)
        {
            int length = 0;
            foreach (var hash in fluidUpdateHash)
                length += hash.Count;
            Flat.BlockBaseData.StartFluidUpdateHash1Vector(builder, length);
            foreach (var hash in fluidUpdateHash)
                foreach (var pos in hash)
                    pos.ToDisk(builder);
            return builder.EndVector();
        }

        public override void ToRAM(Flat.BlockBaseData blockDiskData, CountdownEvent countdown)
        {
            countdown.AddCount();
            for (int i = blockDiskData.FluidUpdateHash1Length - 1; i >= 0; i--)
            {
                var pos = blockDiskData.FluidUpdateHash1(i).Value.ToRAM();
                AddHashSet(fluidUpdateHash1[pos.y], pos);
            }
            for (int i = blockDiskData.FluidUpdateHash2Length - 1; i >= 0; i--)
            {
                var pos = blockDiskData.FluidUpdateHash2(i).Value.ToRAM();
                AddHashSet(fluidUpdateHash2[pos.y], pos);
            }
            for (int i = blockDiskData.FluidUpdateHash3Length - 1; i >= 0; i--)
            {
                var pos = blockDiskData.FluidUpdateHash3(i).Value.ToRAM();
                AddHashSet(fluidUpdateHash3[pos.y], pos);
            }
            var colliderDataList = GreedyCollider.CreateColliderDataList(this, new(0, 0), new(Block.Size.x - 1, Block.Size.y - 1));
            TimeManager.Inst.AddToQueue_MainThreadUpdate_Clear(() =>
            {
                GreedyCollider.CreateColliderAction(this, colliderDataList);
#if PRO_RENDER
                BlockMaterial.SetBlock(this);
#endif
                countdown.Signal();
            });
        }
    }
}
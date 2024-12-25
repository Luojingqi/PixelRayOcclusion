using Cysharp.Threading.Tasks;
using PRO.DataStructure;
using PRO.Disk;
using PRO.Renderer;
using PRO.Tool;
using System.Collections.Generic;
using UnityEngine;
namespace PRO
{
    public class Block : BlockBase
    {
        /// <summary>
        /// ÿ������ռ���ص�
        /// </summary>
        public static readonly Vector2Byte Size = new Vector2Byte(64, 64);
        #region ����ת��
        /// <summary>
        /// ���������to������
        /// </summary>
        public static Vector2Int WorldToBlock(Vector2 worldPos) => new Vector2Int((int)Mathf.Round(worldPos.x / Block.Size.x / Pixel.Size - 0.5f), (int)Mathf.Round(worldPos.y / Block.Size.y / Pixel.Size - 0.5f));
        public static Vector2Int WorldToGloab(Vector2 worldPos) => new Vector2Int((int)(worldPos.x / Pixel.Size), (int)(worldPos.y / Pixel.Size));
        /// <summary>
        /// ������to���������
        /// </summary>
        public static Vector3 BlockToWorld(Vector2Int blockPos) => new Vector3(blockPos.x * Block.Size.x * Pixel.Size, blockPos.y * Block.Size.y * Pixel.Size);
        /// <summary>
        /// ���������to�����꣨�ֲ���
        /// </summary>
        public static Vector2Byte WorldToPixel(Vector3 worldPos)
        {
            int x = (int)(worldPos.x / Pixel.Size) % Block.Size.x;
            int y = (int)(worldPos.y / Pixel.Size) % Block.Size.y;
            if (x < 0) x += Block.Size.x;
            if (y < 0) y += Block.Size.y;
            return new Vector2Byte(x, y);
        }
        public static Vector2Int PixelToGloab(Vector2Int blockPos, Vector2Byte pixelPos) => new Vector2Int(blockPos.x * Block.Size.x + pixelPos.x, blockPos.y * Block.Size.y + pixelPos.y);
        public static Vector2Int GloabToBlock(Vector2Int gloabPos)
        {
            if (gloabPos.x < 0) gloabPos.x -= Block.Size.x - 1;
            if (gloabPos.y < 0) gloabPos.y -= Block.Size.y - 1;
            return new Vector2Int(gloabPos.x / Block.Size.x, gloabPos.y / Block.Size.y);
        }
        public static Vector3 GloabToWorld(Vector2Int gloabPos) => new Vector3(gloabPos.x * Pixel.Size, gloabPos.y * Pixel.Size);
        /// <summary>
        /// ��������Ƿ�Ƿ�
        /// </summary>
        public static bool Check(Vector2Byte pos)
        {
            if (pos.x < 0 || pos.x >= Block.Size.x || pos.y < 0 || pos.y >= Block.Size.y) return false;
            else return true;
        }

        /// <summary>
        /// �����������ǰ�㲻�������ڣ���������������͵�
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

        #region ��̬�����
        private static Transform BlockNode;
        private static GameObjectPool<Block> BlockPool;
        public static void InitBlockPool()
        {
            BlockNode = new GameObject("BlockNode").transform;
            #region ����Block��ʼGameObject
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

        public static Block TakeOut()
        {
            Block block = BlockPool.TakeOutT();
            block.transform.parent = BlockNode;
            return block;
        }

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
            block.name = "Block(Clone)";
            BlockPool.PutIn(block.gameObject);
        }
        #endregion 
        public override void Init()
        {
            base.Init();
            for (int i = 0; i < Block.Size.y; i++)
            {
                fluidUpdateHash1[i] = new HashSet<Vector2Byte>();
                fluidUpdateHash2[i] = new HashSet<Vector2Byte>();
                fluidUpdateHash3[i] = new HashSet<Vector2Byte>();
            }
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
        /// ��ȡ�㣨����㲻�ڴ�����ᱻ��������Ӧ���飩
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


        #region ����
        /// <summary>
        /// ����1�ĸ��¸���˥��
        /// </summary>
        private static int updateProbabilityDecay1 = 3;


        //������¶���
        private HashSet<Vector2Byte>[] fluidUpdateHash1 = new HashSet<Vector2Byte>[Block.Size.y];
        private HashSet<Vector2Byte>[] fluidUpdateHash2 = new HashSet<Vector2Byte>[Block.Size.y];
        private HashSet<Vector2Byte>[] fluidUpdateHash3 = new HashSet<Vector2Byte>[Block.Size.y];
        public static void AddFluidUpdateHash(Vector2Int pos_G)
        {
            var block = SceneManager.Inst.NowScene.GetBlock(Block.GloabToBlock(pos_G));
            if (block != null)
            {
                Pixel pixel = block.GetPixel(block.GloabToPixel(pos_G));
                switch (pixel.info.fluidType)
                {
                    case 1: AddHashSet(block.fluidUpdateHash1[pixel.pos.y], pixel.pos); break;
                    case 2: AddHashSet(block.fluidUpdateHash2[pixel.pos.y], pixel.pos); break;
                    case 3: AddHashSet(block.fluidUpdateHash3[pixel.pos.y], pixel.pos); break;
                }
            }
        }
        private static void AddHashSet(HashSet<Vector2Byte> hash, Vector2Byte pos)
        {
            if (hash.Contains(pos) == false)
                hash.Add(pos);
        }
        public void UpdateFluid1()
        {
            for (int i = 0; i < fluidUpdateHash1.Length; i++)
            {
                _posList.Clear();
                foreach (var pos in fluidUpdateHash1[i])
                    _posList.Add(pos);
                foreach (var pos in _posList)
                {
                    _queue.Clear();
                    _hash.Clear();
                    Pixel pixel = GetPixel(pos);
                    //���λΪfalseʱ������˵��޷��������Ƴ����¶���
                    bool stopUpdate = true;
                    if (pixel.info.fluidType != 1)
                        goto end;
                    //���ݸ��������������ĸ������ƶ�
                    AddQueueHash(pixel.posG + Vector2Int.down);
                    Random.InitState((int)(Time.deltaTime * 1000000));
                    if (Random.Range(0, 100) >= 50)
                    {
                        AddQueueHash(pixel.posG + Vector2Int.right);
                        AddQueueHash(pixel.posG + Vector2Int.left);
                    }
                    else
                    {
                        AddQueueHash(pixel.posG + Vector2Int.left);
                        AddQueueHash(pixel.posG + Vector2Int.right);
                    }

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

                    int updateProbability = 100;
                    while (_queue.Count > 0)
                    {
                        Vector2Int nextPosG = _queue.Dequeue();
                        Block nextBlock = SceneManager.Inst.NowScene.GetBlock(Block.GloabToBlock(nextPosG));
                        if (nextBlock == null)
                        {
                            updateProbability -= updateProbabilityDecay1;
                            continue;
                        }

                        Pixel nextPixel = nextBlock.GetPixel(nextBlock.GloabToPixel(nextPosG));

                        if ((nextPixel.info.typeName == "����") ||    //��һ����Ϊ����
                            (nextPixel.info.fluidType == 1 && pixel.info.fluidDensity > nextPixel.info.fluidDensity) ||  //�¸���ΪҺ�壬���ܶȸ�����
                            (nextPixel.info.fluidType == 2))  //�¸���Ϊ����
                        {
                            if (Random.Range(0, 100) < updateProbability)
                            {
                                //���������㸽��3x3�ĵ㶼����������¶���
                                for (int y = -2; y <= 2; y++)
                                    for (int x = -2; x <= 2; x++)
                                    {
                                        //  var tempG = nextPosG + new Vector2Int(0, 0);
                                        var tempG = nextPosG + new Vector2Int(x, y);
                                        Block timpBlock = SceneManager.Inst.NowScene.GetBlock(Block.GloabToBlock(tempG));
                                        if (timpBlock == null) continue;
                                        var tempP = timpBlock.GloabToPixel(tempG);
                                        AddHashSet(timpBlock.fluidUpdateHash1[tempP.y], tempP);
                                    }
                                SwapFluid(nextPosG, pixel.posG);
                                stopUpdate = false;

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

            }
        }
        public void UpdateFluid3()
        {
            for (int i = 0; i < fluidUpdateHash3.Length; i++)
            {
                _posList.Clear();
                foreach (var pos in fluidUpdateHash3[i])
                    _posList.Add(pos);
                foreach (var pos in _posList)
                {
                    _queue.Clear();
                    _hash.Clear();
                    Pixel pixel = GetPixel(pos);
                    //���λΪfalseʱ������˵��޷��������Ƴ����¶���
                    bool stopUpdate = true;
                    if (pixel.info.fluidType != 3)
                        goto end;
                    //���ݸ��������������ĸ������ƶ�
                    AddQueueHash(pixel.posG + Vector2Int.down);
                    Random.InitState((int)(Time.deltaTime * 1000000));
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
                        Block nextBlock = SceneManager.Inst.NowScene.GetBlock(Block.GloabToBlock(nextPosG));
                        if (nextBlock == null) continue;

                        Pixel nextPixel = nextBlock.GetPixel(nextBlock.GloabToPixel(nextPosG));

                        if ((nextPixel.info.typeName == "����") ||    //��һ����Ϊ����
                            (nextPixel.info.fluidType == 1) ||  //�¸���ΪҺ��
                            (nextPixel.info.fluidType == 2) ||  //�¸���Ϊ����
                            (nextPixel.info.fluidType == 3 && pixel.info.fluidDensity > nextPixel.info.fluidDensity && Random.Range(0, 100) >= 50))//�¸���Ϊ���壬���ܶȱ������ʱ����ʻ����
                        {
                            //���������㸽��3x3�ĵ㶼����������¶���
                            for (int y = -1; y <= 1; y++)
                                for (int x = -1; x <= 1; x++)
                                {
                                    //  var tempG = nextPosG + new Vector2Int(0, 0);
                                    var tempG = nextPosG + new Vector2Int(x, y);
                                    Block timpBlock = SceneManager.Inst.NowScene.GetBlock(Block.GloabToBlock(tempG));
                                    if (timpBlock == null) continue;
                                    var tempP = timpBlock.GloabToPixel(tempG);
                                    AddHashSet(timpBlock.fluidUpdateHash3[tempP.y], tempP);
                                }
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
            var block0 = SceneManager.Inst.NowScene.GetBlock(GloabToBlock(p0_G));
            var block1 = SceneManager.Inst.NowScene.GetBlock(GloabToBlock(p1_G));
            var p0 = block0.GetPixel(block0.GloabToPixel(p0_G));
            var p1 = block1.GetPixel(block1.GloabToPixel(p1_G));
            var temp = p0.pos;
            p0.pos = p1.pos;
            p1.pos = temp;
            var p0_Clone = p0.Clone();
            var p1_Clone = p1.Clone();
            block1.SetPixel(p0_Clone, false, false);
            block0.SetPixel(p1_Clone, false, false);
            block1.DrawPixelAsync(p0_Clone.pos, BlockMaterial.GetPixelColorInfo(p0_Clone.colorName).color);
            block0.DrawPixelAsync(p1_Clone.pos, BlockMaterial.GetPixelColorInfo(p1_Clone.colorName).color);
        }

        #region �����������ʱ����
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

        public Transform colliderNode;
        public readonly BoxCollider2D[,] allCollider = new BoxCollider2D[Block.Size.x, Block.Size.y];
        protected void ChangeCollider(PixelTypeInfo old, Pixel nowPixel)
        {

            //bool oldCollider = (old == null || !old.collider) ? false : true;
            ////ԭ������ײ�䣬�����оʹ�������֮ɾ��
            //if (oldCollider == false && nowPixel.info.collider) GreedyCollider.TryExpandCollider((Block)this, nowPixel.pos);
            //else if (oldCollider && nowPixel.info.collider == false) GreedyCollider.TryShrinkCollider((Block)this, nowPixel.pos);
            //Log.Print(oldCollider +"|"+nowPixel.info.collider);
        }
    }
}
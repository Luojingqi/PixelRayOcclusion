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

    public class BlockManager : MonoBehaviour
    {

        public static BlockManager Inst;

        public void Awake() { Inst = this; }

        public CrossList<Block> BlockCrossList = new CrossList<Block>();
        public CrossList<BackgroundBlock> BackgroundCrossList = new CrossList<BackgroundBlock>();

        #region ��������
        /// <summary>
        /// �ڿ����괦��������
        /// </summary>
        /// <param name="blockPos"></param>
        public void CreateBlock(Vector2Int blockPos)
        {
            if (BlockCrossList[blockPos] != null) return;
            var block = BlockPool.TakeOutT();
            BlockCrossList[blockPos] = block;
            block.transform.position = Block.BlockToWorld(blockPos);
            block.transform.parent = BlockNode;
            block.BlockPos = blockPos;
        }
        public void CreateBackground(Vector2Int blockPos)
        {
            if (BackgroundCrossList[blockPos] != null) return;
            var back = BackgroundPool.TakeOutT();
            BackgroundCrossList[blockPos] = back;
            back.transform.position = Block.BlockToWorld(blockPos);
            back.transform.parent = BlockCrossList[blockPos].transform;
            back.BlockPos = blockPos;
        }
        #endregion

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


        private GameObjectPool<Block> BlockPool;
        private GameObjectPool<BackgroundBlock> BackgroundPool;
        public GameObjectPool<BoxCollider2D> BoxCollider2DPool;

        public void Start()
        {
            BlockMaterial.Init();
            BlockNode = new GameObject("BlockNode").transform;
            PoolNode = new GameObject("PoolNode").transform;
            InitBlockPool();
            InitBackgroundPool();
            InitBoxCollider2DPool();

            DrawThread.Init(() =>
           {
               //��������
               //ThreadPool.QueueUserWorkItem((obj) =>
               //{
               //    StarManager.Inst.CreateStar(new(50, 50), 20, 20, 3, 10);
               //    StarManager.Inst.CreateStar(new(47, 52), 10, 10, 2, 10);
               //    StarManager.Inst.CreateStar(new(52, 48), 30, 10, 1, 10);
               //    StarManager.Inst.CreateStar(new(52, 48), 31, 10, 1, 10);
               //    StarManager.Inst.CreateStar(new(52, 48), 32, 10, 1, 10);
               //    StarManager.Inst.CreateStar(new(52, 48), 33, 10, 1, 10);
               //    //StarManager.Inst.CreateStar(new(45, 55), 19, 10, 2, 10);
               //    for (int i = 0; i <= 0; i++)
               //    {
               //        for (int j = 0; j <= 0; j++)
               //        {
               //            var block = BlockManager.Inst.BlockCrossList[i][j];
               //            var colliderDataList = GreedyCollider.CreateColliderDataList(block, new(0, 0), new(Block.Size.x - 1, Block.Size.y - 1));
               //            lock (BlockManager.Inst.mainThreadEventLock)
               //                BlockManager.Inst.mainThreadEvent += () => { GreedyCollider.CreateColliderAction(block, colliderDataList); };
               //            BlockManager.Inst.En_Lock_DrawApplyQueue(block);
               //        }
               //    }
               //});

               BlockMaterial.FirstBind();
           });
        }
        Transform BlockNode;
        Transform PoolNode;

        private void InitBlockPool()
        {
            #region ����Block��ʼGameObject
            GameObject go = new GameObject("Block");
            SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
            renderer.sharedMaterial = BlockShareMaterialManager.ShareMaterial;
            go.gameObject.SetActive(false);
            go.AddComponent<Block>();
            #endregion
            GameObject blockPoolGo = new GameObject("BlockPool");
            blockPoolGo.transform.parent = PoolNode;
            BlockPool = new GameObjectPool<Block>(go, blockPoolGo.transform, 20, true);
            BlockPool.CreateEventT += (g, t) =>
            {
                t.Init();
            };
            go.transform.parent = blockPoolGo.transform;
        }
        private void InitBackgroundPool()
        {
            #region ����Background��ʼGameObject
            GameObject go = new GameObject("BackgroundBlock");
            SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
            renderer.sharedMaterial = BackgroundShareMaterialManager.ShareMaterial;
            go.gameObject.SetActive(false);
            go.AddComponent<BackgroundBlock>();
            #endregion
            GameObject backgroundPoolGo = new GameObject("BackgroundPool");
            backgroundPoolGo.transform.parent = PoolNode;
            BackgroundPool = new GameObjectPool<BackgroundBlock>(go, backgroundPoolGo.transform, 20, true);
            BackgroundPool.CreateEventT += (g, t) =>
            {
                t.Init();
            };
            go.transform.parent = backgroundPoolGo.transform;
        }
        private void InitBoxCollider2DPool()
        {
            GameObject boxCollider2DPoolGo = new GameObject("BoxCollider2DPool");
            boxCollider2DPoolGo.transform.parent = PoolNode;
            BoxCollider2D boxCollider = new GameObject("boxCollider").AddComponent<BoxCollider2D>();
            boxCollider.gameObject.SetActive(false);
            boxCollider.gameObject.layer = 9;
            boxCollider.transform.parent = boxCollider2DPoolGo.transform;
            BoxCollider2DPool = new GameObjectPool<BoxCollider2D>(boxCollider.gameObject, boxCollider2DPoolGo.transform, 10000, true);
        }

        [NonSerialized]
        /// <summary>
        /// ��Ҫ���µ�����
        /// </summary>
        public List<Block> BlockUpdateList = new List<Block>();
        /// <summary>
        /// �����Ҫ���µ�����
        /// </summary>
        public void AddUpdateBlock(Block block)
        {
            int l = 0;
            int r = BlockUpdateList.Count - 1;
            int b = 0;
            while (l <= r)
            {
                b = (l + r) / 2;
                if (block.BlockPos.y < BlockUpdateList[b].BlockPos.y)
                    r = b - 1;
                else if (block.BlockPos.y > BlockUpdateList[b].BlockPos.y)
                    l = b + 1;
                else break;
            }
            BlockUpdateList.Add(block);
            if (block.BlockPos.y > BlockUpdateList[b].BlockPos.y) b++;


            for (int i = b; i < BlockUpdateList.Count - 1; i++)
            {
                Block temp = BlockUpdateList[i];
                BlockUpdateList[i] = BlockUpdateList[BlockUpdateList.Count - 1];
                BlockUpdateList[BlockUpdateList.Count - 1] = temp;
            }
        }


        string n = "��Դ2";
        public void Update()
        {

            Vector3 m = Input.mousePosition;
            m.z = 1;
            m = Camera.main.ScreenToWorldPoint(m);
            Vector2Int blockPos = Block.WorldToBlock(m);
            Vector2Int gloabPos = Block.WorldToGloab(m);
            Vector2Byte pixelPos = Block.WorldToPixel(m);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ThreadPool.QueueUserWorkItem((obj) =>
                {
                    StarManager.Inst.CreateStar(new(10, 10), 20, 20, "͸��ǽ", 10);
                    StarManager.Inst.CreateStar(new(10, 10), 21, 20, "͸��ǽ", 10);
                    StarManager.Inst.CreateStar(new(15, 15), 10, 10, "͸��ǽ", 10);
                    StarManager.Inst.CreateStar(new(15, 15), 30, 10, "��͸��ǽ", 10);
                    StarManager.Inst.CreateStar(new(15, 15), 31, 10, "��͸��ǽ", 10);
                    StarManager.Inst.CreateStar(new(15, 15), 33, 10, "��͸��ǽ", 10);
                    //StarManager.Inst.CreateStar(new(0, 0), 19, 10, "��Դ", 10);
                    for (int i = 0; i <= 0; i++)
                    {
                        for (int j = 0; j <= 0; j++)
                        {
                            var block = BlockManager.Inst.BlockCrossList[i][j];
                            var colliderDataList = GreedyCollider.CreateColliderDataList(block, new(0, 0), new(Block.Size.x - 1, Block.Size.y - 1));
                            lock (BlockManager.Inst.mainThreadEventLock)
                                BlockManager.Inst.mainThreadEvent += () => { GreedyCollider.CreateColliderAction(block, colliderDataList); };
                        }
                    }
                });
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                //mat.pixelColorIndexToShaderArray[0] = new PixelColorIndexToShader() { color = new Vector4(0, 1, 0, 1) };
                //mat.pixelColorIndexToShaderBufffer.SetData(mat.pixelColorIndexToShaderArray);
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
                n = "��Դ0";
            if (Input.GetKeyDown(KeyCode.Alpha2))
                n = "��Դ1";
            if (Input.GetKeyDown(KeyCode.Alpha3))
                n = "��Դ2";
            if (Input.GetKeyDown(KeyCode.Alpha4))
                n = "��Դ3";
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Block block = BlockCrossList[blockPos];
                block.SetPixel(Pixel.TakeOut(n, pixelPos));
                block.DrawPixelAsync();
            }
            BlockMaterial.Update();
            if (Monitor.TryEnter(mainThreadEventLock))
            {
                try
                {
                    mainThreadEvent?.Invoke();
                    mainThreadEvent = null;
                }
                finally
                {
                    Monitor.Exit(mainThreadEventLock);
                }
            }

            if (Monitor.TryEnter(DrawApplyQueue))
            {
                try
                {
                    while (DrawApplyQueue.Count > 0)
                    {
                        BlockBase blockBase = DrawApplyQueue.Dequeue();
                        blockBase.spriteRenderer.sprite.texture.Apply();
                        switch (blockBase)
                        {
                            case Block block: BlockMaterial.SetBlock(block); break;
                            case BackgroundBlock background: BlockMaterial.SetBackgroundBlock(background); break;
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(DrawApplyQueue);
                }
            }

        }

        #region ����������߳���
        /// <summary>
        /// ����ͼ��������У����߳���ӣ���Ⱦ�߳�ȡ��
        /// </summary>
        public readonly Queue<DrawGraphTaskData> DrawGraphTaskQueue = new Queue<DrawGraphTaskData>();
        /// <summary>
        /// ������������Ҫ�ύ�Ķ��У���Ⱦ�߳���ӣ����߳�ȡ��������ֻ���ṩһ��ѡ�񣬵�������ʾRO��ʹ������
        /// </summary>
        public readonly Queue<BlockBase> DrawApplyQueue = new Queue<BlockBase>();
        /// <summary>
        /// ����&��ס �ύ�������ö���(�������ύ���Կ�)
        /// </summary>
        public void En_Lock_DrawApplyQueue(BlockBase block)
        {
            lock (DrawApplyQueue)
                DrawApplyQueue.Enqueue(block);
        }
        /// <summary>
        /// ���һ������ͼ�ε�����
        /// </summary>
        /// <param name="drawTask"></param>
        public async void EnqueueDrawGraphTaskAsync(DrawGraphTaskData drawTask)
        {
            while (true)
            {
                bool queueLock = false;
                try
                {
                    queueLock = Monitor.TryEnter(DrawGraphTaskQueue);

                    if (queueLock)
                    {
                        DrawGraphTaskQueue.Enqueue(drawTask);
                        break;
                    }
                    else
                    {
                        await UniTask.Yield();
                    }
                }
                finally
                {
                    if (queueLock)
                        Monitor.Exit(DrawGraphTaskQueue);
                }
            }
        }
        /// <summary>
        /// ���̵߳��¼���
        /// </summary>
        public readonly object mainThreadEventLock = new object();
        /// <summary>
        /// ���߳��¼�������¼���Ҫ���������߳���mainThreadEventLock
        /// </summary>
        public event Action mainThreadEvent;
        #endregion
    }


}
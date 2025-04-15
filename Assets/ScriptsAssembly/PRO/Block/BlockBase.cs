using Cysharp.Threading.Tasks;
using PRO.Disk;
using PRO.DataStructure;
using PRO.Tool;
using System.Collections.Generic;
using System.Threading;
using Unity.Collections;
using UnityEngine;
using static PRO.Renderer.ComputeShaderManager;
using System.Runtime.InteropServices;
namespace PRO
{
    public class BlockBase : MonoBehaviour
    {
        #region ����ת��
        /// <summary>
        /// �����꣨�ֲ���to���������
        /// </summary>
        public Vector3 PixelToWorld(Vector2Byte pixelPos) => new Vector3((BlockPos.x * Block.Size.x + pixelPos.x) * Pixel.Size, (BlockPos.y * Block.Size.y + pixelPos.y) * Pixel.Size);
        public Vector2Int PixelToGlobal(Vector2Byte pixelPos) => new Vector2Int(BlockPos.x * Block.Size.x + pixelPos.x, BlockPos.y * Block.Size.y + pixelPos.y);

        public enum BlockType
        {
            Block,
            BackgroundBlock,
        }
        #endregion

        public virtual void Init()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            Texture2D texture = Texture2DPool.TakeOut(Block.Size.x, Block.Size.y);
            spriteRenderer.sprite = Texture2DPool.CreateSprite(texture);
            textureData = new TextureData(spriteRenderer.sprite.texture);
        }
        public SpriteRenderer spriteRenderer;

        public Screen screen;
        /// <summary>
        /// �������꣬worldPos.x / Block.Size.x
        /// </summary>
        public Vector2Int BlockPos { get { return blockPos; } set { blockPos = value; } }
        [SerializeField]
        protected Vector2Int blockPos;
        protected BlockType _blockType;
        public BlockType blockType => _blockType;
        #region ���ص㼯
        /// <summary>
        /// ���ص㼯
        /// </summary>
        protected readonly Pixel[,] allPixel = new Pixel[Block.Size.x, Block.Size.y];
        public Pixel GetPixel(Vector2Byte pos) => allPixel[pos.x, pos.y];

        /// <summary>
        /// ����ĳ���㣨����ΪPixel.pos�����м��������ǵõ����첽����ͬ��������ɫ����Ȼ����ʵʱ����
        /// </summary>
        public void SetPixel(Pixel pixel, bool drawPixelAsync = true, bool updateCollider = true, bool updateLiquidOrGas = true)
        {
            if (pixel == null) return;
            pixel.posG = PixelToGlobal(pixel.pos);
            pixel.blockBase = this;

            PixelTypeInfo removeInfo = null;
            Pixel removePixel = allPixel[pixel.pos.x, pixel.pos.y];
            //�ɵ�������һ�������ҽ������ɱ��ƻ��������;ô���0
            //  if (removePixel != null && removePixel.building != null && removePixel.building.CanByBroken == false && removePixel.durability > 0) return;
            if (removePixel != null)
            {
                if (removePixel.colorInfo.luminousRadius > 0 && pixel.colorInfo.luminousRadius == 0)//&& removePixel.colorInfo.lightRadius <= BlockMaterial.LightRadiusMax)
                    RemoveLightSource(removePixel);

                removeInfo = removePixel.typeInfo;

                //�л�����������״̬������||���
                foreach (var building in removePixel.buildingSet)
                {
                    building.PixelSwitch(building.GetBuilding_Pixel(pixel.posG, removePixel.blockBase.blockType), pixel);
                }

                removePixel.buildingSet.Clear();
                Pixel.PutIn(removePixel);
            }


            allPixel[pixel.pos.x, pixel.pos.y] = pixel;

            textureData.SetPixelInfoToShader(pixel);

            if (pixel.colorInfo.luminousRadius > 0 && pixel.colorInfo.luminousRadius <= BlockMaterial.LightRadiusMax)
                AddLightSource(pixel);

            if (this is Block)
            {
                Block block = this as Block;
                if (updateCollider)
                    block.ChangeCollider(removeInfo, pixel);
                if (updateLiquidOrGas)
                    for (int y = -1; y <= 1; y++)
                        for (int x = -1; x <= 1; x++)
                            Block.AddFluidUpdateHash(pixel.posG + new Vector2Int(x, y));
            }

            if (pixel.typeInfo.typeName == "����") queue_����.Enqueue(pixel.pos);

            if (drawPixelAsync) DrawPixelAsync(pixel.pos, pixel.colorInfo.color);
        }

        #endregion

        #region ��Դ����

        public readonly Dictionary<Vector2Byte, LightSource> lightSourceDic = new Dictionary<Vector2Byte, LightSource>();
        private void AddLightSource(Pixel pixel)
        {
            var lightSource = new LightSource(pixel.posG, pixel.colorInfo.luminousColor, pixel.colorInfo.luminousRadius);
            if (lightSourceDic.ContainsKey(pixel.pos))
                lightSourceDic[pixel.pos] = lightSource;
            else
                lightSourceDic.Add(pixel.pos, lightSource);
        }

        private void RemoveLightSource(Pixel pixel)
        {
            lightSourceDic.Remove(pixel.pos);
        }
        #endregion

        #region ��Ⱦ����

        /// <summary>
        /// ��ͼ���ݣ����ڶ��̷߳���
        /// </summary>
        public TextureData textureData;
        public struct TextureData
        {
            /// <summary>
            /// �������ɫ���ݣ�������Ⱦ�߳��޸���ɫ���ϴ���gpu
            /// </summary>
            public NativeArray<float> nativeArray;
            /// <summary>
            /// ������ÿ�����id������shader�������
            /// </summary>
            public BlockPixelInfo[] PixelInfoToShaderArray;

            public TextureData(Texture2D texture)
            {
                this.nativeArray = texture.GetRawTextureData<float>();
                PixelInfoToShaderArray = new BlockPixelInfo[texture.width * texture.height];
            }
            [StructLayout(LayoutKind.Sequential)]
            public struct BlockPixelInfo
            {
                /// <summary>
                /// ʹ�õ���ɫid
                /// </summary>
                public int colorInfoId;
                /// <summary>
                /// �;ö�
                /// </summary>
                public float durability;
                /// <summary>
                /// ͸����Ӱ��ϵ��
                /// </summary>
                public float affectsTransparency;
            };
            public void SetPixelInfoToShader(Pixel pixel)
            {
                PixelInfoToShaderArray[pixel.pos.y * Block.Size.x + pixel.pos.x] = new BlockPixelInfo()
                {
                    colorInfoId = pixel.colorInfo.index,
                    durability = Mathf.Clamp(Mathf.Abs((float)pixel.durability / pixel.typeInfo.durability), 0, 1),
                    affectsTransparency = pixel.affectsTransparency,
                };
            }
        }
        public void SetPixelInfoToShader(Vector2Byte pos) => textureData.SetPixelInfoToShader(GetPixel(pos));

        /// <summary>
        /// ������У���Ⱦ������ӽ�������Ⱦ�̷߳����޸�
        /// </summary>
        public Queue<DrawPixelTask?> DrawPixelTaskQueue = new Queue<DrawPixelTask?>();


        protected async void Enqueue(DrawPixelTask? drawTask)
        {
            while (true)
            {
                bool queueLock = false;
                try
                {
                    queueLock = Monitor.TryEnter(DrawPixelTaskQueue);

                    if (queueLock)
                    {
                        DrawPixelTaskQueue.Enqueue(drawTask);
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
                        Monitor.Exit(DrawPixelTaskQueue);
                }
            }
        }
        public void DrawPixelAsync(Vector2Byte pos, Color32 color) => Enqueue(new DrawPixelTask(pos, color));
        public async void DrawPixelAsync(Vector2Byte[] pos, Color32[] color)
        {
            int length = Mathf.Min(pos.Length, color.Length);
            while (true)
            {
                bool queueLock = false;
                try
                {
                    queueLock = Monitor.TryEnter(DrawPixelTaskQueue);

                    if (queueLock)
                    {
                        for (int i = 0; i < length; i++)
                        {
                            DrawPixelTask drawTask = new DrawPixelTask();
                            drawTask.pos = pos[i];
                            drawTask.color = color[i];
                            DrawPixelTaskQueue.Enqueue(drawTask);
                        }
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
                        Monitor.Exit(DrawPixelTaskQueue);
                }
            }
        }
        #endregion

        #region ȼ��
        int kk = 0;

        public void RandomUpdatePixelList(int time)
        {
            int num = queue_����.Count;
            while (num-- > 0)
            {
                var v = queue_����.Dequeue();
                //Debug.Log(kk + "|" + v);

                UpdatePixel_ȼ��(GetPixel(v), time);
            }
            kk++;
        }
        private static Vector2Int[] ring = new Vector2Int[] { new(1, 0), new(-1, 0), new(0, 1), new(0, -1), new(1, 1), new(1, -1), new(-1, 1), new(-1, -1) };
        private Queue<Vector2Byte> queue_���� = new Queue<Vector2Byte>();
        public void UpdatePixel_ȼ��(Pixel pixel, int time)
        {
            if (pixel.typeInfo.typeName != "����") return;

            int num = 0;
            for (int i = 0; i < 1; i++)
            {
                Vector2Int posG = pixel.posG + ring[Random.Range(0, ring.Length)];
                BlockBase block = null;
                if (this is Block) block = SceneManager.Inst.NowScene.GetBlock(Block.GlobalToBlock(posG));
                else block = SceneManager.Inst.NowScene.GetBackground(Block.GlobalToBlock(posG));
                if (block == null) continue;
                Pixel nextPixel = block.GetPixel(Block.GlobalToPixel(posG));
                if (nextPixel.typeInfo.typeName == "����") num++;
                if (nextPixel.typeInfo.burnOdds == 0) continue;
                if (UnityEngine.Random.Range(0, 100) < 10)
                {
                    Pixel pixel_ȼ����ɢ = Pixel.TakeOut(pixel.typeInfo, pixel.colorInfo, nextPixel.pos);
                    pixel_ȼ����ɢ.durability = -Random.Range(nextPixel.typeInfo.burnTimeRange.x, nextPixel.typeInfo.burnTimeRange.y);
                    //ȼ����ɢ
                    block.SetPixel(pixel_ȼ����ɢ);
                }
            }
            pixel.durability += time;
            if (pixel.durability >= 0)
            {
                SetPixel(Pixel.����.Clone(pixel.pos));
                return;
            }

            Pixel newPixel = Pixel.TakeOut("����", num, pixel.pos);
            newPixel.durability = pixel.durability;
            SetPixel(newPixel);
        }

        #endregion


        /// <summary>
        /// �����ƻ�����
        /// </summary>
        /// <param name="hardness">��Ӳ�ȣ�-1�������Ӽ�Ӳ��ֱ�Ӷ��;ö��ƻ�</param>
        /// <param name="durability">�ƻ����;ö�</param>
        public void TryDestroyPixel(Vector2Byte pixelPos, int hardness = -1, int durability = int.MaxValue)
        {
            Pixel pixel = GetPixel(pixelPos);
            if (pixel.typeInfo.typeName == "����") return;
            if (hardness == -1 && pixel.typeInfo.hardness != -1 || hardness >= pixel.typeInfo.hardness && pixel.typeInfo.durability >= 0)
            {
                pixel.durability -= durability;
                if (pixel.durability <= 0)
                    pixel.blockBase.SetPixel(Pixel.����.Clone(pixel.pos));
            }
        }
    }
}
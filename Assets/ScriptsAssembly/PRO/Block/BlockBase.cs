using Cysharp.Threading.Tasks;
using PRO.DataStructure;
using PRO.Disk;
using PRO.Tool;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Unity.Collections;
using UnityEngine;
using static PRO.Renderer.ComputeShaderManager;
namespace PRO
{
    public partial class BlockBase : MonoBehaviour, IScene
    {
        #region ����ת��
        /// <summary>
        /// �����꣨�ֲ���to���������
        /// </summary>
        public Vector3 PixelToWorld(Vector2Byte pixelPos) => new Vector3((float)((BlockPos.x * Block.Size.x + pixelPos.x) * (double)Pixel.Size), (float)((BlockPos.y * Block.Size.y + pixelPos.y) * (double)Pixel.Size));
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

        public SceneEntity Scene => _screen;
        protected SceneEntity _screen;
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

        public void Update_����ȼ��(int time)
        {
            int num = queue_����.Count;
            while (num-- > 0)
            {
                var v = queue_����.Dequeue();
                UpdatePixel_ȼ��(GetPixel(v), time);
            }
        }
        private static Vector2Int[] ring = new Vector2Int[] { new(1, 0), new(-1, 0), new(0, 1), new(0, -1), new(1, 1), new(1, -1), new(-1, 1), new(-1, -1) };
        private Queue<Vector2Byte> queue_���� = new Queue<Vector2Byte>(50);
        public void UpdatePixel_ȼ��(Pixel pixel, int time)
        {
            if (pixel.typeInfo.typeName != "����") return;

            pixel.durability += time;
            if (pixel.durability >= 0)
            {
                SetPixel(Pixel.����.Clone(pixel.pos));
                return;
            }
            else
            {
                #region �������Χһ�㳢����ɢ
                {
                    Pixel nextPixel = Scene.GetPixel((BlockType)Random.Range(0, 2), pixel.posG + ring[Random.Range(0, ring.Length)]);
                    if (Random.Range(0, 100) < nextPixel.typeInfo.burnOdds)
                        nextPixel.blockBase.SetPixel(Pixel.TakeOut(pixel.typeInfo, pixel.colorInfo, nextPixel.pos, -Random.Range(nextPixel.typeInfo.burnTimeRange.x, nextPixel.typeInfo.burnTimeRange.y)));
                }
                #endregion

                #region ������������
                if (Random.Range(0, 100) < 50)
                {
                    Pixel upPixel0 = Scene.GetPixel(BlockType.Block, pixel.posG + Vector2Int.up);
                    Pixel upPixel1 = Scene.GetPixel(BlockType.BackgroundBlock, pixel.posG + Vector2Int.up);
                    if (upPixel0 != null && upPixel0.typeInfo.typeName != "����" && upPixel1 != null && upPixel1.typeInfo.typeName != "����")
                    {
                        var particle = ParticleManager.Inst.GetPool("��������/����").TakeOut(Scene);
                        particle.SetGlobal(pixel.posG);
                        particle.Renderer.color = pixel.colorInfo.color;
                    }
                }
                #endregion


                #region ��ģ����̬�ֲ�����ǿ�ȱ仯
                int length = pixel.typeInfo.availableColors.Length / 2;
                float p = Mathf.Clamp(Mathf.Abs((float)pixel.durability / pixel.typeInfo.durability), 0, 1);
                int colorIndex = length + 1 - (int)(p / (1f / length));
                if (Random.Range(0, 100) < 50) ;
                else if (Random.Range(0, 100) < 45)
                    colorIndex += Random.Range(0, 2) == 0 ? -1 : 1;
                else if (Random.Range(0, 100) < 35)
                    colorIndex += Random.Range(0, 2) == 0 ? -2 : 2;
                else if (Random.Range(0, 100) < 23)
                    colorIndex += Random.Range(0, 2) == 0 ? -3 : 3;
                colorIndex = Mathf.Clamp(colorIndex, 0, length - 1);
                #endregion


                #region �ж���ΧһȦ�м�������
                int num = 0;
                for (int i = 0; i < 8; i++)
                {
                    Vector2Int posG = pixel.posG + ring[i];
                    Vector2Byte pos = Block.GlobalToPixel(posG);
                    {
                        BlockBase block = Scene.GetBlockBase(BlockType.Block, Block.GlobalToBlock(posG));
                        if (block != null)
                        {
                            Pixel nextPixel = block.GetPixel(pos);
                            if (nextPixel != null && nextPixel.typeInfo.typeName == "����") num++;
                        }
                    }
                    {
                        BlockBase block = Scene.GetBlockBase(BlockType.BackgroundBlock, Block.GlobalToBlock(posG));
                        if (block != null)
                        {
                            Pixel nextPixel = block.GetPixel(pos);
                            if (nextPixel != null && nextPixel.typeInfo.typeName == "����") num++;
                        }
                    }
                }
                #endregion
                //���ڻ������ĵĻ��治���⣬��ֹ̫��
                SetPixel(Pixel.TakeOut(pixel.typeInfo, BlockMaterial.GetPixelColorInfo(pixel.typeInfo.availableColors[num >= 5 ? colorIndex + 8 : colorIndex]), pixel.pos, pixel.durability));
            }
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


        public void Hide()
        {
            gameObject.SetActive(false);

        }
    }
}
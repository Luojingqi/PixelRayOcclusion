using Cysharp.Threading.Tasks;
using Google.FlatBuffers;
using PRO.DataStructure;
using PRO.Disk;
using PRO.Tool;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Unity.Collections;
using UnityEngine;
using static PRO.Renderer.ComputeShaderManager;
namespace PRO
{
    public abstract partial class BlockBase : SerializedMonoBehaviour, IScene
    {
        #region 坐标转换
        /// <summary>
        /// 点坐标（局部）to世界坐标点
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
        /// 区块坐标，worldPos.x / Block.Size.x
        /// </summary>
        public Vector2Int BlockPos { get { return blockPos; } set { blockPos = value; } }
        [SerializeField]
        protected Vector2Int blockPos;
        protected BlockType _blockType;
        public BlockType blockType => _blockType;

        #region 像素点集
        [HideInInspector]
        /// <summary>
        /// 像素点集
        /// </summary>
        protected readonly Pixel[,] allPixel = new Pixel[Block.Size.x, Block.Size.y];
        public Pixel GetPixel(Vector2Byte pos) => allPixel[pos.x, pos.y];

        public virtual void FillPixel()
        {
            if (GetPixel(new Vector2Byte(0, 0)) == null)
            {
                var typeInfo = Pixel.GetPixelTypeInfo("空气");
                var colorInfo = Pixel.GetPixelColorInfo(typeInfo.availableColors[0]);
                for (int y = 0; y < Block.Size.y; y++)
                    for (int x = 0; x < Block.Size.x; x++)
                    {
#if PRO_RENDER
                        var pixel = new Pixel(typeInfo, colorInfo, this, new Vector2Byte(x, y));
                        allPixel[x, y] = pixel;
                        this.textureData.SetPixelInfoToShader(pixel);
                        this.DrawPixelSync(new(x, y), colorInfo.color);
#else
                        allPixel[x, y] = new Pixel(typeInfo, colorInfo, this, new Vector2Byte(x, y));
#endif
                    }
            }
        }
        public void PutIn()
        {
            _screen = null;
            DrawPixelTaskQueue.Clear();
            lightSourceDic.Clear();
            queue_火焰.Clear();
        }

        #endregion

        #region 光源集合

        public readonly Dictionary<Vector2Byte, LightSource> lightSourceDic = new Dictionary<Vector2Byte, LightSource>();
        public void AddLightSource(Vector2Byte pos, PixelColorInfo colorInfo)
        {
            var lightSource = new LightSource(Block.PixelToGlobal(blockPos, pos), colorInfo.luminousColor, colorInfo.luminousRadius);
            if (lightSourceDic.ContainsKey(pos))
                lightSourceDic[pos] = lightSource;
            else
                lightSourceDic.Add(pos, lightSource);
        }

        public void RemoveLightSource(Vector2Byte pos)
        {
            lightSourceDic.Remove(pos);
        }
        #endregion

        #region 渲染任务

        /// <summary>
        /// 贴图数据，用于多线程访问
        /// </summary>
        public TextureData textureData;
        public struct TextureData
        {
            /// <summary>
            /// 纹理的颜色数据，用于渲染线程修改颜色并上传到gpu
            /// </summary>
            public NativeArray<float> nativeArray;
            /// <summary>
            /// 纹理中每个点的id，用于shader计算光照
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
                /// 使用的颜色id
                /// </summary>
                public int colorInfoId;
                /// <summary>
                /// 耐久度
                /// </summary>
                public float durability;
                /// <summary>
                /// 透明度影响系数
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
        /// 任务队列，渲染任务添加进入由渲染线程访问修改
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

        #region 燃烧

        public void Update_火焰燃烧(int time)
        {
            int num = queue_火焰.Count;
            while (num-- > 0)
            {
                var v = queue_火焰.Dequeue();
                UpdatePixel_燃烧(GetPixel(v), time);
            }
        }
        private static Vector2Int[] ring = new Vector2Int[] { new(1, 0), new(-1, 0), new(0, 1), new(0, -1), new(1, 1), new(1, -1), new(-1, 1), new(-1, -1) };
        public Queue<Vector2Byte> queue_火焰 = new Queue<Vector2Byte>(50);
        public void UpdatePixel_燃烧(Pixel pixel, int time)
        {
            if (pixel.typeInfo.typeName != "火焰") return;

            pixel.durability += time;
            if (pixel.durability >= 0)
            {
                pixel.Replace(Pixel.空气);
                return;
            }
            else
            {
                #region 随机向周围一点尝试扩散
                {
                    Pixel nextPixel = Scene.GetPixel((BlockType)Random.Range(0, 2), pixel.posG + ring[Random.Range(0, ring.Length)]);
                    if (Random.Range(0, 100) < nextPixel.typeInfo.burnOdds)
                    {
                        int durability = -Random.Range(nextPixel.typeInfo.burnTimeRange.x, nextPixel.typeInfo.burnTimeRange.y);
                        nextPixel.Replace(pixel.typeInfo, pixel.colorInfo);
                        nextPixel.durability = durability;
                    }
                }
                #endregion

                #region 产生火苗粒子
                if (Random.Range(0, 100) < 50)
                {
                    Pixel upPixel0 = Scene.GetPixel(BlockType.Block, pixel.posG + Vector2Int.up);
                    Pixel upPixel1 = Scene.GetPixel(BlockType.BackgroundBlock, pixel.posG + Vector2Int.up);
                    if (upPixel0 != null && upPixel0.typeInfo.typeName != "火焰" && upPixel1 != null && upPixel1.typeInfo.typeName != "火焰")
                    {
                        var particle = ParticleManager.Inst.GetPool("特殊粒子/火苗").TakeOut(Scene);
                        particle.SetGlobal(pixel.posG);
                        particle.Renderer.color = pixel.colorInfo.color;
                    }
                }
                #endregion


                #region 简单模拟正态分布火焰强度变化
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


                #region 判断周围一圈有几个火焰
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
                            if (nextPixel != null && nextPixel.typeInfo.typeName == "火焰") num++;
                        }
                    }
                    {
                        BlockBase block = Scene.GetBlockBase(BlockType.BackgroundBlock, Block.GlobalToBlock(posG));
                        if (block != null)
                        {
                            Pixel nextPixel = block.GetPixel(pos);
                            if (nextPixel != null && nextPixel.typeInfo.typeName == "火焰") num++;
                        }
                    }
                }
                #endregion
                {
                    //处在火焰中心的火焰不发光，防止太亮
                    int durability = pixel.durability;
                    pixel.Replace(pixel.typeInfo, Pixel.GetPixelColorInfo(pixel.typeInfo.availableColors[num >= 5 ? colorIndex + 8 : colorIndex]));
                    pixel.durability = durability;
                }
            }
        }

        #endregion


        /// <summary>
        /// 尝试破坏像素
        /// </summary>
        /// <param name="hardness">坚硬度，-1代表无视坚硬度直接对耐久度破坏，但是无法破坏坚硬度为-1的点</param>
        /// <param name="durability">破坏的耐久度</param>
        public void TryDestroyPixel(Vector2Byte pixelPos, int hardness = -1, int durability = int.MaxValue)
        {
            Pixel pixel = GetPixel(pixelPos);
            if (pixel.typeInfo.typeName == "空气") return;
            if (hardness == -1 && pixel.typeInfo.hardness != -1 || hardness >= pixel.typeInfo.hardness)
            {
                pixel.durability -= durability;
                if (pixel.durability <= 0)
                    pixel.Replace(Pixel.空气);
            }
        }


        public abstract System.Action ToDisk(FlatBufferBuilder builder);
        public abstract void ToRAM(Flat.BlockBaseData blockDiskData, CountdownEvent countdown);
    }
}
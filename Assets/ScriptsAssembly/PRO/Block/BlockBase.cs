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
        #region 坐标转换
        /// <summary>
        /// 点坐标（局部）to世界坐标点
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
        /// 区块坐标，worldPos.x / Block.Size.x
        /// </summary>
        public Vector2Int BlockPos { get { return blockPos; } set { blockPos = value; } }
        [SerializeField]
        protected Vector2Int blockPos;
        protected BlockType _blockType;
        public BlockType blockType => _blockType;
        #region 像素点集
        /// <summary>
        /// 像素点集
        /// </summary>
        protected readonly Pixel[,] allPixel = new Pixel[Block.Size.x, Block.Size.y];
        public Pixel GetPixel(Vector2Byte pos) => allPixel[pos.x, pos.y];

        /// <summary>
        /// 设置某个点（坐标为Pixel.pos），切记设置完后记得调用异步或者同步更新颜色，不然不会实时更新
        /// </summary>
        public void SetPixel(Pixel pixel, bool drawPixelAsync = true, bool updateCollider = true, bool updateLiquidOrGas = true)
        {
            if (pixel == null) return;
            pixel.posG = PixelToGlobal(pixel.pos);
            pixel.blockBase = this;

            PixelTypeInfo removeInfo = null;
            Pixel removePixel = allPixel[pixel.pos.x, pixel.pos.y];
            //旧点所属于一个建筑且建筑不可被破坏，并且耐久大于0
            //  if (removePixel != null && removePixel.building != null && removePixel.building.CanByBroken == false && removePixel.durability > 0) return;
            if (removePixel != null)
            {
                if (removePixel.colorInfo.luminousRadius > 0 && pixel.colorInfo.luminousRadius == 0)//&& removePixel.colorInfo.lightRadius <= BlockMaterial.LightRadiusMax)
                    RemoveLightSource(removePixel);

                removeInfo = removePixel.typeInfo;

                //切换建筑这个点的状态（死亡||存活
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

            if (pixel.typeInfo.typeName == "火焰") queue_火焰.Enqueue(pixel.pos);

            if (drawPixelAsync) DrawPixelAsync(pixel.pos, pixel.colorInfo.color);
        }

        #endregion

        #region 光源集合

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
        int kk = 0;

        public void RandomUpdatePixelList(int time)
        {
            int num = queue_火焰.Count;
            while (num-- > 0)
            {
                var v = queue_火焰.Dequeue();
                //Debug.Log(kk + "|" + v);

                UpdatePixel_燃烧(GetPixel(v), time);
            }
            kk++;
        }
        private static Vector2Int[] ring = new Vector2Int[] { new(1, 0), new(-1, 0), new(0, 1), new(0, -1), new(1, 1), new(1, -1), new(-1, 1), new(-1, -1) };
        private Queue<Vector2Byte> queue_火焰 = new Queue<Vector2Byte>();
        public void UpdatePixel_燃烧(Pixel pixel, int time)
        {
            if (pixel.typeInfo.typeName != "火焰") return;

            int num = 0;
            for (int i = 0; i < 1; i++)
            {
                Vector2Int posG = pixel.posG + ring[Random.Range(0, ring.Length)];
                BlockBase block = null;
                if (this is Block) block = SceneManager.Inst.NowScene.GetBlock(Block.GlobalToBlock(posG));
                else block = SceneManager.Inst.NowScene.GetBackground(Block.GlobalToBlock(posG));
                if (block == null) continue;
                Pixel nextPixel = block.GetPixel(Block.GlobalToPixel(posG));
                if (nextPixel.typeInfo.typeName == "火焰") num++;
                if (nextPixel.typeInfo.burnOdds == 0) continue;
                if (UnityEngine.Random.Range(0, 100) < 10)
                {
                    Pixel pixel_燃烧扩散 = Pixel.TakeOut(pixel.typeInfo, pixel.colorInfo, nextPixel.pos);
                    pixel_燃烧扩散.durability = -Random.Range(nextPixel.typeInfo.burnTimeRange.x, nextPixel.typeInfo.burnTimeRange.y);
                    //燃烧扩散
                    block.SetPixel(pixel_燃烧扩散);
                }
            }
            pixel.durability += time;
            if (pixel.durability >= 0)
            {
                SetPixel(Pixel.空气.Clone(pixel.pos));
                return;
            }

            Pixel newPixel = Pixel.TakeOut("火焰", num, pixel.pos);
            newPixel.durability = pixel.durability;
            SetPixel(newPixel);
        }

        #endregion


        /// <summary>
        /// 尝试破坏像素
        /// </summary>
        /// <param name="hardness">坚硬度，-1代表无视坚硬度直接对耐久度破坏</param>
        /// <param name="durability">破坏的耐久度</param>
        public void TryDestroyPixel(Vector2Byte pixelPos, int hardness = -1, int durability = int.MaxValue)
        {
            Pixel pixel = GetPixel(pixelPos);
            if (pixel.typeInfo.typeName == "空气") return;
            if (hardness == -1 && pixel.typeInfo.hardness != -1 || hardness >= pixel.typeInfo.hardness && pixel.typeInfo.durability >= 0)
            {
                pixel.durability -= durability;
                if (pixel.durability <= 0)
                    pixel.blockBase.SetPixel(Pixel.空气.Clone(pixel.pos));
            }
        }
    }
}
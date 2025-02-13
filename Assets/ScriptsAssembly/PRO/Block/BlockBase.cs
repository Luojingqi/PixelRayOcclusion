using Cysharp.Threading.Tasks;
using PRO.Disk;
using PRO.DataStructure;
using PRO.Tool;
using System;
using System.Collections.Generic;
using System.Threading;
using Unity.Collections;
using UnityEngine;
using static PRO.Renderer.ComputeShaderManager;
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
        #endregion

        public virtual void Init()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            Texture2D texture = Texture2DPool.TakeOut(Block.Size.x, Block.Size.y);
            spriteRenderer.sprite = Texture2DPool.CreateSprite(texture);
            textureData = new TextureData(spriteRenderer.sprite.texture);
        }
        public SpriteRenderer spriteRenderer;
        /// <summary>
        /// 区块坐标，worldPos.x / Block.Size.x
        /// </summary>
        public Vector2Int BlockPos { get { return blockPos; } set { blockPos = value; } }
        [SerializeField]
        protected Vector2Int blockPos;
        #region 像素点集
        /// <summary>
        /// 像素点集
        /// </summary>
        protected readonly Pixel[,] allPixel = new Pixel[Block.Size.x, Block.Size.y];
        public Pixel GetPixel(Vector2Byte pos) => allPixel[pos.x, pos.y];

        /// <summary>
        /// 设置某个点（坐标为Pixel.pos），切记设置完后记得调用异步或者同步更新颜色，不然不会实时更新
        /// </summary>
        public void SetPixel(Pixel pixel, bool updateCollider = true, bool updateLiquidOrGas = true)
        {
            pixel.posG = PixelToGlobal(pixel.pos);

            PixelTypeInfo removeInfo = null;
            Pixel removePixel = allPixel[pixel.pos.x, pixel.pos.y];
            //旧点所属于一个建筑且建筑不可被破坏
            if (removePixel != null && removePixel.building != null && removePixel.building.CanByBroken) return;
            if (removePixel != null)
            {
                if (removePixel.colorInfo.lightRadius > 0 && removePixel.colorInfo.lightRadius <= BlockMaterial.LightRadiusMax)
                    RemoveLightSource(removePixel);

                removeInfo = removePixel.typeInfo;


                if (removePixel.building != null)
                {
                    removePixel.building.PixelSwitch(removePixel.building.GetBuilding_Pixel(pixel.posG), pixel);
                    removePixel.building = null;
                }


                Pixel.PutIn(removePixel);
            }


            allPixel[pixel.pos.x, pixel.pos.y] = pixel;

            textureData.PixelIDToShader[pixel.pos.y * Block.Size.x + pixel.pos.x] = pixel.colorInfo.index;

            if (pixel.colorInfo.lightRadius > 0 && pixel.colorInfo.lightRadius <= BlockMaterial.LightRadiusMax)
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
        }

        #endregion

        #region 光源集合

        public readonly Dictionary<Vector2Byte, LightSource> lightSourceDic = new Dictionary<Vector2Byte, LightSource>();
        private void AddLightSource(Pixel pixel)
        {
            Vector2Int gloabPos = Block.PixelToGlobal(blockPos, pixel.pos);
            if (lightSourceDic.ContainsKey(pixel.pos))
            {
                lightSourceDic[pixel.pos] = new LightSource(gloabPos, pixel.colorInfo.color, pixel.colorInfo.lightRadius);
            }
            else
            {
                lightSourceDic.Add(pixel.pos, new LightSource(gloabPos, pixel.colorInfo.color, pixel.colorInfo.lightRadius));
            }
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
            public int[] PixelIDToShader;

            public TextureData(Texture2D texture)
            {
                this.nativeArray = texture.GetRawTextureData<float>();
                PixelIDToShader = new int[texture.width * texture.height];
            }
        }

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
        public void DrawPixelAsync() => Enqueue(null);
        public void DrawPixelAsync(Vector2Byte pos, Color32 color) => Enqueue(new DrawPixelTask(pos, color));
        public async void DrawPixelAsync(Vector2Byte[] pos, Color32[] color)
        {
            int length = Math.Min(pos.Length, color.Length);
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
    }
}
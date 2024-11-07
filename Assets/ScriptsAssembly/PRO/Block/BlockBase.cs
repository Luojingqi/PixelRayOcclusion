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
        public virtual void Init()
        {
            materialPropertyBlock = new MaterialPropertyBlock();
            spriteRenderer = GetComponent<SpriteRenderer>();
            Texture2D texture = DrawTool.CreateTexture();
            spriteRenderer.sprite = DrawTool.CreateSprite(texture);
            textureData = new TextureData(spriteRenderer.sprite.texture);
        }
        public SpriteRenderer spriteRenderer;
        public MaterialPropertyBlock materialPropertyBlock;
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
        /// 设置某个点，切记设置完后记得调用异步或者同步更新颜色，不然不会实时更新
        /// </summary>
        public void SetPixel(Pixel pixel, bool updateCollider = true)
        {
            PixelTypeInfo removeInfo = RemovePixel(pixel.pos);
            if (this is Block && updateCollider) ChangeCollider(removeInfo, pixel);
            allPixel[pixel.pos.x, pixel.pos.y] = pixel;
            PixelColorInfo pixelColorInfo = BlockMaterial.GetPixelColorInfo(pixel.colorName);
            textureData.PixelIDToShader[pixel.pos.y * Block.Size.x + pixel.pos.x] = pixelColorInfo.index;
            if (pixelColorInfo.lightSourceType != null && pixelColorInfo.lightSourceType != "null") AddLightSource(pixel, pixelColorInfo);
        }

        private PixelTypeInfo RemovePixel(Vector2Byte pos)
        {
            PixelTypeInfo ret = null;
            Pixel pixel = allPixel[pos.x, pos.y];
            if (pixel == null) return ret;
            PixelColorInfo pixelColorInfo = BlockMaterial.GetPixelColorInfo(pixel.colorName);
            if (pixelColorInfo.lightSourceType != null && pixelColorInfo.lightSourceType != "null") RemoveLightSource(pixel);
            ret = pixel.info;
            Pixel.PutIn(pixel);
            allPixel[pos.x, pos.y] = null;
            return ret;
        }

        private void ChangeCollider(PixelTypeInfo old, Pixel nowPixel)
        {

            //bool oldCollider = (old == null || !old.collider) ? false : true;
            ////原本无碰撞箱，现在有就创建，反之删除
            //if (oldCollider == false && nowPixel.info.collider) GreedyCollider.TryExpandCollider((Block)this, nowPixel.pos);
            //else if (oldCollider && nowPixel.info.collider == false) GreedyCollider.TryShrinkCollider((Block)this, nowPixel.pos);
            //Log.Print(oldCollider +"|"+nowPixel.info.collider);
        }
        #endregion

        #region 光源集合

        public readonly Dictionary<Vector2Byte, LightSource> lightSourceDic = new Dictionary<Vector2Byte, LightSource>();
        private void AddLightSource(Pixel pixel, PixelColorInfo pixelColorInfo)
        {
            Vector2Int gloabPos = Block.PixelToGloab(blockPos, pixel.pos);
            var lightSourceInfo = BlockMaterial.GetLightSourceInfo(pixelColorInfo.lightSourceType);
            if (lightSourceInfo == null) return;
            //lightSourceDic.Clear();
            if (lightSourceDic.ContainsKey(pixel.pos))
            {
                lightSourceDic[pixel.pos] = new LightSource(gloabPos, pixelColorInfo.color, lightSourceInfo);
            }
            else
            {
                lightSourceDic.Add(pixel.pos, new LightSource(gloabPos, pixelColorInfo.color, lightSourceInfo));
            }
            // if (lightSourceDic.Count >= 10) lightSourceDic.Clear();

        }

        private void RemoveLightSource(Pixel pixel)
        {
            lightSourceDic.Remove(pixel.pos);
            //while (true)
            //{
            //    if (Monitor.TryEnter(lightSourceDic))
            //    {
            //        lightSourceDic.Remove(pixel.pos);
            //        lightSourceBufferNeedUpdate = true;
            //    }
            //    else
            //    {
            //        await UniTask.Yield();
            //    }
            //}
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
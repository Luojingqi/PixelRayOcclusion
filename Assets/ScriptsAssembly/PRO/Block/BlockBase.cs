using Cysharp.Threading.Tasks;
using PRO.Data;
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
        /// �������꣬worldPos.x / Block.Size.x
        /// </summary>
        public Vector2Int BlockPos { get { return blockPos; } set { blockPos = value; } }
        [SerializeField]
        protected Vector2Int blockPos;
        #region ���ص㼯
        /// <summary>
        /// ���ص㼯
        /// </summary>
        protected readonly Pixel[,] allPixel = new Pixel[Block.Size.x, Block.Size.y];
        public Pixel GetPixel(Vector2Byte pos) => allPixel[pos.x, pos.y];

        /// <summary>
        /// ����ĳ���㣬�м��������ǵõ����첽����ͬ��������ɫ��ʹ�ù���׷������ν���ò�����
        /// </summary>
        public void SetPixel(Pixel pixel)
        {
            RemovePixel(pixel.pos);
            PixelColorInfo pixelColorInfo = BlockMaterial.GetPixelColorInfo(pixel.name);
            if (pixelColorInfo == null) return;
            allPixel[pixel.pos.x, pixel.pos.y] = pixel;
            textureData.PixelIDToShader[pixel.pos.y * Block.Size.x + pixel.pos.x] = pixelColorInfo.index;
            if (pixelColorInfo.lightSourceType != "null") AddLightSource(pixel, pixelColorInfo);
        }

        private void RemovePixel(Vector2Byte pos)
        {
            Pixel pixel = allPixel[pos.x, pos.y];
            if (pixel == null) return;
            PixelColorInfo pixelColorInfo = BlockMaterial.GetPixelColorInfo(pixel.name);
            if (pixelColorInfo.lightSourceType != "null") RemoveLightSource(pixel);

            if (pixel is Liquid) Liquid.PutIn(pixel as Liquid);
            else Pixel.PutIn(pixel);
            allPixel[pos.x, pos.y] = null;
        }
        #endregion

        #region ��Դ����

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

        #region ��Ⱦ����

        /// <summary>
        /// ��ͼ���ݣ����ڶ��̷߳���
        /// </summary>
        public TextureData textureData;
        public struct TextureData
        {
            /// <summary>
            /// ��ɫ���ݣ�������Ⱦ�߳��޸���ɫ���ϴ���gpu
            /// </summary>
            public NativeArray<float> nativeArray;
            /// <summary>
            /// ������ÿ�����id������shader�������
            /// </summary>
            public int[] PixelIDToShader;

            public TextureData(Texture2D texture)
            {
                this.nativeArray = texture.GetRawTextureData<float>();
                PixelIDToShader = new int[texture.width * texture.height];
            }
        }

        /// <summary>
        /// ������У���Ⱦ�������ӽ�������Ⱦ�̷߳����޸�
        /// </summary>
        public Queue<DrawPixelTask> DrawPixelTaskQueue = new Queue<DrawPixelTask>();


        protected async void Enqueue(DrawPixelTask drawTask)
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
        public void DrawPixelAsync()
        {
            Enqueue(new DrawPixelTask() { skip = true });
        }
        public void DrawPixelAsync(Vector2Byte pos, Color32 color)
        {
            DrawPixelTask drawTask = new DrawPixelTask();
            drawTask.pos = pos;
            drawTask.color = color;
            Enqueue(drawTask);
        }
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
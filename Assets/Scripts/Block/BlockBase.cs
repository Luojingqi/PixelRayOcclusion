using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using Unity.Collections;
using UnityEngine;

public class BlockBase : MonoBehaviour
{
    public virtual void Init()
    {
        materialPropertyBlock = new MaterialPropertyBlock();
        spriteRenderer = GetComponent<SpriteRenderer>();
        Texture2D texture = DrawTool.CreateTexture();
        spriteRenderer.sprite = DrawTool.CreateSprite(texture);
        textureData = new TextureData(spriteRenderer.sprite.texture);
        //spriteRenderer.SetPropertyBlock(materialPropertyBlock);
    }
    public SpriteRenderer spriteRenderer;
    public MaterialPropertyBlock materialPropertyBlock;
    /// <summary>
    /// 区块坐标，worldPos.x / Block.Size.x
    /// </summary>
    public Vector2Int BlockPos
    {
        get { return blockPos; }
        set
        {
            blockPos = value;
            //materialPropertyBlock.SetVector("BlockPos", new(blockPos.x, blockPos.y, 0, 0));
        }
    }
    [SerializeField]
    protected Vector2Int blockPos;
    #region 像素点集
    /// <summary>
    /// 像素点集
    /// </summary>
    protected readonly Pixel[,] allPixel = new Pixel[Block.Size.x, Block.Size.y];
    public Pixel GetPixel(Vector2Byte pos) => allPixel[pos.x, pos.y];

    /// <summary>
    /// 设置某个点，切记设置完后记得调用异步或者同步更新颜色，使用光线追踪无所谓调用不调用
    /// </summary>
    public void SetPixel(Pixel pixel)
    {
        allPixel[pixel.pos.x, pixel.pos.y] = pixel;
        textureData.PixelIDToShader[pixel.pos.y * Block.Size.x + pixel.pos.x] = pixel.id;
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
        /// 颜色数据，用于渲染线程修改颜色并上传到gpu
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
    public void DrawPixelAsync(Vector2Byte pos, Color color)
    {
        DrawPixelTask drawTask = new DrawPixelTask();
        drawTask.pos = pos;
        drawTask.color = color;
        Enqueue(drawTask);
    }
    public async void DrawPixelAsync(Vector2Byte[] pos, Color[] color)
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

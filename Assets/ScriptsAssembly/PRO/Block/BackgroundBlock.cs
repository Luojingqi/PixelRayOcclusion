using Google.FlatBuffers;
using PRO.DataStructure;
using PRO.Renderer;
using PRO.Flat.Ex;
using System;
using System.Threading;
using UnityEngine;
using PRO.Tool;

namespace PRO
{
    public class BackgroundBlock : BlockBase
    {
        #region ¾²Ì¬¶ÔÏó³Ø
        private static BackgroundBlock prefab;
        public static void InitPrefab()
        {
            GameObject go = new GameObject("BackgroundBlock");
            SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
            renderer.sharedMaterial = BackgroundShareMaterialManager.ShareMaterial;
            BackgroundBlock background = go.AddComponent<BackgroundBlock>();
            prefab = background;
            prefab.transform.SetParent(SceneManager.Inst.PoolNode);
        }
        public static BackgroundBlock Create()
        {
            var ret = GameObject.Instantiate(prefab);
            ret.Init();
            return ret;
        }
        public void TakeOut(SceneEntity scene, Vector2Int blockPos)
        {
            BlockPos = blockPos;
            _screen = scene;
        }

        public void PutIn(CountdownEvent countdown)
        {
            countdown.AddCount();
            PutIn();
            for (int y = 0; y < Block.Size.y; y++)
                for (int x = 0; x < Block.Size.x; x++)
                    GetPixel(new(x, y)).Clear();
            TimeManager.Inst.AddToQueue_MainThreadUpdate_Clear(() =>
            {
#if PRO_RENDER
                spriteRenderer.SetPropertyBlock(BlockMaterial.NullMaterialPropertyBlock);
#endif
                countdown.Signal();
            });
        }
        #endregion

        public override void FillPixel()
        {
            if (GetPixel(new Vector2Byte(0, 0)) == null)
            {
                var typeInfo = Pixel.GetPixelTypeInfo("±³¾°");
                var colorInfo = Pixel.GetPixelColorInfo("±³¾°É«2");
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

        public override void Init()
        {
            base.Init();
            spriteRenderer.sortingOrder = -10;

            _blockType = BlockType.BackgroundBlock;
        }

        public override Action ToDisk(FlatBufferBuilder builder)
        {
            int count = queue_»ðÑæ.Count;
            Flat.BlockBaseData.StartBackgroundFlameQueueVector(builder, count);
            for (int i = 0; i < count; i++)
            {
                var pos = queue_»ðÑæ.Dequeue();
                pos.ToDisk(builder);
                queue_»ðÑæ.Enqueue(pos);
                Debug.Log(pos);
            }
            var blockFlameQueueOffset = builder.EndVector();
            return () =>
            {
                Flat.BlockBaseData.AddBackgroundFlameQueue(builder, blockFlameQueueOffset);
            };
        }

        public override void ToRAM(Flat.BlockBaseData blockDiskData, CountdownEvent countdown)
        {
#if PRO_RENDER
            countdown.AddCount();
            TimeManager.Inst.AddToQueue_MainThreadUpdate_Clear(() =>
            {
                BlockMaterial.SetBackgroundBlock(this);
                countdown.Signal();
            });
#endif
        }
    }
}

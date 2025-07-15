using Google.FlatBuffers;
using PRO.Renderer;
using PRO.Tool;
using System;
using System.Threading;
using UnityEngine;

namespace PRO
{
    public class BackgroundBlock : BlockBase
    {
        #region 静态对象池
        private static GameObjectPool<BackgroundBlock> BackgroundPool;

        public static void InitPool()
        {
            #region 加载Background初始GameObject
            GameObject go = new GameObject("BackgroundBlock");
            SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
            renderer.sharedMaterial = BackgroundShareMaterialManager.ShareMaterial;
            BackgroundBlock background = go.AddComponent<BackgroundBlock>();
            #endregion
            GameObject backgroundPoolGo = new GameObject("BackgroundPool");
            backgroundPoolGo.transform.parent = SceneManager.Inst.PoolNode;
            BackgroundPool = new GameObjectPool<BackgroundBlock>(background, backgroundPoolGo.transform);
            BackgroundPool.CreateEvent += t => t.Init();
        }
        public static BackgroundBlock TakeOut(SceneEntity scene)
        {
            var background = BackgroundPool.TakeOut();
            background._screen = scene;
            return background;
        }

        public static void PutIn(BackgroundBlock background, CountdownEvent countdown)
        {
            countdown.AddCount();
            for (int y = 0; y < Block.Size.y; y++)
            {
                for (int x = 0; x < Block.Size.x; x++)
                {
                    Pixel pixel = background.allPixel[x, y];
                    background.allPixel[x, y] = null;
                    Pixel.PutIn(pixel);
                }
            }
            background._screen = null;

            TimeManager.Inst.AddToQueue_MainThreadUpdate_Clear(() =>
            {
                background.spriteRenderer.SetPropertyBlock(BlockMaterial.NullMaterialPropertyBlock);
                BackgroundPool.PutIn(background);
                countdown.Signal();
            });
        }
        #endregion

        public override void Init()
        {
            base.Init();
            spriteRenderer.sortingOrder = -10;

            _blockType = BlockType.BackgroundBlock;
        }

        public override Action ToDisk(FlatBufferBuilder builder)
        {
            return null;
        }

        public override void ToRAM(Flat.BlockBaseData blockDiskData, CountdownEvent countdown)
        {
            countdown.AddCount();
            TimeManager.Inst.AddToQueue_MainThreadUpdate_Clear(() =>
            {
                BlockMaterial.SetBackgroundBlock(this);
                countdown.Signal();
            });
        }
    }
}

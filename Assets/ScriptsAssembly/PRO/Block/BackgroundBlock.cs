using Cysharp.Threading.Tasks;
using PRO.Renderer;
using PRO.Tool;
using System;
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
            go.gameObject.SetActive(false);
            go.AddComponent<BackgroundBlock>();
            #endregion
            GameObject backgroundPoolGo = new GameObject("BackgroundPool");
            backgroundPoolGo.transform.parent = SceneManager.Inst.PoolNode;
            BackgroundPool = new GameObjectPool<BackgroundBlock>(go, backgroundPoolGo.transform);
            BackgroundPool.CreateEventT += (g, t) =>
            {
                t.Init();
            };
            go.transform.parent = backgroundPoolGo.transform;
        }
        public static BackgroundBlock TakeOut() => BackgroundPool.TakeOutT();

        public static void PutIn(BackgroundBlock block, Action<BuildingBase> byUnloadAllPixelAction)
        {
            block.gameObject.SetActive(false);
            for (int y = 0; y < Block.Size.y; y++)
            {
                for (int x = 0; x < Block.Size.x; x++)
                {
                    Pixel pixel = block.allPixel[x, y];
                    block.allPixel[x, y] = null;
                    Pixel.PutIn(pixel, byUnloadAllPixelAction);
                }
            }
            block.spriteRenderer.SetPropertyBlock(BlockMaterial.NullMaterialPropertyBlock);
            BackgroundPool.PutIn(block.gameObject);
        }
        #endregion

        public override void Init()
        {
            base.Init();
            spriteRenderer.sortingOrder = -1;
        }

    }
}

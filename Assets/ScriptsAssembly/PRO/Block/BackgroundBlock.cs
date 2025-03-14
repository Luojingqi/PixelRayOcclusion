using PRO.Renderer;
using PRO.Tool;
using System;
using UnityEngine;

namespace PRO
{
    public class BackgroundBlock : BlockBase
    {
        #region ��̬�����
        private static GameObjectPool<BackgroundBlock> BackgroundPool;

        public static void InitPool()
        {
            #region ����Background��ʼGameObject
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

        public static void PutIn(BackgroundBlock background, Action<BuildingBase> byUnloadAllPixelAction)
        {
            background.gameObject.SetActive(false);
            for (int y = 0; y < Block.Size.y; y++)
            {
                for (int x = 0; x < Block.Size.x; x++)
                {
                    Pixel pixel = background.allPixel[x, y];
                    background.allPixel[x, y] = null;
                    Pixel.PutIn(pixel, byUnloadAllPixelAction);
                }
            }
            background.spriteRenderer.SetPropertyBlock(BlockMaterial.NullMaterialPropertyBlock);
            BackgroundPool.PutIn(background.gameObject);
        }
        #endregion

        public override void Init()
        {
            base.Init();
            spriteRenderer.sortingOrder = -1;
        }

    }
}

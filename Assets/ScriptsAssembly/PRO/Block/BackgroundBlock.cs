using Cysharp.Threading.Tasks;
using PRO.Renderer;
using PRO.Tool;
using UnityEngine;

namespace PRO
{
    public class BackgroundBlock : BlockBase
    {
        #region ��̬�����
        private static GameObjectPool<BackgroundBlock> BackgroundPool;

        public static void InitBackgroundPool()
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
            BackgroundPool = new GameObjectPool<BackgroundBlock>(go, backgroundPoolGo.transform, 20, true);
            BackgroundPool.CreateEventT += (g, t) =>
            {
                t.Init();
            };
            go.transform.parent = backgroundPoolGo.transform;
        }
        public static BackgroundBlock TakeOut() => BackgroundPool.TakeOutT();

        public static async void PutIn(BackgroundBlock block)
        {
            block.gameObject.SetActive(false);
            for (int y = 0; y < Block.Size.y; y++)
            {
                for (int x = 0; x < Block.Size.x; x++)
                {
                    Pixel pixel = block.allPixel[x, y];
                    block.allPixel[x, y] = null;
                    Pixel.PutIn(pixel);
                }
                await UniTask.Yield();
            }
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

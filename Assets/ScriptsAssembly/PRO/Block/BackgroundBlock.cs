using PRO.Proto.Block;
using PRO.Renderer;
using PRO.Tool;
using System.Text;
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
        public static BackgroundBlock TakeOut(SceneEntity scene)
        {
            var background = BackgroundPool.TakeOutT();
            background._screen = scene;
            return background;
        }

        public static void PutIn(BackgroundBlock background)
        {
            background.gameObject.SetActive(false);
            for (int y = 0; y < Block.Size.y; y++)
            {
                for (int x = 0; x < Block.Size.x; x++)
                {
                    Pixel pixel = background.allPixel[x, y];
                    background.allPixel[x, y] = null;
                    Pixel.PutIn(pixel);
                }
            }
            background.spriteRenderer.SetPropertyBlock(BlockMaterial.NullMaterialPropertyBlock);

            background._screen = null;

            BackgroundPool.PutIn(background.gameObject);
        }
        #endregion

        public override void Init()
        {
            base.Init();
            spriteRenderer.sortingOrder = -10;

            _blockType = BlockType.BackgroundBlock;
        }

        public override void ToDisk(ref BlockBaseData data)
        {

        }

        public override void ToRAM(BlockBaseData data)
        {
            SceneManager.Inst.AddMainThreadEvent_Clear_Lock(() =>
            {
                BlockMaterial.SetBackgroundBlock(this);
            });
        }
    }
}

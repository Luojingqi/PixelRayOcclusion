using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;

namespace PRO.Tool
{
    public static class Texture2DPool
    {
        private static List<Texture2D> texture2DList = new List<Texture2D>();
        private static Dictionary<Texture2D, Sprite> texture_Sprite_Dic = new Dictionary<Texture2D, Sprite>();
        /// <summary>
        /// 取出一个宽高大于等于输入宽高的纹理，反复复用
        /// </summary>
        public static Texture2D TakeOut(int width, int hight)
        {
            int minWidth = int.MaxValue;
            int minHeight = int.MaxValue;
            int index = -1;
            for (int i = 0; i < texture2DList.Count; i++)
            {
                Texture2D tex = texture2DList[i];
                if (width < tex.width && hight < tex.height)
                    if (tex.width < minWidth && tex.height < minHeight)
                    {
                        minWidth = tex.width; minHeight = tex.height;
                        index = i;
                    }
            }
            if (index == -1) return NewTexture2D(width, hight);
            else
            {
                Texture2D ret = texture2DList[index];
                texture2DList.RemoveAt(index);
                return ret;
            }
        }
        public static void PutIn(Texture2D texture2D)
        {
            FillTransparentColor(texture2D);
            texture2DList.Add(texture2D);
        }
        /// <summary>
        /// 将一个纹理填充为透明
        /// </summary>
        public static void FillTransparentColor(this Texture2D texture2D)
        {
            NativeArray<float> ptr = texture2D.GetRawTextureData<float>();
            int length = texture2D.width * texture2D.height;
            for (int i = 0; i < length; i++)
            {
                int strat = i * 4;
                ptr[strat++] = 0;
                ptr[strat++] = 0;
                ptr[strat++] = 0;
                ptr[strat] = 0;
            }
        }

        private static Texture2D NewTexture2D(int width, int hight)
        {
            Texture2D texture = new Texture2D(width, hight, TextureFormat.RGBAFloat, false, true);
            texture.filterMode = FilterMode.Point;
            texture.FillTransparentColor();
            return texture;
        }
        /// <summary>
        /// 获取与此贴图唯一绑定的精灵，用于复用，会存储映射关系
        /// </summary>
        public static Sprite GetOnlySprite(Texture2D texture)
        {
            if (texture_Sprite_Dic.TryGetValue(texture, out Sprite ret)) return ret;
            else
            {
                ret = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0f, 0f), 1 / Pixel.Size);
                texture_Sprite_Dic.Add(texture, ret);
                return ret;
            }
        }
        /// <summary>
        /// 创建此贴图的精灵，不存储映射关系
        /// </summary>
        public static Sprite CreateSprite(Texture2D texture) => Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0f, 0f), 1 / Pixel.Size);


        private static GameObjectPool<SpriteRenderer> spriteRendererPool;

        public static void InitPool()
        {
            SpriteRenderer spriteRenderer = new GameObject("SpriteRenderer").AddComponent<SpriteRenderer>();
            spriteRenderer.gameObject.SetActive(false);
            spriteRendererPool = new GameObjectPool<SpriteRenderer>(spriteRenderer.gameObject, SceneManager.Inst.PoolNode);
        }
        public static SpriteRenderer TakeOutSpriteRenderer() => spriteRendererPool.TakeOutT();
        public static void PutIn(SpriteRenderer spriteRenderer)
        {
            spriteRenderer.sprite = null;
            spriteRendererPool.PutIn(spriteRenderer.gameObject);
        }

        #region 加载磁盘图片
        /// <summary>
        /// 异步加载本地磁盘图片文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static async Task<Texture2D> LoadTexture2DAsync(string filePath)
        {
            byte[] byteArray = null;
            if (File.Exists(filePath))
                byteArray = await File.ReadAllBytesAsync(filePath);
            if (byteArray == null) return null;
            Texture2D texture = new Texture2D(0, 0);
            texture.LoadImage(byteArray);
            return texture;
        }
        /// <summary>
        /// 加载本地磁盘图片文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Texture2D LoadTexture2D(string filePath)
        {
            byte[] byteArray = null;
            if (File.Exists(filePath))
                byteArray = File.ReadAllBytes(filePath);
            if (byteArray == null) return null;
            Texture2D texture = new Texture2D(0, 0);
            texture.LoadImage(byteArray);
            return texture;
        }
        #endregion
    }
}

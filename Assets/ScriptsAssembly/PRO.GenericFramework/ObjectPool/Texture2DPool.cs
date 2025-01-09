using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;

namespace PRO.Tool
{
    public class Texture2DPool 
    {
        private List<Texture2D> texture2DList = new List<Texture2D>();
        public Texture2D TakeOut(int width, int hight)
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
        public void PutIn(Texture2D texture2D)
        {
            ClearTexture2D(texture2D);
            texture2DList.Add(texture2D);
        }

        public static void ClearTexture2D(Texture2D texture2D)
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

        private Texture2D NewTexture2D(int width,int hight)
        {
            Texture2D texture = new Texture2D(width, hight);
            texture.filterMode = FilterMode.Point;
            return texture;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using PRO.DataStructure;
using Unity.Collections;
using UnityEngine;
namespace PRO.Tool
{
    public static class DrawTool
    {
        /// <summary>
        /// ���ر��ش���ͼƬ�ļ�
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
        /// <summary>
        /// �첽���ر��ش���ͼƬ�ļ�
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

        public static Texture2D CreateTexture()
        {
            Texture2D texture = new Texture2D(Block.Size.x, Block.Size.y, TextureFormat.RGBAFloat, false, true);
            texture.filterMode = FilterMode.Point;
            return texture;
        }
        public static Sprite CreateSprite(Texture2D texture)
        {
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0f, 0f), 1 / Pixel.Size);
        }
        #region ���Ƶ�
        public static void DrawPixelSync(this BlockBase block, Vector2Byte pos, Color color)
        {
            if (pos.x >= Block.Size.x || pos.x < 0 || pos.y >= Block.Size.y || pos.y < 0)
                return;
            int index = (Block.Size.x * pos.y + pos.x) * 4;
            block.textureData.nativeArray[index++] = color.r;
            block.textureData.nativeArray[index++] = color.g;
            block.textureData.nativeArray[index++] = color.b;
            block.textureData.nativeArray[index++] = color.a;
        }
        public static void DrawPixelSync(List<Vector2Int> list, Color32 color)
        {
            foreach (var v in list)
            {
                var block = BlockManager.Inst.BlockCrossList[Block.GloabToBlock(v)];
                block.DrawPixelSync(block.GloabToPixel(v), color);
            }
        }
        public static Color32 GetColor32Sync(this BlockBase block, Vector2Int pos)
        {
            if (Block.Relocation(block, pos, out Vector2Int rightBlock, out Vector2Byte rightPos))
            {
                Color ret = new Color32(0, 0, 0, 0);
                int index = (Block.Size.x * pos.y + pos.x) * 4;
                ret.r = block.textureData.nativeArray[index++];
                ret.g = block.textureData.nativeArray[index++];
                ret.b = block.textureData.nativeArray[index++];
                ret.a = block.textureData.nativeArray[index++];
                return ret;
            }
            else
            {
                var newBlock = BlockManager.Inst.BlockCrossList[rightBlock];
                if (newBlock == null) return new Color32(0, 0, 0, 0);
                else return newBlock.GetColor32Sync((Vector2Int)rightPos);
            }
        }
        public static Color32 GetColor32Sync(NativeArray<byte> png, int width, Vector2Int pos)
        {
            Color32 ret = new Color32(0, 0, 0, 0);
            int index = (width * pos.y + pos.x) * 4;
            ret.r = png[index++];
            ret.g = png[index++];
            ret.b = png[index++];
            ret.a = png[index++];
            return ret;

        }
        #endregion

        #region ��ȡ����ͼ����Ҫ�ĵ�
        /// <summary>
        /// ��ȡһ��ֱ��
        /// </summary>
        public static List<Vector2Int> GetLine(Vector2Int pos_G0, Vector2Int pos_G1)
        {
            List<Vector2Int> ret = new List<Vector2Int>();
            int dx = Math.Abs(pos_G1.x - pos_G0.x);
            int dy = Math.Abs(pos_G1.y - pos_G0.y);

            int sx = pos_G0.x < pos_G1.x ? 1 : -1;
            int sy = pos_G0.y < pos_G1.y ? 1 : -1;
            int err = dx - dy;

            while (true)
            {

                ret.Add(pos_G0);
                if (pos_G0.x == pos_G1.x && pos_G0.y == pos_G1.y)
                    break;

                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    pos_G0.x += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    pos_G0.y += sy;
                }
            }
            return ret;
        }
        /// <summary>
        /// ���һ��Բ��
        /// </summary>
        public static List<Vector2Int> GetRing(Vector2Int pos_G, int r)
        {
            List<Vector2Int> ret = new List<Vector2Int>();
            int x = r;
            int y = 0;
            int decision = 1 - x;

            while (x >= y)
            {
                ret.Add(new Vector2Int(pos_G.x + x, pos_G.y + y));
                ret.Add(new Vector2Int(pos_G.x + x, pos_G.y - y));
                ret.Add(new Vector2Int(pos_G.x - x, pos_G.y + y));
                ret.Add(new Vector2Int(pos_G.x - x, pos_G.y - y));
                ret.Add(new Vector2Int(pos_G.x + y, pos_G.y + x));
                ret.Add(new Vector2Int(pos_G.x - y, pos_G.y + x));
                ret.Add(new Vector2Int(pos_G.x + y, pos_G.y - x));
                ret.Add(new Vector2Int(pos_G.x - y, pos_G.y - x));
                y += 1;
                if (decision <= 0)
                    decision += 2 * y + 1;
                else
                {
                    x -= 1;
                    decision += 2 * (y - x) + 1;
                }
            }
            return ret;
        }
        /// <summary>
        /// ��ȡһ��ʵ��Բ
        /// </summary>
        public static List<Vector2Int> GetCircle(Vector2Int pos_G, int r)
        {
            int rr = r * r;
            List<Vector2Int> list = new List<Vector2Int>();
            for (int x = -r; x <= r; x++)
                for (int y = -r; y <= r; y++)
                    if (x * x + y * y <= rr) list.Add(new Vector2Int(x + pos_G.x, y + pos_G.y));
            return list;
        }
        /// <summary>
        /// ��ȡһ������λ�
        /// </summary>
        public static List<Vector2Int> GetPolygon(Vector2Int pos_G, int r, int n, float rotate = 0)
        {
            List<Vector2Int> ret = new List<Vector2Int>();
            Vector2Int[] D = new Vector2Int[n];
            for (int i = 0; i < D.Length; i++)
            {
                D[i] = pos_G + new Vector2Int((int)(r * Mathf.Cos(i * Mathf.PI / n * 2 + rotate)), (int)(r * Mathf.Sin(i * Mathf.PI / n * 2 + rotate)));
            }
            for (int i = 0; i < D.Length; i++)
            {
                ret.AddRange(GetLine(D[i % n], D[(i + 1) % n]));
            }
            return ret;
        }
        /// <summary>
        /// ��ȡһ���˱���
        /// </summary>
        public static List<Vector2Int> GetOctagon(Vector2Int pos_G, int r)
        {
            return GetPolygon(pos_G, r, 8, Mathf.PI / 8);
        }

        #endregion



        /// <summary>
        /// �Ƚ�������ɫ
        /// </summary>
        public static bool Equals(Color32 c0, Color32 c1) => c0.a == c1.a && c0.b == c1.b && c0.r == c1.r && c0.g == c1.g;
    }
}
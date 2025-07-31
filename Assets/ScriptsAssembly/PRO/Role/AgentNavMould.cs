using UnityEngine;

namespace PRO
{
    public class AgentNavMould
    {
        public struct Mould
        {
            public readonly Vector2Int size;
            public readonly Vector2Int offset;
            /// <summary>
            /// 体积
            /// </summary>
            public readonly int area;
            public Vector2Int center => new Vector2Int(size.x / 2 - offset.x, size.y / 2 - offset.y);
            public Vector2 centerW => new Vector2(size.x / 2 - offset.x, (size.y + 1) / 2 - offset.y) * Pixel.Size;
            public Mould(Vector2Int size, Vector2Int offset)
            {
                this.size = size;
                this.offset = offset;
                this.area = size.x * size.y;
            }
        }
        public readonly Mould mould;
        /// <summary>
        /// 检查盒子，角色模型的内边框
        /// </summary>
        public readonly Vector2Int[] chackBox;
        /// <summary>
        /// 角色每次可以移动的下一个点的集合
        /// </summary>
        public readonly Vector2Int[] walkRing;

        public AgentNavMould(Mould mould)
        {
            this.mould = mould;
            chackBox = new Vector2Int[mould.size.x * 2 + mould.size.y * 2 - 4];
            int index = 0;
            for (int x = 0; x < mould.size.x; x++)
            {
                chackBox[index++] = new Vector2Int(x, 0);
                chackBox[index++] = new Vector2Int(x, mould.size.y - 1);
            }
            for (int y = 1; y < mould.size.y - 1; y++)
            {
                chackBox[index++] = new Vector2Int(0, y);
                chackBox[index++] = new Vector2Int(mould.size.x - 1, y);
            }
            index = 0;
            walkRing = new Vector2Int[10];
            for (int x = -1; x <= 1; x++)
                for (int y = -2; y <= 2; y++)
                    if (x != 0)
                        walkRing[index++] = new Vector2Int(x, y);
        }
    }
}

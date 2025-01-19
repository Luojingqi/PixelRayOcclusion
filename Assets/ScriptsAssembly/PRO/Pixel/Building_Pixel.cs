using PRO.Disk;
using UnityEngine;

namespace PRO
{
    public class Building_Pixel
    {
        private PixelTypeInfo typeInfo;
        private PixelColorInfo colorInfo;
        /// <summary>
        /// 偏移
        /// </summary>
        private Vector2Int offset;

        public Pixel Pixel;
        public PixelTypeInfo TypeInfo => typeInfo;
        public PixelColorInfo ColorInfo => colorInfo;
        /// <summary>
        /// 偏移
        /// </summary>
        public Vector2Int Offset => offset;


        /// <summary>
        /// 创建一个死亡点
        /// </summary>
        public Building_Pixel(PixelTypeInfo typeInfo, PixelColorInfo colorInfo, Vector2Int offset)
        {
            this.typeInfo = typeInfo;
            this.colorInfo = colorInfo;
            this.offset = offset;
        }
        /// <summary>
        /// 为当前点创建一个存活点
        /// </summary>
        public Building_Pixel(Pixel pixel, Vector2Int offset)
        {
            this.typeInfo = pixel.typeInfo;
            this.colorInfo = pixel.colorInfo;
            this.Pixel = pixel;
            this.offset = offset;
        }

        public enum State
        {
            Survival,
            Death
        }
        public State GetState()
        {
            if (Pixel == null) return State.Death;
            if (Pixel.typeInfo == typeInfo && Pixel.colorInfo == colorInfo) return State.Survival;
            return State.Death;
        }
    }
}

using PRO.Disk;
using PRO.Tool;
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
        /// 创建一个未加载的蓝图点
        /// </summary>
        public Building_Pixel Init(PixelTypeInfo typeInfo, PixelColorInfo colorInfo, Vector2Int offset)
        {
            this.typeInfo = typeInfo;
            this.colorInfo = colorInfo;
            this.offset = offset;
            return this;
        }
        /// <summary>
        /// 为当前点创建一个已加载的存活点
        /// </summary>
        public Building_Pixel Init(Pixel pixel, Vector2Int offset)
        {
            this.typeInfo = pixel.typeInfo;
            this.colorInfo = pixel.colorInfo;
            this.Pixel = pixel;
            this.offset = offset;
            return this;
        }

        public enum State
        {
            Unload,
            Survival,
            Death
        }
        public State GetState()
        {
            if (Pixel == null) return State.Unload;
            if (Pixel.typeInfo == typeInfo && Pixel.colorInfo == colorInfo) return State.Survival;
            return State.Death;
        }

        private static ObjectPool<Building_Pixel> pool = new ObjectPool<Building_Pixel>();
        public static void PutIn(Building_Pixel building_Pixel)
        {
            building_Pixel.typeInfo = null;
            building_Pixel.colorInfo = null;
            building_Pixel.Pixel = null;
        }
        public static Building_Pixel TakeOut()
        {
            return pool.TakeOut();
        }
    }
}

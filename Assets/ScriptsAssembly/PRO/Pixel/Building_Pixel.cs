using PRO.Disk;
using PRO.Tool;
using UnityEngine;

namespace PRO
{
    public class Building_Pixel
    {
        private PixelTypeInfo _typeInfo;
        private PixelColorInfo _colorInfo;
        /// <summary>
        /// 偏移
        /// </summary>
        private Vector2Int _offset;
        private BlockBase.BlockType _blockType;

        public Pixel pixel;
        public PixelTypeInfo typeInfo => _typeInfo;
        public PixelColorInfo colorInfo => _colorInfo;
        /// <summary>
        /// 偏移
        /// </summary>
        public Vector2Int offset => _offset;

        public BlockBase.BlockType blockType => _blockType;


        /// <summary>
        /// 创建一个未加载的蓝图点
        /// </summary>
        public Building_Pixel Init(PixelTypeInfo typeInfo, PixelColorInfo colorInfo, Vector2Int offset, BlockBase.BlockType blockType)
        {
            this._typeInfo = typeInfo;
            this._colorInfo = colorInfo;
            this._offset = offset;
            this._blockType = blockType;
            return this;
        }
        /// <summary>
        /// 为当前点创建一个已加载的存活点
        /// </summary>
        public Building_Pixel Init(Pixel pixel, Vector2Int offset)
        {
            this._typeInfo = pixel.typeInfo;
            this._colorInfo = pixel.colorInfo;
            this.pixel = pixel;
            this._offset = offset;
            this._blockType = pixel.blockBase.blockType;
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
            if (pixel == null) return State.Unload;
            if (pixel.typeInfo == _typeInfo && pixel.colorInfo == _colorInfo) return State.Survival;
            return State.Death;
        }

        private static ObjectPool<Building_Pixel> pool = new ObjectPool<Building_Pixel>();
        public static void PutIn(Building_Pixel building_Pixel)
        {
            building_Pixel._typeInfo = null;
            building_Pixel._colorInfo = null;
            building_Pixel.pixel = null;
        }
        public static Building_Pixel TakeOut()
        {
            return pool.TakeOut();
        }
    }
}

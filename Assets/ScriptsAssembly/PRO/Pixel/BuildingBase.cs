using System.Collections.Generic;
using UnityEngine;

namespace PRO
{
    /// <summary>
    /// 建筑的基类，一个建筑是由一堆点组合的合集
    /// </summary>
    public class BuildingBase
    {
        public string GUID;
        public string Name;
        public Dictionary<Vector2Int, Building_Pixel> PixelDic = new Dictionary<Vector2Int, Building_Pixel>();
        public Vector2Int global;
        public BoxCollider2D collider;
        /// <summary>
        /// 这个建筑是否可以被破坏
        /// </summary>
        public bool CanByBroken = true;
    }
    public class Building_Pixel
    {
        private string typeName;
        private string colorName;


        public Pixel Pixel;
        public string TypeName => typeName;
        public string ColorName => colorName;

        public Building_Pixel(string typeName, string colorName)
        {
            this.typeName = typeName;
            this.colorName = colorName;
            this.Pixel = null;
        }
        public Building_Pixel(Pixel pixel)
        {
            this.typeName = pixel.typeInfo.typeName;
            this.colorName = pixel.colorInfo.colorName;
            this.Pixel = pixel;
        }
    }
}

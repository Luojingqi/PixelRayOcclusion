//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PRO.Disk
{
    
    
    public class PixelColorInfo
    {
        
        private string colorname;
        
        private UnityEngine.Color32 _color;
        
        private float lightpathcolormixing;
        
        private float affectslightintensity;
        
        private int luminousradius;
        
        private UnityEngine.Color32 luminouscolor;
        
        private float selfluminous;
        
        private int _index;
        
        /// <summary>
        ///颜色的名称
        ///</summary>
        public string colorName
        {
            get
            {
                return this.colorname;
            }
            set
            {
                this.colorname = value;
            }
        }
        
        /// <summary>
        ///颜色
        ///</summary>
        public UnityEngine.Color32 color
        {
            get
            {
                return this._color;
            }
            set
            {
                this._color = value;
            }
        }
        
        /// <summary>
        ///光路混色
        ///</summary>
        public float lightPathColorMixing
        {
            get
            {
                return this.lightpathcolormixing;
            }
            set
            {
                this.lightpathcolormixing = value;
            }
        }
        
        /// <summary>
        ///光强影响的系数
        ///</summary>
        public float affectsLightIntensity
        {
            get
            {
                return this.affectslightintensity;
            }
            set
            {
                this.affectslightintensity = value;
            }
        }
        
        /// <summary>
        ///发光半径
        ///</summary>
        public int luminousRadius
        {
            get
            {
                return this.luminousradius;
            }
            set
            {
                this.luminousradius = value;
            }
        }
        
        /// <summary>
        ///发光颜色
        ///</summary>
        public UnityEngine.Color32 luminousColor
        {
            get
            {
                return this.luminouscolor;
            }
            set
            {
                this.luminouscolor = value;
            }
        }
        
        /// <summary>
        ///自发光强度
        ///</summary>
        public float selfLuminous
        {
            get
            {
                return this.selfluminous;
            }
            set
            {
                this.selfluminous = value;
            }
        }
        
        /// <summary>
        ///不填
        ///</summary>
        public int index
        {
            get
            {
                return this._index;
            }
            set
            {
                this._index = value;
            }
        }
    }
}

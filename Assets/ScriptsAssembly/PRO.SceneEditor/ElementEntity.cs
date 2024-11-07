using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRO.SceneEditor
{
    internal class ElementEntity
    {
        public int height;
        public int width;
        public string name;
        public Pixel[] pixels;
        internal class Pixel
        {
            public string typeName;
            public string colorName;
        }
    }
}

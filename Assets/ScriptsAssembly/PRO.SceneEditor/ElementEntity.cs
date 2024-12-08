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

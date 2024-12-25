namespace PRO.SceneEditor
{
    internal class Element_Disk
    {
        public int height;
        public int width;
        public string name;
        public Pixel[] pixels;
        public byte[] pngBytes;
        internal class Pixel
        {
            public string typeName;
            public string colorName;
        }
    }
}

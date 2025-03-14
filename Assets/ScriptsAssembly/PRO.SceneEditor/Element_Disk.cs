namespace PRO.SceneEditor
{
    public class Element_Disk
    {
        public int height;
        public int width;
        public string name;
        public Pixel[] pixels;
        public byte[] pngBytes;
        public class Pixel
        {
            public string typeName;
            public string colorName;
        }
    }
}

using PRO.Tool;

namespace PRO.Disk.Scene
{
    public class PixelToDisk
    {
        public string typeName;
        public string colorName;
        public PixelToDisk()
        {
        }

        public PixelToDisk(Pixel pixel)
        {
            typeName = pixel.info.typeName;
            colorName = pixel.colorName;
        }
        
    }
}
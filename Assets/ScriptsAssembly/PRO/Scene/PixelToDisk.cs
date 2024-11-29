using PRO.Tool;

namespace PRO.Disk.Scene
{
    /// <summary>
    /// 像素数据存储到磁盘与从磁盘中加载的类
    /// </summary>
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
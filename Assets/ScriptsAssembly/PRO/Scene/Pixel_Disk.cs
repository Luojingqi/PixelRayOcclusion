using PRO.Tool;

namespace PRO.Disk.Scene
{
    /// <summary>
    /// 像素数据存储到磁盘与从磁盘中加载的类
    /// </summary>
    public class Pixel_Disk
    {
        public string typeName;
        public string colorName;
        public Pixel_Disk()
        {
        }

        public Pixel_Disk(Pixel pixel)
        {
            typeName = pixel.info.typeName;
            colorName = pixel.colorName;
        }
        
    }
}
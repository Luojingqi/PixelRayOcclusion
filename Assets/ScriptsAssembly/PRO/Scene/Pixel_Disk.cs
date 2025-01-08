using PRO.Tool;

namespace PRO.Disk.Scene
{
    /// <summary>
    /// 像素数据存储到磁盘与从磁盘中加载的类，通过名称存储于加载
    /// </summary>
    public class Pixel_Disk_Name
    {
        public string typeName;
        public string colorName;
        public Pixel_Disk_Name()
        {
        }

        public Pixel_Disk_Name(Pixel pixel)
        {
            typeName = pixel.typeInfo.typeName;
            colorName = pixel.colorInfo.colorName;
        }
    }
}
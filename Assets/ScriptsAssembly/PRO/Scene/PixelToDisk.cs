using PRO.Tool;

namespace PRO.Disk.Scene
{
    /// <summary>
    /// �������ݴ洢��������Ӵ����м��ص���
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
using PRO.Tool;

namespace PRO.Disk.Scene
{
    /// <summary>
    /// �������ݴ洢��������Ӵ����м��ص���
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
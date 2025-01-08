using PRO.Tool;

namespace PRO.Disk.Scene
{
    /// <summary>
    /// �������ݴ洢��������Ӵ����м��ص��࣬ͨ�����ƴ洢�ڼ���
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
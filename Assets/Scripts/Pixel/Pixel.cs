using PRO.DataStructure;
using PRO.Tool;
namespace PRO
{
    public class Pixel
    {
        /// <summary>
        /// 每个像素点占世界空间的大小
        /// </summary>
        public static float Size = 0.05f;
        /// <summary>
        /// 在块内坐标
        /// </summary>
        public Vector2Byte pos;
        /// <summary>
        /// 名称
        /// </summary>
        public string name;

        private static ObjectPool<Pixel> pixelPool = new ObjectPool<Pixel>(Block.Size.x * Block.Size.y * 50, true);
        public static Pixel TakeOut(string name, Vector2Byte pixelPos)
        {
            Pixel pixel = pixelPool.TakeOut();
            pixel.name = name;
            pixel.pos = pixelPos;
            return pixel;
        }
        public static void PutIn(Pixel pixel)
        {
            pixel.name = null;
            pixelPool.PutIn(pixel);
        }
    }
}
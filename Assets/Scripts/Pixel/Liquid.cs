using PRO.DataStructure;
using PRO.Tool;
namespace PRO
{
    public class Liquid : Pixel
    {
        /// <summary>
        /// 液体压强
        /// </summary>
        public int pressure;

        private static ObjectPool<Liquid> liquidPool = new ObjectPool<Liquid>(Block.Size.x * Block.Size.y * 10, true);
        public new static Liquid TakeOut(string name, Vector2Byte pixelPos)
        {
            Liquid liquid = null;
            lock (liquidPool) { liquid = liquidPool.TakeOut(); }
            liquid.name = name;
            liquid.pos = pixelPos;
            return liquid;
        }
        public static void PutIn(Liquid liquid)
        {
            liquid.name = null;
            liquidPool.PutIn(liquid);
        }
    }
}
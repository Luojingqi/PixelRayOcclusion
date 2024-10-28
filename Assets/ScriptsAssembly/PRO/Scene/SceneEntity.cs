using PRO.DataStructure;

namespace PRO
{
    public class SceneEntity
    {
        public CrossList<Block> BlockCrossList = new CrossList<Block>();
        public CrossList<BackgroundBlock> BackgroundCrossList = new CrossList<BackgroundBlock>();
    }
}
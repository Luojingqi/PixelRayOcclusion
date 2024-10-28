using UnityEngine;
namespace PRO.Scene
{
    public class BlockToDisk
    {

        public Vector2Int blockPos;
        public Pixel[,] allPixel;

        public BlockToDisk(Block block)
        {
            this.blockPos = block.BlockPos;
            
        }
    }
}
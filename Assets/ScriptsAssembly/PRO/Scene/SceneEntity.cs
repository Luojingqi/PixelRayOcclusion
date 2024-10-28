using PRO.DataStructure;
using PRO.Tool;
using UnityEngine;

namespace PRO
{
    public class SceneEntity
    {
        private CrossList<Block> BlockInRAM = new CrossList<Block>();
        private CrossList<BackgroundBlock> BackgroundInRAM = new CrossList<BackgroundBlock>();

        private string rootPath;
        public SceneEntity(string rootPath)
        {
            this.rootPath = rootPath;
        }

        public Block GetBlock(Vector2Int blockPos)
        {
            return BlockInRAM[blockPos];
        }
        public BackgroundBlock GetBackground(Vector2Int blockPos)
        {
            return BackgroundInRAM[blockPos];
        }

        public void LoadBlockData(Vector2Int blockPos)
        {
            
        }

        public void UnloadBlockData(Vector2Int blockPos) 
        {

        }

    }
}
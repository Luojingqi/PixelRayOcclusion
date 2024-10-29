using PRO.DataStructure;
using PRO.Renderer;
using PRO.SceneToDisk;
using PRO.Tool;
using UnityEngine;

namespace PRO
{
    public class SceneEntity
    {
        private CrossList<Block> BlockInRAM = new CrossList<Block>();
        private CrossList<BackgroundBlock> BackgroundInRAM = new CrossList<BackgroundBlock>();

        private string rootPath;
        public string name;
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
            if (JsonTool.LoadText($@"{rootPath}\{blockPos}\block.json", out string blockText)
                && JsonTool.LoadText($@"{rootPath}\{blockPos}\background.json", out string backgroundText))
            {
                BlockToDisk blockToDisk = JsonTool.ToObject<BlockToDisk>(blockText);
                BackgroundToDisk backgroundToDisk = JsonTool.ToObject<BackgroundToDisk>(backgroundText);

            }
            else
            {
                Log.Print($"无法加载区块{blockPos}");
            }
        }

        public void UnloadBlockData(Vector2Int blockPos)
        {
            BlockToDisk blockToDisk = new BlockToDisk(GetBlock(blockPos));
            BackgroundToDisk backgroundToDisk = new BackgroundToDisk(GetBackground(blockPos));
        }

        public void Unload()
        {
            
        }

        public void CreateBlock(Vector2Int blockPos)
        {
            var block = Block.TakeOut();
            BlockInRAM[blockPos] = block;
            block.transform.position = Block.BlockToWorld(blockPos);
           // block.transform.parent = BlockNode;
            block.BlockPos = blockPos;
        }
        public void CreateBackground(Vector2Int blockPos)
        {
            var back = BackgroundBlock.TakeOut();
            BackgroundInRAM[blockPos] = back;
            back.transform.position = Block.BlockToWorld(blockPos);
            back.transform.parent = GetBlock(blockPos).transform;
            back.BlockPos = blockPos;
        }
    }
}
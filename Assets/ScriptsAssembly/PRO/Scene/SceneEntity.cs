using PRO.DataStructure;
using PRO.Disk.Scene;
using PRO.Tool;
using System.IO;
using UnityEngine;

namespace PRO
{
    /// <summary>
    /// 场景实体类，在游戏运行时存储场景运行数据
    /// </summary>
    public class SceneEntity
    {
        private CrossList<Block> BlockInRAM = new CrossList<Block>();
        private CrossList<BackgroundBlock> BackgroundInRAM = new CrossList<BackgroundBlock>();

        public SceneCatalog sceneCatalog { get; private set; }
        public SceneEntity(SceneCatalog sceneCatalog)
        {
            this.sceneCatalog = sceneCatalog;
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
            if (JsonTool.LoadText($@"{sceneCatalog.directoryInfo}\Block\{blockPos}\block.json", out string blockText)
                && JsonTool.LoadText($@"{sceneCatalog.directoryInfo}\Block\{blockPos}\background.json", out string backgroundText))
            {
                BlockToDisk blockToDisk = JsonTool.ToObject<BlockToDisk>(blockText);
                BackgroundToDisk backgroundToDisk = JsonTool.ToObject<BackgroundToDisk>(backgroundText);
                Block block = CreateBlock(blockPos);
                BackgroundBlock background = CreateBackground(blockPos);
                for (int x = 0; x < Block.Size.x; x++)
                    for (int y = 0; y < Block.Size.y; y++)
                    {
                        block.SetPixel(Pixel.TakeOut(blockToDisk.allPixel[x, y], new(x, y)));
                        background.SetPixel(Pixel.TakeOut(backgroundToDisk.allPixel[x, y], new(x, y)));
                    }
                block.DrawPixelAsync();
                background.DrawPixelAsync();
            }
            else
            {
                Log.Print($"无法加载区块{blockPos}");
            }
        }

        public void SaveBlockData(Vector2Int blockPos)
        {
            string path = $@"{sceneCatalog.directoryInfo}\Block\{blockPos}";
            if (Directory.Exists(path) == false) Directory.CreateDirectory(path);
            BlockToDisk blockToDisk = new BlockToDisk(GetBlock(blockPos));
            BackgroundToDisk backgroundToDisk = new BackgroundToDisk(GetBackground(blockPos));
            JsonTool.StoreObject(@$"{path}\block.json", blockToDisk);
            JsonTool.StoreObject($@"{path}\background.json", backgroundToDisk);
        }

        public void Unload()
        {

        }
        /// <summary>
        /// 创建一个块的游戏物体，内部像素点数据为空
        /// </summary>
        /// <param name="blockPos"></param>
        /// <returns></returns>
        public Block CreateBlock(Vector2Int blockPos)
        {
            var block = Block.TakeOut();
            block.name = $"Block{blockPos}";
            BlockInRAM[blockPos] = block;
            block.transform.position = Block.BlockToWorld(blockPos);
            block.BlockPos = blockPos;
            return block;
        }
        /// <summary>
        /// 创建一个背景的游戏物体，内部像素点数据为空
        /// </summary>
        /// <param name="blockPos"></param>
        /// <returns></returns>
        public BackgroundBlock CreateBackground(Vector2Int blockPos)
        {
            var back = BackgroundBlock.TakeOut();
            BackgroundInRAM[blockPos] = back;
            back.transform.position = Block.BlockToWorld(blockPos);
            back.transform.parent = GetBlock(blockPos).transform;
            back.BlockPos = blockPos;
            return back;
        }
    }
}
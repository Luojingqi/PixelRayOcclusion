using PRO.DataStructure;
using PRO.Disk.Scene;
using PRO.Tool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace PRO
{
    /// <summary>
    /// 场景实体类，在游戏运行时存储场景运行数据
    /// </summary>
    public class SceneEntity
    {
        public SceneCatalog sceneCatalog { get; private set; }
        public SceneEntity(SceneCatalog sceneCatalog)
        {
            this.sceneCatalog = sceneCatalog;
        }
        #region 区块存储于获取
        private CrossList<Block> BlockInRAM = new CrossList<Block>();
        private CrossList<BackgroundBlock> BackgroundInRAM = new CrossList<BackgroundBlock>();
        /// <summary>
        /// key：guid  value：building
        /// </summary>
        private Dictionary<string, Building> BuildingInRAM = new Dictionary<string, Building>();

        public Block GetBlock(Vector2Int blockPos)
        {
            return BlockInRAM[blockPos];
        }
        public BackgroundBlock GetBackground(Vector2Int blockPos)
        {
            return BackgroundInRAM[blockPos];
        }
        public Building GetBuilding(string guid)
        {
            return BuildingInRAM[guid];
        }
        #endregion

        #region 从磁盘中加载与保存区块
        public void LoadBlockData(Vector2Int blockPos)
        {
            if (JsonTool.LoadText($@"{sceneCatalog.directoryInfo}\Block\{blockPos}\block.txt", out string blockText)
                && JsonTool.LoadText($@"{sceneCatalog.directoryInfo}\Block\{blockPos}\background.txt", out string backgroundText))
            {
                Block block = CreateBlock(blockPos);
                BackgroundBlock background = CreateBackground(blockPos);
                BlockToDiskEx.ToRAM(blockText, block);
                BlockToDiskEx.ToRAM(backgroundText, background);
                block.DrawPixelAsync();
                background.DrawPixelAsync();
                var colliderDataList = GreedyCollider.CreateColliderDataList(block, new(0, 0), new(Block.Size.x - 1, Block.Size.y - 1)); //此行其实可以交由多线程处理
                GreedyCollider.CreateColliderAction(block, colliderDataList);
            }
            else
            {
                Log.Print($"无法加载区块{blockPos}，可能区块文件不存在", Color.red);
            }
        }

        public void SaveBlockData(Vector2Int blockPos)
        {
            string path = $@"{sceneCatalog.directoryInfo}\Block\{blockPos}";
            if (Directory.Exists(path) == false) Directory.CreateDirectory(path);
            JsonTool.StoreText($@"{path}\block.txt", BlockToDiskEx.ToDisk(GetBlock(blockPos)));
            JsonTool.StoreText($@"{path}\background.txt", BlockToDiskEx.ToDisk(GetBackground(blockPos)));
        }

        public void LoadBuilding(string guid)
        {
            if (JsonTool.LoadText($@"{sceneCatalog.directoryInfo}\Building\{guid}.json", out string buildingText))
            {
                Building building = JsonTool.ToObject<Building>(buildingText);
                BuildingInRAM.Add(guid, building);
            }
            else
            {
                Log.Print($"无法加载建筑{guid}，可能建筑文件不存在", Color.red);
            }
        }
        #endregion


        public void Unload()
        {

        }
        #region 创建区块的空游戏物体
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
        #endregion
    }
}
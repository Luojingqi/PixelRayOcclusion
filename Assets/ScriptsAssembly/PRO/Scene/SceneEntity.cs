using PRO.DataStructure;
using PRO.Disk.Scene;
using PRO.Tool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        #region 获取已经实例化的对象
        public HashSet<Vector2Int> BlockBaseInRAM = new HashSet<Vector2Int>();
        private CrossList<Block> BlockInRAM = new CrossList<Block>();
        private CrossList<BackgroundBlock> BackgroundInRAM = new CrossList<BackgroundBlock>();
        /// <summary>
        /// key：guid  value：building
        /// </summary>
        public Dictionary<string, BuildingBase> BuildingInRAM = new Dictionary<string, BuildingBase>();

        public Block GetBlock(Vector2Int blockPos)
        {
            return BlockInRAM[blockPos];
        }
        public BackgroundBlock GetBackground(Vector2Int blockPos)
        {
            return BackgroundInRAM[blockPos];
        }
        public BuildingBase GetBuilding(string guid)
        {
            BuildingInRAM.TryGetValue(guid, out var building);
            return building;
        }
        #endregion

        #region 从磁盘中加载与保存
        public void LoadBlockData(Vector2Int blockPos)
        {
            if (JsonTool.LoadText($@"{sceneCatalog.directoryInfo}\Block\{blockPos}\block.txt", out string blockText)
                && JsonTool.LoadText($@"{sceneCatalog.directoryInfo}\Block\{blockPos}\background.txt", out string backgroundText))
            {
                Block block = CreateBlock(blockPos);
                BackgroundBlock background = CreateBackground(blockPos);
                BlockToDiskEx.ToRAM(blockText, block, this);
                BlockToDiskEx.ToRAM(backgroundText, background, this);
                block.DrawPixelAsync();
                background.DrawPixelAsync();
                BlockBaseInRAM.Add(blockPos);
                var colliderDataList = GreedyCollider.CreateColliderDataList(block, new(0, 0), new(Block.Size.x - 1, Block.Size.y - 1)); //此行其实可以交由多线程处理
                GreedyCollider.CreateColliderAction(block, colliderDataList);
                //for (int y = -1; y <= 1; y++)
                //    for (int x = -1; x <= 1; x++)
                //    {
                //        GetBlock(blockPos + new Vector2Int(x, y))?.NavBuildAll();
                //    }


            }
            else
            {
                Log.Print($"无法加载区块{blockPos}，可能区块文件不存在", Color.red);
            }
        }
        /// <summary>
        /// 卸载一个区块，上方的建筑也会被一并卸载
        /// </summary>
        /// <param name="blockPos"></param>
        public void UnloadBlockData(Vector2Int blockPos)
        {
            Action<BuildingBase> action = (building) =>
            {
                BuildingInRAM.Remove(building.GUID);
                GameObject.Destroy(building.gameObject);
            };
            Block.PutIn(GetBlock(blockPos), action);
            BackgroundBlock.PutIn(GetBackground(blockPos), action);
            BlockInRAM[blockPos] = null;
            BackgroundInRAM[blockPos] = null;
            BlockBaseInRAM.Remove(blockPos);
        }
        /// <summary>
        /// 卸载并且保存一个区块，上方的建筑也会被一并保存
        /// </summary>
        /// <param name="blockPos"></param>
        public void UnloadAndSaveBlockData(Vector2Int blockPos)
        {
            Action<BuildingBase> action = (building) =>
            {
                SaveBuilding(building.GUID);
                BuildingInRAM.Remove(building.GUID);
                GameObject.Destroy(building.gameObject);
            };
            SaveBlockData(blockPos);
            Block.PutIn(GetBlock(blockPos), action);
            BackgroundBlock.PutIn(GetBackground(blockPos), action);
            BlockInRAM[blockPos] = null;
            BackgroundInRAM[blockPos] = null;
            BlockBaseInRAM.Remove(blockPos);
        }
        /// <summary>
        /// 只保存一个区块，上方的建筑不会被保存
        /// </summary>
        /// <param name="blockPos"></param>
        public void SaveBlockData(Vector2Int blockPos)
        {
            string path = $@"{sceneCatalog.directoryInfo}\Block\{blockPos}";
            if (Directory.Exists(path) == false) Directory.CreateDirectory(path);
            JsonTool.StoreText($@"{path}\block.txt", BlockToDiskEx.ToDisk(GetBlock(blockPos), this));
            JsonTool.StoreText($@"{path}\background.txt", BlockToDiskEx.ToDisk(GetBackground(blockPos), this));
        }

        public void LoadBuilding(string guid)
        {
            if (JsonTool.LoadText($@"{sceneCatalog.directoryInfo}\Building\{guid}.txt", out string buildingText) &&
                sceneCatalog.buildingTypeDic.TryGetValue(guid, out Type type))
            {

                BuildingBase building = BuildingBase.New(type, guid);
                building.Deserialize(buildingText, buildingText.Length);
                BuildingInRAM.Add(guid, building);
            }
            else
            {
                Log.Print($"无法加载建筑{guid}，可能建筑文件不存在", Color.red);
            }
        }
        public void SaveBuilding(string guid)
        {
            JsonTool.StoreText($@"{sceneCatalog.directoryInfo}\Building\{guid}.txt", GetBuilding(guid).Serialize());
        }
        #endregion


        public void Unload()
        {
            foreach (var blockPos in BlockBaseInRAM.ToList())
                UnloadBlockData(blockPos);
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
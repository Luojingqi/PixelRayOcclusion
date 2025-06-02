using PRO.DataStructure;
using PRO.Disk.Scene;
using PRO.Tool;
using PRO.Tool.Serialize.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace PRO
{
    /// <summary>
    /// 场景实体类，在游戏运行时存储场景运行数据
    /// </summary>
    public class SceneEntity : ITime_Update
    {
        private struct OneBlock
        {
            public Block Block;
            public BackgroundBlock BackgroundBlock;
            public BlockBase this[int index]
            {
                get
                {
                    switch (index)
                    {
                        default:
                        case 0: return Block;
                        case 1: return BackgroundBlock;
                    }
                }
            }
        }

        public SceneCatalog sceneCatalog { get; private set; }
        public SceneEntity(SceneCatalog sceneCatalog)
        {
            this.sceneCatalog = sceneCatalog;
        }
        #region 获取已经实例化的对象
        public HashSet<Vector2Int> BlockBaseInRAM = new HashSet<Vector2Int>();
        private CrossList<OneBlock> BlockBaseCrossList = new CrossList<OneBlock>();
        public List<Particle> ActiveParticle = new List<Particle>();
        /// <summary>
        /// key：guid  value：building
        /// </summary>
        public Dictionary<string, BuildingBase> BuildingInRAM = new Dictionary<string, BuildingBase>();

        public Block GetBlock(Vector2Int blockPos) => BlockBaseCrossList[blockPos].Block;
        public BackgroundBlock GetBackground(Vector2Int blockPos) => BlockBaseCrossList[blockPos].BackgroundBlock;
        public BlockBase GetBlockBase(BlockBase.BlockType blockType, Vector2Int blockPos) => BlockBaseCrossList[blockPos][(int)blockType];

        public BuildingBase GetBuilding(string guid)
        {
            BuildingInRAM.TryGetValue(guid, out var building);
            return building;
        }

        public Pixel GetPixel(BlockBase.BlockType blockType, Vector2Int globalPos)
        {
            var blockBase = GetBlockBase(blockType, Block.GlobalToBlock(globalPos));
            if (blockBase != null) return blockBase.GetPixel(Block.GlobalToPixel(globalPos));
            return null;
        }
        #endregion

        #region 从磁盘中加载与保存

        /// <summary>
        /// 使用多线程加载一个区块，区块文件不存在时会创建一个空区块
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="blockPos"></param>
        /// <param name="endActionUnity">每次加载完成一个区块都会传递方法让主线程执行</param>
        /// <param name="endAction">每次加载完成一个区块都会执行</param>
        public void ThreadLoadOrCreateBlock(Vector2Int blockPos)
        {
            BlockBaseInRAM.Add(blockPos);
            var block = CreateBlock(blockPos);
            var background = CreateBackground(blockPos);
            block.UnLoadCountdown = BlockMaterial.proConfig.AutoUnLoadBlockCountdownTime * 2;
            background.UnLoadCountdown = BlockMaterial.proConfig.AutoUnLoadBlockCountdownTime * 2;
            ThreadPool.QueueUserWorkItem((obj) =>
            {
                if (IOTool.LoadProto($@"{sceneCatalog.directoryInfo}\Block\{blockPos}\block", Proto.BlockBaseData.Parser, out var blockData)
                && IOTool.LoadProto($@"{sceneCatalog.directoryInfo}\Block\{blockPos}\background", Proto.BlockBaseData.Parser, out var backgroundData))
                {

                    ThreadPool.QueueUserWorkItem((obj) =>
                    {
                        try
                        {
                            block.ToRAM(blockData, this);
                        }
                        catch (Exception e) { TimeManager.Inst.AddToQueue_MainThreadUpdate_Clear(() => Log.Print($"线程报错：{e}", Color.red)); }
                    });
                    ThreadPool.QueueUserWorkItem((obj) =>
                    {
                        try
                        {
                            background.ToRAM(backgroundData, this);
                        }
                        catch (Exception e) { TimeManager.Inst.AddToQueue_MainThreadUpdate_Clear(() => Log.Print($"线程报错：{e}", Color.red)); }
                    });
                }
                else
                {
                    ThreadPool.QueueUserWorkItem((obj) =>
                    {
                        var block_ = obj as Block;
                        try
                        {
                            for (int x = 0; x < Block.Size.x; x++)
                                for (int y = 0; y < Block.Size.y; y++)
                                {

                                    Pixel pixel;
                                    lock (Pixel.pixelPool)
                                        pixel = Pixel.pixelPool.TakeOut();
                                    Pixel.空气.CloneTo(pixel, new Vector2Byte(x, y));
                                    block_.SetPixel(pixel, false, false, false);
                                    block_.DrawPixelSync(new Vector2Byte(x, y), pixel.colorInfo.color);
                                }
                            TimeManager.Inst.AddToQueue_MainThreadUpdate_Clear(() => BlockMaterial.SetBlock(block_));
                        }
                        catch (Exception e) { TimeManager.Inst.AddToQueue_MainThreadUpdate_Clear(() => Log.Print($"线程报错：{e}", Color.red)); }
                    }, block);
                    ThreadPool.QueueUserWorkItem((obj) =>
                    {
                        var background_ = obj as BackgroundBlock;
                        try
                        {
                            for (int x = 0; x < Block.Size.x; x++)
                                for (int y = 0; y < Block.Size.y; y++)
                                {
                                    Pixel pixel;
                                    lock (Pixel.pixelPool)
                                        pixel = Pixel.TakeOut("背景", "背景色2", new(x, y));
                                    background_.SetPixel(pixel, false, false, false);
                                    background_.DrawPixelSync(new Vector2Byte(x, y), pixel.colorInfo.color);
                                }
                            TimeManager.Inst.AddToQueue_MainThreadUpdate_Clear(() => BlockMaterial.SetBackgroundBlock(background_));
                        }
                        catch (Exception e) { TimeManager.Inst.AddToQueue_MainThreadUpdate_Clear(() => Log.Print($"线程报错：{e}", Color.red)); }
                    }, background);
                }
            });
        }
        /// <summary>
        /// 卸载一个区块，上方的建筑也会被一并卸载
        /// </summary>
        /// <param name="blockPos"></param>
        public void UnloadBlockData(Vector2Int blockPos)
        {
            try
            {
                BlockBaseInRAM.Remove(blockPos);
                var block = GetBlock(blockPos);
                var background = GetBackground(blockPos);
                BlockBaseCrossList[blockPos] = new OneBlock();
                Block.PutIn(block);
                BackgroundBlock.PutIn(background);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        /// <summary>
        /// 保存一个区块
        /// </summary>
        /// <param name="blockPos">区块坐标</param>
        /// <param name="SaveBuilding">是否保存上方的建筑</param>
        public void SaveBlockData(Vector2Int blockPos, bool isSaveBuilding)
        {
            string path = $@"{sceneCatalog.directoryInfo}\Block\{blockPos}";
            if (Directory.Exists(path) == false) Directory.CreateDirectory(path);
            var blockData = GetBlock(blockPos).ToDisk();
            var backgroundData = GetBackground(blockPos).ToDisk();

            if (isSaveBuilding)
            {
                foreach (string guid in blockData.BuildingGuidIndexDic.Keys)
                    SaveBuilding(guid);
                foreach (string guid in backgroundData.BuildingGuidIndexDic.Keys)
                    SaveBuilding(guid);
            }
            IOTool.SaveProto($@"{path}\block", blockData);
            IOTool.SaveProto($@"{path}\background", backgroundData);
            blockData.ClearPutIn();
            backgroundData.ClearPutIn();
        }
        public static Dictionary<string, Type> BuildingTypeDic = new Dictionary<string, Type>();
        public void LoadBuilding(string guid)
        {
            if (IOTool.LoadProto($@"{sceneCatalog.directoryInfo}\Building\{guid}", Proto.BuildingBaseData.Parser, out var diskData))
            {
                BuildingBase building = BuildingBase.New(BuildingTypeDic[diskData.Name], guid, this);
                building.ToRAM(diskData);
                BuildingInRAM.Add(guid, building);
            }
            else
            {
                Log.Print($"无法加载建筑{guid}，可能建筑文件不存在", Color.red);
            }
        }
        public void SaveBuilding(string guid)
        {
            SaveBuilding(BuildingInRAM[guid]);
        }
        public void SaveBuilding(BuildingBase buiding)
        {
            var diskData = buiding.ToDisk();
            IOTool.SaveProto($@"{sceneCatalog.directoryInfo}\Building\{buiding.GUID}", diskData);
            diskData.ClearPutIn();
        }
        #endregion

        /// <summary>
        /// 卸载所有已加载区块，上方的建筑也会被一并卸载
        /// </summary>
        public void Unload()
        {
            foreach (var blockPos in BlockBaseInRAM.ToArray())
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
            var block = Block.TakeOut(this);
            block.name = $"Block{blockPos}";
            var oneBlock = BlockBaseCrossList[blockPos];
            BlockBaseCrossList[blockPos] = new OneBlock() { Block = block, BackgroundBlock = oneBlock.BackgroundBlock };
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
            var back = BackgroundBlock.TakeOut(this);
            var oneBlock = BlockBaseCrossList[blockPos];
            BlockBaseCrossList[blockPos] = new OneBlock() { Block = oneBlock.Block, BackgroundBlock = back };
            back.transform.position = Block.BlockToWorld(blockPos);
            back.transform.parent = GetBlock(blockPos).transform;
            back.BlockPos = blockPos;
            return back;
        }
        #endregion


        //public event SceneEntity
        public void TimeUpdate()
        {
            throw new NotImplementedException();
        }
    }
}
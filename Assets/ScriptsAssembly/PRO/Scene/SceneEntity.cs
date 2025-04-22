using PRO.DataStructure;
using PRO.Disk.Scene;
using PRO.Tool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
        public List<Particle> ActiveParticle = new List<Particle>();
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

        public BlockBase GetBlockBase(BlockBase.BlockType blockType, Vector2Int blockPos)
        {
            switch (blockType)
            {
                default:
                case BlockBase.BlockType.Block: return GetBlock(blockPos);
                case BlockBase.BlockType.BackgroundBlock: return GetBackground(blockPos);
            }
        }

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

        #region 同步与多线程加载
        /// <summary>
        /// 使用多线程加载一个区块，区块文件不存在时会创建一个空区块
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="blockPos"></param>
        /// <param name="endActionUnity">每次加载完成一个区块都会传递方法让主线程执行</param>
        /// <param name="endAction">每次加载完成一个区块都会执行</param>
        public void ThreadLoadOrCreateBlock(Vector2Int blockPos, Action<BlockBase> endActionUnity = null, Action<BlockBase> endAction = null)
        {
            BlockBaseInRAM.Add(blockPos);

            var block = CreateBlock(blockPos);
            var background = CreateBackground(blockPos);

            ThreadPool.QueueUserWorkItem((obj) =>
            {
                if (JsonTool.LoadText($@"{sceneCatalog.directoryInfo}\Block\{blockPos}\block.txt", out string blockText)
                    && JsonTool.LoadText($@"{sceneCatalog.directoryInfo}\Block\{blockPos}\background.txt", out string backgroundText))
                {
                    ThreadPool.QueueUserWorkItem((obj) =>
                    {
                        try
                        {
                            BlockToDiskEx.ToRAM(blockText, block, this);
                            var colliderDataList = GreedyCollider.CreateColliderDataList(block, new(0, 0), new(Block.Size.x - 1, Block.Size.y - 1));

                            SceneManager.Inst.AddMainThreadEvent_Clear_Lock(() => { GreedyCollider.CreateColliderAction(block, colliderDataList); endActionUnity?.Invoke(block); });
                            endAction?.Invoke(block);
                        }
                        catch (Exception e) { SceneManager.Inst.AddMainThreadEvent_Clear_Lock(() => Log.Print($"线程报错：{e}", Color.red)); }
                    });
                    ThreadPool.QueueUserWorkItem((obj) =>
                    {
                        try
                        {
                            BlockToDiskEx.ToRAM(backgroundText, background, this);
                            if (endActionUnity != null)
                                SceneManager.Inst.AddMainThreadEvent_Clear_Lock(() => endActionUnity.Invoke(background));
                            endAction?.Invoke(background);
                        }
                        catch (Exception e) { SceneManager.Inst.AddMainThreadEvent_Clear_Lock(() => Log.Print($"线程报错：{e}", Color.red)); }
                    });
                }
                else
                {
                    ThreadPool.QueueUserWorkItem((obj) =>
                    {
                        BlockBase blockBase = obj as BlockBase;
                        try
                        {
                            for (int x = 0; x < Block.Size.x; x++)
                                for (int y = 0; y < Block.Size.y; y++)
                                {
                                    Pixel pixel = Pixel.空气.CloneTo(new Pixel(), new Vector2Byte(x, y));
                                    blockBase.SetPixel(pixel, false, false, false);
                                    blockBase.DrawPixelSync(new Vector2Byte(x, y), pixel.colorInfo.color);
                                }
                            if (endActionUnity != null)
                                SceneManager.Inst.AddMainThreadEvent_Clear_Lock(() => endActionUnity.Invoke(blockBase));
                            endAction?.Invoke(block);
                        }
                        catch (Exception e) { SceneManager.Inst.AddMainThreadEvent_Clear_Lock(() => Log.Print($"线程报错：{e}", Color.red)); }
                    }, block);
                    ThreadPool.QueueUserWorkItem((obj) =>
                    {
                        BlockBase blockBase = obj as BlockBase;
                        try
                        {
                            for (int x = 0; x < Block.Size.x; x++)
                                for (int y = 0; y < Block.Size.y; y++)
                                {
                                    Pixel pixel = Pixel.New("背景", "背景色2", new(x, y));
                                    blockBase.SetPixel(pixel, false, false, false);
                                    blockBase.DrawPixelSync(new Vector2Byte(x, y), pixel.colorInfo.color);
                                }
                            if (endActionUnity != null)
                                SceneManager.Inst.AddMainThreadEvent_Clear_Lock(() => endActionUnity.Invoke(blockBase));
                            endAction?.Invoke(block);
                        }
                        catch (Exception e) { SceneManager.Inst.AddMainThreadEvent_Clear_Lock(() => Log.Print($"线程报错：{e}", Color.red)); }
                    }, background);
                }
            });
        }
        #endregion
        /// <summary>
        /// 卸载一个区块，上方的建筑也会被一并卸载
        /// </summary>
        /// <param name="blockPos"></param>
        public void UnloadBlockData(Vector2Int blockPos)
        {
            Block.PutIn(GetBlock(blockPos));
            BackgroundBlock.PutIn(GetBackground(blockPos));
            BlockInRAM[blockPos] = null;
            BackgroundInRAM[blockPos] = null;
            BlockBaseInRAM.Remove(blockPos);
            var list = SetPool.TakeOut_List<BuildingBase>();
            foreach (var building in BuildingInRAM.Values)
                if (building.AllUnloadPixel.Count == building.AllPixel.Count)
                    list.Add(building);

            foreach (var building in list)
                BuildingInRAM.Remove(building.GUID);
            SetPool.PutIn(list);
        }
        /// <summary>
        /// 卸载并且保存一个区块，上方的建筑也会被一并保存
        /// </summary>
        /// <param name="blockPos"></param>
        public void UnloadAndSaveBlockData(Vector2Int blockPos)
        {
            var list = SetPool.TakeOut_List<BuildingBase>();
            foreach (var building in BuildingInRAM.Values)
                if (building.AllUnloadPixel.Count == building.AllPixel.Count)
                    list.Add(building);
            foreach (var building in list)
            {
                SaveBuilding(building.GUID);
                BuildingInRAM.Remove(building.GUID);
            }
            SetPool.PutIn(list);

            SaveBlockData(blockPos);
            Block.PutIn(GetBlock(blockPos));
            BackgroundBlock.PutIn(GetBackground(blockPos));
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
            if (sceneCatalog.buildingTypeDic.TryGetValue(guid, out Type type) &&
                JsonTool.LoadText($@"{sceneCatalog.directoryInfo}\Building\{guid}.txt", out string buildingText))
            {
                BuildingBase building = BuildingBase.New(type, guid, this);
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

        /// <summary>
        /// 卸载所有已加载区块，上方的建筑也会被一并卸载
        /// </summary>
        public void Unload()
        {
            foreach (var blockPos in BlockBaseInRAM)
                UnloadBlockData(blockPos);
        }


        public void DeleteBuilding(string guid)
        {
            if (BuildingInRAM.TryGetValue(guid, out var building))
            {
                BuildingInRAM.Remove(guid);
                sceneCatalog.buildingTypeDic.Remove(guid);
                File.Delete(@$"{sceneCatalog.directoryInfo.FullName}/Building/{guid}.txt");
                foreach (var item in building.AllPixel.Values)
                {
                    item.pixel.buildingSet.Remove(building);
                    item.pixel = null;
                }
                GameObject.Destroy(building.gameObject);
            }
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
            var back = BackgroundBlock.TakeOut(this);
            BackgroundInRAM[blockPos] = back;
            back.transform.position = Block.BlockToWorld(blockPos);
            back.transform.parent = GetBlock(blockPos).transform;
            back.BlockPos = blockPos;
            return back;
        }
        #endregion
    }
}
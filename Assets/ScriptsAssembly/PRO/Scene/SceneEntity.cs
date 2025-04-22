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
    /// ����ʵ���࣬����Ϸ����ʱ�洢������������
    /// </summary>
    public class SceneEntity
    {
        public SceneCatalog sceneCatalog { get; private set; }
        public SceneEntity(SceneCatalog sceneCatalog)
        {
            this.sceneCatalog = sceneCatalog;
        }
        #region ��ȡ�Ѿ�ʵ�����Ķ���
        public HashSet<Vector2Int> BlockBaseInRAM = new HashSet<Vector2Int>();
        private CrossList<Block> BlockInRAM = new CrossList<Block>();
        private CrossList<BackgroundBlock> BackgroundInRAM = new CrossList<BackgroundBlock>();
        public List<Particle> ActiveParticle = new List<Particle>();
        /// <summary>
        /// key��guid  value��building
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

        #region �Ӵ����м����뱣��

        #region ͬ������̼߳���
        /// <summary>
        /// ʹ�ö��̼߳���һ�����飬�����ļ�������ʱ�ᴴ��һ��������
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="blockPos"></param>
        /// <param name="endActionUnity">ÿ�μ������һ�����鶼�ᴫ�ݷ��������߳�ִ��</param>
        /// <param name="endAction">ÿ�μ������һ�����鶼��ִ��</param>
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
                        catch (Exception e) { SceneManager.Inst.AddMainThreadEvent_Clear_Lock(() => Log.Print($"�̱߳���{e}", Color.red)); }
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
                        catch (Exception e) { SceneManager.Inst.AddMainThreadEvent_Clear_Lock(() => Log.Print($"�̱߳���{e}", Color.red)); }
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
                                    Pixel pixel = Pixel.����.CloneTo(new Pixel(), new Vector2Byte(x, y));
                                    blockBase.SetPixel(pixel, false, false, false);
                                    blockBase.DrawPixelSync(new Vector2Byte(x, y), pixel.colorInfo.color);
                                }
                            if (endActionUnity != null)
                                SceneManager.Inst.AddMainThreadEvent_Clear_Lock(() => endActionUnity.Invoke(blockBase));
                            endAction?.Invoke(block);
                        }
                        catch (Exception e) { SceneManager.Inst.AddMainThreadEvent_Clear_Lock(() => Log.Print($"�̱߳���{e}", Color.red)); }
                    }, block);
                    ThreadPool.QueueUserWorkItem((obj) =>
                    {
                        BlockBase blockBase = obj as BlockBase;
                        try
                        {
                            for (int x = 0; x < Block.Size.x; x++)
                                for (int y = 0; y < Block.Size.y; y++)
                                {
                                    Pixel pixel = Pixel.New("����", "����ɫ2", new(x, y));
                                    blockBase.SetPixel(pixel, false, false, false);
                                    blockBase.DrawPixelSync(new Vector2Byte(x, y), pixel.colorInfo.color);
                                }
                            if (endActionUnity != null)
                                SceneManager.Inst.AddMainThreadEvent_Clear_Lock(() => endActionUnity.Invoke(blockBase));
                            endAction?.Invoke(block);
                        }
                        catch (Exception e) { SceneManager.Inst.AddMainThreadEvent_Clear_Lock(() => Log.Print($"�̱߳���{e}", Color.red)); }
                    }, background);
                }
            });
        }
        #endregion
        /// <summary>
        /// ж��һ�����飬�Ϸ��Ľ���Ҳ�ᱻһ��ж��
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
        /// ж�ز��ұ���һ�����飬�Ϸ��Ľ���Ҳ�ᱻһ������
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
        /// ֻ����һ�����飬�Ϸ��Ľ������ᱻ����
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
                Log.Print($"�޷����ؽ���{guid}�����ܽ����ļ�������", Color.red);
            }
        }
        public void SaveBuilding(string guid)
        {
            JsonTool.StoreText($@"{sceneCatalog.directoryInfo}\Building\{guid}.txt", GetBuilding(guid).Serialize());
        }
        #endregion

        /// <summary>
        /// ж�������Ѽ������飬�Ϸ��Ľ���Ҳ�ᱻһ��ж��
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
        #region ��������Ŀ���Ϸ����
        /// <summary>
        /// ����һ�������Ϸ���壬�ڲ����ص�����Ϊ��
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
        /// ����һ����������Ϸ���壬�ڲ����ص�����Ϊ��
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
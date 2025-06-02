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
    /// ����ʵ���࣬����Ϸ����ʱ�洢������������
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
        #region ��ȡ�Ѿ�ʵ�����Ķ���
        public HashSet<Vector2Int> BlockBaseInRAM = new HashSet<Vector2Int>();
        private CrossList<OneBlock> BlockBaseCrossList = new CrossList<OneBlock>();
        public List<Particle> ActiveParticle = new List<Particle>();
        /// <summary>
        /// key��guid  value��building
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

        #region �Ӵ����м����뱣��

        /// <summary>
        /// ʹ�ö��̼߳���һ�����飬�����ļ�������ʱ�ᴴ��һ��������
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="blockPos"></param>
        /// <param name="endActionUnity">ÿ�μ������һ�����鶼�ᴫ�ݷ��������߳�ִ��</param>
        /// <param name="endAction">ÿ�μ������һ�����鶼��ִ��</param>
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
                        catch (Exception e) { TimeManager.Inst.AddToQueue_MainThreadUpdate_Clear(() => Log.Print($"�̱߳���{e}", Color.red)); }
                    });
                    ThreadPool.QueueUserWorkItem((obj) =>
                    {
                        try
                        {
                            background.ToRAM(backgroundData, this);
                        }
                        catch (Exception e) { TimeManager.Inst.AddToQueue_MainThreadUpdate_Clear(() => Log.Print($"�̱߳���{e}", Color.red)); }
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
                                    Pixel.����.CloneTo(pixel, new Vector2Byte(x, y));
                                    block_.SetPixel(pixel, false, false, false);
                                    block_.DrawPixelSync(new Vector2Byte(x, y), pixel.colorInfo.color);
                                }
                            TimeManager.Inst.AddToQueue_MainThreadUpdate_Clear(() => BlockMaterial.SetBlock(block_));
                        }
                        catch (Exception e) { TimeManager.Inst.AddToQueue_MainThreadUpdate_Clear(() => Log.Print($"�̱߳���{e}", Color.red)); }
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
                                        pixel = Pixel.TakeOut("����", "����ɫ2", new(x, y));
                                    background_.SetPixel(pixel, false, false, false);
                                    background_.DrawPixelSync(new Vector2Byte(x, y), pixel.colorInfo.color);
                                }
                            TimeManager.Inst.AddToQueue_MainThreadUpdate_Clear(() => BlockMaterial.SetBackgroundBlock(background_));
                        }
                        catch (Exception e) { TimeManager.Inst.AddToQueue_MainThreadUpdate_Clear(() => Log.Print($"�̱߳���{e}", Color.red)); }
                    }, background);
                }
            });
        }
        /// <summary>
        /// ж��һ�����飬�Ϸ��Ľ���Ҳ�ᱻһ��ж��
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
        /// ����һ������
        /// </summary>
        /// <param name="blockPos">��������</param>
        /// <param name="SaveBuilding">�Ƿ񱣴��Ϸ��Ľ���</param>
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
                Log.Print($"�޷����ؽ���{guid}�����ܽ����ļ�������", Color.red);
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
        /// ж�������Ѽ������飬�Ϸ��Ľ���Ҳ�ᱻһ��ж��
        /// </summary>
        public void Unload()
        {
            foreach (var blockPos in BlockBaseInRAM.ToArray())
                UnloadBlockData(blockPos);
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
            var oneBlock = BlockBaseCrossList[blockPos];
            BlockBaseCrossList[blockPos] = new OneBlock() { Block = block, BackgroundBlock = oneBlock.BackgroundBlock };
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
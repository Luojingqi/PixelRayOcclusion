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
using static PRO.BlockMaterial;

namespace PRO
{
    /// <summary>
    /// ����ʵ���࣬����Ϸ����ʱ�洢������������
    /// </summary>
    public class SceneEntity
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
        public HashSet<Vector2Int> BlockBaseInRAM = new HashSet<Vector2Int>(50);
        private CrossList<OneBlock> BlockBaseCrossList = new CrossList<OneBlock>();

        public Dictionary<string, Role> Role_Guid_Dic = new Dictionary<string, Role>(20);
        public Dictionary<Transform, Role> Role_Trans_Dic = new Dictionary<Transform, Role>(20);
        private List<Role> RemoveActiveRoleList = new List<Role>(4);
        private Dictionary<Vector2Int, List<Role>> JumpOutSceneRoleDic = new Dictionary<Vector2Int, List<Role>>();


        public HashSet<Particle> ActiveParticleSet = new HashSet<Particle>(100);
        private List<Particle> RemoveActiveParticleList = new List<Particle>(30);
        private Dictionary<Vector2Int, List<Particle>> JumpOutSceneParticleDic = new Dictionary<Vector2Int, List<Particle>>();
        /// <summary>
        /// ���ڴ��е�����
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

        public Role GetRole(string guid)
        {
            Role role = null;
            Role_Guid_Dic.TryGetValue(guid, out role);
            return role;
        }
        public Role GetRole(Transform transform)
        {
            Role role = null;
            Role_Trans_Dic.TryGetValue(transform, out role);
            return role;
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
            block.UnLoadCountdown = BlockMaterial.proConfig.AutoUnLoadBlockCountdownTime;
            ThreadPool.QueueUserWorkItem((obj) =>
            {
                if (IOTool.LoadProto($@"{sceneCatalog.directoryInfo}\Block\{blockPos}\BlockData", Proto.BlockBaseData.Parser, out var blockBaseData))
                {
                    ThreadPool.QueueUserWorkItem((obj) =>
                    {
                        try
                        {
                            block.ToRAM(blockBaseData, this);
                            background.ToRAM(blockBaseData, this);
                            blockBaseData.ClearPutIn();
                        }
                        catch (Exception e) { Log.Print($"�̱߳���{e}", Color.red); }
                    });
                }
                else
                {
                    ThreadPool.QueueUserWorkItem((obj) =>
                    {
                    try
                    {
                        for (int x = 0; x < Block.Size.x; x++)
                            for (int y = 0; y < Block.Size.y; y++)
                            {

                                Pixel pixel;
                                lock (Pixel.pixelPool)
                                    pixel = Pixel.pixelPool.TakeOut();
                                Pixel.����.CloneTo(pixel, new Vector2Byte(x, y));
                                block.SetPixel(pixel, false, false, false);
                                block.DrawPixelSync(new Vector2Byte(x, y), pixel.colorInfo.color);

                                lock (Pixel.pixelPool)
                                        //  pixel = Pixel.pixelPool.TakeOut();
                                        //Pixel.����.CloneTo(pixel, new Vector2Byte(x, y));
                                    pixel = Pixel.TakeOut("����", "����ɫ2", new(x, y));
                                background.SetPixel(pixel, false, false, false);
                                background.DrawPixelSync(new Vector2Byte(x, y), pixel.colorInfo.color);
                            }
                        TimeManager.Inst.AddToQueue_MainThreadUpdate_Clear(() =>
                        {
                            BlockMaterial.SetBlock(block);
                            BlockMaterial.SetBackgroundBlock(background);
                        });
                    }
                    catch (Exception e) { Log.Print($"�̱߳���{e}", Color.red); }
                });
                }
            });
            if (IOTool.LoadProto($@"{sceneCatalog.directoryInfo}\Block\{blockPos}\BlockParticleData", Proto.BlockParticleData.Parser, out var BlockParticleData))
            {
                foreach (var particleData in BlockParticleData.Value)
                {
                    ParticleManager.Inst.GetPool(particleData.LoadPath).TakeOut(this).ToRAM(particleData);
                }
            }
            if (IOTool.LoadProto($@"{sceneCatalog.directoryInfo}\Block\{blockPos}\BlockRoleData", Proto.BlockRoleData.Parser, out var BlockRoleData))
            {
                foreach (var roleData in BlockRoleData.Value)
                {
                    RoleManager.Inst.TakeOut(roleData.RoleTypeName, this).ToRAM(roleData);
                }
            }
        }
        /// <summary>
        /// ж��һ�����飬�Ϸ��Ľ���Ҳ�ᱻһ��ж��
        /// ֱ�ӵ��ã��ڲ���ʹ�ö��߳�
        /// </summary>
        /// <param name="blockPos"></param>
        private void UnloadBlockData(Vector2Int blockPos)
        {
            try
            {
                BlockBaseInRAM.Remove(blockPos);
                ThreadPool.QueueUserWorkItem((obj) =>
                {
                    var block = GetBlock(blockPos);
                    var background = GetBackground(blockPos);
                    BlockBaseCrossList[blockPos] = new OneBlock();
                    Block.PutIn(block);
                    BackgroundBlock.PutIn(background);
                });
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
        /// <param name="isSaveBuilding">�Ƿ񱣴��Ϸ��Ľ���</param>
        private void SaveBlockData(Vector2Int blockPos, bool isSaveBuilding)
        {
            string path = $@"{sceneCatalog.directoryInfo}\Block\{blockPos}";
            if (Directory.Exists(path) == false) Directory.CreateDirectory(path);
            var diskData = Proto.ProtoPool.TakeOut<Proto.BlockBaseData>();
            GetBlock(blockPos).ToDisk(ref diskData);
            GetBackground(blockPos).ToDisk(ref diskData);
            if (isSaveBuilding)
                foreach (string guid in diskData.BuildingGuidIndexDic.Keys)
                    SaveBuilding(guid);
            IOTool.SaveProto($@"{path}\BlockData", diskData);
            File.Delete($@"{path}\BlockParticleData{IOTool.protoExtension}");
            File.Delete($@"{path}\BlockRoleData{IOTool.protoExtension}");
            diskData.ClearPutIn();
        }

        #region Building
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
        private void SaveBuilding(string guid)
        {
            SaveBuilding(BuildingInRAM[guid]);
        }
        private void SaveBuilding(BuildingBase building)
        {
            var diskData = building.ToDisk();
            IOTool.SaveProto($@"{sceneCatalog.directoryInfo}\Building\{building.GUID}", diskData);
            diskData.ClearPutIn();
        }
        #endregion

        public void SaveAll()
        {

            ThreadPool.QueueUserWorkItem((obj) =>
            {
                try
                {
                    #region ����  Block  Building
                    foreach (var blockPos in BlockBaseInRAM)
                        SaveBlockData(blockPos, false);
                    foreach (var building in BuildingInRAM.Values)
                        SaveBuilding(building);
                    #endregion

                    TimeManager.Inst.AddToQueue_MainThreadUpdate_Clear(() =>
                    {
                        #region ����  Particle
                        //���ӷ���
                        foreach (var particle in ActiveParticleSet)
                        {
                            var blockPos = Block.WorldToBlock(particle.transform.position);
                            if (JumpOutSceneParticleDic.TryGetValue(blockPos, out var jumpList))
                                jumpList.Add(particle);
                            else
                                JumpOutSceneParticleDic.Add(blockPos, new List<Particle>() { particle });
                        }
                        var blockParticleData = Proto.ProtoPool.TakeOut<Proto.BlockParticleData>();
                        foreach (var kv in JumpOutSceneParticleDic)
                        {
                            foreach (var particle in kv.Value)
                                if (particle.loadPath != "���ܲ���")
                                    blockParticleData.Value.Add(particle.ToDisk());
                            if (BlockBaseInRAM.Contains(kv.Key) == false)
                                foreach (var particle in kv.Value)
                                    ParticleManager.Inst.PutIn(particle);
                            IOTool.SaveProto($@"{sceneCatalog.directoryInfo}\Block\{kv.Key}\BlockParticleData", blockParticleData);
                            blockParticleData.Clear();
                        }
                        Proto.ProtoPool.PutIn(blockParticleData);
                        JumpOutSceneParticleDic.Clear();
                        #endregion

                        #region ����  Role
                        foreach (var role in Role_Guid_Dic.Values)
                        {
                            var blockPos = Block.WorldToBlock(role.transform.position);
                            if (JumpOutSceneRoleDic.TryGetValue(blockPos, out var jumpList))
                                jumpList.Add(role);
                            else
                                JumpOutSceneRoleDic.Add(blockPos, new List<Role>() { role });
                        }
                        var blockRoleData = Proto.ProtoPool.TakeOut<Proto.BlockRoleData>();
                        foreach (var kv in JumpOutSceneRoleDic)
                        {
                            foreach (var role in kv.Value)
                                blockRoleData.Value.Add(role.ToDisk());
                            if (BlockBaseInRAM.Contains(kv.Key) == false)
                                foreach (var role in kv.Value)
                                    RoleManager.Inst.PutIn(role);
                            IOTool.SaveProto($@"{sceneCatalog.directoryInfo}\Block\{kv.Key}\BlockRoleData", blockRoleData);
                            blockRoleData.Clear();
                            Debug.Log(kv.Key + "����");
                        }
                        Proto.ProtoPool.PutIn(blockRoleData);
                        JumpOutSceneRoleDic.Clear();
                        #endregion
                    });
                }
                catch (Exception e) { Debug.Log(e); }
            });

        

            sceneCatalog.Save();

        }

        public void LoadAll()
        {

        }
        /// <summary>
        /// ж�������Ѽ������飬�Ϸ��Ľ���Ҳ�ᱻһ��ж��
        /// ж������
        /// </summary>
        public void UnLoadAll()
        {
            foreach (var particle in ActiveParticleSet)
                ParticleManager.Inst.GetPool(particle.loadPath).PutIn(particle);
            ActiveParticleSet.Clear();
            foreach (var blockPos in BlockBaseInRAM.ToArray())
                UnloadBlockData(blockPos);
        }
        #endregion

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

        float time���� = 0;
        float timeFluid1 = 0;
        float timeFluid2 = 0;
        float timeFluid3 = 0;

        float UnLoadBlockCountdownTime = 0;
        List<Vector2Int> unLoadBlock = new List<Vector2Int>(20);

        PriorityQueue<Vector2Int> activeBlockUpdateTempQueue = new PriorityQueue<Vector2Int>(50);


        public void TimeUpdate()
        {
            if (Input.GetKeyDown(KeyCode.U))
                SaveAll();

            #region ����  ����
            {
                time���� += TimeManager.deltaTime;
                while (time���� > proConfig.UpdateTime����)
                {
                    time���� -= proConfig.UpdateTime����;
                    int updateTime = (int)(proConfig.UpdateTime���� * 1000);
                    foreach (var blockPos in BlockBaseInRAM)
                        activeBlockUpdateTempQueue.Enqueue(blockPos, blockPos.y);
                    while (activeBlockUpdateTempQueue.Count > 0)
                    {
                        var blockPos = activeBlockUpdateTempQueue.Dequeue();
                        GetBlock(blockPos).Update_����ȼ��(updateTime);
                        GetBackground(blockPos).Update_����ȼ��(updateTime);
                    }
                }
            }
            #endregion

            #region ����  ����1
            {
                timeFluid1 += TimeManager.deltaTime;
                while (timeFluid1 > proConfig.UpdateTimeFluid1)
                {
                    timeFluid1 -= proConfig.UpdateTimeFluid1;
                    foreach (var blockPos in BlockBaseInRAM)
                        activeBlockUpdateTempQueue.Enqueue(blockPos, blockPos.y);
                    while (activeBlockUpdateTempQueue.Count > 0)
                        GetBlock(activeBlockUpdateTempQueue.Dequeue()).UpdateFluid1();
                }
            }
            #endregion

            #region ����  ����2
            {
                timeFluid2 += TimeManager.deltaTime;
                while (timeFluid2 > proConfig.UpdateTimeFluid2)
                {
                    timeFluid2 -= proConfig.UpdateTimeFluid2;
                    foreach (var blockPos in BlockBaseInRAM)
                        activeBlockUpdateTempQueue.Enqueue(blockPos, -blockPos.y);
                    while (activeBlockUpdateTempQueue.Count > 0)
                        GetBlock(activeBlockUpdateTempQueue.Dequeue()).UpdateFluid2();
                }
            }
            #endregion

            #region ����  ����3
            {
                timeFluid3 += TimeManager.deltaTime;
                while (timeFluid3 > proConfig.UpdateTimeFluid3)
                {
                    timeFluid3 -= proConfig.UpdateTimeFluid3;
                    foreach (var blockPos in BlockBaseInRAM)
                        activeBlockUpdateTempQueue.Enqueue(blockPos, blockPos.y);
                    while (activeBlockUpdateTempQueue.Count > 0)
                        GetBlock(activeBlockUpdateTempQueue.Dequeue()).UpdateFluid3();
                }
            }
            #endregion

            #region ����  ����
            {
                int deltaTime = (int)(TimeManager.deltaTime * 1000);
                foreach (var particle in ActiveParticleSet)
                {
                    var blockPos = Block.WorldToBlock(particle.transform.position);
                    if (BlockBaseInRAM.Contains(blockPos) == false)
                    {
                        particle.gameObject.SetActive(false);
                        if (JumpOutSceneParticleDic.TryGetValue(blockPos, out var jumpList))
                            jumpList.Add(particle);
                        else
                            JumpOutSceneParticleDic.Add(blockPos, new List<Particle>() { particle });
                        RemoveActiveParticleList.Add(particle);
                    }
                    else
                    {
                        if (particle.RemainTime > 0)
                            particle.UpdateRemainTime(deltaTime);
                        if (particle.RemainTime <= 0)
                        {
                            RemoveActiveParticleList.Add(particle);
                            if (particle.RecyleState == false)
                                ParticleManager.Inst.GetPool(particle.loadPath).PutIn(particle);
                        }
                    }
                }
                for (int i = RemoveActiveParticleList.Count - 1; i >= 0; i--)
                    ActiveParticleSet.Remove(RemoveActiveParticleList[i]);
                RemoveActiveParticleList.Clear();
            }

            #endregion

            #region ����  ��ɫ
            {
                foreach (var role in Role_Guid_Dic.Values)
                {
                    var blockPos = Block.WorldToBlock(role.transform.position);
                    if (BlockBaseInRAM.Contains(blockPos) == false)
                    {
                        role.gameObject.SetActive(false);
                        if (JumpOutSceneRoleDic.TryGetValue(blockPos, out var jumpList))
                            jumpList.Add(role);
                        else
                            JumpOutSceneRoleDic.Add(blockPos, new List<Role>() { role });
                        RemoveActiveRoleList.Add(role);
                    }
                }
                for (int i = RemoveActiveRoleList.Count - 1; i >= 0; i--)
                {
                    var role = RemoveActiveRoleList[i];
                    Role_Guid_Dic.Remove(role.Guid);
                    Role_Trans_Dic.Remove(role.transform);
                }
                RemoveActiveRoleList.Clear();
            }
            #endregion

            #region ����  ����ж�ؼ��
            UnLoadBlockCountdownTime += TimeManager.deltaTime;
            float checkTime = 1f;
            while (UnLoadBlockCountdownTime > checkTime)
            {
                UnLoadBlockCountdownTime -= checkTime;
                foreach (var blockPos in BlockBaseInRAM)
                {
                    var block = GetBlock(blockPos);
                    block.UnLoadCountdown -= checkTime;
                    if (block.UnLoadCountdown <= 0)
                        unLoadBlock.Add(blockPos);
                }
                var array = unLoadBlock.ToArray();
                foreach (var blockPos in array)
                {
                    ThreadPool.QueueUserWorkItem((obj) => SaveBlockData(blockPos, true));
                    UnloadBlockData(blockPos);
                }
                unLoadBlock.Clear();
            }
            #endregion
        }
    }
}
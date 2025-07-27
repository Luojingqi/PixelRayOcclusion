using Google.FlatBuffers;
using PRO.DataStructure;
using PRO.Disk;
using PRO.Disk.Scene;
using PRO.Flat.Ex;
using PRO.Tool;
using PRO.Tool.Serialize.IO;
using PRO.TurnBased;
using Sirenix.OdinInspector;
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
    /// 场景实体类，在游戏运行时存储场景运行数据
    /// </summary>
    public partial class SceneEntity : SerializedMonoBehaviour
    {
        #region 对象池
        public Transform BlockNode;
        public Transform RoleNode;
        public Transform ParticleNode;
        public Transform BuildingNode;

        private static Transform Node;
        private static Transform PoolNode;
        private static ObjectPoolArbitrary<SceneEntity> pool = new ObjectPoolArbitrary<SceneEntity>(CreateSceneEntity);
        public static void InitPool()
        {
            Node = new GameObject("SceneEntityNode").transform;
            PoolNode = new GameObject("SceneEntityPoolNode").transform;
            PoolNode.SetParent(SceneManager.Inst.PoolNode);
        }
        private static SceneEntity CreateSceneEntity()
        {
            var scene = new GameObject("SceneEntity").AddComponent<SceneEntity>();
            scene.BlockNode = new GameObject("BlockNode").transform;
            scene.BlockNode.SetParent(scene.transform);
            scene.RoleNode = new GameObject("RoleNode").transform;
            scene.RoleNode.SetParent(scene.transform);
            scene.ParticleNode = new GameObject("ParticleNode").transform;
            scene.ParticleNode.SetParent(scene.transform);
            scene.BuildingNode = new GameObject("BuildingNode").transform;
            scene.BuildingNode.SetParent(scene.transform);
            scene.gameObject.SetActive(false);  
            return scene;
        }
        public static SceneEntity TakeOut(SceneCatalog sceneCatalog)
        {
            var scene = pool.TakeOut();
            scene.sceneCatalog = sceneCatalog;
            scene.name = sceneCatalog.name;
            scene.transform.SetParent(Node);
            return scene;
        }
        public static CountdownEvent PutIn(SceneEntity scene)
        {
            pool.PutIn(scene);
            scene.transform.SetParent(PoolNode);
            return scene.UnLoadAll();
        }
        #endregion
        public SceneCatalog sceneCatalog { get; private set; }
        #region 获取已经实例化的对象
        public HashSet<Vector2Int> ActiveBlockBase = new HashSet<Vector2Int>(50);
        private CrossList<Block> BlockBaseCrossList = new CrossList<Block>();

        public Dictionary<string, Role> ActiveRole_Guid = new Dictionary<string, Role>(20);
        public Dictionary<Transform, Role> ActiveRole_Trans = new Dictionary<Transform, Role>(20);
        private List<Role> RemoveActiveRoleList = new List<Role>(4);
        private Dictionary<Vector2Int, List<Role>> JumpOutSceneRoleDic = new Dictionary<Vector2Int, List<Role>>(4);


        public HashSet<Particle> ActiveParticle = new HashSet<Particle>(100);
        private List<Particle> RemoveActiveParticleList = new List<Particle>(30);
        private Dictionary<Vector2Int, List<Particle>> JumpOutSceneParticleDic = new Dictionary<Vector2Int, List<Particle>>(4);
        /// <summary>
        /// 在场景中活跃的区块
        /// key：guid  value：building
        /// </summary>
        public Dictionary<string, BuildingBase> ActiveBuilding = new Dictionary<string, BuildingBase>(24);

        /// <summary>
        /// 在场景中活跃的战斗
        /// </summary>
        public Dictionary<string, RoundFSM> ActiveRound = new Dictionary<string, RoundFSM>(4);
        private List<string> RemoveActiveRoundList = new List<string>(2);

        public Block GetBlock(Vector2Int blockPos) => BlockBaseCrossList[blockPos];
        public BackgroundBlock GetBackground(Vector2Int blockPos) => BlockBaseCrossList[blockPos]?.background;
        public BlockBase GetBlockBase(BlockBase.BlockType blockType, Vector2Int blockPos)
        {
            var block = BlockBaseCrossList[blockPos];
            switch (blockType)
            {
                case BlockBase.BlockType.BackgroundBlock: return block.background;
                default: return block;
            }
        }

        public BuildingBase GetBuilding(string guid)
        {
            ActiveBuilding.TryGetValue(guid, out var building);
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
            ActiveRole_Guid.TryGetValue(guid, out role);
            return role;
        }
        public Role GetRole(Transform transform)
        {
            Role role = null;
            ActiveRole_Trans.TryGetValue(transform, out role);
            return role;
        }


        #endregion

        #region 从磁盘中加载与保存
        #region 加载区块
        /// <summary>
        /// 使用多线程加载一个区块，区块文件不存在时会创建一个空区块
        /// </summary>
        /// <param name="blockPos"></param>
        public void ThreadLoadOrCreateBlock(SceneCatalog catalog, Vector2Int blockPos, CountdownEvent countdown_main = null)
        {
            if (ActiveBlockBase.Contains(blockPos)) return;
            countdown_main?.AddCount();
            ActiveBlockBase.Add(blockPos);
            var block = Block.TakeOut(this, blockPos);
            var background = block.background;
            BlockBaseCrossList[blockPos] = block;
            block.ResetUnLoadCountdown();
            #region 加载  区块
            if (IOTool.LoadFlat($@"{catalog.directoryInfo}\Block\{blockPos}\BlockData", out var builder))
            {
                ThreadPool.QueueUserWorkItem((obj) =>
                {
                    try
                    {
                        block.FillPixel();
                        background.FillPixel();
                        var diskData = Flat.BlockBaseData.GetRootAsBlockBaseData(builder.DataBuffer);
                        PixelTypeInfo[] typeInfoArray = new PixelTypeInfo[diskData.PixelTypeNameArrayLength];
                        PixelColorInfo[] colorInfoArray = new PixelColorInfo[diskData.PixelColorNameArrayLength];
                        BuildingBase[] buildingArray = new BuildingBase[diskData.PixelBuildingGuidArrayLength];
                        for (int i = typeInfoArray.Length - 1; i >= 0; i--)
                            typeInfoArray[typeInfoArray.Length - i - 1] = Pixel.GetPixelTypeInfo(diskData.PixelTypeNameArray(i));
                        for (int i = colorInfoArray.Length - 1; i >= 0; i--)
                            colorInfoArray[colorInfoArray.Length - i - 1] = Pixel.GetPixelColorInfo(diskData.PixelColorNameArray(i));
                        var builder_Extend = FlatBufferBuilder.TakeOut(1024 * 2);
                        for (int i = buildingArray.Length - 1; i >= 0; i--)
                        {
                            string guid = diskData.PixelBuildingGuidArray(i);
                            var building = GetBuilding(guid);
                            if (building == null)
                            {
                                TimeManager.Inst.AddToQueue_MainThreadUpdate_Clear_WaitInvoke(() => LoadBuilding(catalog, guid, builder_Extend));
                                building = GetBuilding(guid);
                                builder_Extend.Clear();
                            }
                            buildingArray[buildingArray.Length - i - 1] = building;
                        }
                        FlatBufferBuilder.PutIn(builder_Extend);
                        for (int i = Block.Size.x * Block.Size.y - 1; i >= 0; i--)
                        {
                            var blockPixelDiskData = diskData.BlockPixelArray(i).Value;
                            var backgroundPixelDiskData = diskData.BackgroundPixelArray(i).Value;
                            int index = Block.Size.x * Block.Size.y - i - 1;
                            Vector2Byte pos = new(index % Block.Size.y, index / Block.Size.y);
                            block.GetPixel(pos).ToRAM(blockPixelDiskData, typeInfoArray, colorInfoArray, buildingArray);
                            background.GetPixel(pos).ToRAM(backgroundPixelDiskData, typeInfoArray, colorInfoArray, buildingArray);
                        }
                        for (int i = diskData.BlockFlameQueueLength - 1; i >= 0; i--)
                            block.queue_火焰.Enqueue(diskData.BlockFlameQueue(i).Value.ToRAM());
                        for (int i = diskData.BackgroundFlameQueueLength - 1; i >= 0; i--)
                            background.queue_火焰.Enqueue(diskData.BackgroundFlameQueue(i).Value.ToRAM());
                        CountdownEvent countdown = new CountdownEvent(1);
                        block.ToRAM(diskData, countdown);
                        background.ToRAM(diskData, countdown);
                        countdown.Signal();
                        countdown.Wait();
                        TimeManager.Inst.AddToQueue_MainThreadUpdate_Clear_WaitInvoke(() =>
                        {
                            #region 加载  particle
                            {
                                if (IOTool.LoadFlat($@"{catalog.directoryInfo}\Block\{blockPos}\BlockParticleData", out var builder))
                                {
                                    var diskData = Flat.BlockParticleData.GetRootAsBlockParticleData(builder.DataBuffer);
                                    for (int i = diskData.ListLength - 1; i >= 0; i--)
                                    {
                                        var particleData = diskData.List(i).Value;
                                        ParticleManager.Inst.ToRAM(this, particleData);
                                    }
                                    FlatBufferBuilder.PutIn(builder);
                                }
                            }
                            #endregion
                            #region 加载  role
                            {
                                if (IOTool.LoadFlat($@"{catalog.directoryInfo}\Block\{blockPos}\BlockRoleData", out var builder))
                                {
                                    var diskData = Flat.BlockRoleData.GetRootAsBlockRoleData(builder.DataBuffer);
                                    for (int i = diskData.ListLength - 1; i >= 0; i--)
                                    {
                                        var roleGuid = diskData.List(i);
                                        RoleManager.Inst.Load(this, catalog, roleGuid);
                                    }
                                    FlatBufferBuilder.PutIn(builder);
                                }
                            }
                            #endregion
                        });

                    }
                    catch (Exception e) { Log.Print($"线程报错：{e}", Color.red); }
                    finally
                    {
                        FlatBufferBuilder.PutIn(builder);
                        countdown_main?.Signal();
                    }
                });
            }
            else
            {
                ThreadPool.QueueUserWorkItem((obj) =>
                {
                    try
                    {
                        block.FillPixel();
                        background.FillPixel();
                        countdown_main?.AddCount();
                        TimeManager.Inst.AddToQueue_MainThreadUpdate_Clear(() =>
                        {
                            SetBlock(block);
                            SetBackgroundBlock(background);
                            countdown_main?.Signal();
                        });
                    }
                    catch (Exception e) { Log.Print($"线程报错：{e}", Color.red); }
                    finally
                    {
                        countdown_main?.Signal();
                    }
                });
            }
            #endregion
        }
        #endregion
        #region 卸载区块
        /// <summary>
        /// 卸载一个区块，上方的建筑也会被一并卸载
        /// 直接调用，内部会使用多线程
        /// </summary>
        /// <param name="blockPos"></param>
        private CountdownEvent UnloadBlockData(Vector2Int blockPos, CountdownEvent countdown)
        {
            countdown.AddCount();
            ActiveBlockBase.Remove(blockPos);
            ThreadPool.QueueUserWorkItem((obj) =>
            {
                try
                {
                    var block = GetBlock(blockPos);
                    BlockBaseCrossList[blockPos] = null;
                    Block.PutIn(block, countdown);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
                countdown.Signal();
            });
            return countdown;
        }
        #endregion
        #region 保存区块
        /// <summary>
        /// 保存一个区块
        /// </summary>
        /// <param name="blockPos">区块坐标</param>
        /// <param name="isSaveBuilding">是否保存上方的建筑</param>
        private void SaveBlockData(SceneCatalog catalog, Vector2Int blockPos, bool isSaveBuilding)
        {
            string path = $@"{catalog.directoryInfo}\Block\{blockPos}";
            if (Directory.Exists(path) == false) Directory.CreateDirectory(path);
            var builder = FlatBufferBuilder.TakeOut(1024 * 400);
            var block = GetBlock(blockPos);
            var background = GetBackground(blockPos);

            var typeNameIndexDic = new Dictionary<string, int>(8);
            var colorNameIndexDic = new Dictionary<string, int>(16);
            var buildingGuidIndexDic = new Dictionary<string, int>(4);
            var blockPixelArrayOffset = ToDiskPixelData(block, typeNameIndexDic, colorNameIndexDic, buildingGuidIndexDic, builder);
            var backgroundPixelArrayOffset = ToDiskPixelData(background, typeNameIndexDic, colorNameIndexDic, buildingGuidIndexDic, builder);
            Action addDataEvent = null;
            addDataEvent += block.ToDisk(builder);
            addDataEvent += background.ToDisk(builder);
            var typeNameArrayOffset = DicKeyToOffset(typeNameIndexDic, builder);
            var colorNameArrayOffset = DicKeyToOffset(colorNameIndexDic, builder);
            var buildingGuidArrayOffset = DicKeyToOffset(buildingGuidIndexDic, builder);
            Flat.BlockBaseData.StartBlockBaseData(builder);
            Flat.BlockBaseData.AddBlockPixelArray(builder, blockPixelArrayOffset);
            Flat.BlockBaseData.AddBackgroundPixelArray(builder, backgroundPixelArrayOffset);
            Flat.BlockBaseData.AddPixelTypeNameArray(builder, typeNameArrayOffset);
            Flat.BlockBaseData.AddPixelColorNameArray(builder, colorNameArrayOffset);
            Flat.BlockBaseData.AddPixelBuildingGuidArray(builder, buildingGuidArrayOffset);
            addDataEvent.Invoke();
            builder.Finish(Flat.BlockBaseData.EndBlockBaseData(builder).Value);

            if (isSaveBuilding)
            {
                var builder_building = FlatBufferBuilder.TakeOut(1024 * 8);
                var builder_Extend = FlatBufferBuilder.TakeOut(1024 * 2);
                foreach (string guid in buildingGuidIndexDic.Keys)
                {
                    SaveBuilding(catalog, ActiveBuilding[guid], builder_building, builder_Extend);
                    builder_building.Clear();
                    builder_Extend.Clear();
                }
                FlatBufferBuilder.PutIn(builder_building);
                FlatBufferBuilder.PutIn(builder_Extend);
            }
            IOTool.SaveFlat($@"{path}\BlockData", builder);
            File.Delete($@"{path}\BlockParticleData{IOTool.flatExtension}");
            File.Delete($@"{path}\BlockRoleData{IOTool.flatExtension}");
            FlatBufferBuilder.PutIn(builder);
        }
        private static VectorOffset DicKeyToOffset(Dictionary<string, int> dic, FlatBufferBuilder builder)
        {
            Span<int> keyOffsetArray = stackalloc int[dic.Count];
            foreach (var kv in dic)
                keyOffsetArray[kv.Value] = builder.CreateString(kv.Key).Value;
            return builder.CreateVector_Offset(keyOffsetArray);
        }
        private static VectorOffset ToDiskPixelData(BlockBase blockBase, Dictionary<string, int> TypeNameIndexDic, Dictionary<string, int> ColorNameIndexDic, Dictionary<string, int> BuildingGuidIndexDic, FlatBufferBuilder builder)
        {
            Span<int> pixelArray = stackalloc int[Block.Size.x * Block.Size.y];
            for (int y = 0; y < Block.Size.y; y++)
                for (int x = 0; x < Block.Size.x; x++)
                {
                    Pixel pixel = blockBase.GetPixel(new Vector2Byte(x, y));

                    if (TypeNameIndexDic.TryGetValue(pixel.typeInfo.typeName, out int typeNameIndex) == false)
                    {
                        typeNameIndex = TypeNameIndexDic.Count;
                        TypeNameIndexDic.Add(pixel.typeInfo.typeName, typeNameIndex);
                    }
                    if (ColorNameIndexDic.TryGetValue(pixel.colorInfo.colorName, out int colorNameIndex) == false)
                    {
                        colorNameIndex = ColorNameIndexDic.Count;
                        ColorNameIndexDic.Add(pixel.colorInfo.colorName, colorNameIndex);
                    }

                    Span<int> buildingList = stackalloc int[pixel.buildingSet.Count];
                    int buildingListIndex = 0;
                    foreach (var building in pixel.buildingSet)
                    {
                        if (BuildingGuidIndexDic.TryGetValue(building.GUID, out int buildingIndex) == false)
                        {
                            buildingIndex = BuildingGuidIndexDic.Count;
                            BuildingGuidIndexDic.Add(building.GUID, buildingIndex);
                        }
                        buildingList[buildingListIndex++] = buildingIndex;
                    }
                    VectorOffset buildingOffset = builder.CreateVector_Data(buildingList);

                    Flat.PixelData.StartPixelData(builder);
                    Flat.PixelData.AddTypeIndex(builder, typeNameIndex);
                    Flat.PixelData.AddColorIndex(builder, colorNameIndex);
                    Flat.PixelData.AddDurability(builder, pixel.durability);
                    Flat.PixelData.AddAffectsTransparency(builder, pixel.affectsTransparency);
                    Flat.PixelData.AddBuildingList(builder, buildingOffset);
                    pixelArray[y * Block.Size.x + x] = Flat.PixelData.EndPixelData(builder).Value;
                }
            return builder.CreateVector_Offset(pixelArray);
        }
        #endregion

        #region Building
        private void LoadBuilding(SceneCatalog catalog, string guid, FlatBufferBuilder builder_Extend)
        {
            if (IOTool.LoadFlat($@"{catalog.directoryInfo}\Building\{guid}", out var builder))
            {
                var diskData = Flat.BuildingBaseData.GetRootAsBuildingBaseData(builder.DataBuffer);
                BuildingBase building = BuildingBase.New(diskData.Name, guid, this);
                building.ToRAM(diskData, builder_Extend);
                FlatBufferBuilder.PutIn(builder);
                builder_Extend.Clear();
            }
            else
            {
                Log.Print($"无法加载建筑{guid}，可能建筑文件不存在", Color.red);
            }
        }
        private void SaveBuilding(SceneCatalog catalog, BuildingBase building, FlatBufferBuilder builder, FlatBufferBuilder builder_Extend)
        {
            building.ToDisk(builder, builder_Extend);
            IOTool.SaveFlat($@"{catalog.directoryInfo}\Building\{building.GUID}", builder);
        }
        #endregion

        #region Round
        public void LoadRound(SceneCatalog catalog, string guid)
        {
            if (IOTool.LoadFlat(@$"{catalog.directoryInfo}\Round\{guid}", out var builder))
            {
                var diskData = TurnBased.Flat.RoundFSMData.GetRootAsRoundFSMData(builder.DataBuffer);
                var round = new RoundFSM(this, diskData.Guid);
                round.ToRAM(diskData);
                FlatBufferBuilder.PutIn(builder);
            }
        }
        private void SaveRound(SceneCatalog catalog, RoundFSM round)
        {
            var builder = FlatBufferBuilder.TakeOut(1024 * 2);
            round.ToDisk(builder);
            IOTool.SaveFlat(@$"{catalog.directoryInfo}\Round\{round.GUID}", builder);
            FlatBufferBuilder.PutIn(builder);
        }
        #endregion


        /// <summary>
        /// 保存场景所有数据，内部使用线程池优化，返回等待信号
        /// </summary>
        /// <returns></returns>
        public CountdownEvent SaveAll(SceneCatalog catalog)
        {
            CountdownEvent countdown = new CountdownEvent(1);
            countdown.AddCount();
            ThreadPool.QueueUserWorkItem((obj) =>
            {
                try
                {
                    #region 保存  Block  Building  Round
                    var countdown_Block = new CountdownEvent(ActiveBlockBase.Count);
                    foreach (var blockPos in ActiveBlockBase)
                        ThreadPool.QueueUserWorkItem((obj) => { SaveBlockData(catalog, blockPos, false); countdown_Block.Signal(); });
                    var builder_building = FlatBufferBuilder.TakeOut(1024 * 8);
                    var builder_Extend = FlatBufferBuilder.TakeOut(1024 * 2);
                    foreach (var building in ActiveBuilding.Values)
                    {
                        SaveBuilding(catalog, building, builder_building, builder_Extend);
                        builder_building.Clear();
                        builder_Extend.Clear();
                    }
                    FlatBufferBuilder.PutIn(builder_building);
                    FlatBufferBuilder.PutIn(builder_Extend);
                    #endregion
                    countdown_Block.Wait();
                    TimeManager.Inst.AddToQueue_MainThreadUpdate_Clear_WaitInvoke(() =>
                    {
                        foreach (var round in ActiveRound.Values)
                            SaveRound(catalog, round);

                        #region 保存  Particle
                        {
                            //粒子分组
                            foreach (var particle in ActiveParticle)
                            {
                                var blockPos = Block.WorldToBlock(particle.transform.position);
                                if (JumpOutSceneParticleDic.TryGetValue(blockPos, out var jumpList))
                                    jumpList.Add(particle);
                                else
                                    JumpOutSceneParticleDic.Add(blockPos, new List<Particle>() { particle });
                            }
                            var builder = FlatBufferBuilder.TakeOut(1024 * 16);
                            var extendBuilder = FlatBufferBuilder.TakeOut(1024);
                            foreach (var kv in JumpOutSceneParticleDic)
                            {
                                Span<int> particleListOffset = stackalloc int[kv.Value.Count];
                                int index = 0;
                                foreach (var particle in kv.Value)
                                    if (particle.loadPath != "技能播放")
                                        particleListOffset[index++] = particle.ToDisk(builder, extendBuilder).Value;
                                var listOffset = builder.CreateVector_Offset(particleListOffset.Slice(0, index));
                                if (ActiveBlockBase.Contains(kv.Key) == false)
                                    foreach (var particle in kv.Value)
                                        ParticleManager.Inst.PutIn(particle);

                                Flat.BlockParticleData.StartBlockParticleData(builder);
                                Flat.BlockParticleData.AddList(builder, listOffset);
                                builder.Finish(Flat.BlockParticleData.EndBlockParticleData(builder).Value);
                                IOTool.SaveFlat($@"{catalog.directoryInfo}\Block\{kv.Key}\BlockParticleData", builder);
                                builder.Clear();
                            }
                            FlatBufferBuilder.PutIn(builder);
                            FlatBufferBuilder.PutIn(extendBuilder);
                            JumpOutSceneParticleDic.Clear();
                        }
                        #endregion

                        #region 保存  Role
                        {
                            //粒子分组
                            foreach (var role in ActiveRole_Guid.Values)
                            {
                                var blockPos = Block.WorldToBlock(role.transform.position);
                                if (JumpOutSceneRoleDic.TryGetValue(blockPos, out var jumpList))
                                    jumpList.Add(role);
                                else
                                    JumpOutSceneRoleDic.Add(blockPos, new List<Role>() { role });
                            }
                            var builder = FlatBufferBuilder.TakeOut(1024 * 32);
                            var roleBuilder = FlatBufferBuilder.TakeOut(1024);
                            foreach (var kv in JumpOutSceneRoleDic)
                            {
                                Span<int> roleListOffset = stackalloc int[kv.Value.Count];
                                int index = 0;
                                foreach (var role in kv.Value)
                                {
                                    roleListOffset[index++] = builder.CreateString(role.GUID).Value;
                                    roleBuilder.Finish(role.ToDisk(roleBuilder).Value);
                                    IOTool.SaveFlat($@"{catalog.directoryInfo}\Role\{role.GUID}", roleBuilder);
                                    roleBuilder.Clear();
                                }

                                var listOffset = builder.CreateVector_Offset(roleListOffset.Slice(0, index));
                                if (ActiveBlockBase.Contains(kv.Key) == false)
                                    foreach (var role in kv.Value)
                                        RoleManager.Inst.PutIn(role);

                                Flat.BlockRoleData.StartBlockRoleData(builder);
                                Flat.BlockRoleData.AddList(builder, listOffset);
                                builder.Finish(Flat.BlockRoleData.EndBlockRoleData(builder).Value);
                                IOTool.SaveFlat($@"{catalog.directoryInfo}\Block\{kv.Key}\BlockRoleData", builder);
                                builder.Clear();
                            }
                            FlatBufferBuilder.PutIn(builder);
                            FlatBufferBuilder.PutIn(roleBuilder);
                            JumpOutSceneRoleDic.Clear();
                        }
                        #endregion
                    });
                }
                catch (Exception e) { Log.Print(e.ToString(), Color.red); }
                countdown.Signal();
                countdown.Wait();
            });
            #region 保存  场景额外数据
            {
                var builder = FlatBufferBuilder.TakeOut(1024);
                var blockPosSet = new HashSet<Vector2Int>(ActiveBlockBase.Count);

                Span<int> roundGuidOffsetArray = stackalloc int[ActiveRound.Count];
                int index = 0;
                foreach (var round in ActiveRound.Values)
                {
                    roundGuidOffsetArray[index++] = builder.CreateString(round.GUID).Value;
                    foreach (var role in round.RoleHash)
                    {
                        var blockPos = Block.WorldToBlock(role.transform.position);
                        for (int y = -1; y <= 1; y++)
                            for (int x = -1; x <= 1; x++)
                                blockPosSet.Add(blockPos + new Vector2Int(x, y));
                    }
                }
                var roundGuidOffset = builder.CreateVector_Offset(roundGuidOffsetArray);

                Flat.SceneEntityData.StartActiveBlockBaseVector(builder, blockPosSet.Count);
                foreach (var blockPos in blockPosSet)
                    blockPos.ToDisk(builder);
                var activeBlockOffset = builder.EndVector();

                Flat.SceneEntityData.StartSceneEntityData(builder);
                Flat.SceneEntityData.AddActiveBlockBase(builder, activeBlockOffset);
                Flat.SceneEntityData.AddRoundGuid(builder, roundGuidOffset);
                builder.Finish(Flat.SceneEntityData.EndSceneEntityData(builder).Value);
                IOTool.SaveFlat($@"{catalog.directoryInfo}\SceneEntityData", builder);
                FlatBufferBuilder.PutIn(builder);
            }
            #endregion

            catalog.Save();
            countdown.Signal();
            return countdown;
        }

        public CountdownEvent LoadAll(SceneCatalog catalog)
        {
            CountdownEvent countdown = new CountdownEvent(1);
            if (IOTool.LoadFlat(@$"{catalog.directoryInfo}\SceneEntityData", out var builder))
            {
                var diskData = Flat.SceneEntityData.GetRootAsSceneEntityData(builder.DataBuffer);
                CountdownEvent countdown_block = new CountdownEvent(1);
                for (int i = diskData.ActiveBlockBaseLength - 1; i >= 0; i--)
                    ThreadLoadOrCreateBlock(catalog, diskData.ActiveBlockBase(i).Value.ToRAM(), countdown_block);
#if PRO_RENDER
                var cameraCenterBlockPos = Block.WorldToBlock(catalog.cameraPos);
                var minLightBufferBlockPos = CameraCenterBlockPos - LightResultBufferBlockSize / 2;
                var minBlockBufferPos = minLightBufferBlockPos - EachBlockReceiveLightSize / 2;
                var maxBlockBufferPos = minBlockBufferPos + LightResultBufferBlockSize - new Vector2Int(1, 1) + EachBlockReceiveLightSize - new Vector2Int(1, 1);
                for (int y = minBlockBufferPos.y; y <= maxBlockBufferPos.y; y++)
                    for (int x = minBlockBufferPos.x; x <= maxBlockBufferPos.x; x++)
                        ThreadLoadOrCreateBlock(catalog, new(x, y), countdown_block);
#endif
                countdown_block.Signal();
                countdown.AddCount();
                ThreadPool.QueueUserWorkItem((obj) =>
                {
                    countdown_block.Wait();
                    TimeManager.Inst.AddToQueue_MainThreadUpdate_Clear_WaitInvoke(() =>
                    {
                        for (int i = diskData.RoundGuidLength - 1; i >= 0; i--)
                            LoadRound(catalog, diskData.RoundGuid(i));
                    });
                    FlatBufferBuilder.PutIn(builder);
                    countdown.Signal();
                });
            }
            else
            {
                for (int y = MinBlockBufferPos.y; y <= MaxBlockBufferPos.y; y++)
                    for (int x = MinBlockBufferPos.x; x <= MaxBlockBufferPos.x; x++)
                        ThreadLoadOrCreateBlock(catalog, new Vector2Int(x, y), countdown);
            }
            countdown.Signal();
            return countdown;
        }
        /// <summary>
        /// 卸载所有已加载区块，上方的建筑也会被一并卸载
        /// 卸载粒子
        /// 卸载角色
        /// 卸载战斗
        /// 返回等待信号
        /// </summary>
        private CountdownEvent UnLoadAll()
        {
            CountdownEvent countdown = new CountdownEvent(1);
            #region 卸载 round
            if (GamePlayMain.Inst.Round != null && ActiveRound.ContainsKey(GamePlayMain.Inst.Round.GUID))
                GamePlayMain.Inst.Round = null;
            ActiveRound.Clear();
            #endregion
            #region 卸载  role
            foreach (var role in ActiveRole_Guid.Values.ToArray())
                RoleManager.Inst.PutIn(role);
            foreach (var list in JumpOutSceneRoleDic.Values)
                for (int i = 0; i < list.Count; i++)
                    RoleManager.Inst.PutIn(list[i]);
            ActiveRole_Guid.Clear();
            ActiveRole_Trans.Clear();
            JumpOutSceneRoleDic.Clear();
            #endregion
            #region 卸载  particle
            foreach (var particle in ActiveParticle)
                ParticleManager.Inst.PutIn(particle);
            foreach (var list in JumpOutSceneParticleDic.Values)
                for (int i = 0; i < list.Count; i++)
                    ParticleManager.Inst.PutIn(list[i]);
            ActiveParticle.Clear();
            JumpOutSceneParticleDic.Clear();
            #endregion
            #region 卸载  block  building
            var blockPosArray = ActiveBlockBase.ToArray();
            for (int i = 0; i < blockPosArray.Length; i++)
                UnloadBlockData(blockPosArray[i], countdown);
            #endregion
            if (SceneManager.Inst.NowScene == this)
                DrawApplyQueue.Clear();
            countdown.Signal();
            return countdown;
        }

        /// <summary>
        /// 重新加载
        /// </summary>
        /// <returns></returns>
        public CountdownEvent ReLoadAll()
        {
            CountdownEvent countdown = new CountdownEvent(1);
            var countdown_unload = UnLoadAll();
            ThreadPool.QueueUserWorkItem((obj) =>
            {
                countdown_unload.Wait();
                TimeManager.Inst.AddToQueue_MainThreadUpdate_Clear(() =>
                {
                    var countdown_load = LoadAll(sceneCatalog);
                    ThreadPool.QueueUserWorkItem((obj) =>
                    {
                        countdown_load.Wait();
                        countdown.Signal();
                    });
                });
                countdown.Wait();
            });
            return countdown;
        }
        #endregion


        #region 更新所用的临时对象
        float time火焰 = 0;
        float timeFluid1 = 0;
        float timeFluid2 = 0;
        float timeFluid3 = 0;

        float UnLoadBlockCountdownTime = 0;
        List<Vector2Int> unLoadBlock = new List<Vector2Int>(20);

        PriorityQueue<Vector2Int> activeBlockUpdateTempQueue = new PriorityQueue<Vector2Int>(50);

        CountdownEvent _countdownEvent = new CountdownEvent(1);
        #endregion

        public void TimeUpdate()
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                TimeManager.enableUpdate = false;
                var countdown = SaveAll(sceneCatalog);
                ThreadPool.QueueUserWorkItem((obj) =>
                {
                    if (countdown.Wait(1000 * 15))
                        Log.Print("保存完成");
                    else
                        Log.Print("保存超时");
                    TimeManager.enableUpdate = true;

                });
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                TimeManager.enableUpdate = false;
                var countdown = ReLoadAll();
                ThreadPool.QueueUserWorkItem((obj) =>
                {
                    if (countdown.Wait(1000 * 15))
                        Log.Print("重载完成");
                    else
                        Log.Print("重载超时");
                    TimeManager.enableUpdate = true;

                });

            }

            #region 更新  火焰
            {
                time火焰 += TimeManager.deltaTime;
                while (time火焰 > proConfig.UpdateTime火焰)
                {
                    time火焰 -= proConfig.UpdateTime火焰;
                    int updateTime = (int)(proConfig.UpdateTime火焰 * 1000);
                    foreach (var blockPos in ActiveBlockBase)
                        activeBlockUpdateTempQueue.Enqueue(blockPos, blockPos.y);
                    while (activeBlockUpdateTempQueue.Count > 0)
                    {
                        var blockPos = activeBlockUpdateTempQueue.Dequeue();
                        GetBlock(blockPos).Update_火焰燃烧(updateTime);
                        GetBackground(blockPos).Update_火焰燃烧(updateTime);
                    }
                }
            }
            #endregion

            #region 更新  流体1
            {
                timeFluid1 += TimeManager.deltaTime;
                while (timeFluid1 > proConfig.UpdateTimeFluid1)
                {
                    timeFluid1 -= proConfig.UpdateTimeFluid1;
                    foreach (var blockPos in ActiveBlockBase)
                        activeBlockUpdateTempQueue.Enqueue(blockPos, blockPos.y);
                    while (activeBlockUpdateTempQueue.Count > 0)
                        GetBlock(activeBlockUpdateTempQueue.Dequeue()).UpdateFluid1();
                }
            }
            #endregion

            #region 更新  流体2
            {
                timeFluid2 += TimeManager.deltaTime;
                while (timeFluid2 > proConfig.UpdateTimeFluid2)
                {
                    timeFluid2 -= proConfig.UpdateTimeFluid2;
                    foreach (var blockPos in ActiveBlockBase)
                        activeBlockUpdateTempQueue.Enqueue(blockPos, -blockPos.y);
                    while (activeBlockUpdateTempQueue.Count > 0)
                        GetBlock(activeBlockUpdateTempQueue.Dequeue()).UpdateFluid2();
                }
            }
            #endregion

            #region 更新  流体3
            {
                timeFluid3 += TimeManager.deltaTime;
                while (timeFluid3 > proConfig.UpdateTimeFluid3)
                {
                    timeFluid3 -= proConfig.UpdateTimeFluid3;
                    foreach (var blockPos in ActiveBlockBase)
                        activeBlockUpdateTempQueue.Enqueue(blockPos, blockPos.y);
                    while (activeBlockUpdateTempQueue.Count > 0)
                        GetBlock(activeBlockUpdateTempQueue.Dequeue()).UpdateFluid3();
                }
            }
            #endregion

            #region 更新  战斗
            foreach (var round in ActiveRound.Values)
            {
                if (round.NowState.EnumName != RoundStateEnum.end)
                    round.Update();
                else
                    RemoveActiveRoundList.Add(round.GUID);
            }
            for (int i = 0; i < RemoveActiveRoundList.Count; i++)
                ActiveRound.Remove(RemoveActiveRoundList[i]);
            RemoveActiveRoundList.Clear();
            #endregion

            #region 更新  粒子
            {
                int deltaTime = (int)(TimeManager.deltaTime * 1000);
                foreach (var particle in ActiveParticle)
                {
                    var blockPos = Block.WorldToBlock(particle.transform.position);
                    if (ActiveBlockBase.Contains(blockPos) == false)
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
                    ActiveParticle.Remove(RemoveActiveParticleList[i]);
                RemoveActiveParticleList.Clear();
            }

            #endregion

            #region 更新  角色
            {
                foreach (var role in ActiveRole_Guid.Values)
                {
                    var blockPos = Block.WorldToBlock(role.transform.position);
                    if (ActiveBlockBase.Contains(blockPos) == false)
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
                    ActiveRole_Guid.Remove(role.GUID);
                    ActiveRole_Trans.Remove(role.transform);
                }
                RemoveActiveRoleList.Clear();
            }
            #endregion

            #region 更新  区块卸载检查
            UnLoadBlockCountdownTime += TimeManager.deltaTime;
            float checkTime = 1f;
            while (UnLoadBlockCountdownTime > checkTime)
            {
                UnLoadBlockCountdownTime -= checkTime;
                foreach (var blockPos in ActiveBlockBase)
                {
                    var block = GetBlock(blockPos);
                    block.UnLoadCountdown -= checkTime;
                    if (block.UnLoadCountdown <= 0)
                        unLoadBlock.Add(blockPos);
                }
                var array = unLoadBlock.ToArray();
                foreach (var blockPos in array)
                {
                    ThreadPool.QueueUserWorkItem((obj) => SaveBlockData(sceneCatalog, blockPos, true));
                    UnloadBlockData(blockPos, _countdownEvent);
                }
                unLoadBlock.Clear();
            }
            #endregion
        }
    }
}
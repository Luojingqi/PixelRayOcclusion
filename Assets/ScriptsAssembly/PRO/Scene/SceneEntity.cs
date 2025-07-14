using Google.FlatBuffers;
using PRO.DataStructure;
using PRO.Disk;
using PRO.Disk.Scene;
using PRO.Tool;
using PRO.Tool.Serialize.IO;
using PRO.TurnBased;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using static PRO.BlockMaterial;
using static UnityEngine.ParticleSystem;

namespace PRO
{
    /// <summary>
    /// 场景实体类，在游戏运行时存储场景运行数据
    /// </summary>
    public partial class SceneEntity
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
        public HashSet<Vector2Int> ActiveBlockBase = new HashSet<Vector2Int>(50);
        private CrossList<OneBlock> BlockBaseCrossList = new CrossList<OneBlock>();

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

        public Block GetBlock(Vector2Int blockPos) => BlockBaseCrossList[blockPos].Block;
        public BackgroundBlock GetBackground(Vector2Int blockPos) => BlockBaseCrossList[blockPos].BackgroundBlock;
        public BlockBase GetBlockBase(BlockBase.BlockType blockType, Vector2Int blockPos) => BlockBaseCrossList[blockPos][(int)blockType];

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
        /// <param name="scene"></param>
        /// <param name="blockPos"></param>
        /// <param name="endActionUnity">每次加载完成一个区块都会传递方法让主线程执行</param>
        /// <param name="endAction">每次加载完成一个区块都会执行</param>
        public void ThreadLoadOrCreateBlock(Vector2Int blockPos)
        {
            ActiveBlockBase.Add(blockPos);
            var block = CreateBlock(blockPos);
            var background = CreateBackground(blockPos);
            block.UnLoadCountdown = BlockMaterial.proConfig.AutoUnLoadBlockCountdownTime;
            #region 加载  区块
            ThreadPool.QueueUserWorkItem((obj) =>
            {
                if (IOTool.LoadFlat($@"{sceneCatalog.directoryInfo}\Block\{blockPos}\BlockData", out var builder))
                {
                    ThreadPool.QueueUserWorkItem((obj) =>
                    {
                        try
                        {
                            var diskData = Flat.BlockBaseData.GetRootAsBlockBaseData(builder.DataBuffer);
                            PixelTypeInfo[] typeInfoArray = new PixelTypeInfo[diskData.PixelTypeNameArrayLength];
                            PixelColorInfo[] colorInfoArray = new PixelColorInfo[diskData.PixelColorNameArrayLength];
                            BuildingBase[] buildingArray = new BuildingBase[diskData.PixelBuildingGuidArrayLength];
                            for (int i = typeInfoArray.Length - 1; i >= 0; i--)
                                typeInfoArray[typeInfoArray.Length - i - 1] = Pixel.GetPixelTypeInfo(diskData.PixelTypeNameArray(i));
                            for (int i = colorInfoArray.Length - 1; i >= 0; i--)
                                colorInfoArray[colorInfoArray.Length - i - 1] = BlockMaterial.GetPixelColorInfo(diskData.PixelColorNameArray(i));
                            for (int i = buildingArray.Length - 1; i >= 0; i--)
                            {
                                string guid = diskData.PixelBuildingGuidArray(i);
                                var building = GetBuilding(guid);
                                if (building == null)
                                {
                                    TimeManager.Inst.AddToQueue_MainThreadUpdate_Clear_WaitInvoke(() => LoadBuilding(guid));
                                    building = GetBuilding(guid);
                                }
                                buildingArray[buildingArray.Length - i - 1] = building;
                            }
                            for (int i = Block.Size.x * Block.Size.y - 1; i >= 0; i--)
                            {
                                var blockPixelDiskData = diskData.BlockPixelArray(i).Value;
                                var backgroundPixelDiskData = diskData.BackgroundPixelArray(i).Value;
                                int index = Block.Size.x * Block.Size.y - i - 1;
                                Vector2Byte pos = new(index % Block.Size.y, index / Block.Size.y);
                                block.SetPixel(PixelToRAM(blockPixelDiskData, pos, typeInfoArray, colorInfoArray, buildingArray), false, false, false);
                                background.SetPixel(PixelToRAM(backgroundPixelDiskData, pos, typeInfoArray, colorInfoArray, buildingArray), false, false, false);
                            }
                            block.ToRAM(diskData);
                            background.ToRAM(diskData);
                            FlatBufferBuilder.PutIn(builder);
                        }
                        catch (Exception e) { Log.Print($"线程报错：{e}", Color.red); }
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
                                    Pixel.空气.CloneTo(pixel, new Vector2Byte(x, y));
                                    block.SetPixel(pixel, false, false, false);
                                    block.DrawPixelSync(new Vector2Byte(x, y), pixel.colorInfo.color);

                                    lock (Pixel.pixelPool)
                                        pixel = Pixel.TakeOut("背景", "背景色2", new(x, y));
                                    background.SetPixel(pixel, false, false, false);
                                    background.DrawPixelSync(new Vector2Byte(x, y), pixel.colorInfo.color);
                                }
                            TimeManager.Inst.AddToQueue_MainThreadUpdate_Clear(() =>
                            {
                                BlockMaterial.SetBlock(block);
                                BlockMaterial.SetBackgroundBlock(background);
                            });
                        }
                        catch (Exception e) { Log.Print($"线程报错：{e}", Color.red); }
                    });
                }
            });
            #endregion
            #region 加载  particle
            {
                if (IOTool.LoadFlat($@"{sceneCatalog.directoryInfo}\Block\{blockPos}\BlockParticleData", out var builder))
                {
                    var diskData = Flat.BlockParticleData.GetRootAsBlockParticleData(builder.DataBuffer);
                    for (int i = diskData.ListLength - 1; i >= 0; i--)
                    {
                        var particleData = diskData.List(i).Value;
                        ParticleManager.Inst.GetPool(particleData.LoadPath).TakeOut(this).ToRAM(particleData);
                    }
                    FlatBufferBuilder.PutIn(builder);
                }
            }
            #endregion
            #region 加载  role
            {
                if (IOTool.LoadFlat($@"{sceneCatalog.directoryInfo}\Block\{blockPos}\BlockRoleData", out var builder))
                {
                    var diskData = Flat.BlockRoleData.GetRootAsBlockRoleData(builder.DataBuffer);
                    for (int i = diskData.ListLength - 1; i >= 0; i--)
                    {
                        var roleGuid = diskData.List(i);
                        RoleManager.Inst.Load(roleGuid, this);
                    }
                    FlatBufferBuilder.PutIn(builder);
                }
            }
            #endregion
        }
        private static Pixel PixelToRAM(Flat.PixelData pixelDiskData, Vector2Byte pos, PixelTypeInfo[] typeInfoArray, PixelColorInfo[] colorInfoArray, BuildingBase[] buildingArray)
        {
            Pixel pixel = null;
            lock (Pixel.pixelPool)
                pixel = Pixel.pixelPool.TakeOut();
            Pixel.InitPixel(pixel, typeInfoArray[pixelDiskData.TypeIndex], colorInfoArray[pixelDiskData.ColorIndex], pos, pixelDiskData.Durability);
            pixel.affectsTransparency = pixelDiskData.AffectsTransparency;
            for (int i = pixelDiskData.BuildingListLength - 1; i >= 0; i--)
            {
                var building = buildingArray[pixelDiskData.BuildingList(i)];
                pixel.buildingSet.Add(building);
                building.ToRAM_PixelSwitch(building.GetBuilding_Pixel(pixel.posG, pixel.blockBase.blockType), pixel);
            }
            return pixel;
        }
        #endregion
        #region 卸载区块
        /// <summary>
        /// 卸载一个区块，上方的建筑也会被一并卸载
        /// 直接调用，内部会使用多线程
        /// </summary>
        /// <param name="blockPos"></param>
        private CountdownEvent UnloadBlockData(Vector2Int blockPos)
        {
            CountdownEvent countdown = new CountdownEvent(1);
            ActiveBlockBase.Remove(blockPos);
            ThreadPool.QueueUserWorkItem((obj) =>
            {
                try
                {
                    var block = GetBlock(blockPos);
                    var background = GetBackground(blockPos);
                    BlockBaseCrossList[blockPos] = new OneBlock();
                    Block.PutIn(block, countdown);
                    BackgroundBlock.PutIn(background, countdown);
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
        private void SaveBlockData(Vector2Int blockPos, bool isSaveBuilding)
        {
            string path = $@"{sceneCatalog.directoryInfo}\Block\{blockPos}";
            if (Directory.Exists(path) == false) Directory.CreateDirectory(path);
            var builder = FlatBufferBuilder.TakeOut(1024 * 300);
            var block = GetBlock(blockPos);
            var background = GetBackground(blockPos);
            {
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
                    foreach (string guid in buildingGuidIndexDic.Keys)
                        SaveBuilding(ActiveBuilding[guid]);
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
        public static Dictionary<string, Type> BuildingTypeDic = new Dictionary<string, Type>();
        public void LoadBuilding(string guid)
        {
            if (IOTool.LoadProto($@"{sceneCatalog.directoryInfo}\Building\{guid}", Proto.BuildingBaseData.Parser, out var diskData))
            {
                BuildingBase building = BuildingBase.New(BuildingTypeDic[diskData.Name], guid, this);
                building.ToRAM(diskData);
                ActiveBuilding.Add(guid, building);
            }
            else
            {
                Log.Print($"无法加载建筑{guid}，可能建筑文件不存在", Color.red);
            }
        }
        private void SaveBuilding(BuildingBase building)
        {
            var diskData = building.ToDisk();
            IOTool.SaveProto($@"{sceneCatalog.directoryInfo}\Building\{building.GUID}", diskData);
            diskData.ClearPutIn();
        }
        #endregion

        #region Round
        public void LoadRound(string guid)
        {
            if (IOTool.LoadFlat(@$"{sceneCatalog.directoryInfo}\Round\{guid}", out var builder))
            {
                var diskData = TurnBased.Flat.RoundFSMData.GetRootAsRoundFSMData(builder.DataBuffer);
                new RoundFSM(this, diskData.Guid);
            }
            FlatBufferBuilder.PutIn(builder);
        }
        private void SaveRound(RoundFSM round)
        {
            var builder = FlatBufferBuilder.TakeOut(1024 * 2);
            round.ToDisk(builder);
            IOTool.SaveFlat(@$"{sceneCatalog.directoryInfo}\Round\{round.GUID}", builder);
            FlatBufferBuilder.PutIn(builder);
        }
        #endregion
        /// <summary>
        /// 保存场景所有数据，内部使用线程池优化，返回等待信号
        /// </summary>
        /// <returns></returns>
        public CountdownEvent SaveAll()
        {
            CountdownEvent countdown = new CountdownEvent(1);
            ThreadPool.QueueUserWorkItem((obj) =>
            {
                try
                {
                    #region 保存  Block  Building  Round
                    countdown.AddCount(ActiveBlockBase.Count);
                    foreach (var blockPos in ActiveBlockBase)
                        ThreadPool.QueueUserWorkItem((obj) => { SaveBlockData(blockPos, false); countdown.Signal(); });
                    foreach (var building in ActiveBuilding.Values)
                        SaveBuilding(building);
                    foreach (var round in ActiveRound.Values)
                        SaveRound(round);
                    #endregion
                    countdown.AddCount();
                    TimeManager.Inst.AddToQueue_MainThreadUpdate_Clear(() =>
                    {
                        #region 保存  Particle
                        {                        //粒子分组
                            foreach (var particle in ActiveParticle)
                            {

                                var blockPos = Block.WorldToBlock(particle.transform.position);
                                if (JumpOutSceneParticleDic.TryGetValue(blockPos, out var jumpList))
                                    jumpList.Add(particle);
                                else
                                    JumpOutSceneParticleDic.Add(blockPos, new List<Particle>() { particle });
                            }
                            var builder = FlatBufferBuilder.TakeOut(1024 * 32);
                            foreach (var kv in JumpOutSceneParticleDic)
                            {
                                Span<int> particleListOffset = stackalloc int[kv.Value.Count];
                                int index = 0;
                                foreach (var particle in kv.Value)
                                    if (particle.loadPath == "技能播放")
                                        particleListOffset[index++] = particle.ToDisk(builder).Value;
                                var listOffset = builder.CreateVector_Offset(particleListOffset.Slice(0, index));
                                if (ActiveBlockBase.Contains(kv.Key) == false)
                                    foreach (var particle in kv.Value)
                                        ParticleManager.Inst.PutIn(particle);

                                Flat.BlockParticleData.StartBlockParticleData(builder);
                                Flat.BlockParticleData.AddList(builder, listOffset);
                                builder.Finish(Flat.BlockParticleData.EndBlockParticleData(builder).Value);
                                IOTool.SaveFlat($@"{sceneCatalog.directoryInfo}\Block\{kv.Key}\BlockParticleData", builder);
                                builder.Clear();
                            }
                            FlatBufferBuilder.PutIn(builder);
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
                                    IOTool.SaveFlat($@"{sceneCatalog.directoryInfo}\Role\{role.GUID}", roleBuilder);
                                    roleBuilder.Clear();
                                }

                                var listOffset = builder.CreateVector_Offset(roleListOffset.Slice(0, index));
                                if (ActiveBlockBase.Contains(kv.Key) == false)
                                    foreach (var role in kv.Value)
                                        RoleManager.Inst.PutIn(role);

                                Flat.BlockRoleData.StartBlockRoleData(builder);
                                Flat.BlockRoleData.AddList(builder, listOffset);
                                builder.Finish(Flat.BlockRoleData.EndBlockRoleData(builder).Value);
                                IOTool.SaveFlat($@"{sceneCatalog.directoryInfo}\Block\{kv.Key}\BlockRoleData", builder);
                                builder.Clear();
                            }
                            FlatBufferBuilder.PutIn(builder);
                            FlatBufferBuilder.PutIn(roleBuilder);
                            JumpOutSceneRoleDic.Clear();
                        }
                        #endregion

                        countdown.Signal();
                    });
                }
                catch (Exception e) { Debug.Log(e); }
                countdown.Signal();
            });
            sceneCatalog.Save();
            return countdown;
        }

        public void LoadAll()
        {
            #region 加载  round
            {
                var files = sceneCatalog.directoryInfo.GetFiles(@$"\Round\*{IOTool.flatExtension}");
                for (int i = 0; i < files.Length; i++)
                {
                    string fileName = files[i].Name;
                    string guid = fileName.Substring(0, fileName.Length - IOTool.flatExtension.Length);
                    LoadRound(guid);
                }
            }
            #endregion
        }
        /// <summary>
        /// 卸载所有已加载区块，上方的建筑也会被一并卸载
        /// 卸载粒子
        /// 卸载角色
        /// 卸载战斗
        /// 返回等待信号
        /// </summary>
        public CountdownEvent UnLoadAll()
        {
            CountdownEvent countdown = new CountdownEvent(1);
            #region 卸载 round
            ActiveRound.Clear();
            #endregion
            #region 卸载  role
            foreach (var role in ActiveRole_Guid.Values)
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
            countdown.AddCount();
            WaitHandle[] countdowns = new WaitHandle[ActiveParticle.Count];
            var blockPosArray = ActiveBlockBase.ToArray();
            for (int i = 0; i < countdowns.Length; i++)
                countdowns[i] = UnloadBlockData(blockPosArray[i]).WaitHandle;
            ThreadPool.QueueUserWorkItem((obj) =>
            {
                WaitHandle.WaitAll(countdowns);
                countdown.Signal();
            });
            #endregion
            countdown.Signal();
            return countdown;
        }
        #endregion

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

        #region 更新所用的临时对象
        float time火焰 = 0;
        float timeFluid1 = 0;
        float timeFluid2 = 0;
        float timeFluid3 = 0;

        float UnLoadBlockCountdownTime = 0;
        List<Vector2Int> unLoadBlock = new List<Vector2Int>(20);

        PriorityQueue<Vector2Int> activeBlockUpdateTempQueue = new PriorityQueue<Vector2Int>(50);
        #endregion

        public void TimeUpdate()
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                var countdown = SaveAll();
                TimeManager.enableUpdate = false;
                ThreadPool.QueueUserWorkItem((obj) =>
                {
                    if (countdown.Wait(1000 * 15))
                        Log.Print("保存完成");
                    else
                        Log.Print("保存超时");
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
                    ThreadPool.QueueUserWorkItem((obj) => SaveBlockData(blockPos, true));
                    UnloadBlockData(blockPos);
                }
                unLoadBlock.Clear();
            }
            #endregion
        }
    }
}
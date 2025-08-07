using PRO.Disk;
using PRO.Tool;
using PRO.Tool.Serialize.IO;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using PRO.Proto.Ex;
using PRO.Skill;
using PROTool;
using System.Buffers;
using System.Reflection;
using Google.FlatBuffers;
using PRO.Flat.Ex;

namespace PRO
{
    /// <summary>
    /// 建筑的基类，一个建筑是由一堆点组合的合集
    /// </summary>
    public abstract class BuildingBase : MonoBehaviour, IScene
    {
        public struct Index
        {
            public Vector2Int offset;
            public BlockBase.BlockType blockType;

            public Index(Vector2Int offset, BlockBase.BlockType blockType)
            {
                this.offset = offset;
                this.blockType = blockType;
            }
        }

        public SceneEntity Scene => _scene;
        protected SceneEntity _scene;
        public string GUID;
        public string Name;
        public Dictionary<Index, Building_Pixel> AllPixel = new Dictionary<Index, Building_Pixel>();
        public HashSet<Index> AllSurvivalPixel = new HashSet<Index>();
        public HashSet<Index> AllDeathPixel = new HashSet<Index>();
        public HashSet<Index> AllUnloadPixel = new HashSet<Index>();

        public Vector2Int Global;
        public Vector2Int Size;
        public BoxCollider2D TriggerCollider;


        /// <summary>
        /// 这个蓝图位置的像素点被更改，将这个蓝图点(已加载)从存活与死亡两种状态转换（是否和蓝图对应，对应代表存活，反之死亡）,子类实现以产生相应的行为
        /// </summary>
        public void PixelSwitch(Building_Pixel building_Pixel, PixelTypeInfo oldTypeInfo, PixelColorInfo oldColorInfo)
        {
            Building_Pixel.State oldState = building_Pixel.GetState(oldTypeInfo, oldColorInfo);
            Building_Pixel.State newState = building_Pixel.GetNowState();
            if (oldState == newState) return;

            var index = new Index(building_Pixel.offset, building_Pixel.blockType);
            if (newState == Building_Pixel.State.Death)
            {
                AllSurvivalPixel.Remove(index);
                AllDeathPixel.Add(index);
                PixelSwitch_Death(building_Pixel);
            }
            else
            {
                AllDeathPixel.Remove(index);
                AllSurvivalPixel.Add(index);
                PixelSwitch_Survival(building_Pixel);
            }
        }
        protected abstract void PixelSwitch_Death(Building_Pixel pixelB);
        protected abstract void PixelSwitch_Survival(Building_Pixel pixelB);
        public Building_Pixel GetBuilding_Pixel(Vector2Int globalPos, BlockBase.BlockType blockType) => AllPixel.GetValueOrDefault(new(PixelPosRotate.New(transform.rotation.eulerAngles).RotatePosInverse(globalPos - Global), blockType));
        public abstract void CreateInit();
        public static BuildingBase New(string typeName, string guid, SceneEntity scene)
        {
            BuildingBase ret = null;
            if (BuildingTypeDic.TryGetValue(typeName, out var type))
                ret = New(type, guid, scene);
            return ret;
        }
        public static BuildingBase New(Type type, string guid, SceneEntity scene)
        {
            GameObject go = new GameObject(type.Name);
            var building = go.AddComponent(type) as BuildingBase;
            building.Name = type.Name;
            building.GUID = guid;
            building.TriggerCollider = go.AddComponent<BoxCollider2D>();
            building.TriggerCollider.isTrigger = true;
            building._scene = scene;
            building.transform.SetParent(scene.BuildingNode);
            scene.ActiveBuilding.Add(guid, building);
            go.layer = (int)GameLayer.Building;
            return building;
        }
        #region 反射创建Building实例
        private static Dictionary<string, Type> BuildingTypeDic;
        public static void InitBuildingType()
        {
            var typeList = ReflectionTool.GetDerivedClasses(typeof(BuildingBase));
            BuildingTypeDic = new(typeList.Count);
            for (int i = 0; i < typeList.Count; i++)
            {
                var type = typeList[i];
                BuildingTypeDic.Add(type.Name, type);
            }
        }
        #endregion
        #region 序列化与反序列化
        /// <summary>
        /// 反序列化过程1：添加一个蓝图点到建筑中(未加载状态)
        /// </summary>
        public virtual void ToRAM_AddBuilding_Pixel(Building_Pixel building_Pixel)
        {
            var index = new Index(building_Pixel.offset, building_Pixel.blockType);
            AllPixel.Add(index, building_Pixel);
            AllUnloadPixel.Add(index);
        }
        /// <summary>
        /// 反序列化过程2：将一个蓝图点由未加载状态改为已加载状态
        /// </summary>
        public virtual void ToRAM_PixelSwitch(Building_Pixel building_Pixel, Pixel pixel)
        {
            building_Pixel.pixel = pixel;
            pixel.buildingSet.Add(this);
            var index = new Index(building_Pixel.offset, building_Pixel.blockType);
            AllUnloadPixel.Remove(index);
            Building_Pixel.State newState = building_Pixel.GetNowState();
            if (newState == Building_Pixel.State.Death)
                AllDeathPixel.Add(index);
            else
                AllSurvivalPixel.Add(index);
        }
        /// <summary>
        /// 卸载一个点
        /// </summary>
        public void UnloadPixel(Building_Pixel building_Pixel)
        {
            var index = new Index(building_Pixel.offset, building_Pixel.blockType);
            if (building_Pixel.GetNowState() == Building_Pixel.State.Survival) AllSurvivalPixel.Remove(index);
            else AllDeathPixel.Remove(index);
            AllUnloadPixel.Add(index);
            if (AllUnloadPixel.Count == AllPixel.Count)
            {
                _scene.ActiveBuilding.Remove(GUID);
                foreach (var item in AllPixel.Values)
                {
                    item.pixel.buildingSet.Remove(this);
                    Building_Pixel.PutIn(item);
                }
                TimeManager.Inst.AddToQueue_MainThreadUpdate_Clear(() => GameObject.Destroy(gameObject));
            }
            else
            {
                building_Pixel.pixel = null;
            }
        }
        public void ToDisk(FlatBufferBuilder builder, FlatBufferBuilder builder_Extend)
        {
            var typeNameOffset = builder.CreateString(GetType().Name);
            var nameOffset = builder.CreateString(Name);
            var typeNameIndexDic = new Dictionary<string, int>();
            var colorNameIndexDic = new Dictionary<string, int>();
            Flat.BuildingBaseData.StartAllPixelVector(builder, AllPixel.Count);
            foreach (var item in AllPixel.Values)
            {
                if (typeNameIndexDic.TryGetValue(item.typeInfo.typeName, out var typeNameIndex) == false)
                {
                    typeNameIndex = typeNameIndexDic.Count;
                    typeNameIndexDic.Add(item.typeInfo.typeName, typeNameIndex);
                }
                if (colorNameIndexDic.TryGetValue(item.colorInfo.colorName, out var colorNameIndex) == false)
                {
                    colorNameIndex = colorNameIndexDic.Count;
                    colorNameIndexDic.Add(item.colorInfo.colorName, colorNameIndex);
                }
                Flat.Building_PixelData.CreateBuilding_PixelData(builder, typeNameIndex, colorNameIndex, item.offset.x, item.offset.y, (int)item.blockType);
            }
            var allPixelOffset = builder.EndVector();
            Span<int> typeNameArrayOffsetArray = stackalloc int[typeNameIndexDic.Count];
            Span<int> colorNameArrayOffsetArray = stackalloc int[colorNameIndexDic.Count];
            foreach (var kv in typeNameIndexDic)
                typeNameArrayOffsetArray[kv.Value] = builder.CreateString(kv.Key).Value;
            foreach (var kv in colorNameIndexDic)
                colorNameArrayOffsetArray[kv.Value] = builder.CreateString(kv.Key).Value;
            var typeNameArrayOffset = builder.CreateVector_Offset(typeNameArrayOffsetArray);
            var colorNameArrayOffset = builder.CreateVector_Offset(colorNameArrayOffsetArray);

            ExtendDataToDisk(builder_Extend);
            var extendDataOffset = builder.CreateVector_Builder(builder_Extend);


            Flat.BuildingBaseData.StartBuildingBaseData(builder);
            Flat.BuildingBaseData.AddTypeName(builder, typeNameOffset);
            Flat.BuildingBaseData.AddName(builder, nameOffset);
            Flat.BuildingBaseData.AddGlobal(builder, Global.ToDisk(builder));
            Flat.BuildingBaseData.AddSize(builder, Size.ToDisk(builder));
            Flat.BuildingBaseData.AddPixelTypeNameArray(builder, typeNameArrayOffset);
            Flat.BuildingBaseData.AddPixelColorNameArray(builder, colorNameArrayOffset);
            Flat.BuildingBaseData.AddAllPixel(builder, allPixelOffset);
        }
        public void ToRAM(Flat.BuildingBaseData diskData, FlatBufferBuilder builder_Extend)
        {
            var typeInfoArray = new PixelTypeInfo[diskData.PixelColorNameArrayLength];
            var colorInfoArray = new PixelColorInfo[diskData.PixelColorNameArrayLength];
            for (int i = typeInfoArray.Length - 1; i >= 0; i--)
                typeInfoArray[typeInfoArray.Length - i - 1] = Pixel.GetPixelTypeInfo(diskData.PixelTypeNameArray(i));
            for (int i = colorInfoArray.Length - 1; i >= 0; i--)
                colorInfoArray[colorInfoArray.Length - i - 1] = Pixel.GetPixelColorInfo(diskData.PixelColorNameArray(i));
            for (int i = diskData.AllPixelLength - 1; i >= 0; i--)
            {
                var pixelDiskData = diskData.AllPixel(i).Value;
                var pixel = Building_Pixel.TakeOut();
                pixel.Init(typeInfoArray[pixelDiskData.TypeIndex], colorInfoArray[pixelDiskData.ColorIndex], pixelDiskData.Offset.ToRAM(), (BlockBase.BlockType)pixelDiskData.BlockType);
                ToRAM_AddBuilding_Pixel(pixel);
            }
            var extendSpan = builder_Extend.DataBuffer.ToSpan(0, diskData.ExtendDataLength);
            for (int i = extendSpan.Length; i >= 0; i--)
                extendSpan[extendSpan.Length - i - 1] = diskData.ExtendData(i);
            ExtendDataToRAM(builder_Extend);
        }

        protected virtual void ExtendDataToRAM(FlatBufferBuilder builder) { }
        protected virtual void ExtendDataToDisk(FlatBufferBuilder builder) { }

        #endregion

        public SpriteRenderer CreateSelectionBox(Color color)
        {
            Vector2Int wh = Size;
            Texture2D texture = Texture2DPool.TakeOut(Size.x + 2, Size.y + 2);
            texture.GetRawTextureData<float>().DrawLine(texture.width, new(0, 0), new(Size.x + 1, 0), color);
            texture.GetRawTextureData<float>().DrawLine(texture.width, new(0, 0), new(0, Size.y + 1), color);
            texture.GetRawTextureData<float>().DrawLine(texture.width, new(Size.x + 1, 0), new(Size.x + 1, Size.y + 1), color);
            texture.GetRawTextureData<float>().DrawLine(texture.width, new(0, Size.y + 1), new(Size.x + 1, Size.y + 1), color);
            texture.Apply();
            SpriteRenderer spriteRenderer = Texture2DPool.TakeOutSpriteRenderer();
            spriteRenderer.sprite = Texture2DPool.GetOnlySprite(texture);
            spriteRenderer.transform.position = Block.GlobalToWorld(Global - Vector2Int.one);
            return spriteRenderer;
        }

        //public SpriteRenderer DrawSprite()
        //{
        //    Texture2D texture = Texture2DPool.TakeOut(Size.x, Size.y);
        //    SpriteRenderer spriteRenderer = Texture2DPool.TakeOutSpriteRenderer();
        //    spriteRenderer.sprite = Texture2DPool.GetOnlySprite(texture);
        //    var data = texture.GetRawTextureData<float>();
        //    foreach (var building in AllSurvivalPixel.Values)
        //        data.DrawPixel(texture.width, building.Offset, building.ColorInfo.color);
        //    texture.Apply();
        //    spriteRenderer.transform.position = Block.GlobalToWorld(global);
        //    return spriteRenderer;
        //}

        /// <summary>
        /// 彻底卸载并删除一个building
        /// </summary>
        public void Delete()
        {
            _scene.ActiveBuilding.Remove(GUID);
            File.Delete(@$"{_scene.sceneCatalog.directoryInfo}/Building/{GUID}{IOTool.protoExtension}");
            foreach (var item in AllPixel.Values)
            {
                item.pixel.buildingSet.Remove(this);
                item.pixel = null;
                Building_Pixel.PutIn(item);
            }
            GameObject.Destroy(gameObject);
        }
    }
}

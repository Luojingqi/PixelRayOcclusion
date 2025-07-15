using PRO.Disk;
using PRO.Tool;
using PRO.Tool.Serialize.IO;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using PRO.Proto.Ex;

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
        /// 这个建筑是否可以被破坏
        /// </summary>
        public bool CanByBroken = true;


        /// <summary>
        /// 这个蓝图位置的像素点被更改，将这个蓝图点(已加载)从存活与死亡两种状态转换（是否和蓝图对应，对应代表存活，反之死亡）,子类实现以产生相应的行为
        /// </summary>
        public void PixelSwitch(Building_Pixel building_Pixel, Pixel pixel)
        {
            Building_Pixel.State oldState = building_Pixel.GetState();
            building_Pixel.pixel = pixel;
            pixel.buildingSet.Add(this);
            Building_Pixel.State newState = building_Pixel.GetState();
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
        public abstract void Init();
        public static BuildingBase New(Type type, string guid, SceneEntity scene)
        {
            GameObject go = new GameObject(type.Name);
            var building = go.AddComponent(type) as BuildingBase;
            building.Name = type.Name;
            building.GUID = guid;
            building.TriggerCollider = go.AddComponent<BoxCollider2D>();
            building.TriggerCollider.isTrigger = true;
            building._scene = scene;

            go.layer = (int)GameLayer.Building;
            return building;
        }
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
            var index = new Index(building_Pixel.offset, building_Pixel.blockType);
            AllUnloadPixel.Remove(index);
            building_Pixel.pixel = pixel;
            pixel.buildingSet.Add(this);
            Building_Pixel.State newState = building_Pixel.GetState();
            if (newState == Building_Pixel.State.Death)
                AllDeathPixel.Add(index);
            else
                AllSurvivalPixel.Add(index);
        }
        /// <summary>
        /// 卸载一个点
        /// </summary>
        public void UnloadPixel(Pixel pixel)
        {
            Building_Pixel building_Pixel = GetBuilding_Pixel(pixel.posG, pixel.blockBase.blockType);
            var index = new Index(building_Pixel.offset, building_Pixel.blockType);
            if (building_Pixel.GetState() == Building_Pixel.State.Survival) AllSurvivalPixel.Remove(index);
            else AllDeathPixel.Remove(index);
            building_Pixel.pixel = null;
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
        }
        public virtual Proto.BuildingBaseData ToDisk()
        {
            var diskData = Proto.ProtoPool.TakeOut<Proto.BuildingBaseData>();
            diskData.TypeName = GetType().Name;
            diskData.Name = Name;
            diskData.Global = Global.ToDisk();
            diskData.Size = Size.ToDisk();
            diskData.CanByBroken = CanByBroken;

            foreach (var building_Pixel in AllPixel.Values)
            {
                if (diskData.TypeNameIndexDic.TryGetValue(building_Pixel.typeInfo.typeName, out int typeNameIndex) == false)
                {
                    typeNameIndex = diskData.TypeNameIndexDic.Count;
                    diskData.TypeNameIndexDic.Add(building_Pixel.typeInfo.typeName, typeNameIndex);
                }
                if (diskData.ColorNameIndexDic.TryGetValue(building_Pixel.colorInfo.colorName, out int colorNameIndex) == false)
                {
                    colorNameIndex = diskData.ColorNameIndexDic.Count;
                    diskData.ColorNameIndexDic.Add(building_Pixel.colorInfo.colorName, colorNameIndex);
                }
                var pixelData = Proto.ProtoPool.TakeOut<Proto.BuildingBaseData.Types.Bulding_Pixel>();
                pixelData.TypeIndex = typeNameIndex;
                pixelData.ColorIndex = colorNameIndex;
                pixelData.Offset = building_Pixel.offset.ToDisk();
                pixelData.BlockType = (int)building_Pixel.blockType;
                diskData.AllPixel.Add(pixelData);
            }
            return diskData;
        }
        public virtual void ToRAM(Proto.BuildingBaseData diskData)
        {
            Dictionary<int, PixelTypeInfo> typeNameDic = new Dictionary<int, PixelTypeInfo>(diskData.TypeNameIndexDic.Count);
            Dictionary<int, PixelColorInfo> colorNameDic = new Dictionary<int, PixelColorInfo>(diskData.ColorNameIndexDic.Count);

            foreach (var kv in diskData.TypeNameIndexDic)
                typeNameDic.Add(kv.Value, Pixel.GetPixelTypeInfo(kv.Key));
            foreach (var kv in diskData.ColorNameIndexDic)
                colorNameDic.Add(kv.Value, BlockMaterial.GetPixelColorInfo(kv.Key));
            for (int i = 0; i < diskData.AllPixel.Count; i++)
            {
                var pixelData = diskData.AllPixel[i];
                Building_Pixel pixel = null;
                lock (Building_Pixel.pool)
                    pixel = Building_Pixel.TakeOut();
                pixel.Init(typeNameDic[pixelData.TypeIndex], colorNameDic[pixelData.ColorIndex], pixelData.Offset.ToRAM(), (Block.BlockType)pixelData.BlockType);
                ToRAM_AddBuilding_Pixel(pixel);
            }
        }
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

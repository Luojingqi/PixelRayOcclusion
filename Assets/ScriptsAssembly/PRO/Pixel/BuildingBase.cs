using PRO.Tool;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static PRO.Tool.JsonTool;

namespace PRO
{
    /// <summary>
    /// 建筑的基类，一个建筑是由一堆点组合的合集
    /// </summary>
    public abstract class BuildingBase : MonoBehaviour
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

        public SceneEntity scene;
        public string GUID;
        public string Name;
        public Dictionary<Index, Building_Pixel> AllPixel = new Dictionary<Index, Building_Pixel>();
        public HashSet<Index> AllSurvivalPixel = new HashSet<Index>();
        public HashSet<Index> AllDeathPixel = new HashSet<Index>();
        public HashSet<Index> AllUnloadPixel = new HashSet<Index>();

        public Vector2Int global;
        public Vector2Int Size;
        public BoxCollider2D TriggerCollider;

        /// <summary>
        /// 这个建筑是否可以被破坏
        /// </summary>
        public bool CanByBroken = true;
        /// <summary>
        /// 这个蓝图位置的像素点被更改，将这个蓝图点从存活与死亡两种状态转换（是否和蓝图对应，对应代表存活，反之死亡）,子类实现以产生相应的行为
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
        public Building_Pixel GetBuilding_Pixel(Vector2Int globalPos, BlockBase.BlockType blockType) => AllPixel.GetValueOrDefault(new(PixelPosRotate.New(transform.rotation.eulerAngles).RotatePosInverse(globalPos - global), blockType));
        public abstract void Init();
        public static BuildingBase New(Type type, string guid, SceneEntity scene)
        {
            if (typeof(BuildingBase).IsAssignableFrom(type) == false) return null;
            GameObject go = new GameObject(type.Name);
            BuildingBase building = (BuildingBase)go.AddComponent(type);
            building.Name = type.Name;
            building.GUID = guid;
            building.TriggerCollider = go.AddComponent<BoxCollider2D>();
            building.TriggerCollider.isTrigger = true;
            building.scene = scene;

            go.layer = (int)GameLayer.Building;
            return building;
        }
        #region 序列化与反序列化
        /// <summary>
        /// 反序列化过程1：添加一个蓝图点到建筑中
        /// </summary>
        public virtual void Deserialize_AddBuilding_Pixel(Building_Pixel building_Pixel)
        {
            var index = new Index(building_Pixel.offset, building_Pixel.blockType);
            AllPixel.Add(index, building_Pixel);
            AllUnloadPixel.Add(index);
        }
        /// <summary>
        /// 反序列化过程2：将一个蓝图点由未加载状态改为已加载状态
        /// </summary>
        public virtual void Deserialize_PixelSwitch(Building_Pixel building_Pixel, Pixel pixel)
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
        }
        public virtual string Serialize()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"|{Name}|{global.x}:{global.y}|{Size.x}:{Size.y}");
            sb.Append($"|{(CanByBroken ? 'T' : 'F')}|");
            Dictionary<string, int> typeNameDic = new Dictionary<string, int>();
            Dictionary<string, int> colorNameDic = new Dictionary<string, int>();
            // |类型索引:颜色索引,类型索引:颜色索引| 
            foreach (var building_Pixel in AllPixel.Values)
            {
                if (typeNameDic.TryGetValue(building_Pixel.typeInfo.typeName, out int typeNameIndex) == false)
                {
                    typeNameIndex = typeNameDic.Count;
                    typeNameDic.Add(building_Pixel.typeInfo.typeName, typeNameIndex);
                }
                if (colorNameDic.TryGetValue(building_Pixel.colorInfo.colorName, out int colorNameIndex) == false)
                {
                    colorNameIndex = colorNameDic.Count;
                    colorNameDic.Add(building_Pixel.colorInfo.colorName, colorNameIndex);
                }
                sb.Append($"{typeNameIndex}:{colorNameIndex}:{building_Pixel.offset.x}:{building_Pixel.offset.y}:{(int)building_Pixel.blockType},");
            }
            if (sb[sb.Length - 1] == '|') sb.Append('|');
            else sb[sb.Length - 1] = '|';
            // |类型索引,类型|
            foreach (var kv in typeNameDic)
            {
                sb.Append($"{kv.Value}:{kv.Key},");
            }
            if (sb[sb.Length - 1] == '|') sb.Append('|');
            else sb[sb.Length - 1] = '|';
            // |颜色索引,颜色|
            foreach (var kv in colorNameDic)
            {
                sb.Append($"{kv.Value}:{kv.Key},");
            }
            //sb[sb.Length - 1] = '|';
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
        public virtual void Deserialize(string text, int lastDelimiter)
        {
            Stack<char> stack = new Stack<char>();
            StringBuilder sb = new StringBuilder();
            Dictionary<int, string> colorNameDic = new Dictionary<int, string>();
            Dictionary<int, string> typeNameDic = new Dictionary<int, string>();
            int index = -1;
            string typeName = null;
            string colorName = null;
            #region 使用的颜色和类型
            Deserialize_Data(text, (num) =>
            {
                if (num == 0) colorName = StackToString(stack, ref sb);
                else if (num == 1) index = StackToInt(stack);
            },
            () => { colorNameDic.Add(index, colorName); },
                ref lastDelimiter, ref stack);
            Deserialize_Data(text, (num) =>
            {
                if (num == 0) typeName = StackToString(stack, ref sb);
                else if (num == 1) index = StackToInt(stack);
            },
            () => { typeNameDic.Add(index, typeName); },
            ref lastDelimiter, ref stack);
            #endregion
            #region AllPixel
            Vector2Int offset = new Vector2Int();
            BlockBase.BlockType blockType = BlockBase.BlockType.Block;
            Deserialize_Data(text, (num) =>
            {
                if (num == 0) blockType = (BlockBase.BlockType)StackToInt(stack);
                else if (num == 1) offset.y = StackToInt(stack);
                else if (num == 2) offset.x = StackToInt(stack);
                else if (num == 3) colorName = colorNameDic[StackToInt(stack)];
                else if (num == 4) typeName = typeNameDic[StackToInt(stack)];
            },
            () => { Deserialize_AddBuilding_Pixel(Building_Pixel.TakeOut().Init(Pixel.GetPixelTypeInfo(typeName), BlockMaterial.GetPixelColorInfo(colorName), offset, blockType)); },
            ref lastDelimiter, ref stack);
            #endregion
            #region CanByBroken
            Deserialize_Data(text, (num) => { CanByBroken = stack.Peek() == 'T' ? true : false; }, null, ref lastDelimiter, ref stack);
            #endregion
            #region Size  global  Name
            Vector2Int vector2Int = new Vector2Int();
            Deserialize_Data(text, (num) =>
            {
                if (num == 0) vector2Int.y = StackToInt(stack);
                else if (num == 1) vector2Int.x = StackToInt(stack);
            },
            () => { Size = vector2Int; }, ref lastDelimiter, ref stack);

            Deserialize_Data(text, (num) =>
            {
                if (num == 0) vector2Int.y = StackToInt(stack);
                else if (num == 1) vector2Int.x = StackToInt(stack);
            },
            () => { global = vector2Int; }, ref lastDelimiter, ref stack);

            Deserialize_Data(text, (num) => { Name = StackToString(stack, ref sb); }, null, ref lastDelimiter, ref stack);
            TriggerCollider.size = (Vector2)Size * Pixel.Size;
            TriggerCollider.offset = TriggerCollider.size / 2f;
            TriggerCollider.transform.position = Block.GlobalToWorld(global);
            #endregion
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
            spriteRenderer.transform.position = Block.GlobalToWorld(global - Vector2Int.one);
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

        public void Delete()
        {
            scene.DeleteBuilding(GUID);
        }
    }
}

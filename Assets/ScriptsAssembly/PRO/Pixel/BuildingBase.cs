using PRO.Disk;
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
        public string GUID;
        public string Name;
        public Dictionary<Vector2Int, Building_Pixel> AllPixel = new Dictionary<Vector2Int, Building_Pixel>();
        private Dictionary<Vector2Int, Building_Pixel> AllSurvivalPixel = new Dictionary<Vector2Int, Building_Pixel>();
        private Dictionary<Vector2Int, Building_Pixel> AllDeathPixel = new Dictionary<Vector2Int, Building_Pixel>();
        private Dictionary<Vector2Int, Building_Pixel> AllUnloadPixel = new Dictionary<Vector2Int, Building_Pixel>();
        public Vector2Int global;
        public Vector2Int Size;
        public BoxCollider2D TriggerCollider;

        /// <summary>
        /// Prospect or Background 在前景还是在背景  true为前景
        /// </summary>
        public bool PorB = true;
        /// <summary>
        /// 这个建筑是否可以被破坏
        /// </summary>
        public bool CanByBroken = true;
        /// <summary>
        /// 将这个点从存活与死亡两种状态转换（是否和蓝图对应，对应代表存活，反之死亡）,子类实现以产生相应的行为
        /// </summary>
        public virtual void PixelSwitch(Building_Pixel building_Pixel, Pixel pixel)
        {
            Building_Pixel.State oldState = building_Pixel.GetState();
            building_Pixel.Pixel = pixel;
            pixel.building = this;
            Building_Pixel.State newState = building_Pixel.GetState();
            if (oldState == newState) return;
            if (newState == Building_Pixel.State.Death)
            {
                AllSurvivalPixel.Remove(building_Pixel.Offset);
                AllDeathPixel.Add(building_Pixel.Offset, building_Pixel);
            }
            else
            {
                AllDeathPixel.Remove(building_Pixel.Offset);
                AllSurvivalPixel.Add(building_Pixel.Offset, building_Pixel);
            }
            if (AllSurvivalPixel.Count <= 0)
            {
                SceneManager.Inst.NowScene.DeleteBuilding(GUID);
            }
        }
        public Building_Pixel GetBuilding_Pixel(Vector2Int globalPos) => AllPixel[globalPos - this.global];
        public abstract void Init();
        public static BuildingBase New(Type type, string guid)
        {
            if (typeof(BuildingBase).IsAssignableFrom(type) == false) return null;
            GameObject go = new GameObject(type.Name);
            BuildingBase building = (BuildingBase)go.AddComponent(type);
            building.GUID = guid;
            building.TriggerCollider = go.AddComponent<BoxCollider2D>();
            building.TriggerCollider.isTrigger = true;
            go.layer = 9;
            building.Init();
            return building;
        }
        #region 序列化与反序列化
        /// <summary>
        /// 反序列化过程：添加一个蓝图点到建筑中
        /// </summary>
        public void Deserialize_AddBuilding_Pixel(Building_Pixel building_Pixel)
        {
            AllPixel.Add(building_Pixel.Offset, building_Pixel);
            AllUnloadPixel.Add(building_Pixel.Offset, building_Pixel);
        }
        /// <summary>
        /// 反序列化过程：将一个蓝图点由未加载状态改为已加载状态
        /// </summary>
        public void Deserialize_PixelSwitch(Building_Pixel building_Pixel, Pixel pixel)
        {
            AllUnloadPixel.Remove(building_Pixel.Offset);
            building_Pixel.Pixel = pixel;
            pixel.building = this;
            Building_Pixel.State newState = building_Pixel.GetState();
            if (newState == Building_Pixel.State.Death)
                AllDeathPixel.Add(building_Pixel.Offset, building_Pixel);
            else
                AllSurvivalPixel.Add(building_Pixel.Offset, building_Pixel);
        }
        /// <summary>
        /// 卸载一个点，当所有点都被卸载将触发回调
        /// </summary>
        public void UnloadPixel(Pixel pixel, Action<BuildingBase> byUnloadAllPixelAction)
        {
            Building_Pixel building_Pixel = GetBuilding_Pixel(pixel.posG);
            if (building_Pixel.GetState() == Building_Pixel.State.Survival) AllSurvivalPixel.Remove(building_Pixel.Offset);
            else AllDeathPixel.Remove(building_Pixel.Offset);

            building_Pixel.Pixel = null;
            AllUnloadPixel.Add(building_Pixel.Offset, building_Pixel);
            if (AllUnloadPixel.Count == AllPixel.Count)
                byUnloadAllPixelAction?.Invoke(this);
        }
        public virtual string Serialize()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"|{Name}|{global.x}:{global.y}|{Size.x}:{Size.y}");
            sb.Append($"|{(PorB ? 'T' : 'F')}|{(CanByBroken ? 'T' : 'F')}|");
            Dictionary<string, int> typeNameDic = new Dictionary<string, int>();
            Dictionary<string, int> colorNameDic = new Dictionary<string, int>();
            // |类型索引:颜色索引,类型索引:颜色索引| 
            foreach (var building_Pixel in AllPixel.Values)
            {
                if (typeNameDic.TryGetValue(building_Pixel.TypeInfo.typeName, out int typeNameIndex) == false)
                {
                    typeNameIndex = typeNameDic.Count;
                    typeNameDic.Add(building_Pixel.TypeInfo.typeName, typeNameIndex);
                }
                if (colorNameDic.TryGetValue(building_Pixel.ColorInfo.colorName, out int colorNameIndex) == false)
                {
                    colorNameIndex = colorNameDic.Count;
                    colorNameDic.Add(building_Pixel.ColorInfo.colorName, colorNameIndex);
                }
                sb.Append($"{typeNameIndex}:{colorNameIndex}:{building_Pixel.Offset.x}:{building_Pixel.Offset.y},");
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
            Deserialize_Data(text, (num) =>
            {
                if (num == 0) offset.y = StackToInt(stack);
                else if (num == 1) offset.x = StackToInt(stack);
                else if (num == 2) colorName = colorNameDic[StackToInt(stack)];
                else if (num == 3) typeName = typeNameDic[StackToInt(stack)];
            },
            () => { Deserialize_AddBuilding_Pixel(new Building_Pixel(Pixel.GetPixelTypeInfo(typeName), BlockMaterial.GetPixelColorInfo(colorName), offset)); },
            ref lastDelimiter, ref stack);
            #endregion
            #region CanByBroken  PorB
            Deserialize_Data(text, (num) => { CanByBroken = stack.Peek() == 'T' ? true : false; }, null, ref lastDelimiter, ref stack);
            Deserialize_Data(text, (num) => { PorB = stack.Peek() == 'T' ? true : false; }, null, ref lastDelimiter, ref stack);
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

        public SpriteRenderer DrawSprite()
        {
            Texture2D texture = Texture2DPool.TakeOut(Size.x, Size.y);
            SpriteRenderer spriteRenderer = Texture2DPool.TakeOutSpriteRenderer();
            spriteRenderer.sprite = Texture2DPool.GetOnlySprite(texture);
            var data = texture.GetRawTextureData<float>();
            foreach (var building in AllSurvivalPixel.Values)
                data.DrawPixel(texture.width, building.Offset, building.ColorInfo.color);
            texture.Apply();
            spriteRenderer.transform.position = Block.GlobalToWorld(global);
            return spriteRenderer;
        }
    }
}

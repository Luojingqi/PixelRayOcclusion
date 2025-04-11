using PRO.DataStructure;
using PRO.Tool;
using System.Collections.Generic;
using System.Text;

namespace PRO.Disk.Scene
{
    /// <summary>
    /// 块数据存储到磁盘与从磁盘中加载的类
    /// </summary>
    public static class BlockToDiskEx
    {
        public static string ToDisk(BlockBase block, SceneEntity sceneEntity)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('|');
            Dictionary<string, int> typeNameDic = new Dictionary<string, int>();
            Dictionary<string, int> colorNameDic = new Dictionary<string, int>();
            Dictionary<string, int> buildingGuidDic = new Dictionary<string, int>();
            for (int y = 0; y < Block.Size.y; y++)
                for (int x = 0; x < Block.Size.x; x++)
                {
                    Pixel pixel = block.GetPixel(new Vector2Byte(x, y));
                    if (typeNameDic.TryGetValue(pixel.typeInfo.typeName, out int typeNameIndex) == false)
                    {
                        typeNameIndex = typeNameDic.Count;
                        typeNameDic.Add(pixel.typeInfo.typeName, typeNameIndex);
                    }
                    if (colorNameDic.TryGetValue(pixel.colorInfo.colorName, out int colorNameIndex) == false)
                    {
                        colorNameIndex = colorNameDic.Count;
                        colorNameDic.Add(pixel.colorInfo.colorName, colorNameIndex);
                    }
                    int buildingIndex = 0;
                    foreach (var building in pixel.buildingSet)
                    {
                        if (buildingGuidDic.TryGetValue(building.GUID, out buildingIndex) == false)
                        {
                            buildingIndex = buildingGuidDic.Count + 1;
                            buildingGuidDic.Add(building.GUID, buildingIndex);
                        }
                        sb.Append($"{buildingIndex}:");
                    }
                    sb.Append($"{typeNameIndex}:{colorNameIndex},");
                }
            sb[sb.Length - 1] = '|';
            foreach (var kv in typeNameDic)
                sb.Append($"{kv.Value}:{kv.Key},");
            sb[sb.Length - 1] = '|';
            foreach (var kv in colorNameDic)
                sb.Append($"{kv.Value}:{kv.Key},");
            sb[sb.Length - 1] = '|';
            foreach (var kv in buildingGuidDic)
                sb.Append($"{kv.Value}:{kv.Key},");
            if (sb[sb.Length - 1] == ',') sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }

        public static void ToRAM(string blockText, BlockBase block, SceneEntity sceneEntity)
        {
            Stack<char> stack = new Stack<char>();
            StringBuilder sb = new StringBuilder();
            Dictionary<int, string> buildingGuidDic = new Dictionary<int, string>();
            Dictionary<int, string> colorNameDic = new Dictionary<int, string>();
            Dictionary<int, string> typeNameDic = new Dictionary<int, string>();
            int lastDelimiter = blockText.Length;
            int index = 0;
            #region buildingGuidDic
            string buildingGuid = null;
            JsonTool.Deserialize_Data(blockText, (num) =>
            {
                if (num == 0) buildingGuid = JsonTool.StackToString(stack, ref sb);
                else if (num == 1) index = JsonTool.StackToInt(stack);
            },
            () =>
            {
                if (buildingGuid != null && buildingGuid.Length > 0)
                {
                    buildingGuidDic.Add(index, buildingGuid);
                    if (sceneEntity.GetBuilding(buildingGuid) == null)
                        SceneManager.Inst.AddMainThreadEvent_Clear_WaitInvoke_Lock(() => sceneEntity.LoadBuilding(buildingGuid));
                }
            },
            ref lastDelimiter, ref stack);
            #endregion
            #region colorNameDic  typeNameDic
            string typeName = null;
            string colorName = null;
            JsonTool.Deserialize_Data(blockText, (num) =>
            {
                if (num == 0) colorName = JsonTool.StackToString(stack, ref sb);
                else if (num == 1) index = JsonTool.StackToInt(stack);
            },
            () => { colorNameDic.Add(index, colorName); },
            ref lastDelimiter, ref stack);
            JsonTool.Deserialize_Data(blockText, (num) =>
            {
                if (num == 0) typeName = JsonTool.StackToString(stack, ref sb);
                else if (num == 1) index = JsonTool.StackToInt(stack);
            },
            () => { typeNameDic.Add(index, typeName); },
            ref lastDelimiter, ref stack);
            #endregion
            List<BuildingBase> buildingList = new List<BuildingBase>();
            int pixelNum = Block.Size.x * Block.Size.y - 1;
            JsonTool.Deserialize_Data(blockText, (num) =>
            {
                int dicIndex = JsonTool.StackToInt(stack);
                if (num == 0) colorName = colorNameDic[dicIndex];
                else if (num == 1) typeName = typeNameDic[dicIndex];
                else
                {
                    var building = sceneEntity.GetBuilding(buildingGuidDic[dicIndex]);
                    if (building != null) buildingList.Add(building);
                }
            },
            () =>
            {
                int x = pixelNum % Block.Size.x;
                int y = pixelNum / Block.Size.y;
                Pixel pixel = Pixel.TakeOut(typeName, colorName, new(x, y));
                block.SetPixel(pixel, true, false, false);
                foreach (var building in buildingList)
                {
                    pixel.buildingSet.Add(building);
                    building.Deserialize_PixelSwitch(building.GetBuilding_Pixel(pixel.posG, pixel.blockBase.blockType), pixel);
                }
                buildingList.Clear();
                --pixelNum;
            },
            ref lastDelimiter, ref stack);
        }
    }
}

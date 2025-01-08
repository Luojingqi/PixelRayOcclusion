using PRO.DataStructure;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PRO.Disk.Scene
{
    /// <summary>
    /// 块数据存储到磁盘与从磁盘中加载的类
    /// </summary>
    public static class BlockToDiskEx
    {
        public static string ToDisk(BlockBase block)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('|');
            Dictionary<string, int> typeNameDic = new Dictionary<string, int>();
            Dictionary<string, int> colorNameDic = new Dictionary<string, int>();
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
                    sb.Append($"{typeNameIndex}:{colorNameIndex},");
                }
            sb[sb.Length - 1] = '|';
            foreach (var kv in typeNameDic)
            {
                sb.Append($"{kv.Value}:{kv.Key},");
            }
            sb[sb.Length - 1] = '|';
            foreach (var kv in colorNameDic)
            {
                sb.Append($"{kv.Value}:{kv.Key},");
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        public static void ToRAM(string blockText, BlockBase block)
        {
            Stack<char> stack = new Stack<char>();
            StringBuilder sb = new StringBuilder();
            Dictionary<int, string> colorNameDic = new Dictionary<int, string>();
            Dictionary<int, string> typeNameDic = new Dictionary<int, string>();
            int lastDelimiter = blockText.Length;
            Deserialize_NameDic(blockText, ref lastDelimiter, ref stack, ref colorNameDic, ref sb);
            Deserialize_NameDic(blockText, ref lastDelimiter, ref stack, ref typeNameDic, ref sb);
            Deserialize_Pixel(blockText, typeNameDic, colorNameDic, ref block, ref lastDelimiter, ref stack);
        }


        private static void Deserialize_NameDic(string text, ref int lastDelimiter, ref Stack<char> stack, ref Dictionary<int, string> nameDic, ref StringBuilder sb)
        {
            stack.Clear();
            int valueNum = 0;
            int index = -1;
            string name = null;
            for (int i = lastDelimiter - 1; i >= 0; i--)
            {
                char c = text[i];
                if (c == '|' || c == ',' || c == ':')
                {
                    switch (valueNum++)
                    {
                        case 0: name = StackToString(stack, ref sb); break;
                        case 1: index = StackToInt(stack); break;
                    }
                    if (c == '|' || c == ',')
                    {
                        nameDic.Add(index, name);
                        stack.Clear();
                        valueNum = 0;
                        if (c == '|')
                        {
                            lastDelimiter = i; return;
                        }
                    }
                }
                else stack.Push(c);
            }
        }
        private static void Deserialize_Pixel(string text, Dictionary<int, string> typeNameDic, Dictionary<int, string> colorNameDic, ref BlockBase block, ref int lastDelimiter, ref Stack<char> stack)
        {
            stack.Clear();
            int valueNum = 0;
            string typeName = null;
            string colorName = null;
            int pixelNum = Block.Size.x * Block.Size.y - 1;
            for (int i = lastDelimiter - 1; i >= 0; i--)
            {
                char c = text[i];
                if (c == '|' || c == ',' || c == ':')
                {
                    switch (valueNum++)
                    {
                        case 0: colorName = colorNameDic[StackToInt(stack)]; break;
                        case 1: typeName = typeNameDic[StackToInt(stack)]; break;
                    }
                    if (c == '|' || c == ',')
                    {
                        int y = pixelNum / Block.Size.y;
                        int x = pixelNum % Block.Size.x;
                        block.SetPixel(Pixel.TakeOut(typeName, colorName, new(x, y)), false, false);
                        --pixelNum;
                        stack.Clear();
                        valueNum = 0;
                        if (c == '|')
                        {
                            lastDelimiter = i; return;
                        }
                    }
                }
                else stack.Push(c);
            }
        }

        private static int StackToInt(Stack<char> stack)
        {
            int ret = 0;
            while (stack.Count > 0)
                ret += (stack.Pop() - '0') * (int)Mathf.Pow(10, stack.Count);
            return ret;
        }
        private static string StackToString(Stack<char> stack, ref StringBuilder sb)
        {
            sb.Clear();
            while (stack.Count > 0)
                sb.Append(stack.Pop());
            return sb.ToString();
        }
    }
}

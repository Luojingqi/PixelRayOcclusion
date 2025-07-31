using PRO.DataStructure;
using System;
using System.Text;
using UnityEngine;

namespace PRO.Tool.Serialize.String
{
    public static class StringTool
    {
        public static int ToInt(this StringBuilder sb)
        {
            int ret = 0;
            int symbol = 1;
            int index = 0;
            int pow = 0;
            if (sb[0] == '-') { symbol = -1; index++; }
            while (index < sb.Length)
                ret += (sb[index++] - '0') * (int)Mathf.Pow(10, sb.Length - ++pow);
            return ret * symbol;
        }
        public static float ToFloat(this StringBuilder sb)
        {
            int index = 0;
            int length = sb.Length;

            // 处理符号
            float sign = 1f;
            if (sb[index] == '-')
            {
                sign = -1f;
                index++;
            }

            int l = 0;
            float r = 0f;
            int bit = 0;
            while (index < length)
            {
                char c = sb[index++];
                if (c != '.')
                    l = l * 10 + (c - '0');
                else
                    while (index < length)
                    {
                        r = r * 10 + (sb[index++] - '0');
                        bit++;
                    }
            }

            float ret = l + r / Mathf.Pow(10, bit);
            ret *= sign;
            return ret;
        }
        public static bool ToBool(this StringBuilder sb)
        {
            return sb[0] == 't';
        }

        public struct Pointer
        {
            public int offset;
            public int objectNum;
            public string tag;
        }


        //public static int Deserialize_嵌套(string text, Func<Pointer, int> action, int offset)
        //{

        //    int objectNum = 0;
        //    while (offset++ < text.Length)
        //    {
        //        if (text[offset] == '{')
        //        {
        //            if (text[offset + 1] == '"')
        //            {
        //                var tagSB = SetPool.TakeOut_SB();
        //                offset += 2;
        //                while (true)
        //                {
        //                    char c = text[offset++];
        //                    if (c != '"')
        //                        tagSB.Append(c);
        //                    else
        //                        break;
        //                }
        //                offset = action.Invoke(new Pointer()
        //                {
        //                    offset = offset,
        //                    objectNum = objectNum++,
        //                    tag = tagSB.PutInReturn()
        //                });
        //            }
        //            else
        //            {
        //                offset = action.Invoke(new Pointer()
        //                {
        //                    offset = offset,
        //                    objectNum = objectNum++,
        //                    tag = null
        //                });
        //            }
        //        }
        //        else if (text[offset] == '}')
        //        {
        //            break;
        //        }
        //    }
        //    return offset;
        //}

        //public static int Deserialize_平行(string text, Action<ListPackage<StringBuilder>> action, int offset)
        //{
        //    var value = SetPool.TakeOut_SB();
        //    var list = SetPool.TakeOut_List<StringBuilder>();
        //    while (offset++ < text.Length)
        //    {
        //        char c = text[offset];
        //        switch (c)
        //        {
        //            case ',':
        //                list.Add(value);
        //                value = SetPool.TakeOut_SB();
        //                break;
        //            case '}':
        //                list.Add(value);
        //                action.Invoke(list);
        //                foreach (var sb in list)
        //                    sb.PutIn();
        //                list.PutIn();
        //                return offset;
        //            default:
        //                value.Append(c);
        //                break;
        //        }
        //    }
        //    return offset;
        //}
        public static StringBuilder AddStart(this StringBuilder sb) => sb.Append('{');
        public static StringBuilder AddStart(this StringBuilder sb, string key)
        {
            sb.Append('{');
            sb.Append('"');
            sb.Append(key);
            sb.Append('"');
            sb.Append(':');
            return sb;
        }
        public static StringBuilder AddEnd(this StringBuilder sb) => sb.Append('}');
        public static StringBuilder Add(this StringBuilder sb, bool value) => sb.Append(value ? 't' : 'f');
        public static StringBuilder Add(this StringBuilder sb, float value, int bit = -1)
        {
            if (value < 0)
            {
                sb.Append('-');
                value = -value;
            }

            int l = (int)value;
            float r = value - l;

            Span<char> span = stackalloc char[10];
            int lbit = 0;
            do
            {
                span[lbit++] = (char)(l % 10 + '0');
                l /= 10;
            } while (l > 0);
            if (bit == -1)
            {
                bit = 7 - lbit;
            }
            while (lbit-- > 0)
                sb.Append(span[lbit]);
            if (r > 0)
            {
                sb.Append('.');
                while (r > 0 && bit-- > 0)
                {
                    r *= 10;
                    int nowL = (int)r;
                    sb.Append((char)('0' + nowL));
                    r -= nowL;
                }
            }
            return sb;
        }
        public static StringBuilder Add(this StringBuilder sb, int value)
        {
            if (value < 0)
            {
                sb.Append('-');
                value = -value;
            }
            Span<char> span = stackalloc char[10];
            int bit = 0;
            do
            {
                span[bit++] = (char)(value % 10 + '0');
                value /= 10;
            } while (value > 0);
            while (bit-- > 0)
                sb.Append(span[bit]);
            return sb;
        }
        public static StringBuilder Add(this StringBuilder sb, Vector2 value)
        {
            sb.Add(value.x).Append(',').Add(value.y);
            return sb;
        }
        public static StringBuilder Add(this StringBuilder sb, Vector2Int value)
        {
            sb.Add(value.x).Append(',').Add(value.y);
            return sb;
        }
        public static StringBuilder Add(this StringBuilder sb, Vector2Byte value)
        {
            sb.Add(value.x).Append(',').Add(value.y);
            return sb;
        }

        public static StringBuilder Add(this StringBuilder sb, Vector3 value)
        {
            sb.Add(value.x).Append(',').Add(value.y).Append(',').Add(value.z);
            return sb;
        }
        public static StringBuilder Add(this StringBuilder sb, Vector3Int value)
        {
            sb.Add(value.x).Append(',').Add(value.y).Append(',').Add(value.z);
            return sb;
        }
        public static StringBuilder Add(this StringBuilder sb, Quaternion value)
        {
            sb.Add(value.x).Append(',').Add(value.y).Append(',').Add(value.z).Append(',').Add(value.w);
            return sb;
        }

        public static StringBuilder Add(this StringBuilder sb, StringBuilder value)
        {
            for (int i = 0; i < value.Length; i++)
                sb.Append(value[i]);
            return sb;
        }
        //public static StringBuilder Add_PutIn(this StringBuilder sb, StringBuilder value)
        //{
        //    for (int i = 0; i < value.Length; i++)
        //        sb.Append(value[i]);
        //    value.PutIn();
        //    return sb;
        //}
    }
}

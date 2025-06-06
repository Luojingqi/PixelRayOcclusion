﻿using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ExcelTool
{
    internal static class TypeNameStandardizing
    {
        //命名空间
        public static string _System = "System.";
        public static string _UnityEngine = "UnityEngine.";
        public static string _SystemCollectionsGeneric = "System.Collections.Generic.";
        public static string _UnityMathematics = "Unity.Mathematics.";

        /// <summary>
        /// 从Excel中读取的类型字符串转换为带命名空间的标准名
        /// </summary>
        /// <param name="typeString"></param>
        /// <returns></returns>
        public static string Run(string typeString)
        {
            StringBuilder typeNameSB = new StringBuilder();
            StringBuilder setSB = new StringBuilder();
            int index = 0;
            while (index < typeString.Length)
            {
                char c = typeString[index];
                if (c == '[')
                    break;
                typeNameSB.Append(c);
                index++;
            }
            while (index < typeString.Length)
                setSB.Append(typeString[index++]);


            string typeName = typeNameSB.ToString();
            switch (typeName.ToLower())
            {
                case "string": typeName = $"{_System}String"; break;
                case "int": typeName = $"{_System}Int32"; break;
                case "bool": typeName = $"{_System}Boolean"; break;
                case "double": typeName = $"{_System}Double"; break;
                case "float": typeName = $"{_System}Single"; break;
                case "vector3": typeName = $"{_UnityEngine}Vector3"; break;
                case "vector4": typeName = $"{_UnityEngine}Vector4"; break;
                case "vector2int": typeName = $"{_UnityEngine}Vector2Int"; break;
                case "vector3int": typeName = $"{_UnityEngine}Vector3Int"; break;
                case "int4": typeName = $"{_UnityMathematics}int4"; break;
                case "uint4": typeName = $"{_UnityMathematics}uint4"; break;
                case "color32": typeName = $"{_UnityEngine}Color32"; break;
                default: typeName = "未定义"; Debug.Log(typeString.ToLower()); break;
            }
            switch (setSB.ToString().ToLower())
            {
                case "": break;
                case "[]": typeName = $"{typeName}[]"; break;
                case "[list]": typeName = $"{_SystemCollectionsGeneric}List<{typeName}>"; break;
                case "[hashset]":typeName = $"{_SystemCollectionsGeneric}HashSet<{typeName}>"; break;
            }
            return typeName;
        }
    }
}

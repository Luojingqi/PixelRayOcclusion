﻿using System;
using Unity.Plastic.Newtonsoft.Json.Linq;

namespace ExcelTool
{
    internal static class ToJsonData
    {
        public static void RunObject(string tpyeName, string key, string value, ref JObject jsonData)
        {
            switch (tpyeName.ToLower())
            {
                default:
                    jsonData[key] = value;
                    break;
                case "int":
                    jsonData[key] = Convert.ToInt32(value);
                    break;
                case "bool":
                    jsonData[key] = Convert.ToBoolean(value);
                    break;
                case "string":
                    jsonData[key] = value;
                    break;
                case "float":
                    jsonData[key] = Convert.ToDouble(value);
                    break;
                case "double":
                    jsonData[key] = Convert.ToDouble(value);
                    break;
                case "vector3":
                    string[] strsVector3 = value.Split(',', '，');
                    JObject jsonDataVector3 = new JObject();
                    jsonDataVector3["x"] = Convert.ToDouble(strsVector3[0]);
                    jsonDataVector3["y"] = Convert.ToDouble(strsVector3[1]);
                    jsonDataVector3["z"] = Convert.ToDouble(strsVector3[2]);
                    jsonData[key] = jsonDataVector3;
                    break;
                case "vector4":
                    string[] strsVector4 = value.Split(',', '，');
                    JObject jsonDataVector4 = new JObject();
                    jsonDataVector4["x"] = Convert.ToDouble(strsVector4[0]);
                    jsonDataVector4["y"] = Convert.ToDouble(strsVector4[1]);
                    jsonDataVector4["z"] = Convert.ToDouble(strsVector4[2]);
                    jsonDataVector4["w"] = Convert.ToDouble(strsVector4[3]);
                    jsonData[key] = jsonDataVector4;
                    break;
                case "vector2int":
                    string[] strsVector2Int = value.Split(',', '，');
                    JObject jsonDataVector2Int = new JObject();
                    jsonDataVector2Int["x"] = Convert.ToInt32(strsVector2Int[0]);
                    jsonDataVector2Int["y"] = Convert.ToInt32(strsVector2Int[1]);
                    jsonData[key] = jsonDataVector2Int;
                    break;
                case "vector3int":
                    string[] strsVector3Int = value.Split(',', '，');
                    JObject jsonDataVector3Int = new JObject();
                    jsonDataVector3Int["x"] = Convert.ToDouble(strsVector3Int[0]);
                    jsonDataVector3Int["y"] = Convert.ToDouble(strsVector3Int[1]);
                    jsonDataVector3Int["z"] = Convert.ToDouble(strsVector3Int[2]);
                    jsonData[key] = jsonDataVector3Int;
                    break;
                case "int4":
                    string[] strsInt4 = value.Split(',', '，');
                    JObject jsonDataInt4 = new JObject();
                    jsonDataInt4["x"] = Convert.ToInt32(strsInt4[0]);
                    jsonDataInt4["y"] = Convert.ToInt32(strsInt4[1]);
                    jsonDataInt4["z"] = Convert.ToInt32(strsInt4[2]);
                    jsonDataInt4["w"] = Convert.ToInt32(strsInt4[3]);
                    jsonData[key] = jsonDataInt4;
                    break;
                case "uint4":
                    string[] strsUInt4 = value.Split(',', '，');
                    JObject jsonDataUInt4 = new JObject();
                    jsonDataUInt4["x"] = Convert.ToUInt32(strsUInt4[0]);
                    jsonDataUInt4["y"] = Convert.ToUInt32(strsUInt4[1]);
                    jsonDataUInt4["z"] = Convert.ToUInt32(strsUInt4[2]);
                    jsonDataUInt4["w"] = Convert.ToUInt32(strsUInt4[3]);
                    jsonData[key] = jsonDataUInt4;
                    break;
                case "color32":
                    string[] strsColor32 = value.Split(',', '，');
                    if (strsColor32.Length < 3) break;
                    JObject jsonDataColor32 = new JObject();
                    jsonDataColor32["r"] = Convert.ToUInt32(strsColor32[0]);
                    jsonDataColor32["g"] = Convert.ToUInt32(strsColor32[1]);
                    jsonDataColor32["b"] = Convert.ToUInt32(strsColor32[2]);
                    if (strsColor32.Length >= 4) jsonDataColor32["a"] = Convert.ToUInt32(strsColor32[3]);
                    jsonData[key] = jsonDataColor32;
                    break;

            }
        }

        public static void RunSet(string setName, string tpyeName, string key, string value, ref JArray array)
        {
            JObject jObject = new JObject();
            switch (setName.ToLower())
            {
                case "[]":
                case "[list]":
                case "[hashset]":
                    RunObject(tpyeName, key, value, ref jObject); break;
        }
            array.Add(jObject[key]);
        }
    }
}

using LitJson;
using System;

namespace ExcelTool
{
    internal static class ToJsonData
    {
        public static void Run(string tpye, string key, string value, ref JsonData jsonData, bool IsList = false)
        {
            switch (tpye.ToLower())
            {
                default:
                    if (!IsList)
                        jsonData[key] = value;
                    else
                        jsonData.Add(value);
                    break;
                case "int":
                    if (!IsList)
                        jsonData[key] = Convert.ToInt32(value);
                    else
                        jsonData.Add(Convert.ToInt32(value));
                    break;
                case "bool":
                    if (!IsList)
                        jsonData[key] = Convert.ToBoolean(value);
                    else
                        jsonData.Add(Convert.ToBoolean(value));
                    break;
                case "string":
                    if (!IsList)
                        jsonData[key] = value;
                    else
                        jsonData.Add(value);
                    break;
                case "float":
                    if (!IsList)
                        jsonData[key] = Convert.ToDouble(value);
                    else
                        jsonData.Add(Convert.ToSingle(value));
                    break;
                case "double":
                    if (!IsList)
                        jsonData[key] = Convert.ToDouble(value);
                    else
                        jsonData.Add(Convert.ToDouble(value));
                    break;
                case "vector3":
                    string[] strsVector3 = value.Split(',');
                    JsonData jsonDataVector3 = new JsonData();
                    jsonDataVector3["x"] = Convert.ToDouble(strsVector3[0]);
                    jsonDataVector3["y"] = Convert.ToDouble(strsVector3[1]);
                    jsonDataVector3["z"] = Convert.ToDouble(strsVector3[2]);
                    if (!IsList)
                        jsonData[key] = jsonDataVector3;
                    else
                        jsonData.Add(jsonDataVector3);
                    break;
                case "vector4":
                    string[] strsVector4 = value.Split(',');
                    JsonData jsonDataVector4 = new JsonData();
                    jsonDataVector4["x"] = Convert.ToDouble(strsVector4[0]);
                    jsonDataVector4["y"] = Convert.ToDouble(strsVector4[1]);
                    jsonDataVector4["z"] = Convert.ToDouble(strsVector4[2]);
                    jsonDataVector4["w"] = Convert.ToDouble(strsVector4[3]);
                    if (!IsList)
                        jsonData[key] = jsonDataVector4;
                    else
                        jsonData.Add(jsonDataVector4);
                    break;
                case "int4":
                    string[] strsInt4 = value.Split(',');
                    JsonData jsonDataInt4 = new JsonData();
                    jsonDataInt4["x"] = Convert.ToInt32(strsInt4[0]);
                    jsonDataInt4["y"] = Convert.ToInt32(strsInt4[1]);
                    jsonDataInt4["z"] = Convert.ToInt32(strsInt4[2]);
                    jsonDataInt4["w"] = Convert.ToInt32(strsInt4[3]);
                    if (!IsList)
                        jsonData[key] = jsonDataInt4;
                    else
                        jsonData.Add(jsonDataInt4);
                    break;
                case "uint4":
                    string[] strsUInt4 = value.Split(',');
                    JsonData jsonDataUInt4 = new JsonData();
                    jsonDataUInt4["x"] = Convert.ToUInt32(strsUInt4[0]);
                    jsonDataUInt4["y"] = Convert.ToUInt32(strsUInt4[1]);
                    jsonDataUInt4["z"] = Convert.ToUInt32(strsUInt4[2]);
                    jsonDataUInt4["w"] = Convert.ToUInt32(strsUInt4[3]);
                    if (!IsList)
                        jsonData[key] = jsonDataUInt4;
                    else
                        jsonData.Add(jsonDataUInt4);
                    break;
                case "color32":
                    string[] strsColor32 = value.Split(',');
                    JsonData jsonDataColor32 = new JsonData();
                    jsonDataColor32["r"] = Convert.ToUInt32(strsColor32[0]);
                    jsonDataColor32["g"] = Convert.ToUInt32(strsColor32[1]);
                    jsonDataColor32["b"] = Convert.ToUInt32(strsColor32[2]);
                    jsonDataColor32["a"] = Convert.ToUInt32(strsColor32[3]);
                    if (!IsList)
                        jsonData[key] = jsonDataColor32;
                    else
                        jsonData.Add(jsonDataColor32);
                    break;
            }
        }
    }
}

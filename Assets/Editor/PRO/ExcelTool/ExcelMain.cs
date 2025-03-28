using OfficeOpenXml;
using PRO.Tool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Unity.Plastic.Newtonsoft.Json;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEngine;
namespace ExcelTool
{
    public static class ExcelMain
    {
        public static void Start()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            #region 加载路径配置文件
            //string exePath = Directory.GetCurrentDirectory();
            string exePath = Application.streamingAssetsPath + @"\Excel\ExcelTool";
            JsonTool.LoadText(exePath + @"\path.json", out string pathText);
            Console.WriteLine($"读取path文件：{exePath + @"\path.json"}");
            var path = JsonTool.ToObject<JsonOnPath>(pathText);
            string xlsxPath = exePath + path.xlsxPath;
            string csPath = exePath + path.csPath;
            string jsonPath = exePath + path.jsonPath;
            DirectoryInfo root = new DirectoryInfo(xlsxPath);
            #endregion


            //HashSet<FileInfo> fileInfoHash = new HashSet<FileInfo>();
            //遍历Excel文件夹下所有xlsx文件
            foreach (var f in root.GetFiles())
            {
                string xlsxFileName = f.Name.Substring(0, f.Name.Length - f.Extension.Length);
                //Console.WriteLine($"文件：{f.Name}");
                //文件扩展名错误跳过
                if (f.Extension != ".xlsx" && f.Extension != ".xlsm"
                    || (xlsxFileName[0] == '~' && xlsxFileName[1] == '$')) continue;

                #region 加载Excel并存储

                ExcelPackage package = new ExcelPackage(xlsxPath + "\\" + f.Name);
                ExcelWorksheet worksheet0 = package.Workbook.Worksheets[0];

                Dictionary<string, string> Name_Type_Dic = new Dictionary<string, string>();
                Dictionary<string, string> Name_Note_Dic = new Dictionary<string, string>();
                for (int col = 2; col <= worksheet0.Dimension.Columns; col++)
                {
                    string attributeName = worksheet0.Cells[1, col].Value?.ToString();
                    string attributeTpye = worksheet0.Cells[2, col].Value?.ToString();
                    string attributeNotes = worksheet0.Cells[3, col].Value?.ToString();
                    try
                    {
                        Name_Type_Dic.Add(attributeName.Trim(' '), attributeTpye.Trim(' '));
                        if (attributeNotes != null)
                            Name_Note_Dic.Add(attributeName.Trim(' '), attributeNotes.Trim(' '));
                    }
                    catch
                    {
                        //跳过
                    }
                }
                #endregion

                #region 生成cs文件
                CreateCS.Run(csPath, xlsxFileName, "PRO.Disk", Name_Type_Dic, Name_Note_Dic);
                Console.WriteLine(f.Name + "\n生成cs文件成功");
                Debug.Log(f.Name + "\n生成cs文件成功");
                #endregion

                #region 生成Json文件
                //全工作表数据
                JArray jsonOnAllWorksheet = new JArray();
                Dictionary<string, JArray> jsonOnWorksheetDic = new Dictionary<string, JArray>();
                for (int workIndex = 0; workIndex < package.Workbook.Worksheets.Count; workIndex++)
                {

                    ExcelWorksheet workbook = package.Workbook.Worksheets[workIndex];
                    JArray jsonOnWorksheet = new JArray();
                    jsonOnWorksheetDic.Add(workbook.Name, jsonOnWorksheet);
                    //遍历每一行
                    for (int row = 4; row <= workbook.Dimension.Rows; row++)
                    {
                        //单行数据
                        JObject JObject = new JObject();
                        //遍历每一列
                        for (int col = 2; col <= 1 + Name_Type_Dic.Count; col++)
                        {
                            string attributeName = workbook.Cells[1, col].Value.ToString().Trim(' ');
                            string attributeTpye = Name_Type_Dic[attributeName];
                            if (workbook.Cells[row, col].Value == null)
                                continue;


                            string value = workbook.Cells[row, col].Value.ToString().Trim(' ');
                            if (value == "")
                                continue;

                            StringBuilder typeNameSB = new StringBuilder();
                            StringBuilder setSB = new StringBuilder();
                            int index = 0;
                            while (index < attributeTpye.Length)
                            {
                                char c = attributeTpye[index];
                                if (c == '[')
                                    break;
                                typeNameSB.Append(c);
                                index++;
                            }
                            while (index < attributeTpye.Length)
                                setSB.Append(attributeTpye[index++]);

                            string typeName = typeNameSB.ToString();
                            if (setSB.Length <= 0)
                                ToJsonData.RunObject(typeName, attributeName, value, ref JObject);
                            else
                            {
                                string setName = setSB.ToString();
                                //数组使用'|'分开
                                string[] strs = value.Split('|', '，');
                                JArray dataList = new JArray();
                                foreach (var str in strs)
                                {
                                    ToJsonData.RunSet(setName, typeName, attributeName, str.Trim(), ref dataList);
                                }
                                JObject[attributeName] = dataList;
                            }
                        }
                        if (JObject.Count > 0)
                        {
                            jsonOnWorksheet.Add(JObject);
                            jsonOnAllWorksheet.Add(JObject);
                        }
                    }
                }
                foreach (var sheet in jsonOnWorksheetDic)
                {
                    string JsonText = JsonConvert.SerializeObject(sheet.Value, Formatting.Indented);
                    JsonText = Regex.Unescape(JsonText);
                    using (StreamWriter sw = new StreamWriter($@"{jsonPath}\{xlsxFileName}^{sheet.Key}.json", false))
                    {
                        sw.Write(JsonText);
                        sw.Close();
                    }
                    Console.WriteLine($"生成Json文件成功{xlsxFileName}^{sheet.Key}");
                    Debug.Log($"生成Json文件成功{xlsxFileName}^{sheet.Key}");
                }
                using (StreamWriter sw = new StreamWriter($@"{jsonPath}\{xlsxFileName}.json", false))
                {
                    string JsonText = JsonConvert.SerializeObject(jsonOnAllWorksheet, Formatting.Indented);
                    sw.Write(Regex.Unescape(JsonText));
                    sw.Close();
                    Console.WriteLine($"生成Json文件成功{xlsxFileName}\n");
                    Debug.Log($"生成Json文件成功{xlsxFileName}\n");
                }
                #endregion

                //fileInfoHash.Add(f);
                package.Dispose();
            }

        }
    }
}
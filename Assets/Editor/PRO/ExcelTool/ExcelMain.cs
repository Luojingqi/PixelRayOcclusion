using LitJson;
using OfficeOpenXml;
using PRO.Tool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
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
                CreateCS.Run(csPath, xlsxFileName, "PRO.Data", Name_Type_Dic, Name_Note_Dic);
                Console.WriteLine(f.Name + "\n生成cs文件成功");
                Debug.Log(f.Name + "\n生成cs文件成功");
                #endregion

                #region 生成Json文件
                //全工作表数据
                JsonData jsonOnAllWorksheet = new JsonData();
                List<JsonData> jsonOnAllWorksheetList = new List<JsonData>();
                for (int workIndex = 0; workIndex < package.Workbook.Worksheets.Count; workIndex++)
                {
                    jsonOnAllWorksheetList.Add(new JsonData());
                    ExcelWorksheet workbook = package.Workbook.Worksheets[workIndex];
                    //遍历每一行
                    for (int row = 4; row <= workbook.Dimension.Rows; row++)
                    {
                        //单行数据
                        JsonData jsonData = new JsonData();
                        //遍历每一列
                        for (int col = 2; col <= 1 + Name_Type_Dic.Count; col++)
                        {
                            string attributeName = workbook.Cells[1, col].Value.ToString().Trim(' ');
                            Name_Type_Dic.TryGetValue(attributeName, out string attributeTpye);
                            if (workbook.Cells[row, col].Value == null)
                                continue;


                            string value = workbook.Cells[row, col].Value.ToString().Trim(' ');
                            if (value == "")
                                continue;
                            bool IsList = attributeTpye.Contains("[]");
                            if (IsList == false)
                                ToJsonData.Run(attributeTpye.ToLower(), attributeName, value, ref jsonData);
                            else
                            {
                                //数组使用'|'分开
                                string[] strs = value.Split('|', '，');
                                string tpyeName = attributeTpye.Split('[')[0].ToLower();
                                JsonData dataList = new JsonData();
                                foreach (var str in strs)
                                {
                                    ToJsonData.Run(tpyeName, attributeName, str.Trim(), ref dataList, true);
                                }
                                jsonData[attributeName] = dataList;
                            }
                        }
                        jsonOnAllWorksheetList[workIndex].Add(jsonData);
                        jsonOnAllWorksheet.Add(jsonData);
                    }
                }

                for (int i = 0; i < jsonOnAllWorksheetList.Count; i++)
                {
                    string JsonText = JsonMapper.ToJson(jsonOnAllWorksheetList[i]);
                    JsonText = Regex.Unescape(JsonText);
                    using (StreamWriter sw = new StreamWriter($@"{jsonPath}\{xlsxFileName}^{i}.json", false))
                    {
                        sw.Write(JsonText);
                        sw.Close();
                    }
                    Console.WriteLine($"生成Json文件成功{xlsxFileName}^{i}");
                    Debug.Log($"生成Json文件成功{xlsxFileName}^{i}");
                }
                using (StreamWriter sw = new StreamWriter($@"{jsonPath}\{xlsxFileName}.json", false))
                {
                    string JsonText = JsonMapper.ToJson(jsonOnAllWorksheet);
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
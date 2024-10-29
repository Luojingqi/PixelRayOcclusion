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

            #region ����·�������ļ�
            //string exePath = Directory.GetCurrentDirectory();
            string exePath = Application.streamingAssetsPath + @"\Excel\ExcelTool";
            JsonTool.LoadText(exePath + @"\path.json", out string pathText);
            Console.WriteLine($"��ȡpath�ļ���{exePath + @"\path.json"}");
            var path = JsonTool.ToObject<JsonOnPath>(pathText);
            string xlsxPath = exePath + path.xlsxPath;
            string csPath = exePath + path.csPath;
            string jsonPath = exePath + path.jsonPath;
            DirectoryInfo root = new DirectoryInfo(xlsxPath);
            #endregion


            //HashSet<FileInfo> fileInfoHash = new HashSet<FileInfo>();
            //����Excel�ļ���������xlsx�ļ�
            foreach (var f in root.GetFiles())
            {
                string xlsxFileName = f.Name.Substring(0, f.Name.Length - f.Extension.Length);
                //Console.WriteLine($"�ļ���{f.Name}");
                //�ļ���չ����������
                if (f.Extension != ".xlsx" && f.Extension != ".xlsm"
                    || (xlsxFileName[0] == '~' && xlsxFileName[1] == '$')) continue;

                #region ����Excel���洢

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
                        //����
                    }
                }
                #endregion

                #region ����cs�ļ�
                CreateCS.Run(csPath, xlsxFileName, "PRO.Data", Name_Type_Dic, Name_Note_Dic);
                Console.WriteLine(f.Name + "\n����cs�ļ��ɹ�");
                Debug.Log(f.Name + "\n����cs�ļ��ɹ�");
                #endregion

                #region ����Json�ļ�
                //ȫ����������
                JsonData jsonOnAllWorksheet = new JsonData();
                List<JsonData> jsonOnAllWorksheetList = new List<JsonData>();
                for (int workIndex = 0; workIndex < package.Workbook.Worksheets.Count; workIndex++)
                {
                    jsonOnAllWorksheetList.Add(new JsonData());
                    ExcelWorksheet workbook = package.Workbook.Worksheets[workIndex];
                    //����ÿһ��
                    for (int row = 4; row <= workbook.Dimension.Rows; row++)
                    {
                        //��������
                        JsonData jsonData = new JsonData();
                        //����ÿһ��
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
                                //����ʹ��'|'�ֿ�
                                string[] strs = value.Split('|', '��');
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
                    Console.WriteLine($"����Json�ļ��ɹ�{xlsxFileName}^{i}");
                    Debug.Log($"����Json�ļ��ɹ�{xlsxFileName}^{i}");
                }
                using (StreamWriter sw = new StreamWriter($@"{jsonPath}\{xlsxFileName}.json", false))
                {
                    string JsonText = JsonMapper.ToJson(jsonOnAllWorksheet);
                    sw.Write(Regex.Unescape(JsonText));
                    sw.Close();
                    Console.WriteLine($"����Json�ļ��ɹ�{xlsxFileName}\n");
                    Debug.Log($"����Json�ļ��ɹ�{xlsxFileName}\n");
                }
                #endregion

                //fileInfoHash.Add(f);
                package.Dispose();
            }

        }
    }
}
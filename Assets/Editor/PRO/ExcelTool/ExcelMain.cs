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
                CreateCS.Run(csPath, xlsxFileName, "PRO.Disk", Name_Type_Dic, Name_Note_Dic);
                Console.WriteLine(f.Name + "\n����cs�ļ��ɹ�");
                Debug.Log(f.Name + "\n����cs�ļ��ɹ�");
                #endregion

                #region ����Json�ļ�
                //ȫ����������
                JArray jsonOnAllWorksheet = new JArray();
                Dictionary<string, JArray> jsonOnWorksheetDic = new Dictionary<string, JArray>();
                for (int workIndex = 0; workIndex < package.Workbook.Worksheets.Count; workIndex++)
                {

                    ExcelWorksheet workbook = package.Workbook.Worksheets[workIndex];
                    JArray jsonOnWorksheet = new JArray();
                    jsonOnWorksheetDic.Add(workbook.Name, jsonOnWorksheet);
                    //����ÿһ��
                    for (int row = 4; row <= workbook.Dimension.Rows; row++)
                    {
                        //��������
                        JObject JObject = new JObject();
                        //����ÿһ��
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
                                //����ʹ��'|'�ֿ�
                                string[] strs = value.Split('|', '��');
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
                    Console.WriteLine($"����Json�ļ��ɹ�{xlsxFileName}^{sheet.Key}");
                    Debug.Log($"����Json�ļ��ɹ�{xlsxFileName}^{sheet.Key}");
                }
                using (StreamWriter sw = new StreamWriter($@"{jsonPath}\{xlsxFileName}.json", false))
                {
                    string JsonText = JsonConvert.SerializeObject(jsonOnAllWorksheet, Formatting.Indented);
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
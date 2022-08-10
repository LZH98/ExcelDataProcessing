using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;

namespace ExcelTest
{
    public class ExcelOperationUtil
    {
        public static void ReadExcelFile(string filePath, int sheetIndex, bool hasHeader)
        {
            Hashtable tableNameHashtable = new Hashtable();
            Hashtable dataFieldHashtable = new Hashtable();
            Hashtable displayNameHashtable = new Hashtable();
            FileInfo fileInfo = new FileInfo(filePath);
            int tableConfigEndIndex = 0;

            // 设置为非商业版证书
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            // Load Excel File Stream
            using (ExcelPackage excelPackage = new ExcelPackage(fileInfo))
            {
                var ws = excelPackage.Workbook.Worksheets[sheetIndex];

                if (!hasHeader)
                    return;

                ExcelRange range;
                ExcelRange valueRange;
                string tableNameValue = String.Empty;
                string dataFieldValue = string.Empty;
                List<JObject> result = new List<JObject>();
                List<JObject> displayList = new List<JObject>();
                for (int i = 1; i < ws.Columns.EndColumn; i++)
                {
                    range = ws.Cells[1, i];

                    #region 字段显示名配置解析

                    if (range.Text.Trim().ToLower() == "displayname")
                    {
                        for (int j = 2; j < ws.Rows.EndRow; j++)
                        {
                            valueRange = ws.Cells[j, i];
                            tableNameValue = valueRange?.Text;
                            if (!string.IsNullOrEmpty(tableNameValue))
                            {
                                if (tableNameValue.Trim().ToLower() == "process")
                                    tableConfigEndIndex = j;
                                else
                                    displayNameHashtable.Add(j.ToString(), tableNameValue);
                            }
                        }
                    }

                    #endregion
                    
                    #region 表名配置解析

                    else if (range.Text.Trim().ToLower() == "tablename")
                    {
                        for (int j = 2; j < ws.Rows.EndRow; j++)
                        {
                            valueRange = ws.Cells[j, i];
                            tableNameValue = valueRange?.Text;
                            if (!string.IsNullOrEmpty(tableNameValue))
                            {
                                if (tableNameValue.Trim().ToLower() == "process")
                                    tableConfigEndIndex = j;
                                else
                                    tableNameHashtable.Add(j.ToString(), tableNameValue);
                            }
                        }
                    }

                    #endregion
                    

                    #region 字段名称配置解析
                    else if (range.Text.Trim().ToLower() == "data")
                    {
                        for (int j = 2; j < tableConfigEndIndex; j++)
                        {
                            valueRange = ws.Cells[j, i];
                            dataFieldValue = valueRange?.Text;
                            if (!string.IsNullOrEmpty(dataFieldValue))
                            {
                                dataFieldHashtable.Add(j.ToString(), dataFieldValue);
                            }
                        }
                    }
                    #endregion

                    #region 输入内容解析
                    
                    else if (range.Text.Trim().ToLower() == "values")
                    {
                        JObject formData = new JObject();
                        JObject displayData = new JObject();
                        for (int j = 2; j < tableConfigEndIndex; j++)
                        {
                            valueRange = ws.Cells[j, i];
                            dataFieldValue = valueRange?.Text;
                            if (!string.IsNullOrEmpty(dataFieldValue))
                            {
                                string tableName = Convert.ToString(tableNameHashtable[j.ToString()]);
                                string dataFieldName = Convert.ToString(dataFieldHashtable[j.ToString()]);
                                string dispalyName = Convert.ToString(displayNameHashtable[j.ToString()]);
                                if (tableName.ToUpper().StartsWith("I_"))
                                {
                                    var mainTable = formData[tableName];
                                    var displayMainTable = displayData[tableName];
                                    if (mainTable == null)
                                    {
                                        mainTable = formData[tableName] = new JObject();
                                        displayMainTable = displayData[tableName] = new JObject();
                                    }
                                        
                                    mainTable[dataFieldName] = dataFieldValue;
                                    displayMainTable[dispalyName] = dataFieldValue;
                                }
                                else if (tableName.ToUpper().StartsWith("C_"))
                                {
                                    var detailTable = formData[tableName];
                                    var displayTable = displayData[tableName];
                                    if (detailTable == null)
                                    {
                                        detailTable = formData[tableName] = new JArray() { new JObject() };
                                        displayTable = displayData[tableName] = new JArray() { new JObject() };
                                    }

                                    detailTable[0][dataFieldName] = dataFieldValue;
                                    displayTable[0][dispalyName] = dataFieldValue;
                                }
                            }
                        }

                        if (formData != null)
                        {
                            Console.WriteLine($"成功处理一条流程数据，数据详情：{JsonConvert.SerializeObject(formData)}");
                            result.Add(formData);
                        }
                            
                        if(displayData != null)
                            displayList.Add(displayData);
                    }
                    
                    #endregion
                }

                Console.WriteLine(tableNameHashtable.Count);
                Console.WriteLine(tableNameHashtable["2"]);
                Console.WriteLine(dataFieldHashtable.Count);
                Console.WriteLine(dataFieldHashtable["2"]);
                Console.WriteLine(JsonConvert.SerializeObject(result));
                Console.WriteLine(JsonConvert.SerializeObject(displayList));
            }
        }
    }
}
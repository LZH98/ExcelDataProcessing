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
            Hashtable nodeInfoHashtable = new Hashtable();
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
                (int, int) processConfigRange = (int.MaxValue,0);
                (int, int) nodeConfigRange = (int.MaxValue,0);

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

                        // 查询流程配置
                        var processConfigFirst = 
                            ws.Cells.FirstOrDefault(f => 
                                f.Start.Column == i && f.Text.Trim().ToLower() == "process");
                        var processConfigLast = 
                            ws.Cells.FirstOrDefault(f => 
                                f.Start.Column == i && f.Text.Trim().ToLower() == "process" && f.Start.Row != processConfigFirst.Start.Row);

                        if (processConfigFirst != null && processConfigLast != null)
                        {
                            processConfigRange.Item1 = processConfigFirst.Start.Row;
                            processConfigRange.Item2 = processConfigLast.Start.Row;
                        }
                        // 查询流程配置

                        // 查询节点配置
                        var nodeConfigFirst = 
                            ws.Cells.FirstOrDefault(f=>
                                f.Start.Column == i && f.Text.Trim().ToLower() == "node");
                        var nodeConfigLast = 
                            ws.Cells.FirstOrDefault(f=>
                                f.Start.Column == i && f.Text.Trim().ToLower() == "node" && f.Start.Row != nodeConfigFirst.Start.Row);
                        if (nodeConfigFirst != null && nodeConfigLast != null)
                        {
                            nodeConfigRange.Item1 = nodeConfigFirst.Start.Row;
                            nodeConfigRange.Item2 = nodeConfigLast.Start.Row;

                            for (int index = nodeConfigRange.Item1; index < nodeConfigRange.Item2; index++)
                            {
                                var value = ws.Cells[index, i];
                                if (!string.IsNullOrEmpty(value?.Text))
                                {
                                    nodeInfoHashtable.Add(index.ToString(), value?.Text);
                                }
                            }
                        }
                        // 查询节点配置
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
                        for (int j = 2; j < ws.Rows.EndRow; j++)
                        {
                            valueRange = ws.Cells[j, i];
                            dataFieldValue = valueRange?.Text;
                            if (!string.IsNullOrEmpty(dataFieldValue) && j < tableConfigEndIndex)
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
                            else if(j > processConfigRange.Item1 && j < processConfigRange.Item2)
                            {
                                
                            }
                            else if(j > nodeConfigRange.Item1 && j < nodeConfigRange.Item2)
                            {
                                switch ((j- nodeConfigRange.Item1) % 3)
                                {
                                    case 1:
                                        Console.WriteLine($"节点：{nodeInfoHashtable[j.ToString()]} 审批人：{valueRange.Text}");
                                        break;
                                    case 2:
                                        Console.WriteLine($"节点：{nodeInfoHashtable[j.ToString()]} 审批意见：{valueRange.Text}");
                                        break;
                                    case 0:
                                        Console.WriteLine($"节点：{nodeInfoHashtable[j.ToString()]} 审批时间:{DateTime.Parse(valueRange.Text).ToString("yyyy-MM-dd HH:mm:ss")}");
                                        break;
                                }
                            }
                        }

                        if (formData != null)
                        {
                            Console.WriteLine($"成功处理一条流程数据，数据详情：{JsonConvert.SerializeObject(formData)}");
                            result.Add(formData);
                        }

                        if (displayData != null)
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
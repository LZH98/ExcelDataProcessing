using ExcelTest.Env;
using ExcelTest.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using SqlSugar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ExcelTest
{
    public class ExcelOperationUtil
    {
        public static async Task ReadExcelFile(string filePath, int sheetIndex, bool hasHeader, bool ischongf)
        {
            if (!hasHeader)
                return;

            FileInfo fileInfo = new FileInfo(filePath);

            // 设置为非商业版证书
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            // Load Excel File Stream
            using (ExcelPackage excelPackage = new ExcelPackage(fileInfo))
            {
                if (ischongf)
                {
                    foreach (ExcelWorksheet item in excelPackage.Workbook.Worksheets)
                    {
                        Console.WriteLine(
                            $"一共读取出{excelPackage.Workbook.Worksheets.Count}个Sheet，当前开始执行{item.Index + 1} 个Sheet");
                        await ReadExcelData(item);
                        Console.WriteLine($"{item.Index + 1}个Sheet执行完毕");
                    }
                }
                else
                {
                    ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[sheetIndex];
                    await ReadExcelData(workSheet);
                }
            }
        }

        private static async Task ReadExcelData(ExcelWorksheet workSheet)
        {
            Hashtable tableNameHashtable = new Hashtable();
            Hashtable dataFieldHashtable = new Hashtable();
            Hashtable displayNameHashtable = new Hashtable();
            Hashtable nodeInfoHashtable = new Hashtable();

            int tableConfigEndIndex = 0;

            string nodeName;
            ExcelRange range;
            ExcelRange valueRange;
            string tableNameValue = string.Empty;
            string dataFieldValue = string.Empty;
            List<JObject> displayList = new List<JObject>();
            (int, int) processConfigRange = (int.MaxValue, 0);
            (int, int) nodeConfigRange = (int.MaxValue, 0);

            List<ProcessDataInfo> processDataInfoList = new List<ProcessDataInfo>();
            List<ProcessNodeConfig> processNodeConfigList = new List<ProcessNodeConfig>();
            Dictionary<string, ProcessNodeConfig> nodesDic = new Dictionary<string, ProcessNodeConfig>();

            for (int i = 1; i < workSheet.Columns.EndColumn; i++)
            {
                range = workSheet.Cells[1, i];

                #region 字段显示名配置解析

                if (range.Text.Trim().ToLower() == "displayname")
                {
                    for (int j = 2; j < workSheet.Rows.EndRow; j++)
                    {
                        valueRange = workSheet.Cells[j, i];
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

                #endregion 字段显示名配置解析

                #region 表名配置解析

                else if (range.Text.Trim().ToLower() == "tablename")
                {
                    for (int j = 2; j < workSheet.Rows.EndRow; j++)
                    {
                        valueRange = workSheet.Cells[j, i];
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
                        workSheet.Cells.FirstOrDefault(f =>
                            f.Start.Column == i && f.Text.Trim().ToLower() == "process");
                    var processConfigLast =
                        workSheet.Cells.FirstOrDefault(f =>
                            f.Start.Column == i && f.Text.Trim().ToLower() == "process" &&
                            f.Start.Row != processConfigFirst.Start.Row);

                    if (processConfigFirst != null && processConfigLast != null)
                    {
                        processConfigRange.Item1 = processConfigFirst.Start.Row;
                        processConfigRange.Item2 = processConfigLast.Start.Row;
                    }
                    // 查询流程配置

                    // 查询节点配置
                    var nodeConfigFirst =
                        workSheet.Cells.FirstOrDefault(f =>
                            f.Start.Column == i && f.Text.Trim().ToLower() == "node");
                    var nodeConfigLast =
                        workSheet.Cells.FirstOrDefault(f =>
                            f.Start.Column == i && f.Text.Trim().ToLower() == "node" &&
                            f.Start.Row != nodeConfigFirst.Start.Row);
                    if (nodeConfigFirst != null && nodeConfigLast != null)
                    {
                        nodeConfigRange.Item1 = nodeConfigFirst.Start.Row;
                        nodeConfigRange.Item2 = nodeConfigLast.Start.Row;

                        for (int index = nodeConfigRange.Item1; index < nodeConfigRange.Item2; index++)
                        {
                            var value = workSheet.Cells[index, i];
                            if (!string.IsNullOrEmpty(value?.Text))
                            {
                                nodeInfoHashtable.Add(index.ToString(), value?.Text);
                            }
                        }
                    }
                    // 查询节点配置
                }

                #endregion 表名配置解析

                #region 字段名称配置解析

                else if (range.Text.Trim().ToLower() == "data")
                {
                    for (int j = 2; j < tableConfigEndIndex; j++)
                    {
                        valueRange = workSheet.Cells[j, i];
                        dataFieldValue = valueRange?.Text;
                        if (!string.IsNullOrEmpty(dataFieldValue))
                        {
                            dataFieldHashtable.Add(j.ToString(), dataFieldValue);
                        }
                    }
                }

                #endregion 字段名称配置解析

                #region 输入内容解析

                else if (range.Text.Trim().ToLower() == "values")
                {
                    JObject formData = new JObject();
                    JObject displayData = new JObject();
                    for (int j = 2; j < workSheet.Rows.EndRow; j++)
                    {
                        valueRange = workSheet.Cells[j, i];
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
                        else if (j > processConfigRange.Item1 && j < processConfigRange.Item2)
                        {
                        }
                        else if (j > nodeConfigRange.Item1 && j < nodeConfigRange.Item2)
                        {
                            nodeName = nodeInfoHashtable[j.ToString()].ToString();

                            if (!nodesDic.TryGetValue(nodeName, out ProcessNodeConfig nodeConfig))
                            {
                                nodesDic[nodeName] = new ProcessNodeConfig()
                                {
                                    ProcessName = workSheet.Name,
                                    NodeName = nodeName
                                };
                            }

                            switch ((j - nodeConfigRange.Item1) % 4)
                            {
                                case 1:
                                    nodesDic[nodeName].OwnerAccount = valueRange?.Text;
                                    Console.WriteLine($"节点：{nodeInfoHashtable[j.ToString()]} 审批人：{valueRange.Text}");
                                    break;

                                case 2:
                                    nodesDic[nodeName].Comment = valueRange?.Text;
                                    Console.WriteLine($"节点：{nodeInfoHashtable[j.ToString()]} 审批意见：{valueRange.Text}");
                                    break;
                                case 3:
                                    nodesDic[nodeName].CreateAt = Convert.ToDateTime(valueRange?.Text);
                                    Console.WriteLine(
                                        $"节点：{nodeInfoHashtable[j.ToString()]} 审批时间:{DateTime.Parse(valueRange.Text).ToString("yyyy-MM-dd HH:mm:ss")}");
                                    break;
                                case 0:
                                    
                                    nodesDic[nodeName].ApprovalAt = Convert.ToDateTime(valueRange?.Text);
                                    Console.WriteLine(
                                        $"节点：{nodeInfoHashtable[j.ToString()]} 审批时间:{DateTime.Parse(valueRange.Text).ToString("yyyy-MM-dd HH:mm:ss")}");
                                    break;
                            }
                        }
                    }

                    if (formData != null)
                    {
                        Console.WriteLine($"成功处理一条流程数据，数据详情：{JsonConvert.SerializeObject(formData)}");

                        string id = System.Guid.NewGuid().ToString("N");

                        ProcessDataInfo processDataInfo = new ProcessDataInfo()
                        {
                            ID = id,
                            ProcessName = workSheet.Name,
                            Comment = nodesDic["发起人"]?.Comment,
                            OwnerAccount = nodesDic["发起人"]?.OwnerAccount,
                            FormData = JsonConvert.SerializeObject(formData),
                            DescribeFormData = JsonConvert.SerializeObject(displayData),
                        };

                        processDataInfoList.Add(processDataInfo);

                        nodesDic.Values.ToList().ForEach(e =>
                        {
                            e.AID = id;
                            processNodeConfigList.Add(e);
                        });
                    }

                    //if (displayData != null)
                    //    displayList.Add(displayData);
                }

                #endregion 输入内容解析
            }

            SqlSugarClient dbContent = SqlSugarConfig.GetConnectOption();
            try
            {
                Console.WriteLine($"一共找到{processDataInfoList.Count}行数据，开始进行数据新增");
                dbContent.Ado.BeginTran();
                await dbContent.Insertable(processDataInfoList).ExecuteCommandAsync();
                await dbContent.Insertable(processNodeConfigList).ExecuteCommandAsync();
                dbContent.Ado.CommitTran();
                Console.WriteLine("表插入成功");
            }
            catch (Exception ex)
            {
                dbContent.RollbackTran();
            }

            //Console.WriteLine(tableNameHashtable.Count);
            //Console.WriteLine(tableNameHashtable["2"]);
            //Console.WriteLine(dataFieldHashtable.Count);
            //Console.WriteLine(dataFieldHashtable["2"]);
            //Console.WriteLine(JsonConvert.SerializeObject(processDataInfoList));
            //Console.WriteLine(JsonConvert.SerializeObject(processNodeConfigList));
        }
    }
}
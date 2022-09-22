using ExcelTest.Env;
using ExcelTest.Models;
using ExcelTest.Utils;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExcelTest.Models.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ExcelTest.Serivce
{
    public class ProcessPostService
    {
        public static async Task BatchPostAsync()
        {
            SysLogUtil sysLogUtil = new SysLogUtil(SysLogUtil.LogTagType.LogToFile);

            SqlSugarClient dbContent = SqlSugarConfig.GetConnectOption();
            List<ProcessDataInfo> processDataInfos = await dbContent.Queryable<ProcessDataInfo>()
                .Where(f => f.TaskID == 0)
                .ToListAsync();

            sysLogUtil.Trace($"本次一共需要发起{processDataInfos.Count}条流程");

            if (processDataInfos.Count < 1)
            {
                sysLogUtil.Trace("本次需要发起的流程条数为0，程序即将退出");
                Console.WriteLine("本次需要发起的流程条数为0，程序即将退出");
                return;
            }

            ProcessPost postData; 
            
            processDataInfos.ForEach(async e =>
            {
                postData = new ProcessPost()
                {
                    OwnerAccount = e.OwnerAccount,
                    FormData = JObject.Parse(e.FormData),
                    Action = e.Action,
                    ProcessName = e.ProcessName,
                    Comment = e.Comment
                };
               bool flag = await ProcessPostAsync(postData, e.ID);
               Console.WriteLine(flag);
            });
        }

        private static async Task<bool> ProcessPostAsync(ProcessPost postData, string id = null)
        {
            SysLogUtil logUtil = new SysLogUtil(SysLogUtil.LogTagType.LogToFile);
            RestApiUtil<ProcessPostResult> restApiUtil = new RestApiUtil<ProcessPostResult>();

            RequestParameter requestParameter = new RequestParameter();
            requestParameter.Url =
                $"{SystemConfig.GetSettingset("AppSetting:BPMHost")}/YZSoft/WebService/YZService.ashx";
            requestParameter.Parameters = new Dictionary<string, object>();
            requestParameter.Headers = new Dictionary<string, string>();
            requestParameter.Parameters.Add("Method", "PostTask");
            requestParameter.Parameters.Add("UserAccount", postData.OwnerAccount);
            requestParameter.RequestBodyData = JsonConvert.SerializeObject(postData);

            ProcessPostResult postResult = await restApiUtil.PostAsync(requestParameter);

            if (!postResult.success)
            {
                logUtil.Error(
                    $"接口调用失败，调用信息:{JsonConvert.SerializeObject(postData)}，返回信息：{JsonConvert.SerializeObject(postResult)}");
                return false;
            }

           await UpdatePostInfoAsync(postResult,id);

           return true;
        }

        private static async Task UpdatePostInfoAsync(ProcessPostResult postResult, string id = null)
        {
            if (string.IsNullOrEmpty(id))
                return;

            SysLogUtil logUtil = new SysLogUtil(SysLogUtil.LogTagType.LogToFile);

            SqlSugarClient dbContent = SqlSugarConfig.GetConnectOption();
            try
            {
                dbContent.Ado.BeginTran();

                await dbContent.Updateable<ProcessDataInfo>(new ProcessDataInfo()
                    {
                        TaskID = postResult.TaskID
                    })
                    .Where(f => f.ID == id)
                    .UpdateColumns(it => it.TaskID)
                    .ExecuteCommandAsync();

                await dbContent.Updateable<ProcessNodeConfig>(new ProcessNodeConfig()
                    {
                        TaskID = postResult.TaskID
                    }).Where(f => f.AID == id)
                    .UpdateColumns(it => it.TaskID)
                    .ExecuteCommandAsync();

                dbContent.Ado.CommitTran();
            }
            catch (Exception exception)
            {
                dbContent.Ado.RollbackTran();
                Console.WriteLine($"执行列更新失败，失败主键: {id},对应TaskID：{postResult.TaskID}");
                logUtil.Error($"执行列更新失败，失败主键: {id},对应TaskID：{postResult.TaskID}");
            }
        }
    }
}
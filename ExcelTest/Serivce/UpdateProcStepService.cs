using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExcelTest.Env;
using ExcelTest.Models;
using SqlSugar;

namespace ExcelTest.Serivce
{
    /// <summary>
    /// 流程步骤信息更新服务
    /// </summary>
    public class UpdateProcStepService
    {
        public static async Task UpdatSetpInfo()
        {
            SqlSugarClient dbContent = SqlSugarConfig.GetConnectOption();

            List<ProcessNodeConfig> processNodeConfigs = await dbContent.Queryable<ProcessNodeConfig>()
                .Where(f => f.TaskID > 0).ToListAsync();

            int result = 0;

            // 遍历历史数据
            processNodeConfigs.ForEach(async f =>
            {
                result = await UpdateProcessStep(f, dbContent);
                Console.WriteLine($"本次执行结果{result}");
            });
            
            Console.WriteLine("流程信息更新完毕");
        }

        /// <summary>
        /// 更新流程节点信息
        /// </summary>
        /// <param name="nodeConfig"></param>
        /// <param name="dbContent"></param>
        /// <returns></returns>
        private static async Task<int> UpdateProcessStep(ProcessNodeConfig nodeConfig, SqlSugarClient dbContent)
        {
            int index = 0;
            try
            {
                dbContent.Ado.BeginTran();

                index = await dbContent.Updateable<BPMInstProcSteps>(new BPMInstProcSteps()
                    {
                        TaskID = nodeConfig.TaskID,
                        NodeName = nodeConfig.NodeName,
                        ExtRecipient = nodeConfig.OwnerAccount,
                        OwnerAccount = nodeConfig.OwnerAccount,
                        ReceiveAt = nodeConfig.CreateAt,
                        FinishAt = nodeConfig.ApprovalAt,
                        Comments = nodeConfig.Comment
                    })
                    .UpdateColumns(it => new
                        { it.ExtRecipient, it.OwnerAccount, it.ReceiveAt, it.FinishAt, it.Comments })
                    .Where(w => w.TaskID == nodeConfig.TaskID)
                    .Where(w => w.NodeName == nodeConfig.NodeName)
                    .ExecuteCommandAsync();

                dbContent.Ado.CommitTran();

                return index;
            }
            catch (Exception e)
            {
                dbContent.RollbackTran();
                Console.WriteLine(e);
                return index;
                // throw;
            }
        }
    }
}
using ExcelTest.Env;
using ExcelTest.Models;
using ExcelTest.Utils;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExcelTest.Serivce
{
    public class ProcessPostService
    {
        public static async Task BatchPost()
        {
            SysLogUtil sysLogUtil = new SysLogUtil(SysLogUtil.LogTagType.LogToFile);

            SqlSugarClient dbContent = SqlSugerConfig.GetConnectOption();
            List<ProcessDataInfo> processDataInfos = await dbContent.Queryable<ProcessDataInfo>()
               .Where(f => f.TaskID == 0)
               .ToListAsync();

            sysLogUtil.Trace($"本次一共需要发起{processDataInfos.Count}条流程");

            if(processDataInfos.Count < 1)
            {
                sysLogUtil.Trace("本次需要发起的流程条数为0，程序即将退出");
                Console.WriteLine("本次需要发起的流程条数为0，程序即将退出");
                return;
            }

            processDataInfos.ForEach(e => { 
            
            });

            Console.WriteLine("111");
        }

        private static async Task ProcessPost()
        {

        }
    }
}
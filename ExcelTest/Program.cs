using ExcelTest.Serivce;
using System;
using System.Threading.Tasks;

namespace ExcelTest
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("程序开始执行");
            Console.WriteLine("===================请选择执行模式====================");
            Console.WriteLine("===0：Excel数据初始化，1：任务发起，2：流程审批时间更新===");
            Console.WriteLine("本次程序执行方式：");
            int key = Convert.ToInt32(Console.ReadLine());

            switch (key)
            {
                case 0:
                    ExcelDataReader();
                    break;
                case 1:
                    await ProcessPostService.BatchPost();
                    break;

                case 2:
                    break;

                default:
                    Console.WriteLine("未找到匹配执行方式，程序即将退出。");
                    break;
            }

            Console.WriteLine("===========程序执行完毕=============");
        }

        private static void ExcelDataReader()
        {
            string filePath = @"./Input/InputData.xlsx";
            ExcelOperationUtil.ReadExcelFile(filePath, 0, true, true);
            //Console.WriteLine("开始执行第一个Sheet数据处理");
            //ExcelOperationUtil.ReadExcelFile(filePath,0,true);
            //Console.WriteLine("第一个Sheet执行完毕");
            //Console.WriteLine("开始执行第二个Sheet数据处理");
            //ExcelOperationUtil.ReadExcelFile(filePath,1,true);
            //Console.WriteLine("第二个Sheet执行完毕");
            //Console.WriteLine("开始执行第三个Sheet数据处理");
            //ExcelOperationUtil.ReadExcelFile(filePath,2,true);
            //Console.WriteLine("第三个Sheet执行完毕");

            Console.WriteLine("==========所有Excel数据清理初始化完毕===========");
        }
    }
}
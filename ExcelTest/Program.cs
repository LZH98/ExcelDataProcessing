using System;

namespace ExcelTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("程序开始执行");
            Console.WriteLine("开始执行第一个Sheet读取");
            string filePath = @"D:\\流程示例.xlsx";
            ExcelOperationUtil.ReadExcelFile(filePath,0,true);
        }
    }
}
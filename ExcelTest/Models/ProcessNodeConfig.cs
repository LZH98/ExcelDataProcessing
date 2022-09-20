using System;

namespace ExcelTest.Models
{
    [SqlSugar.SugarTable("dbo.M_ProcessNodeConfig")]
    public class ProcessNodeConfig
    {
        public string AID { get; set; }
        public string ProcessName { get; set; }
        public int TaskID { get; set; }
        public string NodeName { get; set; }
        public string OwnerAccount { get; set; }
        public string Comment { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime ApprovalAt { get; set; }
    }
}
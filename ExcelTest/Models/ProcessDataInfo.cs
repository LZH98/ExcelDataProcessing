namespace ExcelTest.Models
{
    /// <summary>
    ///
    /// </summary>
    [SqlSugar.SugarTable("dbo.M_ProcessDataInfo")]
    public class ProcessDataInfo
    {
        public string ID { get; set; }
        public string ProcessName { get; set; }
        public string Action { get; set; } = "发起";
        public string Comment { get; set; }
        public string FormData { get; set; }
        public string OwnerAccount { get; set; }
        public string OuCode { get; set; }
        public string DescribeFormData { get; set; }
        public int TaskID { get; set; }
    }
}
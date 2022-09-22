namespace ExcelTest.Models.Response
{
    public class ProcessPostResult
    {
        public bool success { get; set; }
        public int TaskID { get; set; }
        public string SN { get; set; }
        public string OpenUrl { get; set; }
        public string errorMessage { get; set; }
    }
}
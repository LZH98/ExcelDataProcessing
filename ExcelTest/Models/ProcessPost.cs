using System;
using System.Collections.Generic;
using NetTaste;
using Newtonsoft.Json.Linq;

namespace ExcelTest.Models
{
    public class ProcessPost
    {
        /// <summary>
        /// 流程名称
        /// </summary>
        public string ProcessName { get; set; }
        /// <summary>
        /// 提交方式
        /// </summary>
        public string Action { get; set; }
        /// <summary>
        /// 审批意见
        /// </summary>
        public string  Comment { get; set; }
        /// <summary>
        /// 部门ID
        /// </summary>
        public string DeptID { get; set; }
        /// <summary>
        /// 是否草稿
        /// </summary>
        public bool Draft { get; set; }
        public int ExistTaskID { get; set; }
        /// <summary>
        /// 附件信息
        /// </summary>
        public List<BPMAttachmentInfo> AttachmentInfo { get; set; }
        /// <summary>
        /// 表单信息
        /// </summary>
        public JObject FormData { get; set; }
        /// <summary>
        /// 提交人账号
        /// </summary>
        public string OwnerAccount { get; set; }
    }
}
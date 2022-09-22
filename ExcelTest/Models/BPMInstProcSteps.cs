using System;

namespace ExcelTest.Models
{
    [SqlSugar.SugarTable("BPMDB.dbo.BPMInstProcSteps")]
    public class BPMInstProcSteps
    {
        /// <summary>
        /// 无
        /// </summary>
        public int StepID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int TaskID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string ProcessName { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string NodeName { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string OwnerAccount { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string AgentAccount { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public DateTime ReceiveAt { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public DateTime? FinishAt { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string SelAction { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public bool Share { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public bool HumanStep { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string HandlerAccount { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string SubNodeName { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public bool AutoProcess { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? UsedMinutes { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? UsedMinutesWork { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? RecedeFromStep { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int TimeoutNotifyCount { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? RisedConsignID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? BelongConsignID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string ConsignOwnerAccount { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public DateTime? TimeoutFirstNotifyDate { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public DateTime? TimeoutDeadline { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? StandardMinutesWork { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public bool BatchApprove { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public bool Posted { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public bool FormSaved { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? ParentStepID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string NodePath { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? ExtYear { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? ExtStepYear { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string ExtRecipient { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public bool ExtDeleted { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? OwnerPositionID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string ProcessVersion { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string Context { get; set; }
    }
}
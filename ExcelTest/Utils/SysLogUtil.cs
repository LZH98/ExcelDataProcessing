using NLog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ExcelTest.Utils
{
    public class SysLogUtil
    {
        /// <summary>
        /// 实例化nLog，即为获取配置文件相关信息(获取以当前正在初始化的类命名的记录器)
        /// </summary>
        private readonly NLog.Logger _logger;
        public enum LogTagType { LogToFile, LogToDatabase };
        private static List<SysLogUtil> _obj;
        private LogTagType _tagType;

        public SysLogUtil(LogTagType logTagType)
        {
            if (_obj == null)
            {
                _obj = new List<SysLogUtil>();
                _obj.Add(this);
                this._logger = LogManager.GetLogger(logTagType.ToString());
                this._tagType = logTagType;                
            }
            else
            {
                SysLogUtil thisLogger = _obj.Find(log => log._tagType == logTagType);
                if (thisLogger == null) 
                {
                    _obj.Add(this);
                    this._logger = LogManager.GetLogger(logTagType.ToString());
                    this._tagType = logTagType;
                }
                else
                    this._logger = thisLogger._logger;
            }
                
        }

        #region Debug，调试
        public void Debug(string msg)
        {
            if (_tagType == LogTagType.LogToDatabase)
            { 
                throw new Exception("It is forbidden to write debug log in the database, please write the log in text mode");
            }
            else
                _logger.Debug(msg);
        }

        public void Debug(string msg, Exception err)
        {
            if (_tagType == LogTagType.LogToDatabase)
            {
                throw new Exception("It is forbidden to write debug log in the database, please write the log in text mode");
            }
            else
                _logger.Debug(err, msg);
        }
        #endregion

        #region Info，信息
        public void Info(string msg, string taskID = "", string logTitle = "CallExternalInterface")
        {
            if (_tagType == LogTagType.LogToDatabase)
            {
                WriteLog(LogLevel.Info, taskID, "BPM", "Process", logTitle, msg, null);
            }
            else
                _logger.Info(msg);
        }

        public void Info(string msg, Exception err)
        {
            _logger.Info(err, msg);
        }
        #endregion

        #region Warn，警告
        public void Warn(string msg, string taskID = "", string logTitle = "CallExternalInterface")
        {
            if (_tagType == LogTagType.LogToDatabase)
            {
                WriteLog(LogLevel.Warn, taskID, "BPM", "Process", logTitle, msg, null);
            }
            else
                _logger.Warn(msg);
        }

        public void Warn(string msg, Exception err)
        {
            _logger.Warn(err, msg);
        }
        #endregion

        #region Trace，追踪
        public void Trace(string msg, string taskID = "", string logTitle = "CallExternalInterface")
        {
            if (_tagType == LogTagType.LogToDatabase)
            {
                WriteLog(LogLevel.Trace, taskID, "BPM", "Process", logTitle, msg, null);
            }
            else
                _logger.Trace(msg);
        }

        public void Trace(string msg, Exception err)
        {
            _logger.Trace(err, msg);
        }
        #endregion

        #region Error，错误
        public void Error(string msg, string taskID = "", string logTitle = "CallExternalInterface")
        {
            if (_tagType == LogTagType.LogToDatabase)
            {
                WriteLog(LogLevel.Error, taskID, "BPM", "Process", logTitle, msg, null);
            }
            else
                _logger.Error(msg);
        }

        public void Error(string msg, Exception err)
        {
            _logger.Error(err, msg);
        }
        #endregion

        #region Fatal,致命错误
        public void Fatal(string msg, string taskID = "", string logTitle = "CallExternalInterface")
        {
            if (_tagType == LogTagType.LogToDatabase)
            {
                WriteLog(LogLevel.Fatal, taskID, "BPM", "Process", logTitle, msg, null);
            }
            else
                _logger.Fatal(msg);
        }

        public void Fatal(string msg, Exception err)
        {
            _logger.Fatal(err, msg);
        }
        #endregion

        private void WriteLog(LogLevel levle, string taskID, string moduleName, string procName, string logTitle, string logMessage, Exception ex)
        {
            LogEventInfo ei = new LogEventInfo();
            ei.Properties["taskID"] = taskID;
            ei.Properties["moduleName"] = moduleName;
            ei.Properties["procName"] = procName;
            ei.Properties["logLevel"] = levle.ToString();
            ei.Properties["logTitle"] = logTitle;
            ei.Properties["logDate"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            ei.Properties["logMessage"] = logMessage;
            ei.Properties["stackTrace"] = ex == null ? "" : ex.Message + "\n" + ex.InnerException;
            ei.Level = levle;
            _logger.Log(ei);
        }
    }
}

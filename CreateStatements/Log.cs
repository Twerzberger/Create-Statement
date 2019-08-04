using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;

namespace CreateStatements
{
    class Log
    {
        #region "Member variables"
        public static string APP_PATH = Application.StartupPath + "\\";
        public static string APP_LOG_FILE_PATH = APP_PATH + "ApplicationLog_" + DateTime.Now.ToString("MMddyyyy") + ".txt";
        public static string LOG_SUMMARY_FILE_PATH = APP_PATH + "LogSummary_" + DateTime.Now.ToString("MMddyyyy") + ".txt";
        public static string ERROR_LOG_FILE_PATH = APP_PATH + "ErrorLog.txt";
        public static string NEW_LINE = "\r\n";
        #endregion

        #region "Application Log"
        /// <summary>
        /// To write error message to error log
        /// </summary>
        /// <param name="msg"></param>
        public static void WriteToErrorLog(string pLogMessage)
        {
            try
            {
                if (pLogMessage.Trim().Length > 0)
                {
                    System.IO.StreamWriter writer = new System.IO.StreamWriter(ERROR_LOG_FILE_PATH, true);
                    writer.WriteLine(DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + " : " + pLogMessage + NEW_LINE);
                    writer.Flush();
                    writer.Close();
                    writer.Dispose();
                    writer = null;
                }
            }
            catch { }
        }
        /// <summary>
        /// To write error message to error log
        /// </summary>
        /// <param name="msg"></param>
        public static void WriteToApplicationLog(string pLogMessage)
        {
            try
            {
                if (pLogMessage.Trim().Length > 0)
                {
                    System.IO.StreamWriter writer = new System.IO.StreamWriter(APP_LOG_FILE_PATH, true);
                    writer.WriteLine(DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + " : " + pLogMessage + NEW_LINE);
                    writer.Flush();
                    writer.Close();
                    writer.Dispose();
                    writer = null;
                }
            }
            catch { }
        }

        internal static void WriteToLogSummary(string pLogMessage)
        {
            try
            {
                if (pLogMessage.Trim().Length > 0)
                {
                    System.IO.StreamWriter writer = new System.IO.StreamWriter(LOG_SUMMARY_FILE_PATH, true);
                    writer.WriteLine(pLogMessage);
                    writer.Flush();
                    writer.Close();
                    writer.Dispose();
                    writer = null;
                }
            }
            catch { }
        }

        /// <summary>
        /// To get application log
        /// </summary>
        /// <param name="msg"></param>
        public static string GetLog(LogFileType pLogFileType)
        {
            string retStr = "";
            try
            {
                string filePath = "";
                switch (pLogFileType)
                {
                    case LogFileType.Application:
                        filePath = APP_LOG_FILE_PATH;
                        break;
                    case LogFileType.Error:
                        filePath = ERROR_LOG_FILE_PATH;
                        break;
                }
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.StreamReader reader = new System.IO.StreamReader(filePath);
                    retStr = reader.ReadToEnd();
                    reader.Close();
                    reader.Dispose();
                    reader = null;
                }
            }
            catch { }
            return retStr;
        }

        /// <summary>
        /// Used to clear the application log
        /// </summary>
        /// <param name="pLogFileType"></param>
        /// <returns></returns>
        public static void ClearLog(LogFileType pLogFileType)
        {
            try
            {
                string filePath = "";
                switch (pLogFileType)
                {
                    case LogFileType.Application:
                        filePath = APP_LOG_FILE_PATH;
                        break;
                    case LogFileType.Error:
                        filePath = ERROR_LOG_FILE_PATH;
                        break;
                }
                try
                {
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath, false);
                        writer.WriteLine("-------------------------------------------------------------------------");
                        writer.WriteLine("Log cleared at " + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"));
                        writer.WriteLine("-------------------------------------------------------------------------");
                        writer.Flush();
                        writer.Close();
                        writer.Dispose();
                        writer = null;
                    }
                }
                catch { }
            }
            catch { }
        }

        /// <summary>
        /// To handle Log file name
        /// </summary>
        public enum LogFileType
        {
            Application,
            Error
        }
        #endregion

    }
}

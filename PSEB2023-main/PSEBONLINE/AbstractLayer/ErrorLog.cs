using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Text;
using System.IO;

namespace PSEBONLINE.AbstractLayer
{
    public class ErrorLog
    { 

        public bool WriteErrorLog(string LogMessage,string pageName)
        {
            bool Status = false;
            string LogDirectory = ConfigurationManager.AppSettings["LogDirectory"].ToString();
            DateTime CurrentDateTime = DateTime.Now;
            string CurrentDateTimeString = CurrentDateTime.ToString();
            bool chk = CheckCreateLogDirectory(LogDirectory);
            //string logLine = BuildLogLine(CurrentDateTime, LogMessage);
            string logLine = "Error: " + LogMessage;
            LogDirectory = (LogDirectory + "Log_" + DateTime.Now.ToString("dd_MM_yyyy") + ".txt");

            string filepath = System.Web.HttpContext.Current.Server.MapPath(LogDirectory);
            lock (typeof(ErrorLog))
            {
                if (System.IO.File.Exists(filepath))
                {
                    using (StreamWriter stwriter = new StreamWriter(filepath, true))
                    {
                        stwriter.WriteLine("-------------------START-------------" + DateTime.Now);
                        stwriter.WriteLine("Page : " + pageName);
                        stwriter.WriteLine(logLine);
                        stwriter.WriteLine("-------------------END-------------" + DateTime.Now);
                    }
                }
                else
                {
                    StreamWriter stwriter = System.IO.File.CreateText(filepath);
                    stwriter.WriteLine("-------------------START-------------" + DateTime.Now);
                    stwriter.WriteLine("Page : " + pageName);
                    stwriter.WriteLine(logLine);
                    stwriter.WriteLine("-------------------END-------------" + DateTime.Now);
                    stwriter.Close();
                }
                Status = true;
            }
            return Status;
        }
        private bool CheckCreateLogDirectory(string LogPath)
        {
            bool loggingDirectoryExists = false;
            DirectoryInfo oDirectoryInfo = new DirectoryInfo(LogPath);
            if (oDirectoryInfo.Exists)
            {
                loggingDirectoryExists = true;
            }
            else
            {
                try
                {
                    Directory.CreateDirectory(LogPath);
                    loggingDirectoryExists = true;
                }
                catch
                {
                    // Logging failure
                }
            }
            return loggingDirectoryExists;
        }

    }
}
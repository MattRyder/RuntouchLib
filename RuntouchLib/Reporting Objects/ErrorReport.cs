using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Runtouch.Utilities;
using System.Reflection;
using System.Net;
using System.IO;

namespace Runtouch
{
    /// <summary>
    /// An Error Report with exception data and other error messages.
    /// </summary>
    public class ErrorReport
    {
        private string _filename, _rcontent;

        public string Filename
        {
            get { return _filename; }
            set { _filename = value; }
        }
        public string ReportContent
        {
            get { return _rcontent; }
            set { _rcontent = value; }
        }

        public ErrorReport() { }

        public ErrorReport(Exception appException)
        {
            //Filename is the current time + where the Exception happened.
            this.Filename = RTUtilities.getPostDate() + "_" + appException.Source + "_" + new Random().Next() + ".txt";

            this.ReportContent = "Product Name: " + Assembly.GetEntryAssembly().GetName().Name + "\r\n" +
                                 "Time of Exception: " + DateTime.Now + "\r\n" +
                                 "Exception Message: " + appException.Message + "\r\n" +
                                 "Exception Source: " + appException.Source + "\r\n" +
                                 "Exception - Inner Exception: " + appException.InnerException + "\r\n" +
                                 "Exception Stack Trace: " + appException.StackTrace + "\r\n" +
                                 "Exception Target Site: " + appException.TargetSite + "\r\n\r\n\r\n" +

                                 //Now add some info about the target machine (completely anon!)
                                 "Target Machine Info: \r\nClient Operating System: " + Environment.OSVersion + "\r\n" +
                                 "Client .NET Version: " + Environment.Version;
        }
    }
}
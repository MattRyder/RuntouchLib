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
    class ErrorReport
    {
        //Accessors + Mutators
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

        /// <summary>
        /// Upload to the FTP server given in the Server object
        /// </summary>
        /// <param name="server">Server information</param>
        public void Upload(Server server)
        {
            try
            {
                FtpWebRequest wr = (FtpWebRequest)WebRequest.Create("ftp://" + server.IP +
                                                                     "/" + server.RootDirectory +
                                                                     "/" + this.Filename);

                wr.Method = WebRequestMethods.Ftp.UploadFile;
                wr.Credentials = server.Credential;

                byte[] reportContent = Encoding.UTF8.GetBytes(this.ReportContent);
                wr.ContentLength = this.ReportContent.Length;

                //Put the error report into a Stream
                Stream rStream = wr.GetRequestStream();
                rStream.Write(reportContent, 0, reportContent.Length);
                rStream.Close();

                FtpWebResponse wresp = (FtpWebResponse)wr.GetResponse();
                Console.WriteLine("Upload done! Status: " + wresp.StatusDescription);
                wresp.Close();
            }

            catch (Exception e)
            {
                Console.WriteLine("Could not save RunTouch ErrorReport to file: \nReason:\n" +
                  new ErrorReport(e).ReportContent);
            }
        }

        /// <summary>
        /// Save to a local path on the current disk
        /// </summary>
        /// <param name="path">Write a file path (without filename) or leave blank for current working directory</param>
        public void SaveToDisk(string path)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(path + this.Filename))
                {
                    sw.Write(this.ReportContent);
                    sw.Close();
                    Console.WriteLine("Successfully wrote log to disk!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not save RunTouch ErrorReport to file: \nReason:\n" +
                  new ErrorReport(e).ReportContent);
            }
        }
    }
}

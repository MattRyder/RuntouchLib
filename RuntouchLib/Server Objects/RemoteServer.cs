using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Runtouch.Utilities;
using System.IO;

namespace Runtouch.Server
{
    public class RemoteServer : BaseServer
    {
        private NetworkCredential credentials;
        private IPAddress ipaddress;

        protected NetworkCredential Credentials
        {
            get { return credentials; }
            set { credentials = value; }
        }
        public IPAddress IPAddress
        {
            get { return ipaddress; }
            set { ipaddress = value; }
        }


        /// <summary>
        /// A Server object with the aim of connecting to a remote host.
        /// </summary>
        /// <param name="ipAddress">IP or Hostname of the remote server.</param>
        /// <param name="rootDirectory">Root Directory on the server to log to.</param>
        /// <param name="netCredential">Credentials for logging into the server.</param>
        public RemoteServer(string ipAddress, string rootDirectory, NetworkCredential netCredential) 
            : base(rootDirectory)
        {
            this.IPAddress = RTUtilities.hostToIP(ipAddress);
            this.Credentials = netCredential;
        }


        /// <summary>
        /// Tests the given network credentials against the Server for validity on login.
        /// </summary>
        /// <returns>Boolean based on the success of the request.</returns>
        public bool testServerCredentials()
        {
            //Create the connection string to the server:
            string authURL = "FTP://" + this.IPAddress + "/" + this.RootDirectory;
            FtpWebRequest authRequest = (FtpWebRequest)WebRequest.Create(authURL);
            authRequest.Credentials = this.Credentials;
            authRequest.Timeout = 3000;
            authRequest.Method = WebRequestMethods.Ftp.ListDirectory;
            authRequest.KeepAlive = false;

            try
            {
                using (FtpWebResponse authReponse = (FtpWebResponse)authRequest.GetResponse())
                {
                    if (authReponse.WelcomeMessage.Contains("230"))
                    {
                        Console.WriteLine("Server authentication returned FTP Status 230. Login OK");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("WARNING: Server did not return Status Code 230. Are the Server details correct?");
                        return false;
                    }
                }
            }
            catch (WebException wException)
            {
                //Something broke trying to connect to the server!
                throw wException;
            }
        }

        /// <summary>
        /// Uploads an error report to the remote server.
        /// </summary>
        /// <returns>Boolean based on the success of the upload.</returns>
        public bool UploadReport(ErrorReport report)
        {
            FtpWebResponse ftpResponse = null;
            Uri uploadURI = new Uri("FTP://" + this.IPAddress + "/" + this.RootDirectory + "/" + report.Filename);

            //Check for valid credentials, bail out if bad login:
            if (!testServerCredentials()) return false;

            //Credentials verified, let's make the request:
            FtpWebRequest wr = (FtpWebRequest)WebRequest.Create(uploadURI);
            wr.Method = WebRequestMethods.Ftp.UploadFile;
            wr.Credentials = this.Credentials;

            byte[] reportContent = Encoding.UTF8.GetBytes(report.ReportContent);
            wr.ContentLength = report.ReportContent.Length;

            try
            {
                //Put the error report into a Stream, and upload to the FTP Server!
                Stream rStream = wr.GetRequestStream();
                rStream.Write(reportContent, 0, reportContent.Length);
                rStream.Close();

                using (ftpResponse = (FtpWebResponse)wr.GetResponse())
                {
                    Console.WriteLine("Upload Status: (" + ftpResponse.StatusCode + 
                                     ")\nDescription: " + ftpResponse.StatusDescription);
                    ftpResponse.Close();
                }

                return true;
            }

            catch (Exception e)
            {
                //Meta-report to console.
                Console.WriteLine("Could not save RunTouch ErrorReport to file: \nReason:\n" +
                  new ErrorReport(e).ReportContent);
                return false;
            }  
        }

        /// <summary>
        /// Saves the report to a location on the local disk.
        /// </summary>
        /// <param name="report">The Error Report to save</param>
        /// <returns>Boolean based on the success of the upload.</returns>
        public override bool SaveReportToDisk(ErrorReport report)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(this.RootDirectory + report.Filename))
                {
                    sw.Write(report.ReportContent);
                    sw.Close();
                    Console.WriteLine("Wrote ErrorReport '{0}' to location: {1}", report.Filename, this.RootDirectory);

                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not save RunTouch ErrorReport to file!\nReason:" +
                  new ErrorReport(e).ReportContent);

                return false;
            }
        }
    }
}

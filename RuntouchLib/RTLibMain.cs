using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Reflection;
using System.IO;

namespace Runtouch
{
    /// <summary>
    /// Holds the Server information for your error reports, including login details.
    /// </summary>
    public class Server
    {
        private IPAddress _ip;
        private string _rootDirectory;
        private NetworkCredential _credential;

        /// <summary>
        /// IP Address or Domain to connect to. 
        /// </summary>
        public IPAddress IP
        {
            get { return _ip; }
            set { _ip = value; }
        }

        /// <summary>
        /// Root Directory on the Server to post the reports to.
        /// </summary>
        public string RootDirectory
        {
            get { return _rootDirectory; }
            set { _rootDirectory = value; }
        }

        /// <summary>
        /// Network Login Credentials for the Server.
        /// </summary>
        public NetworkCredential Credential
        {
            get { return _credential; }
            set { _credential = value; }
        }

        /// Creates a new instance of a Server object
        /// </summary>
        /// <param name="ip">IP or Domain of the Server</param>
        /// <param name="rootdirectory">Root directory to store data to</param>
        /// <param name="credential">Login Credentials for the FTP Server</param>
        public Server(string ip, string rootdirectory, NetworkCredential credential)
        {
            //Immediately cast the string to IPv4-compatible IPAddress obj:
            this.IP = RT_Tools.dnsHostEntryToIP(Dns.GetHostEntry(ip));
            this.RootDirectory = rootdirectory;
            this.Credential = credential;

            try
            {
                //Check if the credentials and the server check out.
                FtpWebRequest testRequest = (FtpWebRequest)WebRequest.Create("ftp://" + this.IP + "/" + this.RootDirectory);
                testRequest.Credentials = this.Credential;
                testRequest.KeepAlive = false;
                testRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                FtpWebResponse svrResponse = (FtpWebResponse)testRequest.GetResponse();

                if (svrResponse.WelcomeMessage.Contains("230"))
                {
                    Console.WriteLine("Server returned FTP Status Code 230. Successfully logged in.");
                }
                else
                {
                    Console.WriteLine("WARNING: Server did not return Status Code 230. Are the Server details correct?");
                }

            }
            catch (WebException up)
            {
                /* WHY I AM THROWING THIS RATHER THAN HANDLING IT:
                 * If this fails, the whole system fails because you can't send an ErrorReport over FTP
                 * without a valid FTP login in the Server object. 
                 * The developer should be testing this before deployment. */

                throw up; //Lulz. (please don't judge me.)
            }
        }
    }

    /// <summary>
    /// An Error Report with exception data and other error messages.
    /// </summary>
    public class ErrorReport
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
            this.Filename = RT_Tools.generateUnixFriendlyDate() + "_" + appException.Source + "_" + new Random().Next()+".txt";

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
                FtpWebRequest wr = (FtpWebRequest)WebRequest.Create("ftp://" +
                                            server.IP +
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

    public static class RT_Tools
    {
        /// <summary>
        /// Gets an IPv4 Address from the AddressList of an IPHostEntry
        /// </summary>
        /// <param name="host">IPHostEntry with valid AddressList</param>
        public static IPAddress dnsHostEntryToIP(IPHostEntry host)
        {
            foreach (var address in host.AddressList)
            {
                if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return address;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns a UNIX friendly date for servers.
        /// </summary>
        /// <returns></returns>
        public static string generateUnixFriendlyDate()
        {
            DateTime date = DateTime.Now;
            return date.Day.ToString() + "_" +
                   date.Month.ToString() + "_" +
                   date.Year.ToString();
        }



    }
}

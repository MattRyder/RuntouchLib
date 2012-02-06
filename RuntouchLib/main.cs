using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

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

        #region Properties
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
        #endregion

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
            catch (WebException rtWebException)
            {
                /* WHY I AM THROWING THIS RATHER THAN HANDLING IT:
                 * If this fails, the whole system fails because you can't send an ErrorReport over FTP
                 * without a valid FTP login in the Server object. 
                 * The developer should be testing this before deployment. */

                //TODO: Change this to log the exception to local disk.
                throw rtWebException;
            }
        }
    }
}

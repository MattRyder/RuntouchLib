using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Runtouch.Utilities;

namespace Runtouch
{
    class RemoteServer : BaseServer
    {
        private NetworkCredential credential;
        private IPAddress ipaddress;

        protected NetworkCredential Credential
        {
            get { return credential; }
            set { credential = value; }
        }
        public IPAddress IPAddress
        {
            get { return ipaddress; }
            set { ipaddress = value; }
        }

        public RemoteServer(string ipAddress, string rootDirectory, NetworkCredential netCredential) 
            : base(rootDirectory)
        {
            this.IPAddress = RTUtilities.hostToIP(Dns.GetHostEntry(ipAddress));
            this.Credential = netCredential;
        }

        public bool UploadReport()
        {

        }

        public override bool SaveReportToDisk(ErrorReport report)
        {
            throw new NotImplementedException();
        }
    }
}

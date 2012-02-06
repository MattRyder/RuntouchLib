using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace Runtouch.Server
{
    public abstract class BaseServer
    {
        private string rootDirectory;

        public string RootDirectory
        {
            get { return rootDirectory; }
            set { rootDirectory = value; }
        }

        public BaseServer(string rootDirectory)
        {
            this.RootDirectory = rootDirectory;
        }

        /// <summary>
        /// Writes the ErrorReport to file (in case of Server Auth errors) or if the intended action is local storage
        /// </summary>
        /// <param name="report">The error report to write to file</param>
        /// <returns>Boolean dependent on save operation success</returns>
        public abstract bool SaveReportToDisk(ErrorReport report);
    }
}

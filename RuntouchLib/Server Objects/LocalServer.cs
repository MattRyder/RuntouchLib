using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runtouch
{
    class LocalServer : BaseServer
    {
        private string filepath;

        public string FilePath
        {
            get { return filepath; }
            set { filepath = value; }
        }

        public LocalServer(string filePath, string rootDirectory) : base(rootDirectory)
        {
            this.FilePath = filepath; //Validate?
        }

        public override bool SaveReportToDisk(ErrorReport report)
        {
            throw new NotImplementedException();
        }
    }
}

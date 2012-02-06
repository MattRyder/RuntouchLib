using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Runtouch.Server
{
    public class LocalServer : BaseServer
    {
        public LocalServer(string rootDirectory) : base(rootDirectory)
        {
            //If the directory doesn't exist, create it:
            if (!Directory.Exists(rootDirectory))
                Directory.CreateDirectory(rootDirectory);
        }

        public override bool SaveReportToDisk(ErrorReport report)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(this.RootDirectory+report.Filename))
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

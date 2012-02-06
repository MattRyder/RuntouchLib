using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Runtouch;
using Runtouch.Server;
using Runtouch.Utilities;

namespace RuntouchTests
{
    [TestFixture]
    public class RuntouchTestFixture
    {
        [Test]
        public void TestReportContent()
        {
            LocalServer local = new LocalServer("C:/DIR");
            ErrorReport expected = new ErrorReport();
            expected.Filename = "TestReport";
            ErrorReport actual = new ErrorReport(new DivideByZeroException());
            local.SaveReportToDisk(actual);
            Assert.Null(local);
            
        }

        // This test fail for example, replace result or delete this test to see all tests pass
        [Test]
        public void TestFault()
        {
            Assert.IsTrue(false);
        }
    }
}

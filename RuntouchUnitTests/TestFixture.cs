using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Runtouch;
using Runtouch.Server;
using System.Net;

namespace RuntouchUnitTests
{
    [TestFixture]
    public class TestFixture1
    {

        /* SERVER OBJECT TESTS */
        [Test]
        public void TestLocalServerObject()
        {
            LocalServer expLocal = new LocalServer("C:\\TestData");
            Assert.AreEqual("C:\\TestData", expLocal.RootDirectory);
        }

        [Test]
        public void TestRemoteServerObject()
        {
            RemoteServer expRemote = new RemoteServer("ftp.secureftp-test.com",
                                          "/",
                                          new System.Net.NetworkCredential("test", "test"));

            Assert.IsTrue(expRemote.testServerCredentials());
        }


        /* TOOLS METHOD TESTS */
        [Test]
        public void TestHostToIP()
        {
            IPAddress expected = Dns.GetHostAddresses("WWW.google.com")[0];
            IPAddress actual = Runtouch.Utilities.RTUtilities.hostToIP("www.google.com");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestGetPostDate()
        {
            string expected = "06-02-2012";
            string actual = Runtouch.Utilities.RTUtilities.getPostDate();

            Assert.AreEqual(expected, actual);
        }

        /* ERROR REPORT TESTS */


    }
}

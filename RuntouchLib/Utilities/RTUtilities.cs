using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Runtouch.Utilities
{
    public static class RTUtilities
    {
        /// <summary>
        /// Gets an IPv4 Address from the AddressList of an IPHostEntry. Returns null if none found.
        /// </summary>
        /// <param name="host">IPHostEntry with valid AddressList</param>
        public static IPAddress hostToIP(string host)
        {
            IPHostEntry hostEntries = Dns.GetHostEntry(host);
            foreach (var address in hostEntries.AddressList)
            {
                if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    return address;
            }
            return null;
        }

        /// <summary>
        /// Gets the current time in the form DD-MM-YYYY
        /// </summary>
        /// <returns>A string representing the current date</returns>
        public static string getPostDate()
        {
             return DateTime.Now.ToString("dd-MM-yyyy");
        }
    }
}

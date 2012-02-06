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
        public static IPAddress hostToIP(IPHostEntry host)
        {
            foreach (var address in host.AddressList)
            {
                if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    return address;
            }
            return null;
        }

        /// <summary>
        /// Gets the current time in the form DD_MM_YYYY
        /// </summary>
        /// <returns>A string representing the current date</returns>
        public static string getPostDate()
        {
            DateTime date = DateTime.Now;
            return date.Day.ToString() + "_" +
                   date.Month.ToString() + "_" +
                   date.Year.ToString();
        }
    }
}

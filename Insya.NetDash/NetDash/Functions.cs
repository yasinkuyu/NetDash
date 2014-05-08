// Copyright (c) 2014, Insya Interaktif.
// Developer @yasinkuyu
// All rights reserved.

using System.Reflection;
using System.Net;

namespace Insya.NetDash
{
    public static class Functions
    {
        /// <summary>
        /// Get Assembly version
        /// </summary>
        public static string CurrentVersion
        {
            get
            {
                var thisApp = Assembly.GetExecutingAssembly();
                var name = new AssemblyName(thisApp.FullName);
                return name.Version.ToString(3);
            }
        }

        /// <summary>
        /// Setting.ini get SERVERNAME varible
        /// </summary>
        /// <returns></returns>
        public static string GetServerName()
        {
            return string.Format(@"\\{0}\root\CIMV2", Settings.Get("SERVERNAME"));
        }

        /// <summary>
        /// Setting.ini get REMOTE_DOMAIN varible
        /// </summary>
        /// <returns></returns>
        public static string GetServerDomain()
        {
            return Settings.Get("REMOTE_DOMAIN");
        }

        /// <summary>
        /// Setting.ini get REMOTE_USERNAME varible
        /// </summary>
        /// <returns></returns>
        public static string GetServerUserName()
        {
            return Settings.Get("REMOTE_USERNAME");
        }

        /// <summary>
        /// Setting.ini get REMOTE_PASSWORD varible
        /// </summary>
        /// <returns></returns>
        public static string GetServerPassword()
        {
            return Settings.Get("REMOTE_PASSWORD");
        }
        /// <summary>
        /// Get ip address
        /// </summary>
        /// <returns></returns>
        public static string GetIpAddress()
        {
            var strHostName = Dns.GetHostName();
            var ipHostInfo = Dns.GetHostEntry(strHostName);
            var ipAddress = ipHostInfo.AddressList[0];

            return ipAddress.ToString();
        }

    }
}
using System.Reflection;
using System.Net;

namespace Insya.NetDash
{
    public static class Functions
    {

        public static string CurrentVersion
        {
            get
            {
                var thisApp = Assembly.GetExecutingAssembly();
                var name = new AssemblyName(thisApp.FullName);
                return name.Version.ToString(3);
            }
        }

        public static string GetIpAddress()
        {
            var strHostName = Dns.GetHostName();
            var ipHostInfo = Dns.GetHostEntry(strHostName);
            var ipAddress = ipHostInfo.AddressList[0];

            return ipAddress.ToString();
        }

    }
}
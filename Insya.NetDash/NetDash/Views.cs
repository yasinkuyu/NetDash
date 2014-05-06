using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Web;
using Insya.NetDash.Models;

namespace Insya.NetDash.NetDash
{
    public static class Views
    {

        public static dynamic get_platform()
        {
            dynamic returndata = new ExpandoObject();

            var osQuery = new SelectQuery("Select * from Win32_OperatingSystem");
            var searcher = new ManagementObjectSearcher(osQuery);

            foreach (var x in searcher.Get())
            {

                returndata.name = x["Name"].ToString().Split('|').First(); // Windows name and version

                var uptime = ManagementDateTimeConverter.ToDateTime(x.Properties["LastBootUpTime"].Value.ToString());

                returndata.uptime = uptime.ToString("HH"); // Uptime
                returndata.uptimeTick = (Environment.TickCount / (1000 * 60 * 60)) + "";

            }

            returndata.hostname = Dns.GetHostName(); //Hostname
            returndata.kernel = HttpContext.Current.Request.ServerVariables["SERVER_SOFTWARE"]; // Server

            return returndata;
        }

        public static dynamic get_cpus()
        {
            dynamic returndata = new ExpandoObject();

            var cpuQuery = new SelectQuery("Select Name, NumberOfCores, NumberOfLogicalProcessors from Win32_Processor");
            var searcher = new ManagementObjectSearcher(cpuQuery);

            foreach (var x in searcher.Get())
            {
                returndata.name = x["Name"].ToString(); // CPU Name
                returndata.cores = x["NumberOfCores"].ToString(); // Cores
                returndata.threads = x["NumberOfLogicalProcessors"].ToString(); // Threads
            }

            return returndata;
        }

        public static dynamic get_cpu_usage()
        {
            dynamic returndata = new ExpandoObject();

            var cpuQuery = new SelectQuery("Select PercentIdleTime from Win32_PerfFormattedData_PerfOS_Processor");
            var searcher = new ManagementObjectSearcher(cpuQuery);

            foreach (var x in searcher.Get())
            {
                var usage = (UInt64)x["PercentIdleTime"];
                var cpuUsed = 100 - usage;
                var cpuFree = usage;

                returndata.used = cpuUsed;
                returndata.free = cpuFree;
            }

            return returndata;
        }

        public static dynamic get_traffic(string ip)
        {
            dynamic returndata = new ExpandoObject();

            return returndata;
        }

        public static ArrayList get_netstat()
        {
            var stats = new ArrayList();
            var prop = IPGlobalProperties.GetIPGlobalProperties();
            var tcp = prop.GetActiveTcpConnections();

            var i = 1; //-> Fake number
            foreach (var info in tcp)
            {

                var stat = new ArrayList
                {
                    i,
                    info.LocalEndPoint.Address.ToString(),
                    info.LocalEndPoint.Port,
                    info.RemoteEndPoint.Address.ToString()
                };

                stats.Add(stat);

                i++;
            }

            return stats;
        }

        public static ArrayList get_users()
        {
            var scope = new ManagementScope(@"\\.\root\CIMV2");

            var users = new ArrayList();
            var logonQuery = new ObjectQuery("SELECT * FROM Win32_LogonSession"); //Where LogonType=2
            var session = new ManagementObjectSearcher(scope, logonQuery);

            foreach (var login in session.Get())
            {
                var loggedQuery = new ObjectQuery(
                    string.Format("Associators Of {{Win32_LogonSession.LogonId={0}}} Where AssocClass=Win32_LoggedOnUser Role=Dependent",
                        login["LogonId"].ToString()
                    )
                );

                var searcher = new ManagementObjectSearcher(scope, loggedQuery);

                foreach (var user in searcher.Get())
                {
                    var u = new ArrayList();
                    var startTime = ManagementDateTimeConverter.ToDateTime(login["StartTime"].ToString());

                    u.Add(user["Name"]);
                    u.Add(user["Domain"]);
                    u.Add(startTime.ToString("MMM dd, HH:mm"));

                    users.Add(u);
                }
            }

            return users;
        }

        public static ArrayList get_proc()
        {

            // Processes List
            var scope = new ManagementScope(@"\\.\root\CIMV2");
            var processes = new ArrayList();
            var procQuery = new SelectQuery("SELECT CSName, ProcessId, CreationDate,  KernelModeTime, UserModeTime, WorkingSetSize, CommandLine FROM Win32_Process");
            var searcher = new ManagementObjectSearcher(scope, procQuery);

            var pCommand = "";
            var pStart = "";
            var pCreate = "";
            foreach (var p in searcher.Get())
            {

                var pr = new ArrayList();
                var cpu = (((UInt64)p["KernelModeTime"] + (UInt64)p["UserModeTime"]) / 10000000);
                var mem = Convert.ToInt64(p["WorkingSetSize"].ToString()) / 1024;

                var pUser = p["CSName"].ToString();
                var pPid = p["ProcessId"].ToString();
                var pCpu = cpu.ToString();
                var pMem = string.Format("{0}", mem);

                if (p["CreationDate"] != null)
                {
                    var startTime = ManagementDateTimeConverter.ToDateTime(p["CreationDate"].ToString());
                    pStart = startTime.ToString("MMM dd");
                }

                if (p["CreationDate"] != null)
                {
                    var startTime = ManagementDateTimeConverter.ToDateTime(p["CreationDate"].ToString());
                    pCreate = startTime.ToString("HH:mm");
                }

                if (p["CommandLine"] != null)
                    pCommand = p["CommandLine"].ToString().Split('\\').Last();

                pr.Add(pUser); // User
                pr.Add(pPid); // Pid
                pr.Add(pCpu); // Cpu%
                pr.Add(pMem); // Mem%
                pr.Add(pStart); // Start
                pr.Add(pCreate); // Time
                pr.Add(pCommand); // Command

                processes.Add(pr);
            }

            return processes;
        }

        public static dynamic get_traffic()
        {
       
            dynamic returndata = new ExpandoObject();
            
            ManagementObjectCollection collection;
            using (var searcher = new ManagementObjectSearcher("root\\CIMV2", "Select BytesReceivedPersec, BytesSentPersec, BytesTotalPersec FROM Win32_PerfFormattedData_Tcpip_NetworkInterface"))
                collection = searcher.Get();

            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            var mo = enumerator.Current;

            var recv = (float)(ulong)mo["BytesReceivedPersec"];
            var sent = (float)(ulong)mo["BytesSentPersec"];

            returndata.recv = recv;
            returndata.sent = sent;

            //if (enumerator.MoveNext()) ;
            enumerator.MoveNext();

            return returndata;

        }


        public static ArrayList get_ips()
        {
            var datasets = new ArrayList();
            var search = new ManagementObjectSearcher("root\\CIMV2", "Select Caption, IPAddress, MACAddress From Win32_NetworkAdapterConfiguration WHERE IPEnabled = 'TRUE'");
            var count = 0; //-> fake adapter name

            foreach (var x in search.Get())
            {

                var dataset = new ArrayList();
                var ips = (string[])x["IPAddress"];

                dataset.Add("eth" + count); //x["Caption"]
                dataset.Add(ips[0]); //-> IP v4
                dataset.Add(ips[1]); //-> IP v6
                dataset.Add(x["MACAddress"]);

                datasets.Add(dataset);

                count++;
            }

            return datasets;
        }

        public static IList<Disk> get_disk()
        {
            var disks = new List<Disk>();

            var diskQuery = new SelectQuery("Select FreeSpace,Size,Name,VolumeName from Win32_LogicalDisk where DriveType=3");
            var searcher = new ManagementObjectSearcher(diskQuery);

            foreach (var d in searcher.Get())
            {

                var availableValue = d["FreeSpace"];
                var totalSizeValue = d["Size"];

                var total = totalSizeValue == null ? 0 : (ulong)totalSizeValue;
                var free = availableValue == null ? 0 : (ulong)availableValue;
                var used = total - free;
                var percentUsed = 0;

                if (total > 0)
                {
                    percentUsed = (int)(((float)used / (float)total) * 100);
                }

                var disk = new Disk(
                        name: d["Name"].ToString(),
                        volume: d["VolumeName"].ToString(),
                        total: total,
                        used: used,
                        free: free,
                        perUsed: percentUsed
                );

                disks.Add(disk);

            }

            return disks;
        }

        public static dynamic get_disk_rw()
        {
            dynamic returndata = new ExpandoObject();

            var uptimeQuery = new SelectQuery("Select DiskReadBytesPersec, DiskWriteBytesPersec from Win32_PerfRawData_PerfDisk_PhysicalDisk");
            var searcher = new ManagementObjectSearcher(uptimeQuery);

            foreach (var x in searcher.Get())
            {
                var read = Convert.ToInt32(Math.Round(Convert.ToDecimal((((UInt64)x["DiskReadBytesPersec"] / 1024 / 1024 / 1024)))));
                var write = Convert.ToInt32(Math.Round(Convert.ToDecimal((((UInt64)x["DiskWriteBytesPersec"] / 1024 / 1024 / 1024)))));

                returndata.read = read;
                returndata.write = write;

            }

            return returndata;
        }

        public static  dynamic get_mem()
        {
            dynamic returndata = new ExpandoObject();

            var search = new ManagementObjectSearcher("root\\CIMV2", "Select TotalVisibleMemorySize, FreePhysicalMemory from Win32_OPeratingSystem");

            foreach (var x in search.Get())
            {

                var totalMemory = (ulong)x["TotalVisibleMemorySize"];
                var freeMemory = (ulong)x["FreePhysicalMemory"];

                // -> KB to MB
                var totalmem = (double)totalMemory / 1024; ;
                var freemem = Convert.ToInt32(Math.Round(Convert.ToDecimal((((double)freeMemory / 1024)))));
                var usedmem = Convert.ToInt32(Math.Round(Convert.ToDecimal((((totalmem - freemem))))));

                // Multiple return object
                returndata.total = totalmem;
                returndata.usage = usedmem;
                returndata.free = freemem;
            }

            return returndata;
        }

    }
}
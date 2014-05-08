// Copyright (c) 2014, Insya Interaktif.
// Developer @yasinkuyu
// All rights reserved.

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

namespace Insya.NetDash
{
    public static class Views
    {

        public static dynamic get_platform()
        {

            var connection = new ConnectionOptions
            {
                Username = Functions.GetServerUserName(),
                Password = Functions.GetServerPassword(),
                //Authority = string.Format("ntlmdomain:{0}", Functions.GetServerDomain())
            };

            dynamic returndata = new ExpandoObject();

            var scope = new ManagementScope(Functions.GetServerName());

            if (Functions.GetServerName() != "." || Functions.GetServerName() != "root")
                scope.Connect();

            var query = new SelectQuery("Select Name, LastBootUpTime from Win32_OperatingSystem");
            var searcher = new ManagementObjectSearcher(scope, query);

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

            var scope = new ManagementScope(Functions.GetServerName());
            var query = new SelectQuery("Select Name, NumberOfCores, NumberOfLogicalProcessors from Win32_Processor");
            var searcher = new ManagementObjectSearcher(scope, query);

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


            var scope = new ManagementScope(Functions.GetServerName());
            var query = new SelectQuery("Select PercentIdleTime from Win32_PerfFormattedData_PerfOS_Processor");
            var searcher = new ManagementObjectSearcher(scope, query);

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
            var scope = new ManagementScope(Functions.GetServerName());

            var users = new ArrayList();
            var query = new ObjectQuery("SELECT * FROM Win32_LogonSession"); //Where LogonType=2
            var session = new ManagementObjectSearcher(scope, query);

            foreach (var login in session.Get())
            {
                var loggedQuery = new ObjectQuery(
                    string.Format("Associators Of {{Win32_LogonSession.LogonId={0}}} Where AssocClass=Win32_LoggedOnUser Role=Dependent",
                        login["LogonId"].ToString()
                    )
                );

                var searcher = new ManagementObjectSearcher(scope, loggedQuery);

                foreach (var x in searcher.Get())
                {
                    var u = new ArrayList();
                    var startTime = ManagementDateTimeConverter.ToDateTime(login["StartTime"].ToString());

                    u.Add(x["Name"]);
                    u.Add(x["Domain"]);
                    u.Add(startTime.ToString("MMM dd, HH:mm"));

                    users.Add(u);
                }
            }

            return users;
        }

        public static ArrayList get_proc()
        {

            // Processes List
            var processes = new ArrayList();

            var scope = new ManagementScope(Functions.GetServerName());
            var query = new SelectQuery("SELECT CSName, ProcessId, CreationDate,  KernelModeTime, UserModeTime, WorkingSetSize, CommandLine FROM Win32_Process");
            var searcher = new ManagementObjectSearcher(scope, query);

            var pCommand = "";
            var pStart = "";
            var pCreate = "";

            foreach (var x in searcher.Get())
            {

                var pr = new ArrayList();
                var cpu = (((UInt64)x["KernelModeTime"] + (UInt64)x["UserModeTime"]) / 10000000);
                var mem = Convert.ToInt64(x["WorkingSetSize"].ToString()) / 1024;

                var pUser = x["CSName"].ToString();
                var pPid = x["ProcessId"].ToString();
                var pCpu = cpu.ToString();
                var pMem = string.Format("{0}", mem);

                if (x["CreationDate"] != null)
                {
                    var startTime = ManagementDateTimeConverter.ToDateTime(x["CreationDate"].ToString());
                    pStart = startTime.ToString("MMM dd");
                }

                if (x["CreationDate"] != null)
                {
                    var startTime = ManagementDateTimeConverter.ToDateTime(x["CreationDate"].ToString());
                    pCreate = startTime.ToString("HH:mm");
                }

                if (x["CommandLine"] != null)
                    pCommand = x["CommandLine"].ToString().Split('\\').Last();

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

            var scope = new ManagementScope(Functions.GetServerName());
            var query = new SelectQuery("SELECT Name, BytesReceivedPerSec, BytesSentPerSec FROM Win32_PerfRawData_Tcpip_NetworkInterface"); 
            var searcher = new ManagementObjectSearcher(scope, query);

            foreach (var x in searcher.Get())
            {

                var name = x["Name"].ToString();
                var recv = x["BytesReceivedPerSec"];
                var sent = x["BytesSentPerSec"];

                returndata.name = name;
                returndata.recv = recv;
                returndata.sent = sent;

            }

            return returndata;

        }

        public static dynamic get_loadaverage()
        {
            dynamic returndata = new ExpandoObject();

            var scope = new ManagementScope(Functions.GetServerName());
            var query = new SelectQuery("Select * From Win32_PerfFormattedData_PerfOS_System");
            var searcher = new ManagementObjectSearcher(scope, query);

            foreach (var x in searcher.Get())
            {

                returndata.uptime = x["SystemUpTime"];
                returndata.procs = x["Processes"];
                returndata.threads = x["Threads"];

            }

            return returndata;
        }

        public static ArrayList get_ips()
        {
            var datasets = new ArrayList();

            var scope = new ManagementScope(Functions.GetServerName());
            var query = new SelectQuery("Select Caption, IPAddress, MACAddress From Win32_NetworkAdapterConfiguration WHERE IPEnabled = 'TRUE'");
            var searcher = new ManagementObjectSearcher(scope, query);
            var count = 0; //-> fake adapter name

            foreach (var x in searcher.Get())
            {

                var dataset = new ArrayList();
                var ips = (string[])x["IPAddress"];

                dataset.Add("eth" + count);   //x["Caption"]
                dataset.Add(ips[0]);          //-> IP v4
                dataset.Add(ips[1]);          //-> IP v6
                dataset.Add(x["MACAddress"]); //-> Mac address

                datasets.Add(dataset);

                count++;
            }

            return datasets;
        }

        public static IList<Disk> get_disk()
        {
            var disks = new List<Disk>();

            var scope = new ManagementScope(Functions.GetServerName());
            var query = new SelectQuery("Select FreeSpace,Size,Name,VolumeName from Win32_LogicalDisk where DriveType=3");
            var searcher = new ManagementObjectSearcher(scope, query);

            foreach (var x in searcher.Get())
            {

                var availableValue = x["FreeSpace"];
                var totalSizeValue = x["Size"];

                var total = totalSizeValue == null ? 0 : (ulong)totalSizeValue;
                var free = availableValue == null ? 0 : (ulong)availableValue;
                var used = total - free;
                var percentUsed = 0;

                if (total > 0)
                {
                    percentUsed = (int)(((float)used / (float)total) * 100);
                }

                var disk = new Disk(
                        name: x["Name"].ToString(),
                        volume: x["VolumeName"].ToString(),
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

            var scope = new ManagementScope(Functions.GetServerName());
            var query = new SelectQuery("Select DiskReadBytesPersec, DiskWriteBytesPersec from Win32_PerfRawData_PerfDisk_PhysicalDisk");
            var searcher = new ManagementObjectSearcher(scope, query);

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

            var scope = new ManagementScope(Functions.GetServerName());
            var query = new SelectQuery("Select TotalVisibleMemorySize, FreePhysicalMemory from Win32_OPeratingSystem");
            var searcher = new ManagementObjectSearcher(scope, query);

            foreach (var x in searcher.Get())
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
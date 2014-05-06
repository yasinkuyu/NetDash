using System;
using System.Collections;
using System.Diagnostics;
using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using Insya.NetDash.NetDash;

namespace Insya.NetDash.Controllers
{
    [Authorize]
    public class InfoController : Controller
    {

        [GET("info/uptime")]
        public ActionResult GetUptime()
        {
            var os = Views.get_platform();

            return Json(os.uptime, JsonRequestBehavior.AllowGet);
        }

        [GET("info/platform/{name?}")]
        public ActionResult GetOs(string name)
        {
            var os = Views.get_platform();

            var data = "None";

            if (name == "hostname")
                data = os.hostname;

            if (name == "uptime")
                data = os.uptime;

            if (name == "osname")
                data = os.name;

            if (name == "kernel")
                data = os.kernel;

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [GET("info/getcpus/{name?}")]
        public ActionResult GetCpus(string name)
        {

            var data = "None";
            var proc = Views.get_cpus();

            if (name == "type")
                data = proc.name;

            if (name == "count")
                data = proc.cores;

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [GET("info/cpuusage")]
        public ActionResult GetCpuUsage()
        {
            var cpuUsage = Views.get_cpu_usage();

            var cpu = new[] { 
                new { 
                    color = "#0AD11B", // Free
                    value = cpuUsage.free
                }, 
                new { 
                    color = "#F7464A", // Used
                    value =  cpuUsage.used
                } 
            };

            return Json(cpu, JsonRequestBehavior.AllowGet);
        }

        [GET("info/getdisk")]
        public ActionResult GetDisk()
        {

            var disks = new ArrayList();

            foreach (var item in Views.get_disk())
            {

                var disk = new ArrayList
                {
                    string.Format("{0}", item.Volume),
                    string.Format("{0:#,###.##}GB", item.Total/1024/1024/1024),
                    string.Format("{0:#,###.##}GB", item.Used/1024/1024/1024),
                    string.Format("{0:#,###.##}GB", item.Free/1024/1024/1024),
                    string.Format("{0:###.##}%", item.PerUsed),
                    string.Format("{0}\\", item.Name)
                };

                disks.Add(disk);
            }

            return Json(disks, JsonRequestBehavior.AllowGet);

        }

        [GET("info/getdiskio")]
        public ActionResult GetDisko()
        {

            var datasetRead = new ArrayList();
            var datasetWrite = new ArrayList();

            var diskRead = Settings.GetList("disko_read");
            var diskWrite = Settings.GetList("disko_write");

            var disk = Views.get_disk_rw();

            if (diskRead.Count == 0)
            {
                datasetRead.Add(0);
                datasetWrite.Add(0);

                Settings.Set("disko_read", disk.read.ToString());
                Settings.Set("disko_write", disk.read.ToString());
            }
            else
            {

                if (diskRead.Count > 10)
                {
                    diskRead.RemoveAt(0);
                    diskWrite.RemoveAt(0);

                    Settings.Set("disko_read", string.Join(" ", diskRead));
                    Settings.Set("disko_write", string.Join(" ", diskWrite));
                }
                else
                {
                    Settings.Set("disko_read", string.Format("{0} {1}", string.Join(" ", diskRead), disk.read));
                    Settings.Set("disko_write", string.Format("{0} {1}", string.Join(" ", diskWrite), disk.write));
                }

                foreach (var item in diskRead)
                    datasetRead.Add(Convert.ToUInt64(item));

                foreach (var item in diskWrite)
                    datasetWrite.Add(Convert.ToUInt64(item));

            }


            string[] labels = { "", "", "", "", "", "", "", "", "", "", "" };

            var datasets = new[] { 
                new { 
                    pointColor = "rgba(245,134,15,1)", 
                    strokeColor = "rgba(245,134,15,1)",
                    data = datasetRead,
                    fillColor = "rgba(245,134,15,0.5)",
                    pointStrokeColor = "#fff",

                },
                new { 
                    pointColor = "rgba(15,103,245,1)", 
                    strokeColor = "rgba(15,103,245,1)",
                    data = datasetWrite,
                    fillColor = "rgba(15,103,245,0.5)",
                    pointStrokeColor = "#fff",

                }
            };

            return Json(new { labels = labels, datasets = datasets }, JsonRequestBehavior.AllowGet);
        }

        [GET("info/memory")]
        public ActionResult GetMemory()
        {

            string[] labels = { "", "", "", "", "", "", "", "", "", "" };

            var datasetsFree = new ArrayList();
            var datasetsUsed = new ArrayList();

            var memUsage = Settings.GetList("memory_usage");
            var memFree = Settings.GetList("memory_free");

            var memory = Views.get_mem();

            if (memUsage.Count == 0)
            {
                datasetsUsed.Add(0);
                datasetsFree.Add(0);
                Settings.Set("memory_usage", memory.usage.ToString());
                Settings.Set("memory_free", memory.free.ToString());
            }
            else
            {
                 
                if (memUsage.Count > 10)
                {
                    memUsage.RemoveAt(0);
                    memFree.RemoveAt(0);

                    Settings.Set("memory_usage", string.Join(" ", memUsage));
                    Settings.Set("memory_free", string.Join(" ", memFree));
                }
                else
                {
                    Settings.Set("memory_usage", string.Format("{0} {1}", string.Join(" ", memUsage), memory.usage));
                    Settings.Set("memory_free", string.Format("{0} {1}", string.Join(" ", memFree), memory.free));
                }

                foreach (var item in memUsage)
                    datasetsUsed.Add(Convert.ToDouble(item));

                foreach (var item in memFree)
                    datasetsFree.Add(Convert.ToDouble(item));
            }
 
            var datasets = new[] { 
                new { 
                    pointColor = "rgba(249,134,33,1)", 
                    strokeColor = "rgba(249,134,33,1)",
                    data = datasetsUsed,
                    fillColor = "rgba(249,134,33,0.5)",
                    pointStrokeColor = "#fff",

                },
                new
                { 
                    pointColor = "rgba(43,214,66,1)", 
                    strokeColor = "rgba(43,214,66,1)",
                    data = datasetsFree,
                    fillColor = "rgba(43,214,66,0.5)",
                    pointStrokeColor = "#fff",

                }
            };

            return Json(new { labels = labels, datasets = datasets }, JsonRequestBehavior.AllowGet);
        }

        [GET("info/getips")]
        public ActionResult GetGetIPs()
        {

            var datasets = Views.get_ips();

            return Json(datasets, JsonRequestBehavior.AllowGet);
        }

        [GET("info/proc")]
        public ActionResult GetProc()
        {

            var processes = Views.get_proc();

            return Json(processes, JsonRequestBehavior.AllowGet);
        }

        [GET("info/loadaverage")]
        public ActionResult GetLoadAverage()
        {

            TimeSpan upTime;
            using (var pc = new PerformanceCounter("System", "System Up Time"))
            {
                pc.NextValue();
                upTime = TimeSpan.FromSeconds(pc.NextValue());
            }

            var labels = new[] { 
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
            };

            var random = new Random();

            var data2 = new[] { 
                random.Next(0, 1),
                random.Next(0, 1),
                random.Next(0, 2),
                random.Next(0, 1),
                random.Next(0, 2),
                random.Next(0, 2),
                random.Next(0, 2),
                random.Next(0, 1),
                1.1,
                random.Next(0, 2),
            };

            var datasets = new[] { 
                new { 
                    pointColor = "rgba(151,187,205,1)", 
                    strokeColor = "rgba(151,187,205,1)",
                    data = data2,
                    fillColor = "rgba(151,187,205,0.5)",
                    pointStrokeColor = "#fff",

                }
            };

            return Json(new { labels = labels, datasets = datasets }, JsonRequestBehavior.AllowGet);
        }

        [GET("info/gettraffic")]
        public ActionResult GetTraffic()
        {

            string[] labels = { "", "", "", "", "", "", "", "", "", "", "" };
            
            var datasetRecv = new ArrayList();
            var datasetSent = new ArrayList();

            var trafficRevc = Settings.GetList("traffic_recv");
            var trafficSent = Settings.GetList("traffic_sent");

            var traffic = Views.get_traffic();

            if (trafficRevc.Count == 0)
            {
                datasetRecv.Add(0);
                datasetSent.Add(0);
                Settings.Set("traffic_recv", traffic.recv.ToString());
                Settings.Set("traffic_sent", traffic.sent.ToString());
            }
            else
            {

                if (trafficRevc.Count > 10)
                {
                    trafficRevc.RemoveAt(0);
                    trafficSent.RemoveAt(0);

                    Settings.Set("traffic_recv", string.Join(" ", trafficRevc));
                    Settings.Set("traffic_sent", string.Join(" ", trafficSent));
                }
                else
                {
                    Settings.Set("traffic_recv", string.Format("{0} {1}", string.Join(" ", trafficRevc), traffic.recv));
                    Settings.Set("traffic_sent", string.Format("{0} {1}", string.Join(" ", trafficSent), traffic.sent));
                }

                foreach (var item in trafficRevc)
                    datasetRecv.Add(Convert.ToDouble(item));

                foreach (var item in trafficSent)
                    datasetSent.Add(Convert.ToDouble(item));
                 
            }
            
            var datasets = new[] { 
                new { 
                    pointColor = "rgba(105,210,231,1)", 
                    strokeColor = "rgba(105,210,231,1)",
                    data = datasetRecv,
                    fillColor = "rgba(105,210,231,0.5)",
                    pointStrokeColor = "#fff",

                },
                new { 
                    pointColor = "rgba(227,48,81,1)", 
                    strokeColor = "rgba(227,48,81,1)",
                    data = datasetSent,
                    fillColor = "rgba(227,48,81,0.5)",
                    pointStrokeColor = "#fff",

                }
            };

            return Json(new { labels = labels, datasets = datasets }, JsonRequestBehavior.AllowGet);

        }

        [GET("info/getnetstat")]
        public ActionResult GetNetstat()
        {

            var stats = Views.get_netstat();

            return Json(stats, JsonRequestBehavior.AllowGet);
        }

        [GET("__browserLink/requestData/{name?}")]
        public ActionResult GetBrowserLink(string name)
        {
            // Todo
            return Json("browser link test", JsonRequestBehavior.AllowGet);
        }

        [GET("info/getusers")]
        public ActionResult GetUsers()
        {
            var users = Views.get_users();

            return Json(users, JsonRequestBehavior.AllowGet);
        }


    }
}

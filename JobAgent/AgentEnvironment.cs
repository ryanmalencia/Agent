using DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;

namespace JobAgent
{
    public static class AgentEnvironment
    {
        public static bool HasTask = false;
        public static string Agent_Name;
        public static string IP;
        public static string Job_Name = "";

        /// <summary>
        /// Set state to running job
        /// </summary>
        /// <param name="job"></param>
        public static void SetJob(Job job)
        {
            HasTask = true;
            Job_Name = job.JobName;
        }

        /// <summary>
        /// Set state to idle
        /// </summary>
        public static void SetIdle()
        {
            HasTask = false;
            Job_Name = "";
        }

        /// <summary>
        /// Get a string that is representative of this machine's hardware
        /// </summary>
        /// <returns>Hardware string</returns>
        public static string GetHardware()
        {
            string Hardware = "";
            Hardware += GetDiskSpace();
            Hardware += GetRamUsage();
            Hardware += GetCPUUsage();
            return Hardware;
        }

        /// <summary>
        /// Get a string containing the free space on each drive
        /// </summary>
        /// <returns>String containing free space on each drive</returns>
        private static string GetDiskSpace()
        {
            string space = string.Empty;
            ManagementClass mc = new ManagementClass("Win32_LogicalDisk");
            ManagementObjectCollection col = mc.GetInstances();
            foreach (ManagementObject obj in col)
            {
                space += "!Free space on " + obj["Name"] + " drive: " + (int)(Double.Parse(obj["FreeSpace"].ToString()) / 1024 / 1024 / 1024) + " GB!";
            }
            return space;
        }

        /// <summary>
        /// Get a string containing ram usage
        /// </summary>
        /// <returns>String containing ram usage</returns>
        private static string GetRamUsage()
        {
            string usage = string.Empty;
            ManagementClass mc = new ManagementClass("Win32_OperatingSystem");
            ManagementObjectCollection col = mc.GetInstances();
            foreach (ManagementObject obj in col)
            {
                int total = Int32.Parse(obj["TotalVisibleMemorySize"].ToString()) / 1024;
                int free = total - Int32.Parse(obj["FreePhysicalMemory"].ToString()) / 1024;
                int percent = (free * 100) / total;
                usage += "$Ram Usage: " + free + "/" + total + " MB (" + percent + "%)$";
            }
            return usage;
        }

        private static string GetCPUUsage()
        {
            string usage = string.Empty;

            ManagementClass mc = new ManagementClass("Win32_Processor");
            ManagementObjectCollection col = mc.GetInstances();
            foreach (ManagementObject obj in col)
            {
                if (obj["LoadPercentage"] != null)
                {
                    int percent = Int32.Parse(obj["LoadPercentage"].ToString());
                    usage += "&CPU Usage: " + percent + "%&";
                }
            }
            return usage;
        }
    }
}

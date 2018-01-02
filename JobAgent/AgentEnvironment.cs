using DataTypes;
using System;
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
        /// <param name="job">Job to give to agent</param>
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
        public static Hardware GetHardware()
        {
            return new Hardware(Agent_Name, GetDiskSpace(), GetCPUUsage(), GetRamUsage());
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
                if(obj["Name"].ToString().Contains("C"))
                {
                    int total = (int)(Double.Parse(obj["Size"].ToString()) / 1024 / 1024 / 1024);
                    int available = total - (int)(Double.Parse(obj["FreeSpace"].ToString()) / 1024 / 1024 / 1024);
                    int percent = (available * 100)/ total;
                    space += "Drive Usage: " + available + "/" + total + " GB (" + percent + "%)";
                }
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
                usage += "Ram Usage: " + free + "/" + total + " MB (" + percent + "%)";
            }
            return usage;
        }

        /// <summary>
        /// Get a string containing CPU usage
        /// </summary>
        /// <returns>String containing CPU usage</returns>
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
                    usage += "CPU Usage: " + percent + "%";
                }
            }
            return usage;
        }
    }
}

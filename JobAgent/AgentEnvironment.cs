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
                space += "Free space on " + obj["Name"] + " drive: " + (int)(Double.Parse(obj["FreeSpace"].ToString()) / 1024 / 1024 / 1024) + " GB;";
            }
            return space;
        }
    }
}

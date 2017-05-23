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

        public static void SetJob(Job job)
        {
            HasTask = true;
            Job_Name = job.JobName;
        }

        public static void SetIdle()
        {
            HasTask = false;
            Job_Name = "";
        }

        public static string GetHardware()
        {
            string Hardware = "";
            List<string> specs = new List<string>() { "Win32_LogicalDisk", "Win32_DiskDrive", "Win32_PhysicalMemory", "Win32_MemoryDevice" };

            foreach(string str in specs)
            {
                ManagementClass mc = new ManagementClass(str);
                ManagementObjectCollection col = mc.GetInstances();

                foreach(ManagementObject obj in col)
                {
                    Hardware += "Free space on " + obj["Name"] + " drive: " + obj["FreeSpace"] + ";";
                }
            }

            return Hardware;
        }
    }
}

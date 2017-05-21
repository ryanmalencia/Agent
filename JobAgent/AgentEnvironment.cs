using DataTypes;

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
    }
}

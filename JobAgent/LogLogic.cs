using System;
using DataTypes;
using WebAPIClient.APICalls;

namespace JobAgent
{
    public class LogLogic
    {
        public static void ErrorLog(string message)
        {
            Log log = new Log(DateTime.Now, AgentEnvironment.Agent_Name, message, 1);
            Console.WriteLine(message);
            LogAPI.AddLog(log);
        }

        public static void WarnLog(string message)
        {
            Log log = new Log(DateTime.Now, AgentEnvironment.Agent_Name, message, 2);
            Console.WriteLine(message);
            LogAPI.AddLog(log);
        }

        public static void InfoLog(string message)
        {
            Log log = new Log(DateTime.Now, AgentEnvironment.Agent_Name, message, 3);
            Console.WriteLine(message);
            LogAPI.AddLog(log);
        }

        public static void DebugLog(string message)
        {
            Log log = new Log(DateTime.Now, AgentEnvironment.Agent_Name, message, 4);
            Console.WriteLine(message);
            LogAPI.AddLog(log);
        }
    }
}

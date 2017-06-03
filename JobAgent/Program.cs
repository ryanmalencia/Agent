using log4net;
using Microsoft.Owin.Hosting;
using System;

namespace JobAgent
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        static void Main(string[] args)
        {
            string baseAddress = "http://" + StartupLogic.GetLocalIPAddress() + ":7777/";
            StartupLogic.InitializeAgent();
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                AgentLogic.SetIdle();
                Console.WriteLine("JobAgent started. Reachable at this IP: " + StartupLogic.GetLocalIPAddress());
                Console.ReadLine();
            }
        }
    }
}

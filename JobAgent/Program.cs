using Microsoft.Owin.Hosting;
using System;

namespace JobAgent
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseAddress = "http://" + StartupLogic.GetLocalIPAddress() + ":7777/";
            StartupLogic.InitializeAgent();
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                AgentLogic.SetIdle();
                LogLogic.InfoLog("JobAgent started. Reachable at this IP: " + StartupLogic.GetLocalIPAddress());
                Console.ReadLine();
            }
        }
    }
}

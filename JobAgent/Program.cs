using System;
using Microsoft.Owin.Hosting;
using JobAgent.SignalR;

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
                Console.WriteLine("JobAgent started. Reachable at this IP: " + StartupLogic.GetLocalIPAddress());

                Console.ReadLine();
            }
        }
    }
}

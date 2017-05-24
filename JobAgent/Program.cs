using System;
using System.Threading;
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
            Thread HardwareMonitor = new Thread(StartHardwareThread);
            HardwareMonitor.Start();
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                AgentLogic.SetIdle();
                Console.WriteLine("JobAgent started. Reachable at this IP: " + StartupLogic.GetLocalIPAddress());
                Console.ReadLine();
            }
        }

        private static void StartHardwareThread()
        {
            while (true)
            {
                AgentLogic.UpdateHardware();
                Thread.Sleep(1500);
            }
        }
    }
}

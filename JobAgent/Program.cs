using System;
using Microsoft.Owin.Hosting;
using System.Net;
using System.Net.Sockets;
using WebAPIClient.APICalls;
using DataTypes;

namespace JobAgent
{
    class Program
    {
        static void Main(string[] args)
        {
            //string baseAddress = "http://" + System.Environment.MachineName + ":9999/";
            string baseAddress = "http://" + GetLocalIPAddress() + ":7777/";
            AgentEnvironment.Agent_Name = GetHostName();
            Agent agent = AgentAPI.GetAgent(AgentEnvironment.Agent_Name);
            if(agent == null)
            {
                agent = new Agent(AgentEnvironment.Agent_Name, ip: GetLocalIPAddress() + ":7777");
                AgentAPI.AddAgent(agent);
            }
            AgentAPI.SetIdle(AgentEnvironment.Agent_Name);
            //AgentCollection collection = AgentAPI.GetAllAgents();
            //AgentAPI.GiveAgentJob(AgentEnvironment.agent_name);
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                Console.WriteLine("JobAgent started. Reachable at this IP: " + GetLocalIPAddress());
                Console.ReadLine();
            }

        }

        /// <summary>
        /// get local ip adress
        /// </summary>
        /// <returns>ip address as string</returns>
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }

        public static string GetHostName()
        {
            return Dns.GetHostName();
        }
    }
}

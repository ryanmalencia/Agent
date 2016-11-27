using System;
using Microsoft.Owin.Hosting;
using System.Net;
using System.Net.Sockets;
using WebAPIClient.APICalls;
using DataTypes;
using NetFwTypeLib;

namespace JobAgent
{
    class Program
    {
        static void Main(string[] args)
        {
            //string baseAddress = "http://" + System.Environment.MachineName + ":9999/";
            string baseAddress = "http://" + GetLocalIPAddress() + ":7777/";
            CheckForPortAccess();
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

        /// <summary>
        /// Get HostName
        /// </summary>
        /// <returns>string of Hostname</returns>
        public static string GetHostName()
        {
            return Dns.GetHostName();
        }

        public static void CheckForPortAccess()
        {
            INetFwMgr icfMgr = null;
            try
            {
                Type TicfMgr = Type.GetTypeFromProgID("HNetCfg.FwMgr");
                icfMgr = (INetFwMgr)Activator.CreateInstance(TicfMgr);
            }
            catch (Exception)
            {
                return;
            }

            try
            {
                INetFwProfile profile;
                INetFwOpenPort portClass;
                Type TportClass = Type.GetTypeFromProgID("HNetCfg.FWOpenPort");
                portClass = (INetFwOpenPort)Activator.CreateInstance(TportClass);

                // Get the current profile
                profile = icfMgr.LocalPolicy.CurrentProfile;

                // Set the port properties
                portClass.Scope = NetFwTypeLib.NET_FW_SCOPE_.NET_FW_SCOPE_ALL;
                portClass.Enabled = true;
                portClass.Protocol = NetFwTypeLib.NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
                portClass.Name = "JobAgent";
                portClass.Port = 7777;

                // Add the port to the ICF Permissions List
                profile.GloballyOpenPorts.Add(portClass);
                return;
            }
            catch (Exception)
            {

            }
        }
    }
}

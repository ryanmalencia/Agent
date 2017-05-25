using DataTypes;
using NetFwTypeLib;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using WebAPIClient.APICalls;

namespace JobAgent
{
    public class StartupLogic
    {
        public static void InitializeAgent()
        {
            AllowPortAccess();
            Thread HardwareMonitor = new Thread(StartHardwareThread);
            HardwareMonitor.Start();
            AgentEnvironment.Agent_Name = GetHostName();
            AgentEnvironment.IP = GetLocalIPAddress() + ":7777";
            Agent agent = AgentAPI.GetAgent(AgentEnvironment.Agent_Name);
            if (agent == null)
            {
                agent = new Agent(AgentEnvironment.Agent_Name, ip: AgentEnvironment.IP);
                AgentAPI.AddAgent(agent);
            }
            if (AgentEnvironment.IP != agent.IP)
            {
                agent.IP = AgentEnvironment.IP;
                AgentAPI.Update(agent);
            }
        }

        /// <summary>
        /// Create firewall rule to allow communication to this agent
        /// </summary>
        public static void AllowPortAccess()
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
                profile = icfMgr.LocalPolicy.CurrentProfile;
                portClass.Scope = NET_FW_SCOPE_.NET_FW_SCOPE_ALL;
                portClass.Enabled = true;
                portClass.Protocol = NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
                portClass.Name = "JobAgent";
                portClass.Port = 7777;
                profile.GloballyOpenPorts.Add(portClass);
                return;
            }
            catch (Exception) { }
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

        /// <summary>
        /// Hardware thread
        /// </summary>
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

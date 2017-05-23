using System;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace JobAgent.SignalR
{
    public class AgentStatus
    {
        private readonly static Lazy<AgentStatus> _instance = new Lazy<AgentStatus>(() => new AgentStatus(GlobalHost.ConnectionManager.GetHubContext<AgentStatusHub>().Clients));


        private AgentStatus(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;
        }

        private IHubConnectionContext<dynamic> Clients
        {
            get;
            set;
        }

        public static AgentStatus Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        public void UpdateJob(string message)
        {
            Clients.All.updateJob(message);
        }

        public void UpdateHardware(string hardware)
        {
            Clients.All.updateHardware(hardware);
        }
    }
}
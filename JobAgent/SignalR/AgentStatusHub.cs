using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace JobAgent.SignalR
{
    [HubName("AgentStatusHub")]
    public class AgentStatusHub : Hub
    {
        private readonly AgentStatus _agentStatus;

        public AgentStatusHub() : this(AgentStatus.Instance) { }

        public AgentStatusHub(AgentStatus agentStatus)
        {
            _agentStatus = agentStatus;
        }
    }
}
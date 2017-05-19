using System;
using WebAPIClient.APICalls;

namespace JobAgent
{
    public class AgentLogic
    {
        /// <summary>
        /// Set machine to running status
        /// </summary>
        /// <param name="pk_job">Job pk</param>
        public static void SetRunning(int pk_job)
        {
            AgentAPI.GiveAgentJob(AgentEnvironment.Agent_Name, pk_job);
        }

        /// <summary>
        /// Set machine to idle status
        /// </summary>
        public static void SetIdle()
        {
            AgentAPI.SetIdle(AgentEnvironment.Agent_Name);
        }

        /// <summary>
        /// Terminate the program
        /// </summary>
        public static void Kill()
        {
            Console.WriteLine("Killing machine");
            AgentAPI.SetDead(AgentEnvironment.Agent_Name);
            Environment.Exit(1);
        }
    }
}

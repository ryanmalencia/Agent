using System;
using System.Diagnostics;
using DataTypes;
using WebAPIClient.APICalls;
using JobAgent.SignalR;

namespace JobAgent
{
    /// <summary>
    /// Logic actions for Agent
    /// </summary>
    public class AgentLogic
    {
        /// <summary>
        /// Set machine to running status
        /// </summary>
        /// <param name="pk_job">Job pk</param>
        public static void SetRunning(Job job)
        {
            AgentEnvironment.SetJob(job);
            AgentAPI.GiveAgentJob(AgentEnvironment.Agent_Name, job.JobID);
        }

        /// <summary>
        /// Set machine to idle status
        /// </summary>
        public static void SetIdle()
        {
            AgentEnvironment.SetIdle();
            AgentAPI.SetIdle(AgentEnvironment.Agent_Name);
        }

        /// <summary>
        /// Terminate the program
        /// </summary>
        public static void Kill()
        {
            //Console.WriteLine("Killing machine");
            LogLogic.InfoLog("Stopping agent");
            AgentAPI.SetDead(AgentEnvironment.Agent_Name);
            Environment.Exit(1);
        }

        /// <summary>
        /// Shutdown the machine
        /// </summary>
        public static void Shutdown()
        {
            LogLogic.InfoLog("Shutting agent down");
            AgentAPI.SetDead(AgentEnvironment.Agent_Name);
            Process.Start("shutdown", "/s /t 0");
        }

        /// <summary>
        /// Send a string with updated hardware information
        /// </summary>
        public static void UpdateHardware()
        {
            AgentStatus.Instance.UpdateHardware(AgentEnvironment.GetHardware());
        }
    }
}

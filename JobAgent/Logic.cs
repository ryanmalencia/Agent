using DataTypes;
using System.Threading;
using System.IO;
using System;
using System.Diagnostics;
using WebAPIClient.APICalls;

namespace JobAgent
{
    public class Logic
    {
        public static void StartJob(Job thejob)
        {
            Console.WriteLine("Job Received. Starting soon...");
            Thread DoJob = new Thread(StartJobThread);
            DoJob.Start(thejob);
        }

        public static void StartJobThread(object thejob)
        {
            Job job = (Job)thejob;
            Thread.Sleep(9000);

            Console.WriteLine("Job Started");

            SetRunning();

            Process process = new Process();
            process.StartInfo.FileName = job.ExecutablePath;
            process.Start();

            Thread.Sleep(30000);

            if (!process.HasExited)
            {
                process.Kill();
            }

            AgentEnvironment.HasTask = false;

            SetIdle();
            JobAPI.SetJobFinished(job);
            Console.WriteLine("Job Finished");
        }

        public static void SetRunning()
        {
            AgentAPI.GiveAgentJob(AgentEnvironment.Agent_Name);
        }

        public static void SetIdle()
        {
            AgentAPI.SetIdle(AgentEnvironment.Agent_Name);
        }
    }
}

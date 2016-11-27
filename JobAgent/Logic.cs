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
        public static void StartJob(Job job)
        {
            Console.WriteLine("Job Received. Starting soon...");



            Thread DoJob = new Thread(StartJobThread);
            DoJob.Start(job);
        }

        public static void StartJobThread(object job)
        {
            Thread.Sleep(9000);

            Console.WriteLine("Job Started");

            SetRunning();
            Job dojob = (Job)job;

            Process process = new Process();
            process.StartInfo.FileName = @"C:\Windows\System32\notepad.exe";
            process.Start();

            Thread.Sleep(30000);

            if (!process.HasExited)
            {
                process.Kill();
            }

            AgentEnvironment.HasTask = false;

            SetIdle();

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

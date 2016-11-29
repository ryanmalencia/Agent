﻿using DataTypes;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using WebAPIClient.APICalls;

namespace JobAgent
{
    public class Logic
    {
        public static void StartJob(int job_pk)
        {
            Console.WriteLine("Job Received. Starting soon...");
            Thread DoJob = new Thread(StartJobThread);
            Job thejob = JobAPI.GetByPk(job_pk);
            DoJob.Start(thejob);
        }

        public static void StartJobThread(object thejob)
        {
            Job job = (Job)thejob;
            //Thread.Sleep(9000);

            SetRunning();

            Console.WriteLine("Job Started");

            if (job.PrerunGroup != 0)
            {
                Console.WriteLine("Running PreJob Tasks");
                RunJobTasks(job.PrerunGroup);
            }

            if (job.RunGroup != 0)
            {
                Console.WriteLine("Running Job Tasks");
                RunJobTasks(job.RunGroup);
            }

            if (job.PostRunGroup != 0)
            {
                Console.WriteLine("Running PostJob Tasks");
                RunJobTasks(job.PostRunGroup);
            }

            AgentEnvironment.HasTask = false;

            SetIdle();
            JobAPI.SetJobFinished(job);
            Console.WriteLine("Job Finished");
        }

        public static void RunJobTasks(int group)
        {
            JobTaskCollection tasks = JobTaskAPI.GetByGroup(group);

            foreach(JobTask task in tasks.Tasks)
            {
                DoTask(task);
            }
        }

        public static void DoTask(JobTask task)
        {
            switch (task.type)
            {
                case "copyfiles":
                    {
                        CopyFiles(task.info);
                        break;
                    }
                case "runprogram":
                    {
                        RunExecutable(task.info);
                        break;
                    }
                case "deletefiles":
                    {
                        DeleteFiles(task.info);
                        break;
                    }
                default:
                    break;
            }
        }

        private static void CopyFiles(string info)
        {
            string origin = info.Split('?')[0];
            string target = info.Split('?')[1];
            foreach(string file in Directory.GetFiles(origin))
            {
                File.Copy(Path.Combine(file), Path.Combine(target, file.Remove(0, file.LastIndexOf('\\') + 1)), true);
            }
        }

        private static void RunExecutable(string info)
        {
            Process process = new Process();
            process.StartInfo.FileName = info.Split('?')[0];
            process.StartInfo.Arguments = info.Split('?')[1];
            process.Start();

            Thread.Sleep(20000);

            process.Kill();
        }

        private static void DeleteFiles(string info)
        {
            foreach(string file in Directory.GetFiles(Path.Combine(info)))
            {
                File.Delete(file);
            }
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

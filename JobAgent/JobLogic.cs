using DataTypes;
using JobAgent.SignalR;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using WebAPIClient.APICalls;

namespace JobAgent
{
    public class JobLogic
    {
        public static Job job;

        /// <summary>
        /// Start running a job
        /// </summary>
        /// <param name="Job">Job to run</param>
        public static void StartJob(Job Job)
        {
            AgentStatus.Instance.UpdateJob(Job.JobName);
            job = Job;
            Console.WriteLine("Job Received. Starting soon...");
            Thread DoJob = new Thread(StartJobThread);
            DoJob.Start(job.JobID);
        }

        /// <summary>
        /// Start running an admin job
        /// </summary>
        /// <param name="Job">Job to run</param>
        public static void StartAdminJob(Job Job)
        {
            Thread DoAdminJob = new Thread(StartAdminThread);
            job = Job;
            Console.WriteLine("Admin Job Received. Starting soon...");
            Thread DoJob = new Thread(StartJobThread);
            DoJob.Start(job.JobID);
        }

        private static void StartJobThread(object job_pk)
        {
            int pk = (int)job_pk;
            AgentLogic.SetRunning(job);
            SetJobStarted(job);
            Console.WriteLine("Started Job " + job.JobName);
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
            AgentLogic.SetIdle();
            JobAPI.SetJobFinished(job);
            Console.WriteLine("Job Finished");
        }

        private static void StartAdminThread(object job_pk)
        {
            int pk = (int)job_pk;
            while (AgentEnvironment.HasTask) { }
            Console.WriteLine("Administrative Job received...");
            Job job = JobAPI.GetById(pk);
        }

        private static void RunJobTasks(int group)
        {
            JobTaskCollection tasks = JobTaskAPI.GetByGroup(group);
            foreach(JobTask task in tasks.Tasks)
            {
                DoTask(task);
            }
        }

        private static void DoTask(JobTask task)
        {
            switch (task.Type)
            {
                case "copyfiles":
                    {
                        CopyFiles(task.Info);
                        break;
                    }
                case "runprogram":
                    {
                        RunExecutable(task.Info,task.AddInfo);
                        break;
                    }
                case "deletefiles":
                    {
                        DeleteFiles(task.Info);
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
            try
            {
                foreach (string file in Directory.GetFiles(origin))
                {
                    File.Copy(Path.Combine(file), Path.Combine(target, file.Remove(0, file.LastIndexOf('\\') + 1)), true);
                }
            }
            catch(Exception)
            { }
        }

        /// <summary>
        /// Run the given executable
        /// </summary>
        /// <param name="info">Location</param>
        /// <param name="addinfo">Arguments</param>
        private static void RunExecutable(string info, string addinfo)
        {
            try
            {
                Process process = new Process();
                if (info.Contains("?"))
                {
                    process.StartInfo.FileName = info.Split('?')[0];
                    if (!File.Exists(info.Split('?')[0]))
                    {
                        Console.WriteLine("Unable to locate executable");
                        return;
                    }
                    process.StartInfo.Arguments = info.Split('?')[1];
                }
                else
                {
                    process.StartInfo.FileName = info;
                    if (!File.Exists(info))
                    {
                        Console.WriteLine("Unable to locate executable");
                        return;
                    }
                    process.StartInfo.Arguments = addinfo;
                }
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.WorkingDirectory = Path.GetDirectoryName(info).ToString();
                process.Start();
                Thread.Sleep(20000);
                if (!process.HasExited)
                {
                    process.Kill();
                }
            }
            catch(Exception)
            { }
        }

        private static void DeleteFiles(string info)
        {
            foreach(string file in Directory.GetFiles(Path.Combine(info)))
            {
                try
                {
                    File.Delete(file);
                }
                catch(Exception)
                { }
            }
        }

        /// <summary>
        /// Set job to started status 
        /// </summary>
        /// <param name="job">Job to set</param>
        private static void SetJobStarted(Job job)
        {
            JobAPI.SetJobStarted(job);
        }
    }
}

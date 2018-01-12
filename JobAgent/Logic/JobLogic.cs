using DataTypes;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using WebAPIClient.APICalls;

namespace JobAgent.Logic
{
    public class JobLogic
    {
        public static Job job;
        public static string workingDir = "App_Data";
        /// <summary>
        /// Start running a job
        /// </summary>
        /// <param name="Job">Job to run</param>
        public static bool StartJob(Job Job)
        {
            //AgentAPI.UpdateJob(Job.JobName);
            job = Job;
            LogLogic.InfoLog("Job Received. Starting soon...");
            Thread DoJob = new Thread(StartJobThread);
            DoJob.Start(job.JobID);
            return true;
        }

        /// <summary>
        /// Start running an admin job
        /// </summary>
        /// <param name="Job">Job to run</param>
        public static void StartAdminJob(Job Job)
        {
            Thread DoAdminJob = new Thread(StartAdminThread);
            job = Job;
            LogLogic.InfoLog("Admin Job Received. Starting soon...");
            Thread DoJob = new Thread(StartJobThread);
            DoJob.Start(job.JobID);
        }

        private static void StartJobThread(object job_pk)
        {
            int pk = (int)job_pk;
            AgentLogic.SetRunning(job);
            SetJobStarted(job);
            LogLogic.InfoLog("Started Job " + job.JobName);
            if (job.PrerunGroup != 0)
            {
                LogLogic.InfoLog("Running PreJob Tasks");
                RunJobTasks(job.JobName, job.PrerunGroup);
            }
            if (job.RunGroup != 0)
            {
                LogLogic.InfoLog("Running Job Tasks");
                RunJobTasks(job.JobName, job.RunGroup);
            }
            if (job.PostRunGroup != 0)
            {
                LogLogic.InfoLog("Running PostJob Tasks");
                RunJobTasks(job.JobName, job.PostRunGroup);
            }
            AgentLogic.SetIdle();
            JobAPI.SetJobFinished(job);
            LogLogic.InfoLog("Job Finished");
        }

        private static void StartAdminThread(object job_pk)
        {
            int pk = (int)job_pk;
            while (AgentEnvironment.HasTask) { }
            LogLogic.InfoLog("Administrative Job received...");
            Job job = JobAPI.GetById(pk);
        }

        private static void RunJobTasks(string name, int group)
        {
            JobTaskCollection tasks = JobTaskAPI.GetByGroup(group);
            foreach(JobTask task in tasks.Tasks)
            {
                DoTask(name, task);
            }
        }

        private static void DoTask(string name, JobTask task)
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
                        RunExecutable(name, task.Info,task.AddInfo);
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
        private static void RunExecutable(string name, string info, string addinfo)
        {
            try
            {
                Process process = new Process();
                string path = Path.Combine(workingDir, name, info);
                string dir = Path.Combine(workingDir, name);

                if(!Directory.Exists(dir))
                {
                    DownloadAndUnzip(name, "");
                }

                if (info.Contains("?"))
                {
                    process.StartInfo.FileName = info.Split('?')[0];
                    if (!File.Exists(info.Split('?')[0]))
                    {
                        LogLogic.WarnLog("Unable to locate executable");
                        return;
                    }
                    process.StartInfo.Arguments = info.Split('?')[1];
                }
                else
                {
                    process.StartInfo.FileName = path;
                    if (!File.Exists(path))
                    {
                        LogLogic.WarnLog("Unable to locate executable");
                        return;
                    }
                    process.StartInfo.Arguments = addinfo;
                }
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.WorkingDirectory = Path.GetDirectoryName(path).ToString();
                process.Start();
                /*Thread.Sleep(20000);
                if (!process.HasExited)
                {
                    process.Kill();
                }*/
                process.WaitForExit();
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


        private static void DownloadAndUnzip(string filename, string dest)
        {
            FileAPI.Download(filename, dest);
        }
    }
}

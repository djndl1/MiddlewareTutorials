using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using RemotingContract;

namespace RemotingJobServer
{
    internal class JobServerInternal : IJobServer
    {
        private int _nextJobNumber;

        private List<JobInfo> _jobs;

        public JobServerInternal()
        {
            _nextJobNumber = 0;
            _jobs = new List<JobInfo>();
        }

        public event EventHandler<JobEventArgs> JobEvent;

        private void NotifyClients(JobEventArgs args)
        {
            foreach (Delegate deleg in JobEvent.GetInvocationList())
            {
                EventHandler<JobEventArgs> handler = null;
                try
                {
                    handler = (EventHandler<JobEventArgs>)deleg;
                    _ = handler.BeginInvoke(this, args, null, null);
                }
                catch (Exception)
                {
                    // in case of unreachable client, remove the subscription
                    // this issue does not arise with in-process events
                    JobEvent -= handler;
                }
            }
        }

        public void CreateJob(string description)
        {
            var newJob = new JobInfo(_nextJobNumber++, description, "", "");

            _jobs.Add(newJob);

            NotifyClients(new JobEventArgs(JobEventArgs.ReasonCode.New, newJob));
        }

        public List<JobInfo> GetJobs()
        {
            return _jobs;
        }

        public void UpdateJobState(int jobId, string user, string status)
        {
            JobInfo job = _jobs[jobId];

            job.AssignedUser = user;
            job.Status = status;

            // it's a struct after all
            _jobs[jobId] = job;

            NotifyClients(new JobEventArgs(JobEventArgs.ReasonCode.Change, job));
        }
    }
}

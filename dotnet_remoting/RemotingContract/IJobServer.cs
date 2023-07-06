using System;
using System.Collections.Generic;

namespace RemotingContract
{
    public class JobEventWrapper : MarshalByRefObject
    {
        public event EventHandler<JobEventArgs> JobEvent;

        public void LocallyHandleJobEvent(object sender, JobEventArgs e)
        {
            JobEvent?.Invoke(sender, e);
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }

    public interface IJobServer
    {
        event EventHandler<JobEventArgs> JobEvent;

        void CreateJob(string description);

        void UpdateJobState(int jobId, string user, string status);

        List<JobInfo> GetJobs();
    }
}

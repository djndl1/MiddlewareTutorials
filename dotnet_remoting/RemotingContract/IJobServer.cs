using System;
using System.Collections.Generic;

namespace RemotingContract
{
    public interface IJobServer
    {
        event EventHandler<JobEventArgs> JobEvent;

        void CreateJob(string description);

        void UpdateJobState(int jobId, string user, string status);

        List<JobInfo> GetJobs();
    }
}

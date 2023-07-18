using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using RemotingContract;

namespace RemotingJobServer
{
    public class JobServerInternal : MarshalByRefObject, IJobServer
    {
        public event EventHandler<JobEventArgs> JobEvent;

        public void CreateJob(string description)
        {
            throw new NotImplementedException();
        }

        public List<JobInfo> GetJobs()
        {
            throw new NotImplementedException();
        }

        public void UpdateJobState(int jobId, string user, string status)
        {
            throw new NotImplementedException();
        }
    }
}

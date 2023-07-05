using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using RemotingContract;

namespace RemotingContract
{
    [Serializable]
    public class JobEventArgs : EventArgs
    {
        public JobEventArgs(ReasonCode reason, JobInfo jobInfo)
        {
            Reason = reason;
            JobInfo = jobInfo;
        }

        public enum ReasonCode { New, Change }

        public ReasonCode Reason { get; }

        public JobInfo JobInfo { get; set; }

    }
}

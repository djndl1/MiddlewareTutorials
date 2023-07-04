using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using RemotingContract;

namespace RemotingContract
{
    public class JobEventArgs : EventArgs
    {
        public JobEventArgs(ReasonCode reason, JobInfo jobInfo)
        {
            Reason = reason;
            JobInfo = jobInfo;
        }

        public enum ReasonCode { New, Change }

        private ReasonCode Reason { get; }

        private JobInfo JobInfo { get; set; }

    }
}

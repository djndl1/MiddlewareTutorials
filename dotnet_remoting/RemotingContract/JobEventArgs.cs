using System;

namespace RemotingContract;

[Serializable]
public class JobEventArgs(JobEventArgs.ReasonCode reason, JobInfo jobInfo) : EventArgs
{
    public enum ReasonCode { New, Change }

    public ReasonCode Reason { get; } = reason;

    public JobInfo JobInfo { get; set; } = jobInfo;
}

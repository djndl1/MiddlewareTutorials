using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Extensions.Logging;

using RemotingContract;

namespace RemotingJobServer;

public class JobServerInternal : MarshalByRefObject, IJobServer
{
    private int _nextJobNumber;

    private readonly List<JobInfo> _jobs;

    private readonly object _lock = new();

    private readonly ILogger<JobServerInternal> _logger;

    public JobServerInternal()
    {
        _nextJobNumber = 0;
        _jobs = new List<JobInfo>();
        _logger = LoggingFactory.GetLogger<JobServerInternal>();
    }

    ///<summary>
    /// Infinite lifetime
    ///</summary>
    public override object InitializeLifetimeService()
    {
        return null;
    }

    public event EventHandler<JobEventArgs> JobEvent;

    private void NotifyClients(JobEventArgs args)
    {
        if (JobEvent is null) return;

        int cnt = 0;
        foreach (Delegate deleg in JobEvent.GetInvocationList())
        {
            EventHandler<JobEventArgs> handler = null;
            try
            {
                handler = (EventHandler<JobEventArgs>)deleg;
                _ = handler.BeginInvoke(this, args, null, null);
                _logger.LogInformation("Triggering {}.{}",
                                       deleg.Target?.ToString(),
                                       deleg.Method.Name);
                cnt++;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to trigger");
                // in case of unreachable client, remove the subscription
                // this issue does not arise with in-process events
                JobEvent -= handler;
            }
        }

        _logger.LogInformation("{} handlers triggered", cnt);
    }

    public void CreateJob(string description)
    {
        JobInfo newJob;
        lock (_lock)
        {
            newJob = new JobInfo(_nextJobNumber++, description, "", "");
            _jobs.Add(newJob);
        }

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

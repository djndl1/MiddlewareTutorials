using System;

namespace RemotingContract;

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


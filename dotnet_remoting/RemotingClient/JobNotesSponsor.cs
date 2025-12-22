using System;
using System.Runtime.Remoting.Lifetime;

namespace RemotingClient;

public class JobNotesSponsor : MarshalByRefObject, ISponsor
{
    public TimeSpan Renewal(ILease lease)
    {
        if (lease is null)
        {
            return TimeSpan.FromMinutes(1);
        }
        return lease.InitialLeaseTime;
    }
}


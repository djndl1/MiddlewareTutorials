using System;

namespace RemotingContract;

public class EventWrapper<TEventArgs> : MarshalByRefObject
{
    public event EventHandler<TEventArgs> Event;

    public void Handler(object sender, TEventArgs e)
    {
        Event?.Invoke(sender, e);
    }

    public override object InitializeLifetimeService()
    {
        return null;
    }
}

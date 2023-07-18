using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace RemotingContract
{
    public class ClientJobHandler : MarshalByRefObject
    {
        /**
         * constructed on the client and referenced by the server
         * so the method is executed by the client
         */
        public void NotifyJobChange(object sender, JobEventArgs e)
        {
            Console.WriteLine($"ClientJobHandler: JobEvent triggered "
                              + $"at {AppDomain.CurrentDomain.FriendlyName} on thread {Environment.CurrentManagedThreadId}.");
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}

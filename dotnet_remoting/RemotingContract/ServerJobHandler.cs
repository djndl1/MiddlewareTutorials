using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RemotingContract
{
    public static class ServerJobHandler
    {
        /* this one will be executed on the server
         since the server knowns the method definition
         and uses it directly
        */
        public static void NotifyJobChange(object sender, JobEventArgs e)
        {
            Console.WriteLine($"ServerJobHandler: JobEvent triggered "
                              + $"at {AppDomain.CurrentDomain.FriendlyName} on thread {Environment.CurrentManagedThreadId}");
        }
    }
}

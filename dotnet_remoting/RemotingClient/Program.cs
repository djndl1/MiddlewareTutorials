using System;

using System.Diagnostics;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading;
using RemotingContract;

namespace RemotingClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var channel = new TcpClientChannel();
            ChannelServices.RegisterChannel(channel, false);
            // use GetObject, not CreateInstance
            IJobServer jobServer = (IJobServer)Activator.GetObject(
                typeof(IJobServer),
                "tcp://127.0.0.1:4001/JobURI");

            Debug.Assert(jobServer != null);
            jobServer.JobEvent += NotifyJobChange;
            jobServer.JobEvent += (object sender, JobEventArgs job) =>
            {
                Console.WriteLine("JobEvent triggered in a lambda");
            };
            jobServer.CreateJob(Environment.MachineName);

            foreach (var job in jobServer.GetJobs())
            {
                Console.WriteLine(job.Description);
            }

            Thread.Sleep(TimeSpan.FromSeconds(1));
        }

        static void NotifyJobChange(object sender, JobEventArgs e)
        {
            Console.WriteLine($@"JobEvent triggered at {AppDomain.CurrentDomain.FriendlyName}");
        }
    }
}

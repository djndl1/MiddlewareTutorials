using System;

using System.Diagnostics;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading;
using RemotingContract;
using System.Runtime.Serialization.Formatters;
using System.Collections;

namespace RemotingClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // allow a callback from the server
            var serverProvider = new BinaryServerFormatterSinkProvider
            {
                TypeFilterLevel = TypeFilterLevel.Full,
            };
            Hashtable props = new Hashtable
            {
                ["port"] = 0,
            };
            // for the server to call back, the client has to be
            // both a server and a client
            var channel = new TcpChannel(props, null, serverProvider);
            ChannelServices.RegisterChannel(channel, false);

            // use GetObject, not CreateInstance
            IJobServer jobServer = (IJobServer)Activator.GetObject(
                typeof(IJobServer),
                "tcp://127.0.0.1:4001/JobURI");

            Debug.Assert(jobServer != null);

            // the server knows this defintion and calls it on the server
            // each time the client runs, the server subscribes to this method once
            // on the server and is never removed since the subscription is done
            // actually on the server
            jobServer.JobEvent += ServerJobHandler.NotifyJobChange;

            // the definition is directly referenced by the server
            // but the object is on the client so the server calls back
            // to the client
            var clientJobHandler = new ClientJobHandler();
            jobServer.JobEvent += clientJobHandler.NotifyJobChange;

            // the definition is in the interface assembly
            // both the client and the server known the definition
            // but they are not referencing each other.
            // the object is on the clienbt so the server calls back
            // to the client
            var eventWrapper = new JobEventWrapper();
            eventWrapper.JobEvent += clientJobHandler.NotifyJobChange;
            jobServer.JobEvent += eventWrapper.LocallyHandleJobEvent;

            // the event triggered here
            jobServer.CreateJob(Environment.MachineName);

            foreach (var job in jobServer.GetJobs())
            {
                Console.WriteLine(job.Description);
            }

            Console.WriteLine($"Main Thread {Environment.CurrentManagedThreadId}");

            // wait for the event
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }
    }

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

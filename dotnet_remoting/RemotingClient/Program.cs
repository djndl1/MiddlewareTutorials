using System;

using System.Diagnostics;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading;
using RemotingContract;
using System.Runtime.Serialization.Formatters;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Lifetime;
using RemotingJobServer;

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
            RemotingConfiguration.RegisterActivatedClientType(typeof(JobNotesInternal), "tcp://127.0.0.1:4001");

            if (args.Length > 0)
            {
                UseInterfaces();
            }
            else
            {
                UseStandinProxies();
            }
        }

        static void UseStandinProxies()
        {
            // use a standin that has the same asssembly name and namespace as the server
            JobServerInternal jobServerProxy = (JobServerInternal)Activator.GetObject(
                typeof(JobServerInternal),
                "tcp://127.0.0.1:4001/JobURI");

            JobNotesInternal jobNotesProxy = new JobNotesInternal();

            Console.WriteLine(nameof(UseStandinProxies));
            Run(jobNotesProxy, jobServerProxy);
        }

        static void UseInterfaces()
        {
            // use GetObject, not CreateInstance
            IJobServer jobServer = (IJobServer)Activator.GetObject(
                typeof(IJobServer),
                "tcp://127.0.0.1:4001/JobURI");
            IJobNotesFactory factory = (IJobNotesFactory)Activator.GetObject(typeof(IJobNotesFactory), "tcp://127.0.0.1:4001/JobNotes/Factory");
            IJobNotes notes = factory.Create();

            Console.WriteLine(nameof(UseInterfaces));
            Run(notes, jobServer);
        }

        static void Run(IJobNotes notes, IJobServer jobServer)
        {
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
            // the object is on the client so the server calls back
            // to the client
            var eventWrapper = new JobEventWrapper();
            eventWrapper.JobEvent += clientJobHandler.NotifyJobChange;
            jobServer.JobEvent += eventWrapper.LocallyHandleJobEvent;

            // the event triggered here
            jobServer.CreateJob(Environment.MachineName);

            foreach (JobInfo job in jobServer.GetJobs())
            {
                Console.WriteLine(job.Description);
            }

            Console.WriteLine($"Main Thread {Environment.CurrentManagedThreadId}");
            JobInfo firstJob = jobServer.GetJobs().First();

            notes.AddNote(firstJob.Id, "I have some question on this job.");
            notes.AddNote(firstJob.Id, "I have more question on this job.");
            List<string> allJobNotes = notes.GetNotes(firstJob.Id);

            if (notes is MarshalByRefObject refProxy)
            {
                ILease lease = (ILease)RemotingServices.GetLifetimeService(refProxy);
                var sponsor = new JobNotesSponsor();
                lease.Register(sponsor);
                Console.WriteLine("Registered a " + nameof(JobNotesSponsor));
            }

            Console.WriteLine(
               string.Join(Environment.NewLine + Environment.NewLine, allJobNotes));

            Thread.Sleep(TimeSpan.FromSeconds(1));
        }
    }


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
}

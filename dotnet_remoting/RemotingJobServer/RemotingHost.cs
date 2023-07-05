using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Reflection;
using System.Diagnostics;

namespace RemotingJobServer
{
    public static class RemotingHost
    {
        public static void InitializeRemoting(LegacyRemotingConfiguration config)
        {
            if (config.UseAppConfig)
            {
                using (var process = Process.GetCurrentProcess())
                {
                    RemotingConfiguration.Configure(process.MainModule.FileName + ".config", false);
                }
            }
            else
            {
                // register a channel
                var jobHttpChannel = new HttpServerChannel(4000);
                ChannelServices.RegisterChannel(jobHttpChannel, false);

                var jobTcpChannel = new TcpServerChannel(4001);
                ChannelServices.RegisterChannel(jobTcpChannel, false);

                // register the object
                RemotingConfiguration.RegisterWellKnownServiceType(
                    typeof(JobServerInternal),
                    "JobURI",
                    WellKnownObjectMode.Singleton
                );

                Console.WriteLine("Remoting Server started");
            }
        }
    }
}

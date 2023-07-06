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
using System.Runtime.Serialization.Formatters;
using System.Collections;

namespace RemotingJobServer
{
    public static class RemotingHost
    {
        public static void InitializeRemoting(LegacyRemotingConfiguration config)
        {
            if (config.UseAppConfig)
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string exeName = AppDomain.CurrentDomain.FriendlyName;
                string executablePath = Path.Combine(baseDir, exeName);
                RemotingConfiguration.Configure(executablePath + ".config", false);
            }
            else
            {
                // register a channel
                var jobHttpChannel = new HttpServerChannel(4000);
                ChannelServices.RegisterChannel(jobHttpChannel, false);


                BinaryServerFormatterSinkProvider provider = null;
                provider = new BinaryServerFormatterSinkProvider
                {
                    TypeFilterLevel = TypeFilterLevel.Full
                };

                var props = new Hashtable
                {
                    ["port"] = 4001
                };
                var jobTcpChannel = new TcpServerChannel(props, provider);
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

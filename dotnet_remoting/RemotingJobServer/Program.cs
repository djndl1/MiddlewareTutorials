using System;
using Microsoft.Extensions.Hosting;

namespace RemotingJobServer
{
    class Program
    {
        static void Main(string[] args)
        {
            IHostBuilder builder = Host.CreateDefaultBuilder(args);

            builder.ConfigureServices(ServiceConfiguration.ConfigureServices);

            builder.Build().Run();
        }
    }
}

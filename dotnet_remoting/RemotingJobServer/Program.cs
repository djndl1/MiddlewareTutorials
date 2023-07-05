using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System.Threading.Tasks;

namespace RemotingJobServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                IHostBuilder builder = Host.CreateDefaultBuilder(args);
                _ = builder.ConfigureServices(ServiceConfiguration.ConfigureServices);
                var host = builder.Build();

                CompositionRoot.InitializeContainer(host.Services);

                host.Run();
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}

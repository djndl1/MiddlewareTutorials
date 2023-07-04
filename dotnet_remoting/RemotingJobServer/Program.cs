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
            IHostBuilder builder = Host.CreateDefaultBuilder(args);

            builder.ConfigureServices(ServiceConfiguration.ConfigureServices);

            var host = builder.Build();

            var scheduler = await host.Services.GetRequiredService<ISchedulerFactory>()
                .GetScheduler();

            await scheduler.StartDelayed(TimeSpan.FromSeconds(5));

            host.Run();

            await scheduler.Shutdown();
        }
    }
}

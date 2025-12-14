using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using RemotingContract;

namespace RemotingJobServer
{
    public static class ServiceConfiguration
    {
        public static void ConfigureServices(
            HostBuilderContext context,
            IServiceCollection services)
        {
            LegacyRemotingConfiguration legacyRemotingConfig = context.Configuration.GetSection("LegacyRemoting")
                .Get<LegacyRemotingConfiguration>();
            services.AddSingleton(legacyRemotingConfig);

            services.AddTransient<IJobServer, JobServerInternal>();

            services.AddQuartz(q =>
            {
                q.ScheduleJob<RemotingJob>(trigger => trigger.StartNow());
            });

            services.AddHostedService<SchedulerHostedService>();
        }
    }
}

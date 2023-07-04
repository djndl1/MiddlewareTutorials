using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;
using Quartz;
using RemotingContract;

namespace RemotingJobServer
{
    public static class ServiceConfiguration
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IJobServer, JobServerInternal>();

            services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();
                q.ScheduleJob<RemotingJob>(trigger => trigger.StartNow());
            });
        }
    }
}

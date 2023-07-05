using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Extensions.Hosting;
using System.Threading;
using Quartz;
using Microsoft.Extensions.Logging;

namespace RemotingJobServer
{
    public class SchedulerHostedService : IHostedService
    {
        private readonly ISchedulerFactory _schedulerFactory;

        private readonly ILogger<SchedulerHostedService> _logger;

        private IScheduler _scheduler;

        public SchedulerHostedService(
            ISchedulerFactory schedulerFactory,
            ILogger<SchedulerHostedService> logger)
        {
            _schedulerFactory = schedulerFactory;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _scheduler = await _schedulerFactory.GetScheduler();
            await _scheduler.Start();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _scheduler?.Shutdown();
        }
    }
}

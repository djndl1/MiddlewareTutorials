using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Quartz;
using Microsoft.Extensions.Logging;

namespace RemotingJobServer
{
    public class RemotingJob : IJob
    {
        private readonly ILogger<RemotingJob> _logger;

        public RemotingJob(ILogger<RemotingJob> logger)
        {
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Remoting Server Here!");

            return Task.CompletedTask;
        }
    }
}

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

        private readonly LegacyRemotingConfiguration _config;

        public RemotingJob(ILogger<RemotingJob> logger,
                           LegacyRemotingConfiguration remotingConfig)
        {
            _logger = logger;
            _config = remotingConfig;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Remoting Server Here!");

            RemotingHost.InitializeRemoting(_config);

            return Task.CompletedTask;
        }
    }
}

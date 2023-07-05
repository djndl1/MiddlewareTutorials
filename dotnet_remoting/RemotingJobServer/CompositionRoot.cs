using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

namespace RemotingJobServer
{
    public static class CompositionRoot
    {
        public static void InitializeContainer(IServiceProvider container)
        {
            if (container is null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (Container is null)
            {
                Container = container;
            }
        }

        private static IServiceProvider Container { get; set; }

        public static T GetRequiredService<T>()
            => Container.GetRequiredService<T>();
    }
}

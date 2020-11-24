using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace RoccoServe.Framework.Server
{
    internal static class ServiceProviderExtensions
    {
        internal static ILoggerFactory GetLoggerFactory(this IServiceProvider serviceProvider)
        {
            return (ILoggerFactory)serviceProvider?.GetService(typeof(ILoggerFactory)) ?? NullLoggerFactory.Instance;
        }
    }
}

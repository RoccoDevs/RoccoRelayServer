using System;

namespace RoccoServe.Framework.Server.Infrastructure
{
    internal class EmptyServiceProvider : IServiceProvider
    {
        public static IServiceProvider Instance { get; } = new EmptyServiceProvider();

        public object GetService(Type serviceType) => null;
    }
}

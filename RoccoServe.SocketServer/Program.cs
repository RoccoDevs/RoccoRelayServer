using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RoccoServe.Framework.Server;

namespace RoccoServe.SocketServer
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Debug);
                builder.AddConsole();
            });

            var serviceProvider = services.BuildServiceProvider();

            var server = new ServerBuilder(serviceProvider)
                ;
        }
    }
}
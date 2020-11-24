using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RoccoServe.Framework.Server;
using RoccoServe.Framework.Server.Middleware;
using RoccoServe.Framework.Server.Transports.Sockets;

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
                    .UseSockets(sockets =>
                    {
                        sockets.ListenLocalhost(530,
                            builder => builder.UseConnectionLogging().UseConnectionHandler<MyCustomProtocol>());
                    })
                    .Build();
            
            var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<Program>();

            await server.StartAsync();

            foreach (var ep in server.EndPoints)
            {
                logger.LogInformation("Listening on {EndPoint}", ep);
            }

            var tcs = new TaskCompletionSource<object>();
            Console.CancelKeyPress += (sender, e) => tcs.TrySetResult(null);
            await tcs.Task;

            await server.StopAsync();
        }
    }
}
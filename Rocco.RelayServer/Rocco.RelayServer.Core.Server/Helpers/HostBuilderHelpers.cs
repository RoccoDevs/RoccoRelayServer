using System.Net;
using Bedrock.Framework;
using Bedrock.Framework.Protocols;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocco.RelayServer.Core.Client;
using Rocco.RelayServer.Core.Domain;
using Rocco.RelayServer.Core.Interfaces.Services;
using Rocco.RelayServer.Core.Server.ConnectionHandlers;
using Rocco.RelayServer.Core.Server.Services;
using Rocco.RelayServer.Core.Services;

namespace Rocco.RelayServer.Core.Server.Helpers
{
    public static class HostBuilderHelpers
    {
        public static IHostBuilder ConfigureDefaultRelay(this IHostBuilder builder, IPAddress address)
        {
            builder.ConfigureDefaultRelayServices()
                .ConfigureServer(serverBuilder =>
                {
                    serverBuilder.UseSockets(sockets =>
                    {
                        sockets.Listen(address, 530,
                            socketBuilder =>
                            {
                                socketBuilder.UseConnectionLogging()
                                    .UseConnectionHandler<SixtyNineProtocolHandler>();
                            });
                    }).Build();
                });

            return builder;
        }

        public static IHostBuilder ConfigureDefaultRelayWithFakeCaller(this IHostBuilder builder,IPAddress address)
        {
            builder.ConfigureDefaultRelayServices()
                .ConfigureServer(serverBuilder =>
                {
                    serverBuilder.UseSockets(sockets =>
                    {
                        sockets.Listen(address, 530,
                            socketBuilder =>
                            {
                                socketBuilder.UseConnectionLogging()
                                    .UseConnectionHandler<SixtyNineProtocolHandler>();
                            });
                    }).Build();
                })
                .ConfigureServices((_, services) =>
                {
                    //WARNING: THIS IS JUST A DEMO
                    services.AddHostedService<ClientWorker>();
                });

            return builder;
        }

        public static IHostBuilder ConfigureDefaultRelayServices(this IHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton<ConnectionStore>();
                services.AddScoped<IMessageWriter<SixtyNineMessage>, SixtyNineWriter>();
                services.AddScoped<IMessageHandler, MessageHandler>();
                services.AddScoped<IMessageSender, MessageSender>();
                services.AddTransient<IMessageReader<SixtyNineMessage>, SixtyNineReader>();
            });

            return builder;
        }
    }
}
using System.Net;
using Bedrock.Framework;
using Bedrock.Framework.Protocols;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocco.RelayServer.Core.Domain;
using Rocco.RelayServer.Core.Interfaces.Services;
using Rocco.RelayServer.Core.Server.ConnectionHandlers;
using Rocco.RelayServer.Core.Server.Services;
using Rocco.RelayServer.Core.Services;

namespace Rocco.RelayServer;

internal static class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddSingleton<ConnectionStore>();
                services.AddScoped<IMessageWriter<SixtyNineSendibleMessage>, SixtyNineWriter>();
                services.AddScoped<IMessageHandler, MessageHandler>();
                services.AddScoped<IMessageSender, MessageSender>();
                services.AddTransient<IMessageReader<SixtyNineMessage>, SixtyNineReader>();
            })
            .ConfigureLogging((hostContext, loggingBuilder) =>
            {
                loggingBuilder.SetMinimumLevel(LogLevel.Information);
            })
            .ConfigureServer(serverBuilder =>
            {
                {
                    serverBuilder.UseSockets(sockets =>
                    {
                        sockets.Listen(IPAddress.Any, 530,
                            builder =>
                            {
                                builder.UseConnectionLogging()
                                    .UseConnectionHandler<SixtyNineProtocolHandler>();
                            });
                    });
                    serverBuilder.Build();
                }
            });
    }
}
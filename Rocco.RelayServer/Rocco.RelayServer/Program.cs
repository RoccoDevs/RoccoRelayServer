using System.Net;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocco.RelayServer.Core.Server.Helpers;

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
            .ConfigureLogging((_, loggingBuilder) => { loggingBuilder.SetMinimumLevel(LogLevel.Debug); })
            .ConfigureDefaultRelayWithFakeCaller(IPAddress.Any);
    }
}
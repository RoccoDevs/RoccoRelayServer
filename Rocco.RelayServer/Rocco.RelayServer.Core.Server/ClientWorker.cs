using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bedrock.Framework;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Hosting;
using Rocco.RelayServer.Core.Domain;

namespace Rocco.RelayServer.Core.Server
{
    public class ClientWorker : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private ConnectionContext? _connection;

        public ClientWorker(IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime)
        {
            _serviceProvider = serviceProvider;
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.Yield();

            var client = new ClientBuilder(_serviceProvider)
                        .UseSockets()
                        .UseConnectionLogging("Client")
                        .Build();

            _connection = await client.ConnectAsync(new IPEndPoint(IPAddress.IPv6Loopback, 530), _hostApplicationLifetime.ApplicationStopping);

            if (_connection == null)
            {
                return;
            }

            var protocol = new MyClientProtocol(_connection);
            while (!_hostApplicationLifetime.ApplicationStopping.IsCancellationRequested)
            {
                await protocol.SendAsync(new InitMessage("0"), _hostApplicationLifetime.ApplicationStopping);
                await protocol.SendAsync(new PayloadMessage("0", "0", new Memory<byte>(Encoding.UTF8.GetBytes("Payload0"))), _hostApplicationLifetime.ApplicationStopping);
                await protocol.SendAsync(new PayloadMessage("0", "0", new Memory<byte>(Encoding.UTF8.GetBytes("Payload1"))), _hostApplicationLifetime.ApplicationStopping);
                await protocol.SendAsync(new PayloadMessage("0", "0", new Memory<byte>(Encoding.UTF8.GetBytes("Payload2"))), _hostApplicationLifetime.ApplicationStopping);
                await protocol.SendAsync(new PayloadMessage("0", "0", new Memory<byte>(Encoding.UTF8.GetBytes("Payload3"))), _hostApplicationLifetime.ApplicationStopping);
                await protocol.SendAsync(new PayloadMessage("0", "0", new Memory<byte>(Encoding.UTF8.GetBytes("Payload4"))), _hostApplicationLifetime.ApplicationStopping);
                await protocol.SendAsync(new PayloadMessage("0", "0", new Memory<byte>(Encoding.UTF8.GetBytes("Payload5"))), _hostApplicationLifetime.ApplicationStopping);
                await protocol.SendAsync(new PayloadMessage("0", "0", new Memory<byte>(Encoding.UTF8.GetBytes("Payload6"))), _hostApplicationLifetime.ApplicationStopping);
                await protocol.SendAsync(new PayloadMessage("0", "0", new Memory<byte>(Encoding.UTF8.GetBytes("Payload7"))), _hostApplicationLifetime.ApplicationStopping);
                await protocol.SendAsync(new PayloadMessage("0", "0", new Memory<byte>(Encoding.UTF8.GetBytes("Payload8"))), _hostApplicationLifetime.ApplicationStopping);
                await protocol.SendAsync(new PayloadMessage("0", "0", new Memory<byte>(Encoding.UTF8.GetBytes("Payload9"))), _hostApplicationLifetime.ApplicationStopping);

                Thread.Sleep(100);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_connection != null)
            {
                await _connection.DisposeAsync();
            }
        }
    }
}
using System.Threading;
using System.Threading.Tasks;
using Bedrock.Framework.Protocols;
using Microsoft.AspNetCore.Connections;
using Rocco.RelayServer.Core.Domain;
using Rocco.RelayServer.Core.Services;

namespace Rocco.RelayServer.Core.Server
{
    public class MyClientProtocol
    {
        private readonly ConnectionContext _connection;
        private readonly ProtocolReader _reader;
        private readonly SixtyNineWriter _messageWriter;

        public MyClientProtocol(ConnectionContext connection)
        {
            _connection = connection;
            _reader = connection.CreateReader();

            _messageWriter = new SixtyNineWriter();
        }

        public async ValueTask SendAsync(SixtyNineMessage requestMessage, CancellationToken cancellationToken)
        {
            // Write request message length
            _messageWriter.WriteMessage(requestMessage, _connection.Transport.Output);

            await _connection.Transport.Output.FlushAsync(cancellationToken);
        }
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;
using Bedrock.Framework.Protocols;
using Rocco.RelayServer.Core.Domain;
using Rocco.RelayServer.Core.Domain.Exceptions;
using Rocco.RelayServer.Core.Interfaces.Services;

namespace Rocco.RelayServer.Core.Server.Services;

public class MessageSender : IMessageSender, IDisposable
{
    private readonly ConnectionStore _connectionStore;

    private readonly IMessageWriter<SixtyNineSendibleMessage> _messageWriter;

    private readonly SemaphoreSlim _semaphore;

    public MessageSender(IMessageWriter<SixtyNineSendibleMessage> messageWriter, ConnectionStore connectionStore)
    {
        _messageWriter = messageWriter;
        _connectionStore = connectionStore;
        _semaphore = new SemaphoreSlim(1);
    }

    public void Dispose()
    {
        _semaphore.Dispose();
    }


    public async ValueTask TrySendAsync(SixtyNineSendibleMessage requestMessage,
        CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var connection = _connectionStore[requestMessage.Destination] ??
                             throw new ConnectionNotFoundException(requestMessage.Destination);

            var protocolWriter = new ProtocolWriter(connection.Transport.Output);
            await protocolWriter.WriteAsync(_messageWriter, requestMessage, cancellationToken);
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
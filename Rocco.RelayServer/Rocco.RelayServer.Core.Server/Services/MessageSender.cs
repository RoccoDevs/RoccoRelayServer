namespace Rocco.RelayServer.Core.Server.Services;

public class MessageSender : IMessageSender, IDisposable
{
    private readonly ConnectionStore _connectionStore;

    private readonly IMessageWriter<SixtyNineMessage> _messageWriter;

    private readonly SemaphoreSlim _semaphore;

    public MessageSender(IMessageWriter<SixtyNineMessage> messageWriter, ConnectionStore connectionStore)
    {
        _messageWriter = messageWriter;
        _connectionStore = connectionStore;
        _semaphore = new SemaphoreSlim(1);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }


    public async ValueTask TrySendAsync(SixtyNineMessage requestMessage,
        CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var connection = requestMessage.Destination is not null
                ? _connectionStore[requestMessage.Destination] ??
                  throw new ConnectionNotFoundException(requestMessage.Destination)
                : throw new ConnectionNotFoundException("null");

            var protocolWriter = new ProtocolWriter(connection.Transport.Output);
            await protocolWriter.WriteAsync(_messageWriter, requestMessage, cancellationToken);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        _semaphore.Dispose();
    }
}
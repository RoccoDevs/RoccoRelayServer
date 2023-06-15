namespace Rocco.RelayServer.Core.Server.ConnectionHandlers;

public class SixtyNineProtocolHandler : ConnectionHandler
{
    private readonly ConnectionStore _connectionStore;
    private readonly ILogger _logger;
    private readonly IMessageHandler _messageHandler;
    private readonly IMessageReader<SixtyNineMessage> _messageReader;
    private readonly IMessageSender _messageSender;

    public SixtyNineProtocolHandler(ILogger<SixtyNineProtocolHandler> logger, IMessageHandler messageHandler,
        IMessageSender messageSender, ConnectionStore connectionStore,
        IMessageReader<SixtyNineMessage> messageReader)
    {
        _logger = logger;
        _messageHandler = messageHandler;
        _messageSender = messageSender;
        _connectionStore = connectionStore;
        _messageReader = messageReader;
    }

    public override async Task OnConnectedAsync(ConnectionContext connection)
    {
        var reader = connection.CreateReader();
        while (!connection.ConnectionClosed.IsCancellationRequested)
        {
            try
            {
                var result = await reader.ReadAsync(_messageReader);
                var message = result.Message;


                var returnMessage = _messageHandler.HandleMessage(connection, message);

                if (returnMessage == null || string.IsNullOrEmpty(returnMessage.Destination)) continue;

                try
                {
                    await _messageSender.TrySendAsync(returnMessage,
                        connection.ConnectionClosed);

                    continue;
                }
                catch (ConnectionNotFoundException e)
                {
                    _logger.LogError("connection not found: {@Message}", e.Message);
                }
                catch (ArgumentNullException e)
                {
                    _logger.LogError(
                        "Client {@ConnectionId} threw: {@Message} /n trace: {@StackTrace}", connection.ConnectionId,
                        e.Message, e.StackTrace);
                }
                catch (Exception e)
                {
                    _logger.LogError("Connection {@ConnectionId} threw: {@Message} /n trace: {@StackTrace}",
                        connection.ConnectionId, e.Message, e.StackTrace);
                }


                if (result.IsCompleted) break;
            }
            catch (ConnectionResetException)
            {
                var newSource = new CancellationTokenSource();
                newSource.Cancel();
                connection.ConnectionClosed = newSource.Token;
            }
            finally
            {
                reader.Advance();
            }
        }

        _logger.LogInformation("Disconnected: {@Connection}", connection.ConnectionId);
        _connectionStore.Remove(connection);
    }
}
namespace Rocco.RelayServer.Core.Server.Services;

public class MessageHandler : IMessageHandler
{
    private readonly ConnectionStore _connectionStore;

    private readonly ILogger<MessageHandler> _logger;

    public MessageHandler(ConnectionStore connectionStore, ILogger<MessageHandler> logger)
    {
        _connectionStore = connectionStore;
        _logger = logger;
    }

    public SixtyNineMessage? HandleMessage(ConnectionContext connection, SixtyNineMessage message)
    {
        return message switch
        {
            null => null,
            InitMessage x => HandleInitMessage(x, connection),
            CloseMessage => HandleCloseMessage(connection),
            ErrorMessage x => x,
            PayloadMessage x => x,
            _ => new ErrorMessage(connection.ConnectionId, "Invalid type"u8.ToArray())
        };
    }

    private SixtyNineMessage? HandleCloseMessage(ConnectionContext connectionContext)
    {
        _connectionStore.Remove(connectionContext);
        return null;
    }

    private SixtyNineMessage HandleInitMessage(InitMessage socketMessage,
        ConnectionContext connectionContext)
    {
        var (source, _) = socketMessage;
        if (source != null)
        {
            if (!_connectionStore.Contains(source))
            {
                _logger.LogInformation("Client with id: {@Source} connected", source);
                connectionContext.ConnectionId = source;
                _connectionStore.Add(connectionContext);
            }
            else
            {
                return new ErrorMessage(connectionContext.ConnectionId,
                    Encoding.UTF8.GetBytes($"ClientId: {source} is already initialized"));
            }
        }
        else
        {
            _connectionStore.Add(connectionContext);
        }

        return new InitResponseMessage(connectionContext.ConnectionId);
    }
}
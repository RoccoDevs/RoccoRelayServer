using System.Text;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;
using Rocco.RelayServer.Core.Domain;
using Rocco.RelayServer.Core.Interfaces.Services;

namespace Rocco.RelayServer.Core.Server.Services;

/// <summary>
///     Class MessageHandler.
///     Implements the <see cref="Rocco.RelayServer.Core.Interfaces.Services.IMessageHandler" />
/// </summary>
/// <seealso cref="Rocco.RelayServer.Core.Interfaces.Services.IMessageHandler" />
/// <autogeneratedoc />
public class MessageHandler : IMessageHandler
{
    /// <summary>
    ///     The connection store
    /// </summary>
    /// <autogeneratedoc />
    private readonly ConnectionStore _connectionStore;

    /// <summary>
    ///     The logger
    /// </summary>
    /// <autogeneratedoc />
    private readonly ILogger<MessageHandler> _logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MessageHandler" /> class.
    /// </summary>
    /// <param name="connectionStore">The connection store.</param>
    /// <param name="logger">The logger.</param>
    /// <autogeneratedoc />
    public MessageHandler(ConnectionStore connectionStore, ILogger<MessageHandler> logger)
    {
        _connectionStore = connectionStore;
        _logger = logger;
    }

    public SixtyNineSendibleMessage? HandleMessage(ConnectionContext connection, SixtyNineMessage message)
    {
        return message switch
        {
            null => null,
            InitMessage x => HandleInitMessage(x, connection),
            CloseMessage => HandleCloseMessage(connection),
            ErrorMessage x => x,
            PayloadMessage x => x,
            _ => new ErrorMessage(connection.ConnectionId, Encoding.UTF8.GetBytes("Invalid type"))
        };
    }

    /// <summary>
    ///     Handles the close message.
    /// </summary>
    /// <param name="connectionContext">The connection context.</param>
    /// <returns>System.Nullable&lt;SixtyNineSendibleMessage&gt;.</returns>
    /// <autogeneratedoc />
    internal SixtyNineSendibleMessage? HandleCloseMessage(ConnectionContext connectionContext)
    {
        _connectionStore.Remove(connectionContext);
        return null;
    }

    /// <summary>
    ///     Handles the initialize message.
    /// </summary>
    /// <param name="socketMessage">The socket message.</param>
    /// <param name="connectionContext">The connection context.</param>
    /// <returns>SixtyNineSendibleMessage.</returns>
    /// <autogeneratedoc />
    internal SixtyNineSendibleMessage HandleInitMessage(InitMessage socketMessage,
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
namespace Rocco.RelayServer.Core.Interfaces.Services;

public interface IMessageSender
{
    ValueTask TrySendAsync(SixtyNineMessage requestMessage,
        CancellationToken cancellationToken = default);
}
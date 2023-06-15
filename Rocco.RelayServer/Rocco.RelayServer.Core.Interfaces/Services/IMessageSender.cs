namespace Rocco.RelayServer.Core.Interfaces.Services;

public interface IMessageSender
{
    ValueTask TrySendAsync(SixtyNineSendibleMessage requestMessage,
        CancellationToken cancellationToken = default);
}
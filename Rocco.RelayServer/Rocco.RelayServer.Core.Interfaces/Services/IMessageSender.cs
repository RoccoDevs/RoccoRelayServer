using System.Threading;
using System.Threading.Tasks;
using Rocco.RelayServer.Core.Domain;

namespace Rocco.RelayServer.Core.Interfaces.Services;

public interface IMessageSender
{
    ValueTask TrySendAsync(SixtyNineSendibleMessage requestMessage,
        CancellationToken cancellationToken = default);
}
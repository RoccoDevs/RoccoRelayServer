using System.Threading;
using System.Threading.Tasks;
using TheDesolatedTunnels.RelayServer.Core.Domain;

namespace TheDesolatedTunnels.RelayServer.Core.Interfaces.Services
{
    /// <summary>
    ///     Interface IMessageSender
    /// </summary>
    /// <autogeneratedoc />
    public interface IMessageSender
    {
        /// <summary>
        ///     Tries the send asynchronous.
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        /// <param name="cancellationToken">
        ///     The cancellation token that can be used by other objects or threads to receive notice
        ///     of cancellation.
        /// </param>
        /// <returns>ValueTask&lt;FlushResult&gt;.</returns>
        /// <autogeneratedoc />
        ValueTask TrySendAsync(SixtyNineSendibleMessage requestMessage,
            CancellationToken cancellationToken = default);
    }
}
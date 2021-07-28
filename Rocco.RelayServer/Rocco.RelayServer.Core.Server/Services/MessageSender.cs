using System;
using System.Threading;
using System.Threading.Tasks;
using Bedrock.Framework.Protocols;
using Rocco.RelayServer.Core.Domain;
using Rocco.RelayServer.Core.Domain.Exceptions;
using Rocco.RelayServer.Core.Interfaces.Services;

namespace Rocco.RelayServer.Core.Server.Services
{
    /// <summary>
    ///     Class MessageSender.
    ///     Implements the <see cref="Rocco.RelayServer.Core.Interfaces.Services.IMessageSender" />
    /// </summary>
    /// <seealso cref="Rocco.RelayServer.Core.Interfaces.Services.IMessageSender" />
    /// <autogeneratedoc />
    public class MessageSender : IMessageSender, IDisposable
    {
        /// <summary>
        ///     The connection store
        /// </summary>
        /// <autogeneratedoc />
        private readonly ConnectionStore _connectionStore;

        /// <summary>
        ///     The message writer
        /// </summary>
        /// <autogeneratedoc />
        private readonly IMessageWriter<SixtyNineMessage> _messageWriter;

        private readonly SemaphoreSlim _semaphore;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MessageSender" /> class.
        /// </summary>
        /// <param name="messageWriter">The message writer.</param>
        /// <param name="connectionStore">The connection store.</param>
        /// <autogeneratedoc />
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

        /// <summary>
        ///     try send as an asynchronous operation.
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        /// <param name="cancellationToken">
        ///     The cancellation token that can be used by other objects or threads to receive notice
        ///     of cancellation.
        /// </param>
        /// <returns>A Task&lt;FlushResult&gt; representing the asynchronous operation.</returns>
        /// <exception cref="ConnectionNotFoundException"></exception>
        /// <autogeneratedoc />
        public async ValueTask TrySendAsync(SixtyNineMessage requestMessage,
            CancellationToken cancellationToken = default)
        {
            await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                var connection = requestMessage.Destination is not null ? _connectionStore[requestMessage.Destination] ??
                                 throw new ConnectionNotFoundException(requestMessage.Destination):throw new ConnectionNotFoundException("null");

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
}
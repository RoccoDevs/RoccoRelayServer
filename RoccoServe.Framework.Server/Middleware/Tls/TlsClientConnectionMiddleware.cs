﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.Extensions.Logging;
using RoccoServe.Framework.Server.Infrastructure;

namespace RoccoServe.Framework.Server.Middleware.Tls
{
    internal class TlsClientConnectionMiddleware
    {
        private readonly ConnectionDelegate _next;
        private readonly TlsOptions _options;
        private readonly ILogger _logger;
        private readonly X509Certificate2 _certificate;

        public TlsClientConnectionMiddleware(ConnectionDelegate next, TlsOptions options, ILoggerFactory loggerFactory)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _next = next;

            // capture the certificate now so it can't be switched after validation
            _certificate = options.LocalCertificate;

            if (_certificate != null)
            {
                EnsureCertificateIsAllowedForClientAuth(_certificate);
            }

            _options = options;
            _logger = loggerFactory?.CreateLogger<TlsServerConnectionMiddleware>();
        }

        public async Task OnConnectionAsync(ConnectionContext context)
        {
            await Task.Yield();

            var feature = new TlsConnectionFeature();
            context.Features.Set<ITlsConnectionFeature>(feature);
            context.Features.Set<ITlsHandshakeFeature>(feature);

            var memoryPool = context.Features.Get<IMemoryPoolFeature>()?.MemoryPool;

            var inputPipeOptions = new StreamPipeReaderOptions
            (
                pool: memoryPool,
                bufferSize: memoryPool.GetMinimumSegmentSize(),
                minimumReadSize: memoryPool.GetMinimumAllocSize(),
                leaveOpen: true
            );

            var outputPipeOptions = new StreamPipeWriterOptions
            (
                pool: memoryPool,
                leaveOpen: true
            );

            SslDuplexPipe sslDuplexPipe = null;

            if (_options.RemoteCertificateMode == RemoteCertificateMode.NoCertificate)
            {
                sslDuplexPipe = new SslDuplexPipe(context.Transport, inputPipeOptions, outputPipeOptions);
            }
            else
            {
                sslDuplexPipe = new SslDuplexPipe(context.Transport, inputPipeOptions, outputPipeOptions, s => new SslStream(
                    s,
                    leaveInnerStreamOpen: false,
                    userCertificateValidationCallback: (sender, certificate, chain, sslPolicyErrors) =>
                    {
                        if (certificate == null)
                        {
                            return _options.RemoteCertificateMode != RemoteCertificateMode.RequireCertificate;
                        }

                        if (_options.RemoteCertificateValidation == null)
                        {
                            if (sslPolicyErrors != SslPolicyErrors.None)
                            {
                                return false;
                            }
                        }

                        var certificate2 = ConvertToX509Certificate2(certificate);
                        if (certificate2 == null)
                        {
                            return false;
                        }

                        if (_options.RemoteCertificateValidation != null)
                        {
                            if (!_options.RemoteCertificateValidation(certificate2, chain, sslPolicyErrors))
                            {
                                return false;
                            }
                        }

                        return true;
                    }));
            }

            var sslStream = sslDuplexPipe.Stream;

            using (var cancellationTokeSource = new CancellationTokenSource(_options.HandshakeTimeout))
            {
                try
                {
                    var sslOptions = new SslClientAuthenticationOptions
                    {
                        ClientCertificates = new X509CertificateCollection(new[] { _certificate }),
                        EnabledSslProtocols = _options.SslProtocols,
                        CertificateRevocationCheckMode = _options.CheckCertificateRevocation ? X509RevocationMode.Online : X509RevocationMode.NoCheck,
                        ApplicationProtocols = new List<SslApplicationProtocol>(),
                    };

                    _options.OnAuthenticateAsClient?.Invoke(context, sslOptions);

                    await sslStream.AuthenticateAsClientAsync(sslOptions, cancellationTokeSource.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    _logger?.LogDebug(2, "Authentication timed out");
                    await sslStream.DisposeAsync().ConfigureAwait(false);
                    return;
                }
                catch (Exception ex) when (ex is IOException || ex is AuthenticationException)
                {
                    _logger?.LogDebug(1, ex, "Authentication failed");
                    await sslStream.DisposeAsync().ConfigureAwait(false);
                    return;
                }
            }

            feature.ApplicationProtocol = sslStream.NegotiatedApplicationProtocol.Protocol;
            context.Features.Set<ITlsApplicationProtocolFeature>(feature);
            feature.LocalCertificate = ConvertToX509Certificate2(sslStream.LocalCertificate);
            feature.RemoteCertificate = ConvertToX509Certificate2(sslStream.RemoteCertificate);
            feature.CipherAlgorithm = sslStream.CipherAlgorithm;
            feature.CipherStrength = sslStream.CipherStrength;
            feature.HashAlgorithm = sslStream.HashAlgorithm;
            feature.HashStrength = sslStream.HashStrength;
            feature.KeyExchangeAlgorithm = sslStream.KeyExchangeAlgorithm;
            feature.KeyExchangeStrength = sslStream.KeyExchangeStrength;
            feature.Protocol = sslStream.SslProtocol;

            var originalTransport = context.Transport;

            try
            {
                context.Transport = sslDuplexPipe;

                // Disposing the stream will dispose the sslDuplexPipe
                await using (sslStream)
                await using (sslDuplexPipe)
                {
                    await _next(context).ConfigureAwait(false);
                    // Dispose the inner stream (SslDuplexPipe) before disposing the SslStream
                    // as the duplex pipe can hit an ODE as it still may be writing.
                }
            }
            finally
            {
                // Restore the original so that it gets closed appropriately
                context.Transport = originalTransport;
            }
        }

        protected static void EnsureCertificateIsAllowedForClientAuth(X509Certificate2 certificate)
        {
            if (!CertificateLoader.IsCertificateAllowedForClientAuth(certificate))
            {
                throw new InvalidOperationException($"Invalid client certificate for client authentication: {certificate.Thumbprint}");
            }
        }

        private static X509Certificate2 ConvertToX509Certificate2(X509Certificate certificate)
        {
            if (certificate is null)
            {
                return null;
            }

            return certificate as X509Certificate2 ?? new X509Certificate2(certificate);
        }
    }
}

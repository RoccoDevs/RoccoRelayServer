using System;

namespace RoccoServe.Framework.Server.Middleware.Tls
{
    public interface ITlsApplicationProtocolFeature
    {
        ReadOnlyMemory<byte> ApplicationProtocol { get; }
    }
}

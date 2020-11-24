using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNetCore.Connections;

namespace RoccoServe.Framework.Server
{
    public abstract class ServerBinding
    {
        public virtual ConnectionDelegate Application { get; }

        public abstract IAsyncEnumerable<IConnectionListener> BindAsync(CancellationToken cancellationToken = default);
    }
}

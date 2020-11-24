﻿using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.AspNetCore.Connections;

namespace RoccoServe.Framework.Server
{
    public class EndPointBinding : ServerBinding
    {
        private readonly ConnectionDelegate _application;
        public EndPointBinding(EndPoint endPoint, ConnectionDelegate application, IConnectionListenerFactory connectionListenerFactory)
        {
            EndPoint = endPoint;
            _application = application;
            ConnectionListenerFactory = connectionListenerFactory;
        }

        private EndPoint EndPoint { get; }
        private IConnectionListenerFactory ConnectionListenerFactory { get; }

        public override ConnectionDelegate Application => _application;

        public override async IAsyncEnumerable<IConnectionListener> BindAsync([EnumeratorCancellation]CancellationToken cancellationToken)
        {
            yield return await ConnectionListenerFactory.BindAsync(EndPoint, cancellationToken);
        }

        public override string ToString()
        {
            return EndPoint?.ToString();
        }
    }
}

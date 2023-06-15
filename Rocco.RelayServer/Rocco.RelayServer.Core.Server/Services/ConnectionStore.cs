namespace Rocco.RelayServer.Core.Server.Services;

public class ConnectionStore
{
    private readonly ConcurrentDictionary<string, ConnectionContext> _connections = new(StringComparer.Ordinal);

    public virtual ConnectionContext? this[string connectionId]
    {
        get
        {
            _connections.TryGetValue(connectionId, out var connection);
            return connection;
        }
    }

    public virtual bool Contains(string connectionId)
    {
        return _connections.ContainsKey(connectionId);
    }

    public int Count()
    {
        return _connections.Count;
    }

    public virtual void Add(ConnectionContext connection)
    {
        _connections.TryAdd(connection.ConnectionId, connection);
    }

    public virtual void Remove(ConnectionContext connection)
    {
        _connections.TryRemove(connection.ConnectionId, out _);
    }
}
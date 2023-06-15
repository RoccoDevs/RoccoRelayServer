namespace Rocco.RelayServer.Core.Domain.Exceptions;

public class ConnectionNotFoundException : Exception
{
    public ConnectionNotFoundException(string connectionId) : base(connectionId)
    {
    }
}
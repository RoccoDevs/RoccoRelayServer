namespace Rocco.RelayServer.Core.Interfaces.Services;

public interface IMessageHandler
{
    SixtyNineMessage? HandleMessage(ConnectionContext connection, SixtyNineMessage message);
}
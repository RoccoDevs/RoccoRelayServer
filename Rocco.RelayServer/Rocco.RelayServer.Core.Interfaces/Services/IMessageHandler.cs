namespace Rocco.RelayServer.Core.Interfaces.Services;

public interface IMessageHandler
{
    SixtyNineSendibleMessage? HandleMessage(ConnectionContext connection, SixtyNineMessage message);
}
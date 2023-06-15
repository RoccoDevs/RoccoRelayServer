namespace Rocco.RelayServer.Core.Client;

public class ClientTest
{
    private readonly ConnectionContext _connection;
    private readonly ProtocolReader _reader;
    private readonly SixtyNineWriter _messageWriter;

    public ClientTest(ConnectionContext connection)
    {
        _connection = connection;
        _reader = connection.CreateReader();

        _messageWriter = new SixtyNineWriter();
    }

    public async ValueTask SendAsync(SixtyNineMessage requestMessage, CancellationToken cancellationToken)
    {
        // Write request message length
        _messageWriter.WriteMessage(requestMessage, _connection.Transport.Output);

        await _connection.Transport.Output.FlushAsync(cancellationToken);
    }
}
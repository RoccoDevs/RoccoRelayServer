namespace Rocco.RelayServer.Core.Domain;

public abstract record SixtyNineMessage(string? Destination, SixtyNineMessageType PayloadType,
    Memory<byte>? Payload = default, string? Source = null);

public record InitMessage(string? Source,string? Destination = null) : SixtyNineMessage(Destination,
    SixtyNineMessageType.Init,Source: Source);

public record InitResponseMessage(string Destination) : SixtyNineMessage(Destination,
    SixtyNineMessageType.Init);

public record PayloadMessage(string Source, string Destination, Memory<byte>? Payload) : SixtyNineMessage(
    Destination, SixtyNineMessageType.Payload, Payload, Source);

public record ErrorMessage
    (string Destination, Memory<byte>? Payload, string? Source = null) : SixtyNineMessage(Destination,
        SixtyNineMessageType.Error, Payload, Source);

public record CloseMessage() : SixtyNineMessage(null, SixtyNineMessageType.Close);
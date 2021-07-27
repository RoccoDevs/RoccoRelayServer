using System;

namespace Rocco.RelayServer.Core.Domain
{
    public abstract record SixtyNineMessage(string? Source, SixtyNineMessageType PayloadType);

    public abstract record SixtyNineSendibleMessage(string Destination, SixtyNineMessageType PayloadType,
        Memory<byte>? Payload = default, string? Source = null) : SixtyNineMessage(Source,
        PayloadType);

    public record InitMessage(string? Source) : SixtyNineMessage(Source,
        SixtyNineMessageType.Init);

    public record InitResponseMessage(string Destination) : SixtyNineSendibleMessage(Destination,
        SixtyNineMessageType.Init);

    public record PayloadMessage(string Source, string Destination, Memory<byte>? Payload) : SixtyNineSendibleMessage(
        Destination, SixtyNineMessageType.Payload, Payload, Source);

    public record ErrorMessage
        (string Destination, Memory<byte>? Payload, string? Source = null) : SixtyNineSendibleMessage(Destination,
            SixtyNineMessageType.Error, Payload, Source);

    public record CloseMessage() : SixtyNineMessage(null, SixtyNineMessageType.Init);
}
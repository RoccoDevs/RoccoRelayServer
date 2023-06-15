namespace Rocco.RelayServer.Core.Services;

public class SixtyNineReader : IMessageReader<SixtyNineMessage>
{
    public virtual bool TryParseMessage(in ReadOnlySequence<byte> input, ref SequencePosition consumed,
        ref SequencePosition examined, out SixtyNineMessage message)
    {
        var sequenceReader = new SequenceReader<byte>(input);
        if (!sequenceReader.TryReadBigEndian(out int length) || input.Length < length + 4)
        {
            message = default!;
            return false;
        }

        var completed = false;
        string payloadType = default!;
        string source = default!;
        string destination = default!;
        Memory<byte> payload = default!;

        var rawPayload = input.Slice(sequenceReader.Position, length);

        //This reader is taken from Microsoft.AspNetCore.Internal
        var reader = new Utf8JsonReader(rawPayload, true, default);
        reader.CheckRead();

        reader.EnsureObjectStart();

        do
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.PropertyName when reader.ValueTextEquals(SixtyNinePropertyNames
                    .PayloadTypePropertyNameBytes
                    .EncodedUtf8Bytes):
                {
                    payloadType = reader.ReadAsString(SixtyNinePropertyNames.PayloadTypePropertyName)
                                  ?? throw new InvalidDataException(
                                      $"Expected '{SixtyNinePropertyNames.PayloadTypePropertyName}' to be of type {JsonTokenType.String}.");
                    break;
                }
                case JsonTokenType.PropertyName
                    when reader.ValueTextEquals(SixtyNineWriter.SourcePropertyNameBytes.EncodedUtf8Bytes):
                {
                    source = reader.ReadAsString(SixtyNineWriter.SourcePropertyName)
                             ?? throw new InvalidDataException(
                                 $"Expected '{SixtyNineWriter.SourcePropertyName}' to be of type {JsonTokenType.String}.");
                    break;
                }
                case JsonTokenType.PropertyName when reader.ValueTextEquals(SixtyNineWriter.DestinationPropertyNameBytes
                    .EncodedUtf8Bytes):
                {
                    destination = reader.ReadAsString(SixtyNineWriter.DestinationPropertyName)
                                  ?? throw new InvalidDataException(
                                      $"Expected '{SixtyNineWriter.DestinationPropertyName}' to be of type {JsonTokenType.String}.");
                    break;
                }
                case JsonTokenType.PropertyName when reader.ValueTextEquals(
                    SixtyNineWriter.PayloadPropertyNameBytes.EncodedUtf8Bytes):
                {
                    reader.Read();

                    if (reader.HasValueSequence)
                    {
                        payload = new byte[reader.ValueSequence.Length];
                        reader.ValueSequence.CopyTo(payload.Span);
                    }
                    else if (!reader.ValueSpan.IsEmpty)
                    {
                        payload = new byte[reader.ValueSpan.Length];
                        reader.ValueSpan.CopyTo(payload.Span);
                    }
                    else
                    {
                        payload = null;
                    }

                    break;
                }
                case JsonTokenType.PropertyName:
                {
                    reader.CheckRead();
                    reader.Skip();
                    break;
                }
                case JsonTokenType.EndObject:
                {
                    completed = true;
                    break;
                }
            }
        } while (!completed && reader.CheckRead());

        consumed = rawPayload.End;
        examined = consumed;

        message = GetSixtyNineMessageFromType(payloadType, source, destination, payload);
        return message != null;
    }


    private static SixtyNineMessage GetSixtyNineMessageFromType(string payloadType, string source,
        string destination, Memory<byte>? payload)
    {
        return payloadType switch
        {
            "INIT" => new InitMessage(source),
            "MESSAGE" => new PayloadMessage(source, destination, payload),
            "ERROR" => new ErrorMessage(destination, payload, source),
            "CLOSE" => new CloseMessage(),
            _ => throw new InvalidDataException(
                $"Expected '{SixtyNineWriter.PayloadPropertyName}' to be of type {JsonTokenType.String}.")
        };
    }
}
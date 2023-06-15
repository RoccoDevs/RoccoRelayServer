namespace Rocco.RelayServer.Core.Services;

public class SixtyNineWriter : IMessageWriter<SixtyNineSendibleMessage>
{
    public static readonly string SourcePropertyName = "source";
    public static readonly string DestinationPropertyName = "destination";
    public static readonly string PayloadPropertyName = "payload";


    public static readonly JsonEncodedText SourcePropertyNameBytes = JsonEncodedText.Encode(SourcePropertyName);

    public static readonly JsonEncodedText DestinationPropertyNameBytes =
        JsonEncodedText.Encode(DestinationPropertyName);

    public static readonly JsonEncodedText PayloadPropertyNameBytes = JsonEncodedText.Encode(PayloadPropertyName);


    public void WriteMessage(SixtyNineSendibleMessage message, IBufferWriter<byte> stream)
    {
        PrefixBufferWriter prefixWriter = new(stream);
        ReusableUtf8JsonWriter jsonWriter = new(prefixWriter);
        WriteContent(message, jsonWriter);
        prefixWriter.Complete();
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static void WriteContent(SixtyNineSendibleMessage message, ReusableUtf8JsonWriter reusableWriter)
    {
        try
        {
            var writer = reusableWriter.GetJsonWriter();

            writer.WriteStartObject();

            switch (message)
            {
                case ErrorMessage m:
                    WriteErrorMessage(m, writer);
                    break;
                case InitResponseMessage m:
                    WriteInitResponseMessage(m, writer);
                    break;
                case PayloadMessage m:
                    WritePayLoadMessage(m, writer);
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported message type: {message.GetType().FullName}");
            }

            writer.WriteEndObject();
            writer.Flush();
            Debug.Assert(writer.CurrentDepth == 0);
        }
        finally
        {
            ReusableUtf8JsonWriter.Return(reusableWriter);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static void WriteErrorMessage(ErrorMessage message, Utf8JsonWriter writer)
    {
        WritePayloadType(message, writer);

        if (message.Source is not null)
        {
            WriteSource(message, writer);
        }

        WriteDestination(message.Destination, writer);

        if (message.Payload is not null)
        {
            WritePayload(message.Payload.Value, writer);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static void WriteInitResponseMessage(InitResponseMessage message, Utf8JsonWriter writer)
    {
        WritePayloadType(message, writer);
        WriteDestination(message.Destination, writer);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static void WritePayLoadMessage(PayloadMessage message, Utf8JsonWriter writer)
    {
        WritePayloadType(message, writer);
        WriteSource(message, writer);
        WriteDestination(message.Destination, writer);
        if (message.Payload is not null)
        {
            WritePayload(message.Payload.Value, writer);
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static void WritePayloadType(SixtyNineSendibleMessage message, Utf8JsonWriter writer)
    {
        var payloadType = message.PayloadType switch
        {
            SixtyNineMessageType.Init => SixtyNinePropertyNames.PayloadTypeInitPropertyValue,
            SixtyNineMessageType.Payload => SixtyNinePropertyNames.PayloadTypePayloadPropertyValue,
            SixtyNineMessageType.Error => SixtyNinePropertyNames.PayloadTypeErrorPropertyValue,
            SixtyNineMessageType.Close => SixtyNinePropertyNames.PayloadTypeClosePropertyValue,
            _ => throw new InvalidOperationException($"Unsupported message type: {message.PayloadType}")
        };

        writer.WriteString(SixtyNinePropertyNames.PayloadTypePropertyNameBytes, payloadType);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static void WriteSource(SixtyNineSendibleMessage message, Utf8JsonWriter writer)
    {
        writer.WriteString(SourcePropertyNameBytes, message.Source);
    }


    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static void WriteDestination(string destination, Utf8JsonWriter writer)
    {
        writer.WriteString(DestinationPropertyNameBytes, destination);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static void WritePayload(Memory<byte> payload, Utf8JsonWriter writer)
    {
        writer.WriteString(PayloadPropertyNameBytes, payload.Span);
    }
}
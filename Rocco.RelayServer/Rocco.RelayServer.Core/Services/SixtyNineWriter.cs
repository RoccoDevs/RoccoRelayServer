using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Bedrock.Framework.Protocols;
using Rocco.RelayServer.Core.Domain;
using Rocco.RelayServer.Core.Helpers;
using Rocco.RelayServer.Core.Microsoft;

namespace Rocco.RelayServer.Core.Services;

/// <summary>
///     Class SixtyNineWriter.
///     Implements the <see cref="IMessageWriter{TMessage}" />
/// </summary>
/// <seealso cref="IMessageWriter{TMessage}" />
/// <autogeneratedoc />
public class SixtyNineWriter : IMessageWriter<SixtyNineSendibleMessage>
{
    public const string SourcePropertyName = "source";
    public const string DestinationPropertyName = "destination";
    public const string PayloadPropertyName = "payload";


    public static readonly JsonEncodedText SourcePropertyNameBytes = JsonEncodedText.Encode(SourcePropertyName);

    public static readonly JsonEncodedText DestinationPropertyNameBytes =
        JsonEncodedText.Encode(DestinationPropertyName);

    public static readonly JsonEncodedText PayloadPropertyNameBytes = JsonEncodedText.Encode(PayloadPropertyName);

    /// <summary>
    ///     Writes the message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="stream">The stream.</param>
    /// <autogeneratedoc />
    public void WriteMessage(SixtyNineSendibleMessage message, IBufferWriter<byte> stream)
    {
        PrefixBufferWriter prefixWriter = new(stream);
        ReusableUtf8JsonWriter jsonWriter = new(prefixWriter);
        WriteContent(message, jsonWriter);
        prefixWriter.Complete();
    }


    /// <summary>
    ///     Writes the content.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="reusableWriter">The stream.</param>
    /// <exception cref="InvalidOperationException">$"Unsupported message type: {message.GetType().FullName}</exception>
    /// <autogeneratedoc />
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

    /// <summary>
    ///     Writes the error message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="writer">The writer.</param>
    /// <autogeneratedoc />
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static void WriteErrorMessage(ErrorMessage message, Utf8JsonWriter writer)
    {
        WritePayloadType(message, writer);

        if (message.Source is not null) WriteSource(message, writer);

        WriteDestination(message.Destination, writer);

        if (message.Payload is not null) WritePayload(message.Payload.Value, writer);
    }

    /// <summary>
    ///     Writes the initialize response message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="writer">The writer.</param>
    /// <autogeneratedoc />
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static void WriteInitResponseMessage(InitResponseMessage message, Utf8JsonWriter writer)
    {
        WritePayloadType(message, writer);
        WriteDestination(message.Destination, writer);
    }

    /// <summary>
    ///     Writes the pay load message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="writer">The writer.</param>
    /// <autogeneratedoc />
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static void WritePayLoadMessage(PayloadMessage message, Utf8JsonWriter writer)
    {
        WritePayloadType(message, writer);
        WriteSource(message, writer);
        WriteDestination(message.Destination, writer);
        if (message.Payload is not null) WritePayload(message.Payload.Value, writer);
    }

    /// <summary>
    ///     Writes the type of the payload.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="writer">The writer.</param>
    /// <autogeneratedoc />
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

    /// <summary>
    ///     Writes the source.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="writer">The writer.</param>
    /// <exception cref="ArgumentNullException">nameof(message)</exception>
    /// <autogeneratedoc />
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static void WriteSource(SixtyNineSendibleMessage message, Utf8JsonWriter writer)
    {
        writer.WriteString(SourcePropertyNameBytes, message.Source);
    }

    /// <summary>
    ///     Writes the destination.
    /// </summary>
    /// <param name="destination">The destination.</param>
    /// <param name="writer">The writer.</param>
    /// <exception cref="ArgumentNullException">nameof(destination)</exception>
    /// <autogeneratedoc />
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static void WriteDestination(string destination, Utf8JsonWriter writer)
    {
        writer.WriteString(DestinationPropertyNameBytes, destination);
    }

    /// <summary>
    ///     Writes the payload.
    /// </summary>
    /// <param name="payload">The payload.</param>
    /// <param name="writer">The writer.</param>
    /// <exception cref="ArgumentNullException">nameof(payload)</exception>
    /// <autogeneratedoc />
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static void WritePayload(Memory<byte> payload, Utf8JsonWriter writer)
    {
        writer.WriteString(PayloadPropertyNameBytes, payload.Span);
    }
}
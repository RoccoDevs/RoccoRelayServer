using System.Text.Json;

namespace Rocco.RelayServer.Core.Helpers;

/// <summary>
///     Class SixtyNinePropertyNames.
/// </summary>
/// <autogeneratedoc />
public static class SixtyNinePropertyNames
{
    /// <summary>
    ///     The payload type property name
    /// </summary>
    /// <autogeneratedoc />
    public const string PayloadTypePropertyName = "payloadType";

    /// <summary>
    ///     The payload type property name bytes
    /// </summary>
    /// <autogeneratedoc />
    public static readonly JsonEncodedText PayloadTypePropertyNameBytes =
        JsonEncodedText.Encode(PayloadTypePropertyName);

    /// <summary>
    ///     The payload type initialize property value
    /// </summary>
    /// <autogeneratedoc />
    public static readonly JsonEncodedText PayloadTypeInitPropertyValue =
        JsonEncodedText.Encode(SixtyNineMessageTypeHelper.Init);

    /// <summary>
    ///     The payload type payload property value
    /// </summary>
    /// <autogeneratedoc />
    public static readonly JsonEncodedText PayloadTypePayloadPropertyValue =
        JsonEncodedText.Encode(SixtyNineMessageTypeHelper.Payload);

    /// <summary>
    ///     The payload type error property value
    /// </summary>
    /// <autogeneratedoc />
    public static readonly JsonEncodedText PayloadTypeErrorPropertyValue =
        JsonEncodedText.Encode(SixtyNineMessageTypeHelper.Error);

    /// <summary>
    ///     The payload type close property value
    /// </summary>
    /// <autogeneratedoc />
    public static readonly JsonEncodedText PayloadTypeClosePropertyValue =
        JsonEncodedText.Encode(SixtyNineMessageTypeHelper.Close);
}
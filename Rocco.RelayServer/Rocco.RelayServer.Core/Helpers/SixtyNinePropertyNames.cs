namespace Rocco.RelayServer.Core.Helpers;

public static class SixtyNinePropertyNames
{
    public const string PayloadTypePropertyName = "payloadType";

    public static readonly JsonEncodedText PayloadTypePropertyNameBytes =
        JsonEncodedText.Encode(PayloadTypePropertyName);

    public static readonly JsonEncodedText PayloadTypeInitPropertyValue =
        JsonEncodedText.Encode(SixtyNineMessageTypeHelper.Init);


    public static readonly JsonEncodedText PayloadTypePayloadPropertyValue =
        JsonEncodedText.Encode(SixtyNineMessageTypeHelper.Payload);

    public static readonly JsonEncodedText PayloadTypeErrorPropertyValue =
        JsonEncodedText.Encode(SixtyNineMessageTypeHelper.Error);

    public static readonly JsonEncodedText PayloadTypeClosePropertyValue =
        JsonEncodedText.Encode(SixtyNineMessageTypeHelper.Close);
}
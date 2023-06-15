namespace Rocco.RelayServer.Core.Helpers;

public static class SixtyNineMessageTypeHelper
{
    public static readonly string Init = "INIT";

    public static readonly string Payload = "MESSAGE";

    public static readonly string Error = "ERROR";

    public static readonly string Close = "CLOSE";

    public static string ToString(this SixtyNineMessageType sixtyNineMessageType)
    {
        return sixtyNineMessageType switch
        {
            SixtyNineMessageType.Init => Init,
            SixtyNineMessageType.Payload => Payload,
            SixtyNineMessageType.Close => Close,
            SixtyNineMessageType.Error => Error,
            _ => throw new InvalidDataException(
                $"Expected '{nameof(SixtyNineMessageType)}' to be of type {sixtyNineMessageType.GetType().Name}.")
        };
    }
}
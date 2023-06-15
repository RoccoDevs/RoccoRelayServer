using System.IO;
using Rocco.RelayServer.Core.Domain;

namespace Rocco.RelayServer.Core.Helpers;

public static class SixtyNineMessageTypeHelper
{
    public const string Init = "INIT";

    public const string Payload = "MESSAGE";

    public const string Error = "ERROR";

    public const string Close = "CLOSE";

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
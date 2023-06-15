using System.IO;
using Rocco.RelayServer.Core.Domain;

namespace Rocco.RelayServer.Core.Helpers;

/// <summary>
///     Class SixtyNineMessageTypeStrings.
/// </summary>
/// <autogeneratedoc />
public static class SixtyNineMessageTypeHelper
{
    /// <summary>
    ///     The initialize
    /// </summary>
    /// <autogeneratedoc />
    public const string Init = "INIT";

    /// <summary>
    ///     The payload
    /// </summary>
    /// <autogeneratedoc />
    public const string Payload = "MESSAGE";

    /// <summary>
    ///     The error
    /// </summary>
    /// <autogeneratedoc />
    public const string Error = "ERROR";

    /// <summary>
    ///     The close
    /// </summary>
    /// <autogeneratedoc />
    public const string Close = "CLOSE";

    /// <summary>
    ///     Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <param name="sixtyNineMessageType">Type of my payload.</param>
    /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
    /// <autogeneratedoc />
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
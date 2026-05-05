namespace Atc.Wpf.Hardware.Services.Internal;

internal static partial class UsbIdParser
{
    [GeneratedRegex(
        @"VID_(?<vid>[0-9A-F]{4})&PID_(?<pid>[0-9A-F]{4})",
        RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture,
        matchTimeoutMilliseconds: 100)]
    private static partial Regex VidPidRegex();

    public static (string? VendorId, string? ProductId) Parse(string deviceId)
    {
        if (string.IsNullOrEmpty(deviceId))
        {
            return (null, null);
        }

        var match = VidPidRegex().Match(deviceId);
        return match.Success
            ? (match.Groups["vid"].Value.ToUpperInvariant(), match.Groups["pid"].Value.ToUpperInvariant())
            : (null, null);
    }
}
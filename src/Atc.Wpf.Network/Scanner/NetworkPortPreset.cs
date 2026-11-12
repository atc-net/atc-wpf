namespace Atc.Wpf.Network.Scanner;

/// <summary>
/// A named list of ports for the network scanner, such as "IP cameras".
/// </summary>
/// <param name="key">A stable key, such as <see cref="IpCamerasKey"/>.</param>
/// <param name="name">The name shown to the user.</param>
/// <param name="ports">The ports.</param>
public sealed class NetworkPortPreset(
    string key,
    string name,
    IReadOnlyList<ushort> ports)
{
    /// <summary>
    /// The key of the well-known ports preset.
    /// </summary>
    public const string WellKnownKey = "well-known";

    /// <summary>
    /// The key of the well-known and common ports preset.
    /// </summary>
    public const string WellKnownAndCommonKey = "well-known-and-common";

    /// <summary>
    /// The key of the web (HTTP and HTTPS) ports preset.
    /// </summary>
    public const string WebKey = "web";

    /// <summary>
    /// The key of the remote access (SSH, Telnet, RDP, VNC) ports preset.
    /// </summary>
    public const string RemoteKey = "remote";

    /// <summary>
    /// The key of the file sharing (FTP, SMB) ports preset.
    /// </summary>
    public const string FileKey = "file";

    /// <summary>
    /// The key of the IP camera ports preset.
    /// </summary>
    public const string IpCamerasKey = "ip-cameras";

    /// <summary>
    /// Gets the stable key.
    /// </summary>
    public string Key { get; } = key;

    /// <summary>
    /// Gets the name shown to the user.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Gets the ports.
    /// </summary>
    public IReadOnlyList<ushort> Ports { get; } = ports;

    /// <summary>
    /// Creates the built-in presets, with names in the current UI culture.
    /// </summary>
    /// <returns>The presets.</returns>
    /// <remarks>
    /// The IP camera ports: 80, 443, 554 and 8554 (RTSP), 2020 (TP-Link Tapo ONVIF), 7070, 8000 (Hikvision),
    /// 8080, 8443, 8899 (ONVIF on some cameras), 34567 (XMEye) and 37777 (Dahua).
    /// </remarks>
    public static IReadOnlyList<NetworkPortPreset> CreateDefaults()
        =>
        [
            new(WellKnownKey, Resources.Resources.PortPresetWellKnown, [.. IPServicePortLists.GetWellKnown()]),
            new(WellKnownAndCommonKey, Resources.Resources.PortPresetWellKnownAndCommon, [.. IPServicePortLists.GetWellKnownOrCommon()]),
            new(WebKey, Resources.Resources.PortPresetWeb, [.. IPServicePortLists.WellKnownOrCommonForHttp.Concat(IPServicePortLists.WellKnownOrCommonForHttps).Distinct().Order()]),
            new(RemoteKey, Resources.Resources.PortPresetRemote, [22, 23, 3389, 5900, 5901]),
            new(FileKey, Resources.Resources.PortPresetFile, [20, 21, 139, 445, 990]),
            new(IpCamerasKey, Resources.Resources.PortPresetIpCameras, [80, 443, 554, 2020, 7070, 8000, 8080, 8443, 8554, 8899, 34567, 37777]),
        ];

    /// <inheritdoc />
    public override string ToString()
        => Name;
}
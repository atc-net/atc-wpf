// ReSharper disable ConvertIfStatementToSwitchStatement
namespace Atc.Wpf.Controls.LabelControls.Internal.Helpers;

internal static class NetworkProtocolHelper
{
    public static string GetSchemeFromProtocol(NetworkProtocolType protocol)
        => protocol switch
        {
            NetworkProtocolType.Ftp => "ftp",
            NetworkProtocolType.Ftps => "ftps",
            NetworkProtocolType.Http => "http",
            NetworkProtocolType.Https => "https",
            NetworkProtocolType.OpcTcp => "opc.tcp",
            NetworkProtocolType.Tcp => "tcp",
            NetworkProtocolType.Udp => "udp",
            _ => string.Empty,
        };

    public static NetworkProtocolType GetProtocolFromScheme(string scheme)
        => scheme.ToLowerInvariant() switch
        {
            "ftp" => NetworkProtocolType.Ftp,
            "ftps" => NetworkProtocolType.Ftps,
            "http" => NetworkProtocolType.Http,
            "https" => NetworkProtocolType.Https,
            "opc.tcp" => NetworkProtocolType.OpcTcp,
            "tcp" => NetworkProtocolType.Tcp,
            "udp" => NetworkProtocolType.Udp,
            _ => NetworkProtocolType.Http,
        };
}
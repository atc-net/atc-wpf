// ReSharper disable InconsistentNaming
namespace Atc.Wpf.Controls;

/// <summary>Specifies a network protocol.</summary>
public enum NetworkProtocolType
{
    /// <summary>No protocol.</summary>
    None,

    /// <summary>File Transfer Protocol (FTP).</summary>
    Ftp,

    /// <summary>FTP over TLS (FTPS).</summary>
    Ftps,

    /// <summary>Hypertext Transfer Protocol (HTTP).</summary>
    Http,

    /// <summary>HTTP over TLS (HTTPS).</summary>
    Https,

    /// <summary>OPC UA binary protocol over TCP (opc.tcp).</summary>
    OpcTcp,

    /// <summary>Transmission Control Protocol (TCP).</summary>
    Tcp,

    /// <summary>User Datagram Protocol (UDP).</summary>
    Udp,
}
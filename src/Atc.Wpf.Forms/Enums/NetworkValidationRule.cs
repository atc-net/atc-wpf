// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace
namespace Atc.Wpf.Forms;

/// <summary>
/// Specifies how a host value is validated.
/// </summary>
public enum NetworkValidationRule
{
    /// <summary>
    /// No validation.
    /// </summary>
    None,

    /// <summary>
    /// The value must be an IPv4 or IPv6 address.
    /// </summary>
    IPAddress,

    /// <summary>
    /// The value must be an IPv4 address.
    /// </summary>
    IPv4Address,

    /// <summary>
    /// The value must be an IPv6 address.
    /// </summary>
    IPv6Address,

    /// <summary>
    /// The value must be a hostname.
    /// </summary>
    Hostname,

    /// <summary>
    /// The value must be an IPv4 address or a hostname.
    /// </summary>
    IPv4AddressOrHostname,

    /// <summary>
    /// The value must be an IPv6 address or a hostname.
    /// </summary>
    IPv6AddressOrHostname,

    /// <summary>
    /// The value must be an IPv4 or IPv6 address, or a hostname.
    /// </summary>
    IPAddressOrHostname,
}
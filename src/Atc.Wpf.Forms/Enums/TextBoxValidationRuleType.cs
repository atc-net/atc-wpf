// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace
namespace Atc.Wpf.Forms;

/// <summary>
/// Defines validation rules for text box input.
/// </summary>
public enum TextBoxValidationRuleType
{
    /// <summary>
    /// No validation applied.
    /// </summary>
    None,

    /// <summary>
    /// Validates email address format.
    /// </summary>
    Email,

    /// <summary>
    /// Validates FTP URL format.
    /// </summary>
    Ftp,

    /// <summary>
    /// Validates FTPS (secure FTP) URL format.
    /// </summary>
    Ftps,

    /// <summary>
    /// Validates FTP or FTPS URL format.
    /// </summary>
    FtpOrFtps,

    /// <summary>
    /// Validates hexadecimal RGB color format (#RRGGBB).
    /// </summary>
    HexRGB,

    /// <summary>
    /// Validates hexadecimal ARGB color format (#AARRGGBB).
    /// </summary>
    HexARGB,

    /// <summary>
    /// Validates HTTP URL format.
    /// </summary>
    Http,

    /// <summary>
    /// Validates HTTPS URL format.
    /// </summary>
    Https,

    /// <summary>
    /// Validates HTTP or HTTPS URL format.
    /// </summary>
    HttpOrHttps,

    /// <summary>
    /// Validates IPv4 or IPv6 address format.
    /// </summary>
    IPAddress,

    /// <summary>
    /// Validates IPv4 address format only.
    /// </summary>
    IPv4Address,

    /// <summary>
    /// Validates IPv6 address format only.
    /// </summary>
    IPv6Address,

    /// <summary>
    /// Validates OPC TCP URL format.
    /// </summary>
    OpcTcp,

    /// <summary>
    /// Validates TCP URL format.
    /// </summary>
    Tcp,

    /// <summary>
    /// Validates UDP URL format.
    /// </summary>
    Udp,

    /// <summary>
    /// Validates weak password strength (minimum 6 characters).
    /// </summary>
    PasswordWeak,

    /// <summary>
    /// Validates medium password strength (minimum 8 characters, mixed case).
    /// </summary>
    PasswordMedium,

    /// <summary>
    /// Validates strong password strength (minimum 12 characters, mixed case, numbers, special characters).
    /// </summary>
    PasswordStrong,
}
namespace Atc.Wpf.Forms.Abstractions;

/// <summary>
/// Represents a labeled network endpoint input control (protocol, host and port).
/// </summary>
public interface ILabelEndpointBox : ILabelControl
{
    /// <summary>
    /// Gets or sets the watermark text shown when the host is empty.
    /// </summary>
    string WatermarkText { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a clear button is shown inside the host text box.
    /// </summary>
    bool ShowClearTextButton { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the up/down buttons on the port box are hidden.
    /// </summary>
    bool HideUpDownButtons { get; set; }

    /// <summary>
    /// Gets or sets the lowest allowed port.
    /// </summary>
    int MinimumPort { get; set; }

    /// <summary>
    /// Gets or sets the highest allowed port.
    /// </summary>
    int MaximumPort { get; set; }

    /// <summary>
    /// Gets or sets the network protocol used for the scheme.
    /// </summary>
    NetworkProtocolType NetworkProtocol { get; set; }

    /// <summary>
    /// Gets or sets the additional validation rule applied to the host.
    /// </summary>
    NetworkValidationRule NetworkValidation { get; set; }

    /// <summary>
    /// Gets or sets the host name or IP address.
    /// </summary>
    string Host { get; set; }

    /// <summary>
    /// Gets or sets the port number.
    /// </summary>
    int Port { get; set; }

    /// <summary>
    /// Gets or sets the endpoint as a URI composed of protocol, host and port.
    /// </summary>
    Uri? Value { get; set; }
}
namespace Atc.Wpf.Network.Diagnostics;

/// <summary>
/// ViewModel that wakes a device with a Wake-on-LAN magic packet.
/// </summary>
public partial class WakeOnLanViewModel : ViewModelBase
{
    /// <summary>
    /// Gets or sets the MAC address of the device to wake, such as <c>00-11-22-33-44-55</c>.
    /// </summary>
    [ObservableProperty]
    private string? macAddress;

    /// <summary>
    /// Gets or sets the address the packet is sent to; the subnet's broadcast address reaches sleeping devices.
    /// </summary>
    [ObservableProperty]
    private string broadcastAddress = IPAddress.Broadcast.ToString();

    /// <summary>
    /// Gets or sets the UDP port; 9 by default (7 on some devices).
    /// </summary>
    [ObservableProperty]
    private int port = WakeOnLanHelper.DefaultPort;

    /// <summary>
    /// Gets or sets the result of the last send.
    /// </summary>
    [ObservableProperty]
    private string? statusMessage;

    /// <summary>
    /// Gets or sets the error message of the last send.
    /// </summary>
    [ObservableProperty(DependentPropertyNames = [nameof(HasError)])]
    private string? errorMessage;

    /// <summary>
    /// Gets a value indicating whether the last send failed.
    /// </summary>
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    /// <summary>
    /// Gets or sets how the packet is sent; by default with <see cref="WakeOnLanHelper"/>.
    /// </summary>
    public Func<string, IPEndPoint, CancellationToken, Task> Sender { get; set; } = (mac, endPoint, cancellationToken)
        => WakeOnLanHelper.SendMagicPacket(mac, endPoint, cancellationToken: cancellationToken);

    /// <summary>
    /// Gets whether a text is a MAC address: 12 hexadecimal digits, optionally separated by <c>-</c>, <c>:</c> or <c>.</c>.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns><c>true</c> for a MAC address.</returns>
    public static bool IsValidMacAddress(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        var digits = text.Trim().Replace("-", string.Empty, StringComparison.Ordinal)
            .Replace(":", string.Empty, StringComparison.Ordinal)
            .Replace(".", string.Empty, StringComparison.Ordinal);
        return digits.Length == 12 && digits.All(char.IsAsciiHexDigit);
    }

    private bool CanSend()
        => IsValidMacAddress(MacAddress) &&
           IPAddress.TryParse(BroadcastAddress, out _) &&
           Port is > 0 and <= ushort.MaxValue;

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "User-facing error handling requires catching all exceptions.")]
    [RelayCommand(CanExecute = nameof(CanSend))]
    private async Task Send(CancellationToken cancellationToken)
    {
        StatusMessage = null;
        ErrorMessage = null;
        var mac = MacAddress!.Trim();
        try
        {
            await Sender(mac, new IPEndPoint(IPAddress.Parse(BroadcastAddress), Port), cancellationToken).ConfigureAwait(true);
            StatusMessage = string.Format(CultureInfo.CurrentCulture, Resources.Resources.MagicPacketSentFormat1, mac);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            ErrorMessage = string.Format(CultureInfo.CurrentCulture, Resources.Resources.WakeOnLanErrorFormat1, ex.Message);
        }
    }
}
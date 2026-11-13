namespace Atc.Wpf.Network.Diagnostics;

/// <summary>
/// A button that wakes the device with <see cref="MacAddress"/> with a Wake-on-LAN magic packet when clicked.
/// It is disabled while the MAC address is not valid.
/// </summary>
public partial class WakeOnLanButton : Button
{
    /// <summary>
    /// The MAC address of the device to wake.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnMacAddressChanged))]
    private string? macAddress;

    /// <summary>
    /// The address the packet is sent to.
    /// </summary>
    [DependencyProperty(DefaultValue = "255.255.255.255")]
    private string broadcastAddress;

    /// <summary>
    /// The UDP port.
    /// </summary>
    [DependencyProperty(DefaultValue = 9)]
    private int port;

    private bool isSending;

    /// <summary>
    /// Initializes a new instance of the <see cref="WakeOnLanButton"/> class.
    /// </summary>
    public WakeOnLanButton()
        => SetCurrentValue(IsEnabledProperty, false);

    /// <summary>
    /// Occurs after a click sent the packet, or failed to.
    /// </summary>
    public event EventHandler<WakeOnLanSentEventArgs>? PacketSent;

    /// <summary>
    /// Gets or sets how the packet is sent; by default with <see cref="WakeOnLanHelper"/>.
    /// </summary>
    public Func<string, IPEndPoint, CancellationToken, Task> Sender { get; set; } = (mac, endPoint, cancellationToken)
        => WakeOnLanHelper.SendMagicPacket(mac, endPoint, cancellationToken: cancellationToken);

    /// <summary>
    /// Sends the magic packet, as a click does.
    /// </summary>
    /// <returns>A task that completes when the packet was sent.</returns>
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Reported through PacketSent.")]
    public async Task SendAsync()
    {
        var mac = MacAddress;
        if (isSending || !WakeOnLanViewModel.IsValidMacAddress(mac))
        {
            return;
        }

        // One packet per click: a second click (or call) while sending is ignored, and the button shows it.
        isSending = true;
        SetCurrentValue(IsEnabledProperty, false);

        Exception? error = null;
        try
        {
            var address = IPAddress.TryParse(BroadcastAddress, out var parsed) ? parsed : IPAddress.Broadcast;
            await Sender(mac!.Trim(), new IPEndPoint(address, Port), CancellationToken.None).ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            error = ex;
        }
        finally
        {
            isSending = false;
            SetCurrentValue(IsEnabledProperty, WakeOnLanViewModel.IsValidMacAddress(MacAddress));
        }

        PacketSent?.Invoke(this, new WakeOnLanSentEventArgs(mac!.Trim(), error));
    }

    /// <inheritdoc />
    protected override void OnClick()
    {
        base.OnClick();
        _ = SendAsync();
    }

    private static void OnMacAddressChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var button = (WakeOnLanButton)d;
        button.SetCurrentValue(IsEnabledProperty, !button.isSending && WakeOnLanViewModel.IsValidMacAddress(e.NewValue as string));
    }
}
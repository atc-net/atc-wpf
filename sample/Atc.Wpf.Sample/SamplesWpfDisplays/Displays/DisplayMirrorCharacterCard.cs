namespace Atc.Wpf.Sample.SamplesWpfDisplays.Displays;

/// <summary>
/// A character LCD card: I2cWrite packets go through a PCF8574 backpack decoder, Commands and Data packets straight to the HD44780 emulator.
/// </summary>
public sealed partial class DisplayMirrorCharacterCard : DisplayMirrorCard
{
    private readonly Dispatcher dispatcher;
    private readonly Pcf8574Hd44780Decoder decoder;

    [ObservableProperty]
    private bool isBacklightOn = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayMirrorCharacterCard"/> class.
    /// </summary>
    /// <param name="displayId">The display id.</param>
    /// <param name="describe">The description: columns (width) 1 to 40, rows (height) 1 to 4.</param>
    public DisplayMirrorCharacterCard(
        byte displayId,
        DisplayMirrorDescribe describe)
        : base(displayId, describe)
    {
        ArgumentNullException.ThrowIfNull(describe);

        dispatcher = Dispatcher.CurrentDispatcher;
        Buffer = new CharacterDisplayBuffer(describe.Width, describe.Height);

        // A 40 x 4 module has a second controller for rows 3 and 4 (a second enable line); one link drives the first only here.
        var controller = new Hd44780Controller(Buffer);
        decoder = new Pcf8574Hd44780Decoder(controller);
        decoder.BacklightChanged += OnBacklightChanged;

        Target = DisplayMirrorTargets.Combine(DisplayMirrorTargets.For(decoder), DisplayMirrorTargets.For(controller));
    }

    /// <summary>
    /// Gets the buffer the control shows.
    /// </summary>
    public CharacterDisplayBuffer Buffer { get; }

    /// <summary>
    /// Gets the columns.
    /// </summary>
    public int Columns => Buffer.Columns;

    /// <summary>
    /// Gets the rows.
    /// </summary>
    public int Rows => Buffer.Rows;

    /// <inheritdoc />
    public override IDisplayMirrorTarget? Target { get; }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            decoder.BacklightChanged -= OnBacklightChanged;
        }

        base.Dispose(disposing);
    }

    // Raised on the receiver's thread under its lock: hand the new state to the UI thread without waiting.
    private void OnBacklightChanged(
        object? sender,
        EventArgs e)
    {
        var isOn = decoder.IsBacklightOn;
        _ = dispatcher.BeginInvoke(() => IsBacklightOn = isOn);
    }
}
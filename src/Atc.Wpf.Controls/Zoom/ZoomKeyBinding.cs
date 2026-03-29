namespace Atc.Wpf.Controls.Zoom;

/// <summary>
/// Maps a keyboard gesture to a zoom action for use with
/// <see cref="ZoomBox.ZoomKeyBindings"/>.
/// </summary>
public sealed class ZoomKeyBinding
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ZoomKeyBinding"/> class.
    /// </summary>
    public ZoomKeyBinding()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ZoomKeyBinding"/> class.
    /// </summary>
    public ZoomKeyBinding(
        Key key,
        ModifierKeys modifiers,
        ZoomActionType action)
    {
        Key = key;
        Modifiers = modifiers;
        Action = action;
    }

    /// <summary>Gets or sets the key.</summary>
    public Key Key { get; set; }

    /// <summary>Gets or sets the modifier keys (Ctrl, Shift, Alt).</summary>
    public ModifierKeys Modifiers { get; set; }

    /// <summary>Gets or sets the zoom action to execute.</summary>
    public ZoomActionType Action { get; set; }

    /// <summary>
    /// Gets the default keyboard bindings used when no custom bindings are set.
    /// </summary>
    public static IList<ZoomKeyBinding> CreateDefaults()
        =>
        [
            new(Key.Add, ModifierKeys.Control, ZoomActionType.ZoomIn),
            new(Key.OemPlus, ModifierKeys.Control, ZoomActionType.ZoomIn),
            new(Key.Subtract, ModifierKeys.Control, ZoomActionType.ZoomOut),
            new(Key.OemMinus, ModifierKeys.Control, ZoomActionType.ZoomOut),
            new(Key.D8, ModifierKeys.Control | ModifierKeys.Alt, ZoomActionType.ZoomFill),
            new(Key.D9, ModifierKeys.Control | ModifierKeys.Alt, ZoomActionType.ZoomFit),
            new(Key.D0, ModifierKeys.Control | ModifierKeys.Alt, ZoomActionType.Zoom100Percent),
        ];
}
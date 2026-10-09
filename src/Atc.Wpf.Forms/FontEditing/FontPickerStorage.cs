namespace Atc.Wpf.Forms.FontEditing;

/// <summary>
/// Static accessor for the shared <see cref="IFontPickerStorage"/> instance used by
/// FontPicker controls when no explicit storage is provided. Apps that need persistence
/// across restarts should assign a custom implementation to <see cref="Current"/> at startup.
/// </summary>
public static class FontPickerStorage
{
    private static IFontPickerStorage current = new InMemoryFontPickerStorage();

    /// <summary>
    /// Gets or sets the shared storage; defaults to an <see cref="InMemoryFontPickerStorage"/> and cannot be set to <see langword="null"/>.
    /// </summary>
    public static IFontPickerStorage Current
    {
        get => current;
        set => current = value ?? throw new ArgumentNullException(nameof(value));
    }
}
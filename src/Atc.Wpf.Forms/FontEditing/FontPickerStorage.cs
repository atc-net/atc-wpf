namespace Atc.Wpf.Forms.FontEditing;

/// <summary>
/// Static accessor for the shared <see cref="IFontPickerStorage"/> instance used by
/// FontPicker controls when no explicit storage is provided. Apps that need persistence
/// across restarts should assign a custom implementation to <see cref="Current"/> at startup.
/// </summary>
public static class FontPickerStorage
{
    private static IFontPickerStorage current = new InMemoryFontPickerStorage();

    public static IFontPickerStorage Current
    {
        get => current;
        set => current = value ?? throw new ArgumentNullException(nameof(value));
    }
}
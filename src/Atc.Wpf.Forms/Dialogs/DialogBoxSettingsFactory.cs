namespace Atc.Wpf.Forms.Dialogs;

/// <summary>
/// Creates preconfigured <see cref="DialogBoxSettings"/> for OK-only information, warning and error dialogs.
/// </summary>
public static class DialogBoxSettingsFactory
{
    /// <summary>
    /// Creates settings for an information dialog with an OK button.
    /// </summary>
    /// <returns>The new settings.</returns>
    public static DialogBoxSettings CreateInformation()
        => new(
            DialogBoxType.Ok,
            LogCategoryType.Information)
        {
            TitleBarText = Atc.Resources.EnumResources.LogCategoryTypeInformation,
            Width = 500,
        };

    /// <summary>
    /// Creates settings for a warning dialog with an OK button.
    /// </summary>
    /// <returns>The new settings.</returns>
    public static DialogBoxSettings CreateWarning()
        => new(
            DialogBoxType.Ok,
            LogCategoryType.Warning)
        {
            TitleBarText = Atc.Resources.EnumResources.LogCategoryTypeWarning,
            Width = 500,
        };

    /// <summary>
    /// Creates settings for an error dialog with an OK button.
    /// </summary>
    /// <returns>The new settings.</returns>
    public static DialogBoxSettings CreateError()
        => new(
            DialogBoxType.Ok,
            LogCategoryType.Error)
        {
            TitleBarText = Atc.Resources.EnumResources.LogCategoryTypeError,
            Width = 500,
        };
}
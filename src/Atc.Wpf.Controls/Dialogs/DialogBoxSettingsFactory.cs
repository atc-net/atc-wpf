namespace Atc.Wpf.Controls.Dialogs;

public static class DialogBoxSettingsFactory
{
    public static DialogBoxSettings CreateInformation()
        => new(
            DialogBoxType.Ok,
            LogCategoryType.Information)
        {
            TitleBarText = Atc.Resources.EnumResources.LogCategoryTypeInformation,
            Width = 500,
        };

    public static DialogBoxSettings CreateWarning()
        => new(
            DialogBoxType.Ok,
            LogCategoryType.Warning)
        {
            TitleBarText = Atc.Resources.EnumResources.LogCategoryTypeWarning,
            Width = 500,
        };

    public static DialogBoxSettings CreateError()
        => new(
            DialogBoxType.Ok,
            LogCategoryType.Error)
        {
            TitleBarText = Atc.Resources.EnumResources.LogCategoryTypeError,
            Width = 500,
        };
}
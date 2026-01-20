// ReSharper disable CheckNamespace
namespace Atc.Wpf.Forms;

using ThemingMiscellaneous = Atc.Wpf.Theming.Resources.Miscellaneous;

public partial class LabelThemeSelector
{
    [DependencyProperty(DefaultValue = RenderColorIndicatorType.Square)]
    private RenderColorIndicatorType renderColorIndicatorType;

    public LabelThemeSelector()
    {
        InitializeComponent();

        if (string.IsNullOrEmpty(LabelText))
        {
            LabelText = ThemingMiscellaneous.Theme;
        }

        CultureManager.UiCultureChanged += OnUiCultureChanged;
    }

    private void OnUiCultureChanged(
        object? sender,
        UiCultureEventArgs e)
    {
        var oldTranslation = ThemingMiscellaneous.ResourceManager.GetString(nameof(ThemingMiscellaneous.Theme), e.OldCulture);
        if (oldTranslation is not null && oldTranslation.Equals(LabelText, StringComparison.Ordinal))
        {
            LabelText = ThemingMiscellaneous.Theme;
        }
    }
}
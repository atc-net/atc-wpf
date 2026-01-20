// ReSharper disable CheckNamespace
namespace Atc.Wpf.Forms;

using ThemingMiscellaneous = Atc.Wpf.Theming.Resources.Miscellaneous;

public partial class LabelAccentColorSelector
{
    [DependencyProperty(DefaultValue = RenderColorIndicatorType.Square)]
    private RenderColorIndicatorType renderColorIndicatorType;

    public LabelAccentColorSelector()
    {
        InitializeComponent();

        if (string.IsNullOrEmpty(LabelText))
        {
            LabelText = ThemingMiscellaneous.Accent;
        }

        CultureManager.UiCultureChanged += OnUiCultureChanged;
    }

    private void OnUiCultureChanged(
        object? sender,
        UiCultureEventArgs e)
    {
        var oldTranslation = ThemingMiscellaneous.ResourceManager.GetString(nameof(ThemingMiscellaneous.Accent), e.OldCulture);
        if (oldTranslation is not null && oldTranslation.Equals(LabelText, StringComparison.Ordinal))
        {
            LabelText = ThemingMiscellaneous.Accent;
        }
    }
}
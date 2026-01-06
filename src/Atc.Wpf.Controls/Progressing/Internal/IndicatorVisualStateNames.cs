namespace Atc.Wpf.Controls.Progressing.Internal;

internal sealed class IndicatorVisualStateNames : MarkupExtension
{
    private static IndicatorVisualStateNames? activeState;
    private static IndicatorVisualStateNames? inactiveState;

    public static IndicatorVisualStateNames ActiveState
        => activeState ??= new IndicatorVisualStateNames("Active");

    public static IndicatorVisualStateNames InactiveState
        => inactiveState ??= new IndicatorVisualStateNames("Inactive");

    public string Name { get; }

    public override object ProvideValue(IServiceProvider serviceProvider)
        => Name;

    private IndicatorVisualStateNames(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        Name = name;
    }
}
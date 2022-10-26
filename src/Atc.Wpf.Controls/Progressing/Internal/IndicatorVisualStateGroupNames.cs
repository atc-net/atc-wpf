namespace Atc.Wpf.Controls.Progressing.Internal;

internal class IndicatorVisualStateGroupNames : MarkupExtension
{
    private static IndicatorVisualStateGroupNames? internalActiveStates;
    private static IndicatorVisualStateGroupNames? sizeStates;

    public static IndicatorVisualStateGroupNames ActiveStates
        => internalActiveStates ??= new IndicatorVisualStateGroupNames("ActiveStates");

    public static IndicatorVisualStateGroupNames SizeStates
        => sizeStates ??= new IndicatorVisualStateGroupNames("SizeStates");

    public string Name { get; }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return Name;
    }

    private IndicatorVisualStateGroupNames(
        string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        Name = name;
    }
}
namespace Atc.Wpf.Theming.Helpers;

[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static class HeaderedControlHelper
{
    public static readonly DependencyProperty HeaderBackgroundProperty = DependencyProperty.RegisterAttached(
        "HeaderBackground",
        typeof(Brush),
        typeof(HeaderedControlHelper),
        new UIPropertyMetadata(Panel.BackgroundProperty.DefaultMetadata.DefaultValue));

    public static Brush GetHeaderBackground(
        UIElement element)
        => (Brush)element.GetValue(HeaderBackgroundProperty);

    public static readonly DependencyProperty HeaderHorizontalContentAlignmentProperty = DependencyProperty.RegisterAttached(
        "HeaderHorizontalContentAlignment",
        typeof(HorizontalAlignment),
        typeof(HeaderedControlHelper),
        new FrameworkPropertyMetadata(HorizontalAlignment.Stretch));

    public static HorizontalAlignment GetHeaderHorizontalContentAlignment(
        UIElement element)
        => (HorizontalAlignment)element.GetValue(HeaderHorizontalContentAlignmentProperty);

    public static readonly DependencyProperty HeaderMarginProperty = DependencyProperty.RegisterAttached(
        "HeaderMargin",
        typeof(Thickness),
        typeof(HeaderedControlHelper),
        new UIPropertyMetadata(new Thickness(0)));

    public static Thickness GetHeaderMargin(
        UIElement element)
        => (Thickness)element.GetValue(HeaderMarginProperty);
}
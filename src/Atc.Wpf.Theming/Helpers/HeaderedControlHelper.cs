namespace Atc.Wpf.Theming.Helpers;

[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static class HeaderedControlHelper
{
    public static readonly DependencyProperty HeaderForegroundProperty = DependencyProperty.RegisterAttached(
        "HeaderForeground",
        typeof(Brush),
        typeof(HeaderedControlHelper),
        new UIPropertyMetadata(Brushes.White));

    public static Brush GetHeaderForeground(
        UIElement element)
        => (Brush)element.GetValue(HeaderForegroundProperty);

    public static void SetHeaderForeground(
        UIElement element,
        Brush value)
        => element.SetValue(HeaderForegroundProperty, value);

    public static readonly DependencyProperty HeaderBackgroundProperty = DependencyProperty.RegisterAttached(
        "HeaderBackground",
        typeof(Brush),
        typeof(HeaderedControlHelper),
        new UIPropertyMetadata(Panel.BackgroundProperty.DefaultMetadata.DefaultValue));

    public static Brush GetHeaderBackground(
        UIElement element)
        => (Brush)element.GetValue(HeaderBackgroundProperty);

    public static void SetHeaderBackground(
        UIElement element,
        Brush value)
        => element.SetValue(HeaderBackgroundProperty, value);

    public static readonly DependencyProperty HeaderHorizontalContentAlignmentProperty = DependencyProperty.RegisterAttached(
        "HeaderHorizontalContentAlignment",
        typeof(HorizontalAlignment),
        typeof(HeaderedControlHelper),
        new FrameworkPropertyMetadata(HorizontalAlignment.Stretch));

    public static HorizontalAlignment GetHeaderHorizontalContentAlignment(
        UIElement element)
        => (HorizontalAlignment)element.GetValue(HeaderHorizontalContentAlignmentProperty);

    public static void SetHeaderHorizontalContentAlignment(
        UIElement element,
        HorizontalAlignment value)
        => element.SetValue(HeaderHorizontalContentAlignmentProperty, value);

    public static readonly DependencyProperty HeaderVerticalContentAlignmentProperty = DependencyProperty.RegisterAttached(
        "HeaderVerticalContentAlignment",
        typeof(VerticalAlignment),
        typeof(HeaderedControlHelper),
        new FrameworkPropertyMetadata(VerticalAlignment.Stretch));

    public static VerticalAlignment GetHeaderVerticalContentAlignment(
        UIElement element)
        => (VerticalAlignment)element.GetValue(HeaderVerticalContentAlignmentProperty);

    public static void SetHeaderVerticalContentAlignment(
        UIElement element,
        VerticalAlignment value)
        => element.SetValue(HeaderVerticalContentAlignmentProperty, value);

    public static readonly DependencyProperty HeaderMarginProperty = DependencyProperty.RegisterAttached(
        "HeaderMargin",
        typeof(Thickness),
        typeof(HeaderedControlHelper),
        new UIPropertyMetadata(new Thickness(0)));

    public static Thickness GetHeaderMargin(
        UIElement element)
        => (Thickness)element.GetValue(HeaderMarginProperty);

    public static void SetHeaderMargin(
        UIElement element,
        Thickness value)
        => element.SetValue(HeaderMarginProperty, value);

    public static readonly DependencyProperty HeaderFontFamilyProperty = DependencyProperty.RegisterAttached(
        "HeaderFontFamily",
        typeof(FontFamily),
        typeof(HeaderedControlHelper),
        new FrameworkPropertyMetadata(
            SystemFonts.MessageFontFamily,
            FrameworkPropertyMetadataOptions.Inherits));

    public static FontFamily GetHeaderFontFamily(
        UIElement element)
        => (FontFamily)element.GetValue(HeaderFontFamilyProperty);

    public static void SetHeaderFontFamily(
        UIElement element,
        FontFamily value)
        => element.SetValue(HeaderFontFamilyProperty, value);

    public static readonly DependencyProperty HeaderFontSizeProperty = DependencyProperty.RegisterAttached(
        "HeaderFontSize",
        typeof(double),
        typeof(HeaderedControlHelper),
        new FrameworkPropertyMetadata(
            SystemFonts.MessageFontSize,
            FrameworkPropertyMetadataOptions.Inherits));

    public static double GetHeaderFontSize(
        UIElement element)
        => (double)element.GetValue(HeaderFontSizeProperty);

    public static void SetHeaderFontSize(
        UIElement element,
        double value)
        => element.SetValue(HeaderFontSizeProperty, value);

    public static readonly DependencyProperty HeaderFontStretchProperty = DependencyProperty.RegisterAttached(
        "HeaderFontStretch",
        typeof(FontStretch),
        typeof(HeaderedControlHelper),
        new FrameworkPropertyMetadata(
            TextElement.FontStretchProperty.DefaultMetadata.DefaultValue,
            FrameworkPropertyMetadataOptions.Inherits));

    public static FontStretch GetHeaderFontStretch(
        UIElement element)
        => (FontStretch)element.GetValue(HeaderFontStretchProperty);

    public static void SetHeaderFontStretch(
        UIElement element,
        FontStretch value)
        => element.SetValue(HeaderFontStretchProperty, value);

    public static readonly DependencyProperty HeaderFontWeightProperty = DependencyProperty.RegisterAttached(
        "HeaderFontWeight",
        typeof(FontWeight),
        typeof(HeaderedControlHelper),
        new FrameworkPropertyMetadata(
            SystemFonts.MessageFontWeight,
            FrameworkPropertyMetadataOptions.Inherits));

    public static FontWeight GetHeaderFontWeight(
        UIElement element)
        => (FontWeight)element.GetValue(HeaderFontWeightProperty);

    public static void SetHeaderFontWeight(
        UIElement element,
        FontWeight value)
        => element.SetValue(HeaderFontWeightProperty, value);
}
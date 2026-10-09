namespace Atc.Wpf.Helpers;

/// <summary>Provides attached properties for styling the header of headered controls, such as header brushes, alignment, margin and font.</summary>
[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static class HeaderedControlHelper
{
    /// <summary>Identifies the HeaderForeground attached property.</summary>
    public static readonly DependencyProperty HeaderForegroundProperty = DependencyProperty.RegisterAttached(
        "HeaderForeground",
        typeof(Brush),
        typeof(HeaderedControlHelper),
        new UIPropertyMetadata(Brushes.White));

    /// <summary>Gets the value of the HeaderForeground attached property.</summary>
    public static Brush GetHeaderForeground(UIElement element)
        => (Brush)element.GetValue(HeaderForegroundProperty);

    /// <summary>Sets the value of the HeaderForeground attached property.</summary>
    public static void SetHeaderForeground(
        UIElement element,
        Brush value)
        => element.SetValue(HeaderForegroundProperty, value);

    /// <summary>Identifies the HeaderBackground attached property.</summary>
    public static readonly DependencyProperty HeaderBackgroundProperty = DependencyProperty.RegisterAttached(
        "HeaderBackground",
        typeof(Brush),
        typeof(HeaderedControlHelper),
        new UIPropertyMetadata(Panel.BackgroundProperty.DefaultMetadata.DefaultValue));

    /// <summary>Gets the value of the HeaderBackground attached property.</summary>
    public static Brush GetHeaderBackground(UIElement element)
        => (Brush)element.GetValue(HeaderBackgroundProperty);

    /// <summary>Sets the value of the HeaderBackground attached property.</summary>
    public static void SetHeaderBackground(
        UIElement element,
        Brush value)
        => element.SetValue(HeaderBackgroundProperty, value);

    /// <summary>Identifies the HeaderHorizontalContentAlignment attached property.</summary>
    public static readonly DependencyProperty HeaderHorizontalContentAlignmentProperty = DependencyProperty.RegisterAttached(
        "HeaderHorizontalContentAlignment",
        typeof(HorizontalAlignment),
        typeof(HeaderedControlHelper),
        new FrameworkPropertyMetadata(HorizontalAlignment.Stretch));

    /// <summary>Gets the value of the HeaderHorizontalContentAlignment attached property.</summary>
    public static HorizontalAlignment GetHeaderHorizontalContentAlignment(
        UIElement element)
        => (HorizontalAlignment)element
            .GetValue(HeaderHorizontalContentAlignmentProperty);

    /// <summary>Sets the value of the HeaderHorizontalContentAlignment attached property.</summary>
    public static void SetHeaderHorizontalContentAlignment(
        UIElement element,
        HorizontalAlignment value)
        => element.SetValue(HeaderHorizontalContentAlignmentProperty, value);

    /// <summary>Identifies the HeaderVerticalContentAlignment attached property.</summary>
    public static readonly DependencyProperty HeaderVerticalContentAlignmentProperty = DependencyProperty.RegisterAttached(
        "HeaderVerticalContentAlignment",
        typeof(VerticalAlignment),
        typeof(HeaderedControlHelper),
        new FrameworkPropertyMetadata(VerticalAlignment.Stretch));

    /// <summary>Gets the value of the HeaderVerticalContentAlignment attached property.</summary>
    public static VerticalAlignment GetHeaderVerticalContentAlignment(
        UIElement element)
        => (VerticalAlignment)element
            .GetValue(HeaderVerticalContentAlignmentProperty);

    /// <summary>Sets the value of the HeaderVerticalContentAlignment attached property.</summary>
    public static void SetHeaderVerticalContentAlignment(
        UIElement element,
        VerticalAlignment value)
        => element.SetValue(HeaderVerticalContentAlignmentProperty, value);

    /// <summary>Identifies the HeaderMargin attached property.</summary>
    public static readonly DependencyProperty HeaderMarginProperty = DependencyProperty.RegisterAttached(
        "HeaderMargin",
        typeof(Thickness),
        typeof(HeaderedControlHelper),
        new UIPropertyMetadata(new Thickness(0)));

    /// <summary>Gets the value of the HeaderMargin attached property.</summary>
    public static Thickness GetHeaderMargin(UIElement element)
        => (Thickness)element.GetValue(HeaderMarginProperty);

    /// <summary>Sets the value of the HeaderMargin attached property.</summary>
    public static void SetHeaderMargin(
        UIElement element,
        Thickness value)
        => element.SetValue(HeaderMarginProperty, value);

    /// <summary>Identifies the HeaderFontFamily attached property.</summary>
    public static readonly DependencyProperty HeaderFontFamilyProperty = DependencyProperty.RegisterAttached(
        "HeaderFontFamily",
        typeof(FontFamily),
        typeof(HeaderedControlHelper),
        new FrameworkPropertyMetadata(
            SystemFonts.MessageFontFamily,
            FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>Gets the value of the HeaderFontFamily attached property.</summary>
    public static FontFamily GetHeaderFontFamily(UIElement element)
        => (FontFamily)element.GetValue(HeaderFontFamilyProperty);

    /// <summary>Sets the value of the HeaderFontFamily attached property.</summary>
    public static void SetHeaderFontFamily(
        UIElement element,
        FontFamily value)
        => element.SetValue(HeaderFontFamilyProperty, value);

    /// <summary>Identifies the HeaderFontSize attached property.</summary>
    public static readonly DependencyProperty HeaderFontSizeProperty = DependencyProperty.RegisterAttached(
        "HeaderFontSize",
        typeof(double),
        typeof(HeaderedControlHelper),
        new FrameworkPropertyMetadata(
            SystemFonts.MessageFontSize,
            FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>Gets the value of the HeaderFontSize attached property.</summary>
    public static double GetHeaderFontSize(UIElement element)
        => (double)element.GetValue(HeaderFontSizeProperty);

    /// <summary>Sets the value of the HeaderFontSize attached property.</summary>
    public static void SetHeaderFontSize(
        UIElement element,
        double value)
        => element.SetValue(HeaderFontSizeProperty, value);

    /// <summary>Identifies the HeaderFontStretch attached property.</summary>
    public static readonly DependencyProperty HeaderFontStretchProperty = DependencyProperty.RegisterAttached(
        "HeaderFontStretch",
        typeof(FontStretch),
        typeof(HeaderedControlHelper),
        new FrameworkPropertyMetadata(
            TextElement.FontStretchProperty.DefaultMetadata.DefaultValue,
            FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>Gets the value of the HeaderFontStretch attached property.</summary>
    public static FontStretch GetHeaderFontStretch(UIElement element)
        => (FontStretch)element.GetValue(HeaderFontStretchProperty);

    /// <summary>Sets the value of the HeaderFontStretch attached property.</summary>
    public static void SetHeaderFontStretch(
        UIElement element,
        FontStretch value)
        => element.SetValue(HeaderFontStretchProperty, value);

    /// <summary>Identifies the HeaderFontWeight attached property.</summary>
    public static readonly DependencyProperty HeaderFontWeightProperty = DependencyProperty.RegisterAttached(
        "HeaderFontWeight",
        typeof(FontWeight),
        typeof(HeaderedControlHelper),
        new FrameworkPropertyMetadata(
            SystemFonts.MessageFontWeight,
            FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>Gets the value of the HeaderFontWeight attached property.</summary>
    public static FontWeight GetHeaderFontWeight(UIElement element)
        => (FontWeight)element.GetValue(HeaderFontWeightProperty);

    /// <summary>Sets the value of the HeaderFontWeight attached property.</summary>
    public static void SetHeaderFontWeight(
        UIElement element,
        FontWeight value)
        => element.SetValue(HeaderFontWeightProperty, value);
}
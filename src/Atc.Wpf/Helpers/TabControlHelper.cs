namespace Atc.Wpf.Helpers;

[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static class TabControlHelper
{
    public static readonly DependencyProperty UnderlinedProperty = DependencyProperty.RegisterAttached(
        "Underlined",
        typeof(UnderlinedType),
        typeof(TabControlHelper),
        new PropertyMetadata(UnderlinedType.None));

    public static UnderlinedType GetUnderlined(UIElement element)
        => (UnderlinedType)element.GetValue(UnderlinedProperty);

    public static void SetUnderlined(
        UIElement element,
        UnderlinedType value)
        => element.SetValue(UnderlinedProperty, value);

    public static readonly DependencyProperty UnderlineBrushProperty = DependencyProperty.RegisterAttached(
        "UnderlineBrush",
        typeof(Brush),
        typeof(TabControlHelper),
        new FrameworkPropertyMetadata(
            defaultValue: null,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush? GetUnderlineBrush(UIElement element)
        => (Brush?)element.GetValue(UnderlineBrushProperty);

    public static void SetUnderlineBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(UnderlineBrushProperty, value);

    public static readonly DependencyProperty UnderlineSelectedBrushProperty = DependencyProperty.RegisterAttached(
        "UnderlineSelectedBrush",
        typeof(Brush),
        typeof(TabControlHelper),
        new FrameworkPropertyMetadata(
            defaultValue: null,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush? GetUnderlineSelectedBrush(UIElement element)
        => (Brush?)element.GetValue(UnderlineSelectedBrushProperty);

    public static void SetUnderlineSelectedBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(UnderlineSelectedBrushProperty, value);

    public static readonly DependencyProperty UnderlineMouseOverBrushProperty = DependencyProperty.RegisterAttached(
        "UnderlineMouseOverBrush",
        typeof(Brush),
        typeof(TabControlHelper),
        new FrameworkPropertyMetadata(
            defaultValue: null,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush? GetUnderlineMouseOverBrush(UIElement element)
        => (Brush?)element.GetValue(UnderlineMouseOverBrushProperty);

    public static void SetUnderlineMouseOverBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(UnderlineMouseOverBrushProperty, value);

    public static readonly DependencyProperty UnderlineMouseOverSelectedBrushProperty = DependencyProperty.RegisterAttached(
        "UnderlineMouseOverSelectedBrush",
        typeof(Brush),
        typeof(TabControlHelper),
        new FrameworkPropertyMetadata(
            defaultValue: null,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush? GetUnderlineMouseOverSelectedBrush(UIElement element)
        => (Brush?)element.GetValue(UnderlineMouseOverSelectedBrushProperty);

    public static void SetUnderlineMouseOverSelectedBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(UnderlineMouseOverSelectedBrushProperty, value);

    public static readonly DependencyProperty UnderlineMarginProperty = DependencyProperty.RegisterAttached(
        "UnderlineMargin",
        typeof(Thickness),
        typeof(TabControlHelper),
        new UIPropertyMetadata(new Thickness(0)));

    public static Thickness GetUnderlineMargin(UIElement element)
        => (Thickness)element.GetValue(UnderlineMarginProperty);

    public static void SetUnderlineMargin(
        UIElement element,
        Thickness value)
        => element.SetValue(UnderlineMarginProperty, value);

    public static readonly DependencyProperty UnderlinePlacementProperty = DependencyProperty.RegisterAttached(
        "UnderlinePlacement",
        typeof(Dock?),
        typeof(TabControlHelper),
        new PropertyMetadata(propertyChangedCallback: null));

    public static Dock? GetUnderlinePlacement(UIElement element)
        => (Dock?)element.GetValue(UnderlinePlacementProperty);

    public static void SetUnderlinePlacement(
        UIElement element,
        Dock? value)
        => element.SetValue(UnderlinePlacementProperty, value);
}
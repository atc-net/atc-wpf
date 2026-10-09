namespace Atc.Wpf.Helpers;

/// <summary>Provides attached properties for a <see cref="TabControl"/>, such as the tab item underline and its brushes.</summary>
[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static class TabControlHelper
{
    /// <summary>Identifies the Underlined attached property.</summary>
    public static readonly DependencyProperty UnderlinedProperty = DependencyProperty.RegisterAttached(
        "Underlined",
        typeof(UnderlinedType),
        typeof(TabControlHelper),
        new PropertyMetadata(UnderlinedType.None));

    /// <summary>Gets the value of the Underlined attached property.</summary>
    public static UnderlinedType GetUnderlined(UIElement element)
        => (UnderlinedType)element.GetValue(UnderlinedProperty);

    /// <summary>Sets the value of the Underlined attached property.</summary>
    public static void SetUnderlined(
        UIElement element,
        UnderlinedType value)
        => element.SetValue(UnderlinedProperty, value);

    /// <summary>Identifies the UnderlineBrush attached property.</summary>
    public static readonly DependencyProperty UnderlineBrushProperty = DependencyProperty.RegisterAttached(
        "UnderlineBrush",
        typeof(Brush),
        typeof(TabControlHelper),
        new FrameworkPropertyMetadata(
            defaultValue: null,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>Gets the value of the UnderlineBrush attached property.</summary>
    public static Brush? GetUnderlineBrush(UIElement element)
        => (Brush?)element.GetValue(UnderlineBrushProperty);

    /// <summary>Sets the value of the UnderlineBrush attached property.</summary>
    public static void SetUnderlineBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(UnderlineBrushProperty, value);

    /// <summary>Identifies the UnderlineSelectedBrush attached property.</summary>
    public static readonly DependencyProperty UnderlineSelectedBrushProperty = DependencyProperty.RegisterAttached(
        "UnderlineSelectedBrush",
        typeof(Brush),
        typeof(TabControlHelper),
        new FrameworkPropertyMetadata(
            defaultValue: null,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>Gets the value of the UnderlineSelectedBrush attached property.</summary>
    public static Brush? GetUnderlineSelectedBrush(UIElement element)
        => (Brush?)element.GetValue(UnderlineSelectedBrushProperty);

    /// <summary>Sets the value of the UnderlineSelectedBrush attached property.</summary>
    public static void SetUnderlineSelectedBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(UnderlineSelectedBrushProperty, value);

    /// <summary>Identifies the UnderlineMouseOverBrush attached property.</summary>
    public static readonly DependencyProperty UnderlineMouseOverBrushProperty = DependencyProperty.RegisterAttached(
        "UnderlineMouseOverBrush",
        typeof(Brush),
        typeof(TabControlHelper),
        new FrameworkPropertyMetadata(
            defaultValue: null,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>Gets the value of the UnderlineMouseOverBrush attached property.</summary>
    public static Brush? GetUnderlineMouseOverBrush(UIElement element)
        => (Brush?)element.GetValue(UnderlineMouseOverBrushProperty);

    /// <summary>Sets the value of the UnderlineMouseOverBrush attached property.</summary>
    public static void SetUnderlineMouseOverBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(UnderlineMouseOverBrushProperty, value);

    /// <summary>Identifies the UnderlineMouseOverSelectedBrush attached property.</summary>
    public static readonly DependencyProperty UnderlineMouseOverSelectedBrushProperty = DependencyProperty.RegisterAttached(
        "UnderlineMouseOverSelectedBrush",
        typeof(Brush),
        typeof(TabControlHelper),
        new FrameworkPropertyMetadata(
            defaultValue: null,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>Gets the value of the UnderlineMouseOverSelectedBrush attached property.</summary>
    public static Brush? GetUnderlineMouseOverSelectedBrush(UIElement element)
        => (Brush?)element.GetValue(UnderlineMouseOverSelectedBrushProperty);

    /// <summary>Sets the value of the UnderlineMouseOverSelectedBrush attached property.</summary>
    public static void SetUnderlineMouseOverSelectedBrush(
        UIElement element,
        Brush? value)
        => element.SetValue(UnderlineMouseOverSelectedBrushProperty, value);

    /// <summary>Identifies the UnderlineMargin attached property.</summary>
    public static readonly DependencyProperty UnderlineMarginProperty = DependencyProperty.RegisterAttached(
        "UnderlineMargin",
        typeof(Thickness),
        typeof(TabControlHelper),
        new UIPropertyMetadata(new Thickness(0)));

    /// <summary>Gets the value of the UnderlineMargin attached property.</summary>
    public static Thickness GetUnderlineMargin(UIElement element)
        => (Thickness)element.GetValue(UnderlineMarginProperty);

    /// <summary>Sets the value of the UnderlineMargin attached property.</summary>
    public static void SetUnderlineMargin(
        UIElement element,
        Thickness value)
        => element.SetValue(UnderlineMarginProperty, value);

    /// <summary>Identifies the UnderlinePlacement attached property.</summary>
    public static readonly DependencyProperty UnderlinePlacementProperty = DependencyProperty.RegisterAttached(
        "UnderlinePlacement",
        typeof(Dock?),
        typeof(TabControlHelper),
        new PropertyMetadata(propertyChangedCallback: null));

    /// <summary>Gets the value of the UnderlinePlacement attached property.</summary>
    public static Dock? GetUnderlinePlacement(UIElement element)
        => (Dock?)element.GetValue(UnderlinePlacementProperty);

    /// <summary>Sets the value of the UnderlinePlacement attached property.</summary>
    public static void SetUnderlinePlacement(
        UIElement element,
        Dock? value)
        => element.SetValue(UnderlinePlacementProperty, value);
}
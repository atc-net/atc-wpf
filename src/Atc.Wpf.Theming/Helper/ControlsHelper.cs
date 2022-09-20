namespace Atc.Wpf.Theming.Helper;

/// <summary>
/// A helper class that provides various controls.
/// </summary>
public static class ControlsHelper
{
    public static readonly DependencyProperty DisabledVisualElementVisibilityProperty = DependencyProperty.RegisterAttached(
        "DisabledVisualElementVisibility",
        typeof(Visibility),
        typeof(ControlsHelper),
        new FrameworkPropertyMetadata(
            Visibility.Visible,
            FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static Visibility GetDisabledVisualElementVisibility(
        UIElement element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (Visibility)element.GetValue(DisabledVisualElementVisibilityProperty);
    }

    public static void SetDisabledVisualElementVisibility(
        UIElement element,
        Visibility value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValue(DisabledVisualElementVisibilityProperty, value);
    }

    public static readonly DependencyProperty ContentCharacterCasingProperty =
        DependencyProperty.RegisterAttached(
            "ContentCharacterCasing",
            typeof(CharacterCasing),
            typeof(ControlsHelper),
            new FrameworkPropertyMetadata(CharacterCasing.Normal, FrameworkPropertyMetadataOptions.AffectsMeasure),
            value => (CharacterCasing)value >= CharacterCasing.Normal && (CharacterCasing)value <= CharacterCasing.Upper);

    public static CharacterCasing GetContentCharacterCasing(
        UIElement element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (CharacterCasing)element.GetValue(ContentCharacterCasingProperty);
    }

    public static void SetContentCharacterCasing(
        UIElement element,
        CharacterCasing value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValue(ContentCharacterCasingProperty, value);
    }

    public static readonly DependencyProperty RecognizesAccessKeyProperty = DependencyProperty.RegisterAttached(
        "RecognizesAccessKey",
        typeof(bool),
        typeof(ControlsHelper),
        new FrameworkPropertyMetadata(BooleanBoxes.TrueBox));

    public static bool GetRecognizesAccessKey(
        UIElement element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (bool)element.GetValue(RecognizesAccessKeyProperty);
    }

    public static void SetRecognizesAccessKey(
        UIElement element,
        bool value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValue(RecognizesAccessKeyProperty, BooleanBoxes.Box(value));
    }

    public static readonly DependencyProperty FocusBorderBrushProperty = DependencyProperty.RegisterAttached(
        "FocusBorderBrush",
        typeof(Brush),
        typeof(ControlsHelper),
        new FrameworkPropertyMetadata(
            Brushes.Transparent,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush GetFocusBorderBrush(
        DependencyObject d)
    {
        ArgumentNullException.ThrowIfNull(d);

        return (Brush)d.GetValue(FocusBorderBrushProperty);
    }

    public static void SetFocusBorderBrush(
        DependencyObject d,
        Brush value)
    {
        ArgumentNullException.ThrowIfNull(d);

        d.SetValue(FocusBorderBrushProperty, value);
    }

    public static readonly DependencyProperty FocusBorderThicknessProperty = DependencyProperty.RegisterAttached(
        "FocusBorderThickness",
        typeof(Thickness),
        typeof(ControlsHelper),
        new FrameworkPropertyMetadata(
            default(Thickness),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Thickness GetFocusBorderThickness(
        DependencyObject d)
    {
        ArgumentNullException.ThrowIfNull(d);

        return (Thickness)d.GetValue(FocusBorderThicknessProperty);
    }

    public static void SetFocusBorderThickness(
        DependencyObject d,
        Thickness value)
    {
        ArgumentNullException.ThrowIfNull(d);

        d.SetValue(FocusBorderThicknessProperty, value);
    }

    public static readonly DependencyProperty MouseOverBorderBrushProperty = DependencyProperty.RegisterAttached(
        "MouseOverBorderBrush",
        typeof(Brush),
        typeof(ControlsHelper),
        new FrameworkPropertyMetadata(
            Brushes.Transparent,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush GetMouseOverBorderBrush(
        DependencyObject d)
    {
        ArgumentNullException.ThrowIfNull(d);

        return (Brush)d.GetValue(MouseOverBorderBrushProperty);
    }

    public static void SetMouseOverBorderBrush(
        DependencyObject d,
        Brush value)
    {
        ArgumentNullException.ThrowIfNull(d);

        d.SetValue(MouseOverBorderBrushProperty, value);
    }

    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.RegisterAttached(
        "CornerRadius",
        typeof(CornerRadius),
        typeof(ControlsHelper),
        new FrameworkPropertyMetadata(
            default(CornerRadius),
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

    public static CornerRadius GetCornerRadius(
        UIElement element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (CornerRadius)element.GetValue(CornerRadiusProperty);
    }

    public static void SetCornerRadius(
        UIElement element,
        CornerRadius value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValue(CornerRadiusProperty, value);
    }

    public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.RegisterAttached(
        "IsReadOnly",
        typeof(bool),
        typeof(ControlsHelper),
        new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

    public static bool GetIsReadOnly(
        UIElement element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (bool)element.GetValue(IsReadOnlyProperty);
    }

    public static void SetIsReadOnly(
        UIElement element,
        bool value)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValue(IsReadOnlyProperty, BooleanBoxes.Box(value));
    }
}
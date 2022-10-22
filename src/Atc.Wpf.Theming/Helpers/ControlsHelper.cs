namespace Atc.Wpf.Theming.Helpers;

/// <summary>
/// A helper class that provides various controls.
/// </summary>
[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
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
        => (Visibility)element.GetValue(DisabledVisualElementVisibilityProperty);

    public static void SetDisabledVisualElementVisibility(
        UIElement element,
        Visibility value)
        => element.SetValue(DisabledVisualElementVisibilityProperty, value);

    public static readonly DependencyProperty ContentCharacterCasingProperty = DependencyProperty.RegisterAttached(
        "ContentCharacterCasing",
        typeof(CharacterCasing),
        typeof(ControlsHelper),
        new FrameworkPropertyMetadata(
            CharacterCasing.Normal,
            FrameworkPropertyMetadataOptions.AffectsMeasure),
        value => (CharacterCasing)value >= CharacterCasing.Normal && (CharacterCasing)value <= CharacterCasing.Upper);

    public static CharacterCasing GetContentCharacterCasing(
        UIElement element)
        => (CharacterCasing)element.GetValue(ContentCharacterCasingProperty);

    public static void SetContentCharacterCasing(
        UIElement element,
        CharacterCasing value)
        => element.SetValue(ContentCharacterCasingProperty, value);

    public static readonly DependencyProperty RecognizesAccessKeyProperty = DependencyProperty.RegisterAttached(
        "RecognizesAccessKey",
        typeof(bool),
        typeof(ControlsHelper),
        new FrameworkPropertyMetadata(BooleanBoxes.TrueBox));

    public static bool GetRecognizesAccessKey(
        UIElement element)
        => (bool)element.GetValue(RecognizesAccessKeyProperty);

    public static void SetRecognizesAccessKey(
        UIElement element,
        bool value)
        => element.SetValue(RecognizesAccessKeyProperty, BooleanBoxes.Box(value));

    public static readonly DependencyProperty FocusBorderBrushProperty = DependencyProperty.RegisterAttached(
        "FocusBorderBrush",
        typeof(Brush),
        typeof(ControlsHelper),
        new FrameworkPropertyMetadata(
            Brushes.Transparent,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush GetFocusBorderBrush(
        DependencyObject d)
        => (Brush)d.GetValue(FocusBorderBrushProperty);

    public static void SetFocusBorderBrush(
        DependencyObject d,
        Brush value)
        => d.SetValue(FocusBorderBrushProperty, value);

    public static readonly DependencyProperty FocusBorderThicknessProperty = DependencyProperty.RegisterAttached(
        "FocusBorderThickness",
        typeof(Thickness),
        typeof(ControlsHelper),
        new FrameworkPropertyMetadata(
            default(Thickness),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Thickness GetFocusBorderThickness(
        DependencyObject d)
        => (Thickness)d.GetValue(FocusBorderThicknessProperty);

    public static void SetFocusBorderThickness(
        DependencyObject d,
        Thickness value)
        => d.SetValue(FocusBorderThicknessProperty, value);

    public static readonly DependencyProperty MouseOverBackgroundBrushProperty = DependencyProperty.RegisterAttached(
        "MouseOverBackgroundBrush",
        typeof(Brush),
        typeof(ControlsHelper),
        new FrameworkPropertyMetadata(
            Brushes.Transparent,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush GetMouseOverBackgroundBrush(
        DependencyObject d)
        => (Brush)d.GetValue(MouseOverBackgroundBrushProperty);

    public static void SetMouseOverBackgroundBrush(
        DependencyObject d,
        Brush value)
        => d.SetValue(MouseOverBackgroundBrushProperty, value);

    public static readonly DependencyProperty MouseOverForegroundBrushProperty = DependencyProperty.RegisterAttached(
        "MouseOverForegroundBrush",
        typeof(Brush),
        typeof(ControlsHelper),
        new FrameworkPropertyMetadata(
            Brushes.Transparent,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush GetMouseOverForegroundBrush(
        DependencyObject d)
        => (Brush)d.GetValue(MouseOverForegroundBrushProperty);

    public static void SetMouseOverForegroundBrush(
        DependencyObject d,
        Brush value)
        => d.SetValue(MouseOverForegroundBrushProperty, value);

    public static readonly DependencyProperty MouseOverBorderBrushProperty = DependencyProperty.RegisterAttached(
        "MouseOverBorderBrush",
        typeof(Brush),
        typeof(ControlsHelper),
        new FrameworkPropertyMetadata(
            Brushes.Transparent,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush GetMouseOverBorderBrush(
        DependencyObject d)
        => (Brush)d.GetValue(MouseOverBorderBrushProperty);

    public static void SetMouseOverBorderBrush(
        DependencyObject d,
        Brush value)
        => d.SetValue(MouseOverBorderBrushProperty, value);

    public static readonly DependencyProperty PressedBackgroundBrushProperty = DependencyProperty.RegisterAttached(
        "PressedBackgroundBrush",
        typeof(Brush),
        typeof(ControlsHelper),
        new FrameworkPropertyMetadata(
            Brushes.Transparent,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush GetPressedBackgroundBrush(
        DependencyObject d)
        => (Brush)d.GetValue(PressedBackgroundBrushProperty);

    public static void SetPressedBackgroundBrush(
        DependencyObject d,
        Brush value)
        => d.SetValue(PressedBackgroundBrushProperty, value);

    public static readonly DependencyProperty PressedBorderBrushProperty = DependencyProperty.RegisterAttached(
        "PressedBorderBrush",
        typeof(Brush),
        typeof(ControlsHelper),
        new FrameworkPropertyMetadata(
            Brushes.Transparent,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    public static Brush GetPressedBorderBrush(
        DependencyObject d)
        => (Brush)d.GetValue(PressedBorderBrushProperty);

    public static void SetPressedBorderBrush(
        DependencyObject d,
        Brush value)
        => d.SetValue(PressedBorderBrushProperty, value);

    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.RegisterAttached(
        "CornerRadius",
        typeof(CornerRadius),
        typeof(ControlsHelper),
        new FrameworkPropertyMetadata(
            default(CornerRadius),
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

    public static CornerRadius GetCornerRadius(
        UIElement element)
        => (CornerRadius)element.GetValue(CornerRadiusProperty);

    public static void SetCornerRadius(
        UIElement element,
        CornerRadius value)
        => element.SetValue(CornerRadiusProperty, value);

    public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.RegisterAttached(
        "IsReadOnly",
        typeof(bool),
        typeof(ControlsHelper),
        new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

    public static bool GetIsReadOnly(
        UIElement element)
        => (bool)element.GetValue(IsReadOnlyProperty);

    public static void SetIsReadOnly(
        UIElement element,
        bool value)
        => element.SetValue(IsReadOnlyProperty, BooleanBoxes.Box(value));
}
namespace Atc.Wpf.Helpers;

/// <summary>
/// A helper class that provides various controls.
/// </summary>
[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static class ControlsHelper
{
    /// <summary>Identifies the DisabledVisualElementVisibility attached property.</summary>
    public static readonly DependencyProperty DisabledVisualElementVisibilityProperty = DependencyProperty.RegisterAttached(
        "DisabledVisualElementVisibility",
        typeof(Visibility),
        typeof(ControlsHelper),
        new FrameworkPropertyMetadata(
            Visibility.Visible,
            FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure));

    /// <summary>Gets the value of the DisabledVisualElementVisibility attached property.</summary>
    public static Visibility GetDisabledVisualElementVisibility(
        UIElement element)
        => (Visibility)element.GetValue(DisabledVisualElementVisibilityProperty);

    /// <summary>Sets the value of the DisabledVisualElementVisibility attached property.</summary>
    public static void SetDisabledVisualElementVisibility(
        UIElement element,
        Visibility value)
        => element.SetValue(
            DisabledVisualElementVisibilityProperty,
            value);

    /// <summary>Identifies the ContentCharacterCasing attached property.</summary>
    public static readonly DependencyProperty ContentCharacterCasingProperty = DependencyProperty.RegisterAttached(
        "ContentCharacterCasing",
        typeof(CharacterCasing),
        typeof(ControlsHelper),
        new FrameworkPropertyMetadata(
            CharacterCasing.Normal,
            FrameworkPropertyMetadataOptions.AffectsMeasure),
        value => (CharacterCasing)value >= CharacterCasing.Normal && (CharacterCasing)value <= CharacterCasing.Upper);

    /// <summary>Gets the value of the ContentCharacterCasing attached property.</summary>
    public static CharacterCasing GetContentCharacterCasing(UIElement element)
        => (CharacterCasing)element.GetValue(ContentCharacterCasingProperty);

    /// <summary>Sets the value of the ContentCharacterCasing attached property.</summary>
    public static void SetContentCharacterCasing(
        UIElement element,
        CharacterCasing value)
        => element.SetValue(
            ContentCharacterCasingProperty,
            value);

    /// <summary>Identifies the RecognizesAccessKey attached property.</summary>
    public static readonly DependencyProperty RecognizesAccessKeyProperty = DependencyProperty.RegisterAttached(
        "RecognizesAccessKey",
        typeof(bool),
        typeof(ControlsHelper),
        new FrameworkPropertyMetadata(BooleanBoxes.TrueBox));

    /// <summary>Gets the value of the RecognizesAccessKey attached property.</summary>
    public static bool GetRecognizesAccessKey(UIElement element)
        => (bool)element.GetValue(RecognizesAccessKeyProperty);

    /// <summary>Sets the value of the RecognizesAccessKey attached property.</summary>
    public static void SetRecognizesAccessKey(
        UIElement element,
        bool value)
        => element.SetValue(
            RecognizesAccessKeyProperty,
            BooleanBoxes.Box(value));

    /// <summary>Identifies the FocusBorderBrush attached property.</summary>
    public static readonly DependencyProperty FocusBorderBrushProperty = DependencyProperty.RegisterAttached(
        "FocusBorderBrush",
        typeof(Brush),
        typeof(ControlsHelper),
        new FrameworkPropertyMetadata(
            Brushes.Transparent,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>Gets the value of the FocusBorderBrush attached property.</summary>
    public static Brush GetFocusBorderBrush(DependencyObject d)
        => (Brush)d.GetValue(FocusBorderBrushProperty);

    /// <summary>Sets the value of the FocusBorderBrush attached property.</summary>
    public static void SetFocusBorderBrush(
        DependencyObject d,
        Brush value)
        => d.SetValue(
            FocusBorderBrushProperty,
            value);

    /// <summary>Identifies the FocusBorderThickness attached property.</summary>
    public static readonly DependencyProperty FocusBorderThicknessProperty = DependencyProperty.RegisterAttached(
        "FocusBorderThickness",
        typeof(Thickness),
        typeof(ControlsHelper),
        new FrameworkPropertyMetadata(
            default(Thickness),
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>Gets the value of the FocusBorderThickness attached property.</summary>
    public static Thickness GetFocusBorderThickness(DependencyObject d)
        => (Thickness)d.GetValue(FocusBorderThicknessProperty);

    /// <summary>Sets the value of the FocusBorderThickness attached property.</summary>
    public static void SetFocusBorderThickness(
        DependencyObject d,
        Thickness value)
        => d.SetValue(
            FocusBorderThicknessProperty,
            value);

    /// <summary>Identifies the MouseOverBackgroundBrush attached property.</summary>
    public static readonly DependencyProperty MouseOverBackgroundBrushProperty = DependencyProperty.RegisterAttached(
        "MouseOverBackgroundBrush",
        typeof(Brush),
        typeof(ControlsHelper),
        new FrameworkPropertyMetadata(
            Brushes.Transparent,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>Gets the value of the MouseOverBackgroundBrush attached property.</summary>
    public static Brush GetMouseOverBackgroundBrush(DependencyObject d)
        => (Brush)d.GetValue(MouseOverBackgroundBrushProperty);

    /// <summary>Sets the value of the MouseOverBackgroundBrush attached property.</summary>
    public static void SetMouseOverBackgroundBrush(
        DependencyObject d,
        Brush value)
        => d.SetValue(
            MouseOverBackgroundBrushProperty,
            value);

    /// <summary>Identifies the MouseOverForegroundBrush attached property.</summary>
    public static readonly DependencyProperty MouseOverForegroundBrushProperty = DependencyProperty.RegisterAttached(
        "MouseOverForegroundBrush",
        typeof(Brush),
        typeof(ControlsHelper),
        new FrameworkPropertyMetadata(
            Brushes.Transparent,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>Gets the value of the MouseOverForegroundBrush attached property.</summary>
    public static Brush GetMouseOverForegroundBrush(DependencyObject d)
        => (Brush)d.GetValue(MouseOverForegroundBrushProperty);

    /// <summary>Sets the value of the MouseOverForegroundBrush attached property.</summary>
    public static void SetMouseOverForegroundBrush(
        DependencyObject d,
        Brush value)
        => d.SetValue(
            MouseOverForegroundBrushProperty,
            value);

    /// <summary>Identifies the MouseOverBorderBrush attached property.</summary>
    public static readonly DependencyProperty MouseOverBorderBrushProperty = DependencyProperty.RegisterAttached(
        "MouseOverBorderBrush",
        typeof(Brush),
        typeof(ControlsHelper),
        new FrameworkPropertyMetadata(
            Brushes.Transparent,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>Gets the value of the MouseOverBorderBrush attached property.</summary>
    public static Brush GetMouseOverBorderBrush(DependencyObject d)
        => (Brush)d.GetValue(MouseOverBorderBrushProperty);

    /// <summary>Sets the value of the MouseOverBorderBrush attached property.</summary>
    public static void SetMouseOverBorderBrush(
        DependencyObject d,
        Brush value)
        => d.SetValue(
            MouseOverBorderBrushProperty,
            value);

    /// <summary>Identifies the PressedBackgroundBrush attached property.</summary>
    public static readonly DependencyProperty PressedBackgroundBrushProperty = DependencyProperty.RegisterAttached(
        "PressedBackgroundBrush",
        typeof(Brush),
        typeof(ControlsHelper),
        new FrameworkPropertyMetadata(
            Brushes.Transparent,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>Gets the value of the PressedBackgroundBrush attached property.</summary>
    public static Brush GetPressedBackgroundBrush(DependencyObject d)
        => (Brush)d.GetValue(PressedBackgroundBrushProperty);

    /// <summary>Sets the value of the PressedBackgroundBrush attached property.</summary>
    public static void SetPressedBackgroundBrush(
        DependencyObject d,
        Brush value)
        => d.SetValue(
            PressedBackgroundBrushProperty,
            value);

    /// <summary>Identifies the PressedBorderBrush attached property.</summary>
    public static readonly DependencyProperty PressedBorderBrushProperty = DependencyProperty.RegisterAttached(
        "PressedBorderBrush",
        typeof(Brush),
        typeof(ControlsHelper),
        new FrameworkPropertyMetadata(
            Brushes.Transparent,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>Gets the value of the PressedBorderBrush attached property.</summary>
    public static Brush GetPressedBorderBrush(DependencyObject d)
        => (Brush)d.GetValue(PressedBorderBrushProperty);

    /// <summary>Sets the value of the PressedBorderBrush attached property.</summary>
    public static void SetPressedBorderBrush(
        DependencyObject d,
        Brush value)
        => d.SetValue(
            PressedBorderBrushProperty,
            value);

    /// <summary>Identifies the CornerRadius attached property.</summary>
    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.RegisterAttached(
        "CornerRadius",
        typeof(CornerRadius),
        typeof(ControlsHelper),
        new FrameworkPropertyMetadata(
            default(CornerRadius),
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Gets the value of the CornerRadius attached property.</summary>
    public static CornerRadius GetCornerRadius(UIElement element)
        => (CornerRadius)element.GetValue(CornerRadiusProperty);

    /// <summary>Sets the value of the CornerRadius attached property.</summary>
    public static void SetCornerRadius(
        UIElement element,
        CornerRadius value)
        => element.SetValue(
            CornerRadiusProperty,
            value);

    /// <summary>Identifies the IsReadOnly attached property.</summary>
    public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.RegisterAttached(
        "IsReadOnly",
        typeof(bool),
        typeof(ControlsHelper),
        new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>Gets the value of the IsReadOnly attached property.</summary>
    public static bool GetIsReadOnly(UIElement element)
        => (bool)element.GetValue(IsReadOnlyProperty);

    /// <summary>Sets the value of the IsReadOnly attached property.</summary>
    public static void SetIsReadOnly(
        UIElement element,
        bool value)
        => element.SetValue(
            IsReadOnlyProperty,
            BooleanBoxes.Box(value));
}
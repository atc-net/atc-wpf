namespace Atc.Wpf.Helpers;

/// <summary>Provides attached properties for styling a <see cref="RadioButton"/>, such as sizes and brushes for the outer ellipse and check glyph in each visual state.</summary>
[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static class RadioButtonHelper
{
    /// <summary>Identifies the RadioSize attached property.</summary>
    public static readonly DependencyProperty RadioSizeProperty = DependencyProperty.RegisterAttached(
        "RadioSize",
        typeof(double),
        typeof(RadioButtonHelper),
        new FrameworkPropertyMetadata(18.0));

    /// <summary>Gets the value of the RadioSize attached property.</summary>
    public static double GetRadioSize(UIElement element)
        => (double)element.GetValue(RadioSizeProperty);

    /// <summary>Sets the value of the RadioSize attached property.</summary>
    public static void SetRadioSize(
        UIElement element,
        double value)
        => element.SetValue(RadioSizeProperty, value);

    /// <summary>Identifies the RadioCheckSize attached property.</summary>
    public static readonly DependencyProperty RadioCheckSizeProperty = DependencyProperty.RegisterAttached(
        "RadioCheckSize",
        typeof(double),
        typeof(RadioButtonHelper),
        new FrameworkPropertyMetadata(10.0));

    /// <summary>Gets the value of the RadioCheckSize attached property.</summary>
    public static double GetRadioCheckSize(UIElement element)
        => (double)element.GetValue(RadioCheckSizeProperty);

    /// <summary>Sets the value of the RadioCheckSize attached property.</summary>
    public static void SetRadioCheckSize(
        UIElement element,
        double value)
        => element.SetValue(RadioCheckSizeProperty, value);

    /// <summary>Identifies the RadioStrokeThickness attached property.</summary>
    public static readonly DependencyProperty RadioStrokeThicknessProperty = DependencyProperty.RegisterAttached(
        "RadioStrokeThickness",
        typeof(double),
        typeof(RadioButtonHelper),
        new FrameworkPropertyMetadata(1.0));

    /// <summary>Gets the value of the RadioStrokeThickness attached property.</summary>
    public static double GetRadioStrokeThickness(UIElement element)
        => (double)element.GetValue(RadioStrokeThicknessProperty);

    /// <summary>Sets the value of the RadioStrokeThickness attached property.</summary>
    public static void SetRadioStrokeThickness(
        UIElement element,
        double value)
        => element.SetValue(RadioStrokeThicknessProperty, value);

    /// <summary>Identifies the ForegroundPointerOver attached property.</summary>
    public static readonly DependencyProperty ForegroundPointerOverProperty = DependencyProperty.RegisterAttached(
        "ForegroundPointerOver",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the ForegroundPointerOver attached property.</summary>
    public static Brush? GetForegroundPointerOver(UIElement element)
        => (Brush?)element.GetValue(ForegroundPointerOverProperty);

    /// <summary>Sets the value of the ForegroundPointerOver attached property.</summary>
    public static void SetForegroundPointerOver(
        UIElement element,
        Brush? value)
        => element.SetValue(ForegroundPointerOverProperty, value);

    /// <summary>Identifies the ForegroundPressed attached property.</summary>
    public static readonly DependencyProperty ForegroundPressedProperty = DependencyProperty.RegisterAttached(
        "ForegroundPressed",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the ForegroundPressed attached property.</summary>
    public static Brush? GetForegroundPressed(UIElement element)
        => (Brush?)element.GetValue(ForegroundPressedProperty);

    /// <summary>Sets the value of the ForegroundPressed attached property.</summary>
    public static void SetForegroundPressed(
        UIElement element,
        Brush? value)
        => element.SetValue(ForegroundPressedProperty, value);

    /// <summary>Identifies the ForegroundDisabled attached property.</summary>
    public static readonly DependencyProperty ForegroundDisabledProperty = DependencyProperty.RegisterAttached(
        "ForegroundDisabled",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the ForegroundDisabled attached property.</summary>
    public static Brush? GetForegroundDisabled(UIElement element)
        => (Brush?)element.GetValue(ForegroundDisabledProperty);

    /// <summary>Sets the value of the ForegroundDisabled attached property.</summary>
    public static void SetForegroundDisabled(
        UIElement element,
        Brush? value)
        => element.SetValue(ForegroundDisabledProperty, value);

    /// <summary>Identifies the BackgroundPointerOver attached property.</summary>
    public static readonly DependencyProperty BackgroundPointerOverProperty = DependencyProperty.RegisterAttached(
        "BackgroundPointerOver",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the BackgroundPointerOver attached property.</summary>
    public static Brush? GetBackgroundPointerOver(UIElement element)
        => (Brush?)element.GetValue(BackgroundPointerOverProperty);

    /// <summary>Sets the value of the BackgroundPointerOver attached property.</summary>
    public static void SetBackgroundPointerOver(
        UIElement element,
        Brush? value)
        => element.SetValue(BackgroundPointerOverProperty, value);

    /// <summary>Identifies the BackgroundPressed attached property.</summary>
    public static readonly DependencyProperty BackgroundPressedProperty = DependencyProperty.RegisterAttached(
        "BackgroundPressed",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the BackgroundPressed attached property.</summary>
    public static Brush? GetBackgroundPressed(UIElement element)
        => (Brush?)element.GetValue(BackgroundPressedProperty);

    /// <summary>Sets the value of the BackgroundPressed attached property.</summary>
    public static void SetBackgroundPressed(
        UIElement element,
        Brush? value)
        => element.SetValue(BackgroundPressedProperty, value);

    /// <summary>Identifies the BackgroundDisabled attached property.</summary>
    public static readonly DependencyProperty BackgroundDisabledProperty = DependencyProperty.RegisterAttached(
        "BackgroundDisabled",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the BackgroundDisabled attached property.</summary>
    public static Brush? GetBackgroundDisabled(UIElement element)
        => (Brush?)element.GetValue(BackgroundDisabledProperty);

    /// <summary>Sets the value of the BackgroundDisabled attached property.</summary>
    public static void SetBackgroundDisabled(
        UIElement element,
        Brush? value)
        => element.SetValue(BackgroundDisabledProperty, value);

    /// <summary>Identifies the BorderBrushPointerOver attached property.</summary>
    public static readonly DependencyProperty BorderBrushPointerOverProperty = DependencyProperty.RegisterAttached(
        "BorderBrushPointerOver",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the BorderBrushPointerOver attached property.</summary>
    public static Brush? GetBorderBrushPointerOver(UIElement element)
        => (Brush?)element.GetValue(BorderBrushPointerOverProperty);

    /// <summary>Sets the value of the BorderBrushPointerOver attached property.</summary>
    public static void SetBorderBrushPointerOver(
        UIElement element,
        Brush? value)
        => element.SetValue(BorderBrushPointerOverProperty, value);

    /// <summary>Identifies the BorderBrushPressed attached property.</summary>
    public static readonly DependencyProperty BorderBrushPressedProperty = DependencyProperty.RegisterAttached(
        "BorderBrushPressed",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the BorderBrushPressed attached property.</summary>
    public static Brush? GetBorderBrushPressed(UIElement element)
        => (Brush?)element.GetValue(BorderBrushPressedProperty);

    /// <summary>Sets the value of the BorderBrushPressed attached property.</summary>
    public static void SetBorderBrushPressed(
        UIElement element,
        Brush? value)
        => element.SetValue(BorderBrushPressedProperty, value);

    /// <summary>Identifies the BorderBrushDisabled attached property.</summary>
    public static readonly DependencyProperty BorderBrushDisabledProperty = DependencyProperty.RegisterAttached(
        "BorderBrushDisabled",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the BorderBrushDisabled attached property.</summary>
    public static Brush? GetBorderBrushDisabled(UIElement element)
        => (Brush?)element.GetValue(BorderBrushDisabledProperty);

    /// <summary>Sets the value of the BorderBrushDisabled attached property.</summary>
    public static void SetBorderBrushDisabled(
        UIElement element,
        Brush? value)
        => element.SetValue(BorderBrushDisabledProperty, value);

    /// <summary>Identifies the OuterEllipseFill attached property.</summary>
    public static readonly DependencyProperty OuterEllipseFillProperty = DependencyProperty.RegisterAttached(
        "OuterEllipseFill",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the OuterEllipseFill attached property.</summary>
    public static Brush? GetOuterEllipseFill(UIElement element)
        => (Brush?)element.GetValue(OuterEllipseFillProperty);

    /// <summary>Sets the value of the OuterEllipseFill attached property.</summary>
    public static void SetOuterEllipseFill(
        UIElement element,
        Brush? value)
        => element.SetValue(OuterEllipseFillProperty, value);

    /// <summary>Identifies the OuterEllipseFillPointerOver attached property.</summary>
    public static readonly DependencyProperty OuterEllipseFillPointerOverProperty = DependencyProperty.RegisterAttached(
        "OuterEllipseFillPointerOver",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the OuterEllipseFillPointerOver attached property.</summary>
    public static Brush? GetOuterEllipseFillPointerOver(UIElement element)
        => (Brush?)element.GetValue(OuterEllipseFillPointerOverProperty);

    /// <summary>Sets the value of the OuterEllipseFillPointerOver attached property.</summary>
    public static void SetOuterEllipseFillPointerOver(
        UIElement element,
        Brush? value)
        => element.SetValue(OuterEllipseFillPointerOverProperty, value);

    /// <summary>Identifies the OuterEllipseFillPressed attached property.</summary>
    public static readonly DependencyProperty OuterEllipseFillPressedProperty = DependencyProperty.RegisterAttached(
        "OuterEllipseFillPressed",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the OuterEllipseFillPressed attached property.</summary>
    public static Brush? GetOuterEllipseFillPressed(UIElement element)
        => (Brush?)element.GetValue(OuterEllipseFillPressedProperty);

    /// <summary>Sets the value of the OuterEllipseFillPressed attached property.</summary>
    public static void SetOuterEllipseFillPressed(
        UIElement element,
        Brush? value)
        => element.SetValue(OuterEllipseFillPressedProperty, value);

    /// <summary>Identifies the OuterEllipseFillDisabled attached property.</summary>
    public static readonly DependencyProperty OuterEllipseFillDisabledProperty = DependencyProperty.RegisterAttached(
        "OuterEllipseFillDisabled",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the OuterEllipseFillDisabled attached property.</summary>
    public static Brush? GetOuterEllipseFillDisabled(UIElement element)
        => (Brush?)element.GetValue(OuterEllipseFillDisabledProperty);

    /// <summary>Sets the value of the OuterEllipseFillDisabled attached property.</summary>
    public static void SetOuterEllipseFillDisabled(
        UIElement element,
        Brush? value)
        => element.SetValue(OuterEllipseFillDisabledProperty, value);

    /// <summary>Identifies the OuterEllipseStroke attached property.</summary>
    public static readonly DependencyProperty OuterEllipseStrokeProperty = DependencyProperty.RegisterAttached(
        "OuterEllipseStroke",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the OuterEllipseStroke attached property.</summary>
    public static Brush? GetOuterEllipseStroke(UIElement element)
        => (Brush?)element.GetValue(OuterEllipseStrokeProperty);

    /// <summary>Sets the value of the OuterEllipseStroke attached property.</summary>
    public static void SetOuterEllipseStroke(
        UIElement element,
        Brush? value)
        => element.SetValue(OuterEllipseStrokeProperty, value);

    /// <summary>Identifies the OuterEllipseStrokePointerOver attached property.</summary>
    public static readonly DependencyProperty OuterEllipseStrokePointerOverProperty = DependencyProperty.RegisterAttached(
        "OuterEllipseStrokePointerOver",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the OuterEllipseStrokePointerOver attached property.</summary>
    public static Brush? GetOuterEllipseStrokePointerOver(UIElement element)
        => (Brush?)element.GetValue(OuterEllipseStrokePointerOverProperty);

    /// <summary>Sets the value of the OuterEllipseStrokePointerOver attached property.</summary>
    public static void SetOuterEllipseStrokePointerOver(
        UIElement element,
        Brush? value)
        => element.SetValue(OuterEllipseStrokePointerOverProperty, value);

    /// <summary>Identifies the OuterEllipseStrokePressed attached property.</summary>
    public static readonly DependencyProperty OuterEllipseStrokePressedProperty = DependencyProperty.RegisterAttached(
        "OuterEllipseStrokePressed",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the OuterEllipseStrokePressed attached property.</summary>
    public static Brush? GetOuterEllipseStrokePressed(UIElement element)
        => (Brush?)element.GetValue(OuterEllipseStrokePressedProperty);

    /// <summary>Sets the value of the OuterEllipseStrokePressed attached property.</summary>
    public static void SetOuterEllipseStrokePressed(
        UIElement element,
        Brush? value)
        => element.SetValue(OuterEllipseStrokePressedProperty, value);

    /// <summary>Identifies the OuterEllipseStrokeDisabled attached property.</summary>
    public static readonly DependencyProperty OuterEllipseStrokeDisabledProperty = DependencyProperty.RegisterAttached(
        "OuterEllipseStrokeDisabled",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the OuterEllipseStrokeDisabled attached property.</summary>
    public static Brush? GetOuterEllipseStrokeDisabled(UIElement element)
        => (Brush?)element.GetValue(OuterEllipseStrokeDisabledProperty);

    /// <summary>Sets the value of the OuterEllipseStrokeDisabled attached property.</summary>
    public static void SetOuterEllipseStrokeDisabled(
        UIElement element,
        Brush? value)
        => element.SetValue(OuterEllipseStrokeDisabledProperty, value);

    /// <summary>Identifies the OuterEllipseCheckedFill attached property.</summary>
    public static readonly DependencyProperty OuterEllipseCheckedFillProperty = DependencyProperty.RegisterAttached(
        "OuterEllipseCheckedFill",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the OuterEllipseCheckedFill attached property.</summary>
    public static Brush? GetOuterEllipseCheckedFill(UIElement element)
        => (Brush?)element.GetValue(OuterEllipseCheckedFillProperty);

    /// <summary>Sets the value of the OuterEllipseCheckedFill attached property.</summary>
    public static void SetOuterEllipseCheckedFill(
        UIElement element,
        Brush? value)
        => element.SetValue(OuterEllipseCheckedFillProperty, value);

    /// <summary>Identifies the OuterEllipseCheckedFillPointerOver attached property.</summary>
    public static readonly DependencyProperty OuterEllipseCheckedFillPointerOverProperty = DependencyProperty.RegisterAttached(
        "OuterEllipseCheckedFillPointerOver",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the OuterEllipseCheckedFillPointerOver attached property.</summary>
    public static Brush? GetOuterEllipseCheckedFillPointerOver(
        UIElement element)
        => (Brush?)element.GetValue(OuterEllipseCheckedFillPointerOverProperty);

    /// <summary>Sets the value of the OuterEllipseCheckedFillPointerOver attached property.</summary>
    public static void SetOuterEllipseCheckedFillPointerOver(
        UIElement element,
        Brush? value)
        => element.SetValue(OuterEllipseCheckedFillPointerOverProperty, value);

    /// <summary>Identifies the OuterEllipseCheckedFillPressed attached property.</summary>
    public static readonly DependencyProperty OuterEllipseCheckedFillPressedProperty = DependencyProperty.RegisterAttached(
        "OuterEllipseCheckedFillPressed",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the OuterEllipseCheckedFillPressed attached property.</summary>
    public static Brush? GetOuterEllipseCheckedFillPressed(UIElement element)
        => (Brush?)element.GetValue(OuterEllipseCheckedFillPressedProperty);

    /// <summary>Sets the value of the OuterEllipseCheckedFillPressed attached property.</summary>
    public static void SetOuterEllipseCheckedFillPressed(
        UIElement element,
        Brush? value)
        => element.SetValue(OuterEllipseCheckedFillPressedProperty, value);

    /// <summary>Identifies the OuterEllipseCheckedFillDisabled attached property.</summary>
    public static readonly DependencyProperty OuterEllipseCheckedFillDisabledProperty = DependencyProperty.RegisterAttached(
        "OuterEllipseCheckedFillDisabled",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the OuterEllipseCheckedFillDisabled attached property.</summary>
    public static Brush? GetOuterEllipseCheckedFillDisabled(UIElement element)
        => (Brush?)element.GetValue(OuterEllipseCheckedFillDisabledProperty);

    /// <summary>Sets the value of the OuterEllipseCheckedFillDisabled attached property.</summary>
    public static void SetOuterEllipseCheckedFillDisabled(
        UIElement element,
        Brush? value)
        => element.SetValue(OuterEllipseCheckedFillDisabledProperty, value);

    /// <summary>Identifies the OuterEllipseCheckedStroke attached property.</summary>
    public static readonly DependencyProperty OuterEllipseCheckedStrokeProperty = DependencyProperty.RegisterAttached(
        "OuterEllipseCheckedStroke",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the OuterEllipseCheckedStroke attached property.</summary>
    public static Brush? GetOuterEllipseCheckedStroke(UIElement element)
        => (Brush?)element.GetValue(OuterEllipseCheckedStrokeProperty);

    /// <summary>Sets the value of the OuterEllipseCheckedStroke attached property.</summary>
    public static void SetOuterEllipseCheckedStroke(
        UIElement element,
        Brush? value)
        => element.SetValue(OuterEllipseCheckedStrokeProperty, value);

    /// <summary>Identifies the OuterEllipseCheckedStrokePointerOver attached property.</summary>
    public static readonly DependencyProperty OuterEllipseCheckedStrokePointerOverProperty = DependencyProperty.RegisterAttached(
        "OuterEllipseCheckedStrokePointerOver",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the OuterEllipseCheckedStrokePointerOver attached property.</summary>
    public static Brush? GetOuterEllipseCheckedStrokePointerOver(
        UIElement element)
        => (Brush?)element
            .GetValue(OuterEllipseCheckedStrokePointerOverProperty);

    /// <summary>Sets the value of the OuterEllipseCheckedStrokePointerOver attached property.</summary>
    public static void SetOuterEllipseCheckedStrokePointerOver(
        UIElement element,
        Brush? value)
        => element
            .SetValue(OuterEllipseCheckedStrokePointerOverProperty, value);

    /// <summary>Identifies the OuterEllipseCheckedStrokePressed attached property.</summary>
    public static readonly DependencyProperty OuterEllipseCheckedStrokePressedProperty = DependencyProperty.RegisterAttached(
        "OuterEllipseCheckedStrokePressed",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the OuterEllipseCheckedStrokePressed attached property.</summary>
    public static Brush? GetOuterEllipseCheckedStrokePressed(UIElement element)
        => (Brush?)element.GetValue(OuterEllipseCheckedStrokePressedProperty);

    /// <summary>Sets the value of the OuterEllipseCheckedStrokePressed attached property.</summary>
    public static void SetOuterEllipseCheckedStrokePressed(
        UIElement element,
        Brush? value)
        => element.SetValue(OuterEllipseCheckedStrokePressedProperty, value);

    /// <summary>Identifies the OuterEllipseCheckedStrokeDisabled attached property.</summary>
    public static readonly DependencyProperty OuterEllipseCheckedStrokeDisabledProperty = DependencyProperty.RegisterAttached(
        "OuterEllipseCheckedStrokeDisabled",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the OuterEllipseCheckedStrokeDisabled attached property.</summary>
    public static Brush? GetOuterEllipseCheckedStrokeDisabled(UIElement element)
        => (Brush?)element.GetValue(OuterEllipseCheckedStrokeDisabledProperty);

    /// <summary>Sets the value of the OuterEllipseCheckedStrokeDisabled attached property.</summary>
    public static void SetOuterEllipseCheckedStrokeDisabled(
        UIElement element,
        Brush? value)
        => element.SetValue(OuterEllipseCheckedStrokeDisabledProperty, value);

    /// <summary>Identifies the CheckGlyphFill attached property.</summary>
    public static readonly DependencyProperty CheckGlyphFillProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphFill",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckGlyphFill attached property.</summary>
    public static Brush? GetCheckGlyphFill(UIElement element)
        => (Brush?)element.GetValue(CheckGlyphFillProperty);

    /// <summary>Sets the value of the CheckGlyphFill attached property.</summary>
    public static void SetCheckGlyphFill(
        UIElement element,
        Brush? value)
        => element.SetValue(CheckGlyphFillProperty, value);

    /// <summary>Identifies the CheckGlyphFillPointerOver attached property.</summary>
    public static readonly DependencyProperty CheckGlyphFillPointerOverProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphFillPointerOver",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckGlyphFillPointerOver attached property.</summary>
    public static Brush? GetCheckGlyphFillPointerOver(UIElement element)
        => (Brush?)element.GetValue(CheckGlyphFillPointerOverProperty);

    /// <summary>Sets the value of the CheckGlyphFillPointerOver attached property.</summary>
    public static void SetCheckGlyphFillPointerOver(
        UIElement element,
        Brush? value)
        => element.SetValue(CheckGlyphFillPointerOverProperty, value);

    /// <summary>Identifies the CheckGlyphFillPressed attached property.</summary>
    public static readonly DependencyProperty CheckGlyphFillPressedProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphFillPressed",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckGlyphFillPressed attached property.</summary>
    public static Brush? GetCheckGlyphFillPressed(UIElement element)
        => (Brush?)element.GetValue(CheckGlyphFillPressedProperty);

    /// <summary>Sets the value of the CheckGlyphFillPressed attached property.</summary>
    public static void SetCheckGlyphFillPressed(
        UIElement element,
        Brush? value)
        => element.SetValue(CheckGlyphFillPressedProperty, value);

    /// <summary>Identifies the CheckGlyphFillDisabled attached property.</summary>
    public static readonly DependencyProperty CheckGlyphFillDisabledProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphFillDisabled",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckGlyphFillDisabled attached property.</summary>
    public static Brush? GetCheckGlyphFillDisabled(UIElement element)
        => (Brush?)element.GetValue(CheckGlyphFillDisabledProperty);

    /// <summary>Sets the value of the CheckGlyphFillDisabled attached property.</summary>
    public static void SetCheckGlyphFillDisabled(
        UIElement element,
        Brush? value)
        => element.SetValue(CheckGlyphFillDisabledProperty, value);

    /// <summary>Identifies the CheckGlyphStroke attached property.</summary>
    public static readonly DependencyProperty CheckGlyphStrokeProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphStroke",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckGlyphStroke attached property.</summary>
    public static Brush? GetCheckGlyphStroke(UIElement element)
        => (Brush?)element.GetValue(CheckGlyphStrokeProperty);

    /// <summary>Sets the value of the CheckGlyphStroke attached property.</summary>
    public static void SetCheckGlyphStroke(
        UIElement element,
        Brush? value)
        => element.SetValue(CheckGlyphStrokeProperty, value);

    /// <summary>Identifies the CheckGlyphStrokePointerOver attached property.</summary>
    public static readonly DependencyProperty CheckGlyphStrokePointerOverProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphStrokePointerOver",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckGlyphStrokePointerOver attached property.</summary>
    public static Brush? GetCheckGlyphStrokePointerOver(UIElement element)
        => (Brush?)element.GetValue(CheckGlyphStrokePointerOverProperty);

    /// <summary>Sets the value of the CheckGlyphStrokePointerOver attached property.</summary>
    public static void SetCheckGlyphStrokePointerOver(
        UIElement element,
        Brush? value)
        => element.SetValue(CheckGlyphStrokePointerOverProperty, value);

    /// <summary>Identifies the CheckGlyphStrokePressed attached property.</summary>
    public static readonly DependencyProperty CheckGlyphStrokePressedProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphStrokePressed",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckGlyphStrokePressed attached property.</summary>
    public static Brush? GetCheckGlyphStrokePressed(UIElement element)
        => (Brush?)element.GetValue(CheckGlyphStrokePressedProperty);

    /// <summary>Sets the value of the CheckGlyphStrokePressed attached property.</summary>
    public static void SetCheckGlyphStrokePressed(
        UIElement element,
        Brush? value)
        => element.SetValue(CheckGlyphStrokePressedProperty, value);

    /// <summary>Identifies the CheckGlyphStrokeDisabled attached property.</summary>
    public static readonly DependencyProperty CheckGlyphStrokeDisabledProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphStrokeDisabled",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckGlyphStrokeDisabled attached property.</summary>
    public static Brush? GetCheckGlyphStrokeDisabled(UIElement element)
        => (Brush?)element.GetValue(CheckGlyphStrokeDisabledProperty);

    /// <summary>Sets the value of the CheckGlyphStrokeDisabled attached property.</summary>
    public static void SetCheckGlyphStrokeDisabled(
        UIElement element,
        Brush? value)
        => element.SetValue(CheckGlyphStrokeDisabledProperty, value);
}
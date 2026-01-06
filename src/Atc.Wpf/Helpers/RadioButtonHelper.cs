namespace Atc.Wpf.Helpers;

[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static class RadioButtonHelper
{
    public static readonly DependencyProperty RadioSizeProperty = DependencyProperty.RegisterAttached(
        "RadioSize",
        typeof(double),
        typeof(RadioButtonHelper),
        new FrameworkPropertyMetadata(18.0));

    public static double GetRadioSize(UIElement element)
        => (double)element.GetValue(RadioSizeProperty);

    public static void SetRadioSize(
        UIElement element,
        double value)
        => element.SetValue(RadioSizeProperty, value);

    public static readonly DependencyProperty RadioCheckSizeProperty = DependencyProperty.RegisterAttached(
        "RadioCheckSize",
        typeof(double),
        typeof(RadioButtonHelper),
        new FrameworkPropertyMetadata(10.0));

    public static double GetRadioCheckSize(UIElement element)
        => (double)element.GetValue(RadioCheckSizeProperty);

    public static void SetRadioCheckSize(
        UIElement element,
        double value)
        => element.SetValue(RadioCheckSizeProperty, value);

    public static readonly DependencyProperty RadioStrokeThicknessProperty = DependencyProperty.RegisterAttached(
        "RadioStrokeThickness",
        typeof(double),
        typeof(RadioButtonHelper),
        new FrameworkPropertyMetadata(1.0));

    public static double GetRadioStrokeThickness(UIElement element)
        => (double)element.GetValue(RadioStrokeThicknessProperty);

    public static void SetRadioStrokeThickness(
        UIElement element,
        double value)
        => element.SetValue(RadioStrokeThicknessProperty, value);

    public static readonly DependencyProperty ForegroundPointerOverProperty = DependencyProperty.RegisterAttached(
        "ForegroundPointerOver",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    public static Brush? GetForegroundPointerOver(UIElement element)
        => (Brush?)element.GetValue(ForegroundPointerOverProperty);

    public static void SetForegroundPointerOver(
        UIElement element,
        Brush? value)
        => element.SetValue(ForegroundPointerOverProperty, value);

    public static readonly DependencyProperty ForegroundPressedProperty = DependencyProperty.RegisterAttached(
        "ForegroundPressed",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    public static Brush? GetForegroundPressed(UIElement element)
        => (Brush?)element.GetValue(ForegroundPressedProperty);

    public static void SetForegroundPressed(
        UIElement element,
        Brush? value)
        => element.SetValue(ForegroundPressedProperty, value);

    public static readonly DependencyProperty ForegroundDisabledProperty = DependencyProperty.RegisterAttached(
        "ForegroundDisabled",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    public static Brush? GetForegroundDisabled(UIElement element)
        => (Brush?)element.GetValue(ForegroundDisabledProperty);

    public static void SetForegroundDisabled(
        UIElement element,
        Brush? value)
        => element.SetValue(ForegroundDisabledProperty, value);

    public static readonly DependencyProperty BackgroundPointerOverProperty = DependencyProperty.RegisterAttached(
        "BackgroundPointerOver",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    public static Brush? GetBackgroundPointerOver(UIElement element)
        => (Brush?)element.GetValue(BackgroundPointerOverProperty);

    public static void SetBackgroundPointerOver(
        UIElement element,
        Brush? value)
        => element.SetValue(BackgroundPointerOverProperty, value);

    public static readonly DependencyProperty BackgroundPressedProperty = DependencyProperty.RegisterAttached(
        "BackgroundPressed",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    public static Brush? GetBackgroundPressed(UIElement element)
        => (Brush?)element.GetValue(BackgroundPressedProperty);

    public static void SetBackgroundPressed(
        UIElement element,
        Brush? value)
        => element.SetValue(BackgroundPressedProperty, value);

    public static readonly DependencyProperty BackgroundDisabledProperty = DependencyProperty.RegisterAttached(
        "BackgroundDisabled",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    public static Brush? GetBackgroundDisabled(UIElement element)
        => (Brush?)element.GetValue(BackgroundDisabledProperty);

    public static void SetBackgroundDisabled(
        UIElement element,
        Brush? value)
        => element.SetValue(BackgroundDisabledProperty, value);

    public static readonly DependencyProperty BorderBrushPointerOverProperty = DependencyProperty.RegisterAttached(
        "BorderBrushPointerOver",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    public static Brush? GetBorderBrushPointerOver(UIElement element)
        => (Brush?)element.GetValue(BorderBrushPointerOverProperty);

    public static void SetBorderBrushPointerOver(
        UIElement element,
        Brush? value)
        => element.SetValue(BorderBrushPointerOverProperty, value);

    public static readonly DependencyProperty BorderBrushPressedProperty = DependencyProperty.RegisterAttached(
        "BorderBrushPressed",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    public static Brush? GetBorderBrushPressed(UIElement element)
        => (Brush?)element.GetValue(BorderBrushPressedProperty);

    public static void SetBorderBrushPressed(
        UIElement element,
        Brush? value)
        => element.SetValue(BorderBrushPressedProperty, value);

    public static readonly DependencyProperty BorderBrushDisabledProperty = DependencyProperty.RegisterAttached(
        "BorderBrushDisabled",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    public static Brush? GetBorderBrushDisabled(UIElement element)
        => (Brush?)element.GetValue(BorderBrushDisabledProperty);

    public static void SetBorderBrushDisabled(
        UIElement element,
        Brush? value)
        => element.SetValue(BorderBrushDisabledProperty, value);

    public static readonly DependencyProperty OuterEllipseFillProperty = DependencyProperty.RegisterAttached(
        "OuterEllipseFill",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    public static Brush? GetOuterEllipseFill(UIElement element)
        => (Brush?)element.GetValue(OuterEllipseFillProperty);

    public static void SetOuterEllipseFill(
        UIElement element,
        Brush? value)
        => element.SetValue(OuterEllipseFillProperty, value);

    public static readonly DependencyProperty OuterEllipseFillPointerOverProperty = DependencyProperty.RegisterAttached(
        "OuterEllipseFillPointerOver",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    public static Brush? GetOuterEllipseFillPointerOver(UIElement element)
        => (Brush?)element.GetValue(OuterEllipseFillPointerOverProperty);

    public static void SetOuterEllipseFillPointerOver(
        UIElement element,
        Brush? value)
        => element.SetValue(OuterEllipseFillPointerOverProperty, value);

    public static readonly DependencyProperty OuterEllipseFillPressedProperty = DependencyProperty.RegisterAttached(
        "OuterEllipseFillPressed",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    public static Brush? GetOuterEllipseFillPressed(UIElement element)
        => (Brush?)element.GetValue(OuterEllipseFillPressedProperty);

    public static void SetOuterEllipseFillPressed(
        UIElement element,
        Brush? value)
        => element.SetValue(OuterEllipseFillPressedProperty, value);

    public static readonly DependencyProperty OuterEllipseFillDisabledProperty = DependencyProperty.RegisterAttached(
        "OuterEllipseFillDisabled",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    public static Brush? GetOuterEllipseFillDisabled(UIElement element)
        => (Brush?)element.GetValue(OuterEllipseFillDisabledProperty);

    public static void SetOuterEllipseFillDisabled(
        UIElement element,
        Brush? value)
        => element.SetValue(OuterEllipseFillDisabledProperty, value);

    public static readonly DependencyProperty OuterEllipseStrokeProperty = DependencyProperty.RegisterAttached(
        "OuterEllipseStroke",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    public static Brush? GetOuterEllipseStroke(UIElement element)
        => (Brush?)element.GetValue(OuterEllipseStrokeProperty);

    public static void SetOuterEllipseStroke(
        UIElement element,
        Brush? value)
        => element.SetValue(OuterEllipseStrokeProperty, value);

    public static readonly DependencyProperty OuterEllipseStrokePointerOverProperty = DependencyProperty.RegisterAttached(
        "OuterEllipseStrokePointerOver",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    public static Brush? GetOuterEllipseStrokePointerOver(UIElement element)
        => (Brush?)element.GetValue(OuterEllipseStrokePointerOverProperty);

    public static void SetOuterEllipseStrokePointerOver(
        UIElement element,
        Brush? value)
        => element.SetValue(OuterEllipseStrokePointerOverProperty, value);

    public static readonly DependencyProperty OuterEllipseStrokePressedProperty = DependencyProperty.RegisterAttached(
        "OuterEllipseStrokePressed",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    public static Brush? GetOuterEllipseStrokePressed(UIElement element)
        => (Brush?)element.GetValue(OuterEllipseStrokePressedProperty);

    public static void SetOuterEllipseStrokePressed(
        UIElement element,
        Brush? value)
        => element.SetValue(OuterEllipseStrokePressedProperty, value);

    public static readonly DependencyProperty OuterEllipseStrokeDisabledProperty = DependencyProperty.RegisterAttached(
        "OuterEllipseStrokeDisabled",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    public static Brush? GetOuterEllipseStrokeDisabled(UIElement element)
        => (Brush?)element.GetValue(OuterEllipseStrokeDisabledProperty);

    public static void SetOuterEllipseStrokeDisabled(
        UIElement element,
        Brush? value)
        => element.SetValue(OuterEllipseStrokeDisabledProperty, value);

    public static readonly DependencyProperty OuterEllipseCheckedFillProperty = DependencyProperty.RegisterAttached(
        "OuterEllipseCheckedFill",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    public static Brush? GetOuterEllipseCheckedFill(UIElement element)
        => (Brush?)element.GetValue(OuterEllipseCheckedFillProperty);

    public static void SetOuterEllipseCheckedFill(
        UIElement element,
        Brush? value)
        => element.SetValue(OuterEllipseCheckedFillProperty, value);

    public static readonly DependencyProperty OuterEllipseCheckedFillPointerOverProperty = DependencyProperty.RegisterAttached(
        "OuterEllipseCheckedFillPointerOver",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    public static Brush? GetOuterEllipseCheckedFillPointerOver(
        UIElement element)
        => (Brush?)element.GetValue(OuterEllipseCheckedFillPointerOverProperty);

    public static void SetOuterEllipseCheckedFillPointerOver(
        UIElement element,
        Brush? value)
        => element.SetValue(OuterEllipseCheckedFillPointerOverProperty, value);

    public static readonly DependencyProperty OuterEllipseCheckedFillPressedProperty = DependencyProperty.RegisterAttached(
        "OuterEllipseCheckedFillPressed",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    public static Brush? GetOuterEllipseCheckedFillPressed(UIElement element)
        => (Brush?)element.GetValue(OuterEllipseCheckedFillPressedProperty);

    public static void SetOuterEllipseCheckedFillPressed(
        UIElement element,
        Brush? value)
        => element.SetValue(OuterEllipseCheckedFillPressedProperty, value);

    public static readonly DependencyProperty OuterEllipseCheckedFillDisabledProperty = DependencyProperty.RegisterAttached(
        "OuterEllipseCheckedFillDisabled",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    public static Brush? GetOuterEllipseCheckedFillDisabled(UIElement element)
        => (Brush?)element.GetValue(OuterEllipseCheckedFillDisabledProperty);

    public static void SetOuterEllipseCheckedFillDisabled(
        UIElement element,
        Brush? value)
        => element.SetValue(OuterEllipseCheckedFillDisabledProperty, value);

    public static readonly DependencyProperty OuterEllipseCheckedStrokeProperty = DependencyProperty.RegisterAttached(
        "OuterEllipseCheckedStroke",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    public static Brush? GetOuterEllipseCheckedStroke(UIElement element)
        => (Brush?)element.GetValue(OuterEllipseCheckedStrokeProperty);

    public static void SetOuterEllipseCheckedStroke(
        UIElement element,
        Brush? value)
        => element.SetValue(OuterEllipseCheckedStrokeProperty, value);

    public static readonly DependencyProperty OuterEllipseCheckedStrokePointerOverProperty = DependencyProperty.RegisterAttached(
        "OuterEllipseCheckedStrokePointerOver",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    public static Brush? GetOuterEllipseCheckedStrokePointerOver(
        UIElement element)
        => (Brush?)element
            .GetValue(OuterEllipseCheckedStrokePointerOverProperty);

    public static void SetOuterEllipseCheckedStrokePointerOver(
        UIElement element,
        Brush? value)
        => element
            .SetValue(OuterEllipseCheckedStrokePointerOverProperty, value);

    public static readonly DependencyProperty OuterEllipseCheckedStrokePressedProperty = DependencyProperty.RegisterAttached(
        "OuterEllipseCheckedStrokePressed",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    public static Brush? GetOuterEllipseCheckedStrokePressed(UIElement element)
        => (Brush?)element.GetValue(OuterEllipseCheckedStrokePressedProperty);

    public static void SetOuterEllipseCheckedStrokePressed(
        UIElement element,
        Brush? value)
        => element.SetValue(OuterEllipseCheckedStrokePressedProperty, value);

    public static readonly DependencyProperty OuterEllipseCheckedStrokeDisabledProperty = DependencyProperty.RegisterAttached(
        "OuterEllipseCheckedStrokeDisabled",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    public static Brush? GetOuterEllipseCheckedStrokeDisabled(UIElement element)
        => (Brush?)element.GetValue(OuterEllipseCheckedStrokeDisabledProperty);

    public static void SetOuterEllipseCheckedStrokeDisabled(
        UIElement element,
        Brush? value)
        => element.SetValue(OuterEllipseCheckedStrokeDisabledProperty, value);

    public static readonly DependencyProperty CheckGlyphFillProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphFill",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    public static Brush? GetCheckGlyphFill(UIElement element)
        => (Brush?)element.GetValue(CheckGlyphFillProperty);

    public static void SetCheckGlyphFill(
        UIElement element,
        Brush? value)
        => element.SetValue(CheckGlyphFillProperty, value);

    public static readonly DependencyProperty CheckGlyphFillPointerOverProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphFillPointerOver",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    public static Brush? GetCheckGlyphFillPointerOver(UIElement element)
        => (Brush?)element.GetValue(CheckGlyphFillPointerOverProperty);

    public static void SetCheckGlyphFillPointerOver(
        UIElement element,
        Brush? value)
        => element.SetValue(CheckGlyphFillPointerOverProperty, value);

    public static readonly DependencyProperty CheckGlyphFillPressedProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphFillPressed",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    public static Brush? GetCheckGlyphFillPressed(UIElement element)
        => (Brush?)element.GetValue(CheckGlyphFillPressedProperty);

    public static void SetCheckGlyphFillPressed(
        UIElement element,
        Brush? value)
        => element.SetValue(CheckGlyphFillPressedProperty, value);

    public static readonly DependencyProperty CheckGlyphFillDisabledProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphFillDisabled",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    public static Brush? GetCheckGlyphFillDisabled(UIElement element)
        => (Brush?)element.GetValue(CheckGlyphFillDisabledProperty);

    public static void SetCheckGlyphFillDisabled(
        UIElement element,
        Brush? value)
        => element.SetValue(CheckGlyphFillDisabledProperty, value);

    public static readonly DependencyProperty CheckGlyphStrokeProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphStroke",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    public static Brush? GetCheckGlyphStroke(UIElement element)
        => (Brush?)element.GetValue(CheckGlyphStrokeProperty);

    public static void SetCheckGlyphStroke(
        UIElement element,
        Brush? value)
        => element.SetValue(CheckGlyphStrokeProperty, value);

    public static readonly DependencyProperty CheckGlyphStrokePointerOverProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphStrokePointerOver",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    public static Brush? GetCheckGlyphStrokePointerOver(UIElement element)
        => (Brush?)element.GetValue(CheckGlyphStrokePointerOverProperty);

    public static void SetCheckGlyphStrokePointerOver(
        UIElement element,
        Brush? value)
        => element.SetValue(CheckGlyphStrokePointerOverProperty, value);

    public static readonly DependencyProperty CheckGlyphStrokePressedProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphStrokePressed",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    public static Brush? GetCheckGlyphStrokePressed(UIElement element)
        => (Brush?)element.GetValue(CheckGlyphStrokePressedProperty);

    public static void SetCheckGlyphStrokePressed(
        UIElement element,
        Brush? value)
        => element.SetValue(CheckGlyphStrokePressedProperty, value);

    public static readonly DependencyProperty CheckGlyphStrokeDisabledProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphStrokeDisabled",
        typeof(Brush),
        typeof(RadioButtonHelper),
        new PropertyMetadata(default(Brush)));

    public static Brush? GetCheckGlyphStrokeDisabled(UIElement element)
        => (Brush?)element.GetValue(CheckGlyphStrokeDisabledProperty);

    public static void SetCheckGlyphStrokeDisabled(
        UIElement element,
        Brush? value)
        => element.SetValue(CheckGlyphStrokeDisabledProperty, value);
}
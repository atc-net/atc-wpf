namespace Atc.Wpf.Helpers;

[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static class CheckBoxHelper
{
    public static readonly DependencyProperty CheckSizeProperty = DependencyProperty.RegisterAttached(
        "CheckSize",
        typeof(double),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(18.0));

    public static double GetCheckSize(
        DependencyObject d)
        => (double)d.GetValue(CheckSizeProperty);

    public static void SetCheckSize(
        DependencyObject d,
        double value)
        => d.SetValue(CheckSizeProperty, value);

    public static readonly DependencyProperty CheckCornerRadiusProperty = DependencyProperty.RegisterAttached(
        "CheckCornerRadius",
        typeof(CornerRadius),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(
            new CornerRadius(0),
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

    public static CornerRadius GetCheckCornerRadius(
        UIElement element)
        => (CornerRadius)element.GetValue(CheckCornerRadiusProperty);

    public static void SetCheckCornerRadius(
        UIElement element,
        CornerRadius value)
        => element.SetValue(CheckCornerRadiusProperty, value);

    public static readonly DependencyProperty CheckStrokeThicknessProperty = DependencyProperty.RegisterAttached(
        "CheckStrokeThickness",
        typeof(double),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(1d));

    public static double GetCheckStrokeThickness(
        DependencyObject d)
        => (double)d.GetValue(CheckStrokeThicknessProperty);

    public static void SetCheckStrokeThickness(
        DependencyObject d,
        double value)
        => d.SetValue(CheckStrokeThicknessProperty, value);

    public static readonly DependencyProperty CheckGlyphUncheckedProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphUnchecked",
        typeof(object),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(propertyChangedCallback: null));

    public static object? GetCheckGlyphUnchecked(
        DependencyObject d) => d.GetValue(CheckGlyphUncheckedProperty);

    public static void SetCheckGlyphUnchecked(
        DependencyObject d,
        object? value)
        => d.SetValue(CheckGlyphUncheckedProperty, value);

    public static readonly DependencyProperty CheckGlyphUncheckedTemplateProperty
        = DependencyProperty.RegisterAttached(
            "CheckGlyphUncheckedTemplate",
            typeof(DataTemplate),
            typeof(CheckBoxHelper),
            new FrameworkPropertyMetadata(default(DataTemplate)));

    public static DataTemplate? GetCheckGlyphUncheckedTemplate(
        DependencyObject d)
        => (DataTemplate?)d.GetValue(CheckGlyphUncheckedTemplateProperty);

    public static void SetCheckGlyphUncheckedTemplate(
        DependencyObject d,
        DataTemplate? value)
        => d.SetValue(CheckGlyphUncheckedTemplateProperty, value);

    public static readonly DependencyProperty ForegroundUncheckedProperty = DependencyProperty.RegisterAttached(
        "ForegroundUnchecked",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetForegroundUnchecked(
        DependencyObject d)
        => (Brush?)d.GetValue(ForegroundUncheckedProperty);

    public static void SetForegroundUnchecked(
        DependencyObject d,
        Brush? value)
        => d.SetValue(ForegroundUncheckedProperty, value);

    public static readonly DependencyProperty BackgroundUncheckedProperty = DependencyProperty.RegisterAttached(
        "BackgroundUnchecked",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetBackgroundUnchecked(
        DependencyObject d)
        => (Brush?)d.GetValue(BackgroundUncheckedProperty);

    public static void SetBackgroundUnchecked(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BackgroundUncheckedProperty, value);

    public static readonly DependencyProperty BorderBrushUncheckedProperty = DependencyProperty.RegisterAttached(
        "BorderBrushUnchecked",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetBorderBrushUnchecked(
        DependencyObject d)
        => (Brush?)d.GetValue(BorderBrushUncheckedProperty);

    public static void SetBorderBrushUnchecked(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BorderBrushUncheckedProperty, value);

    public static readonly DependencyProperty CheckBackgroundFillUncheckedProperty
        = DependencyProperty.RegisterAttached(
            "CheckBackgroundFillUnchecked",
            typeof(Brush),
            typeof(CheckBoxHelper),
            new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetCheckBackgroundFillUnchecked(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundFillUncheckedProperty);

    public static void SetCheckBackgroundFillUnchecked(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundFillUncheckedProperty, value);

    public static readonly DependencyProperty CheckBackgroundStrokeUncheckedProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundStrokeUnchecked",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetCheckBackgroundStrokeUnchecked(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundStrokeUncheckedProperty);

    public static void SetCheckBackgroundStrokeUnchecked(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundStrokeUncheckedProperty, value);

    public static readonly DependencyProperty CheckGlyphForegroundUncheckedProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphForegroundUnchecked",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetCheckGlyphForegroundUnchecked(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckGlyphForegroundUncheckedProperty);

    public static void SetCheckGlyphForegroundUnchecked(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckGlyphForegroundUncheckedProperty, value);

    public static readonly DependencyProperty ForegroundUncheckedMouseOverProperty
        = DependencyProperty.RegisterAttached(
            "ForegroundUncheckedMouseOver",
            typeof(Brush),
            typeof(CheckBoxHelper),
            new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetForegroundUncheckedMouseOver(
        DependencyObject d)
        => (Brush?)d.GetValue(ForegroundUncheckedMouseOverProperty);

    public static void SetForegroundUncheckedMouseOver(
        DependencyObject d,
        Brush? value)
        => d.SetValue(ForegroundUncheckedMouseOverProperty, value);

    public static readonly DependencyProperty BackgroundUncheckedMouseOverProperty = DependencyProperty.RegisterAttached(
        "BackgroundUncheckedMouseOver",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetBackgroundUncheckedMouseOver(
        DependencyObject d) => (Brush?)d.GetValue(BackgroundUncheckedMouseOverProperty);

    public static void SetBackgroundUncheckedMouseOver(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BackgroundUncheckedMouseOverProperty, value);

    public static readonly DependencyProperty BorderBrushUncheckedMouseOverProperty = DependencyProperty.RegisterAttached(
        "BorderBrushUncheckedMouseOver",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetBorderBrushUncheckedMouseOver(
        DependencyObject d)
        => (Brush?)d.GetValue(BorderBrushUncheckedMouseOverProperty);

    public static void SetBorderBrushUncheckedMouseOver(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BorderBrushUncheckedMouseOverProperty, value);

    public static readonly DependencyProperty CheckBackgroundFillUncheckedMouseOverProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundFillUncheckedMouseOver",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetCheckBackgroundFillUncheckedMouseOver(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundFillUncheckedMouseOverProperty);

    public static void SetCheckBackgroundFillUncheckedMouseOver(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundFillUncheckedMouseOverProperty, value);

    public static readonly DependencyProperty CheckBackgroundStrokeUncheckedMouseOverProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundStrokeUncheckedMouseOver",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetCheckBackgroundStrokeUncheckedMouseOver(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundStrokeUncheckedMouseOverProperty);

    public static void SetCheckBackgroundStrokeUncheckedMouseOver(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundStrokeUncheckedMouseOverProperty, value);

    public static readonly DependencyProperty CheckGlyphForegroundUncheckedMouseOverProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphForegroundUncheckedMouseOver",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetCheckGlyphForegroundUncheckedMouseOver(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckGlyphForegroundUncheckedMouseOverProperty);

    public static void SetCheckGlyphForegroundUncheckedMouseOver(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckGlyphForegroundUncheckedMouseOverProperty, value);

    public static readonly DependencyProperty ForegroundUncheckedPressedProperty = DependencyProperty.RegisterAttached(
        "ForegroundUncheckedPressed",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetForegroundUncheckedPressed(
        DependencyObject d)
        => (Brush?)d.GetValue(ForegroundUncheckedPressedProperty);

    public static void SetForegroundUncheckedPressed(
        DependencyObject d,
        Brush? value)
        => d.SetValue(ForegroundUncheckedPressedProperty, value);

    public static readonly DependencyProperty BackgroundUncheckedPressedProperty = DependencyProperty.RegisterAttached(
        "BackgroundUncheckedPressed",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetBackgroundUncheckedPressed(
        DependencyObject d)
        => (Brush?)d.GetValue(BackgroundUncheckedPressedProperty);

    public static void SetBackgroundUncheckedPressed(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BackgroundUncheckedPressedProperty, value);

    public static readonly DependencyProperty BorderBrushUncheckedPressedProperty = DependencyProperty.RegisterAttached(
        "BorderBrushUncheckedPressed",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetBorderBrushUncheckedPressed(DependencyObject d)
        => (Brush?)d.GetValue(BorderBrushUncheckedPressedProperty);

    public static void SetBorderBrushUncheckedPressed(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BorderBrushUncheckedPressedProperty, value);

    public static readonly DependencyProperty CheckBackgroundFillUncheckedPressedProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundFillUncheckedPressed",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetCheckBackgroundFillUncheckedPressed(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundFillUncheckedPressedProperty);

    public static void SetCheckBackgroundFillUncheckedPressed(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundFillUncheckedPressedProperty, value);

    public static readonly DependencyProperty CheckBackgroundStrokeUncheckedPressedProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundStrokeUncheckedPressed",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetCheckBackgroundStrokeUncheckedPressed(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundStrokeUncheckedPressedProperty);

    public static void SetCheckBackgroundStrokeUncheckedPressed(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundStrokeUncheckedPressedProperty, value);

    public static readonly DependencyProperty CheckGlyphForegroundUncheckedPressedProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphForegroundUncheckedPressed",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetCheckGlyphForegroundUncheckedPressed(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckGlyphForegroundUncheckedPressedProperty);

    public static void SetCheckGlyphForegroundUncheckedPressed(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckGlyphForegroundUncheckedPressedProperty, value);

    public static readonly DependencyProperty ForegroundUncheckedDisabledProperty = DependencyProperty.RegisterAttached(
        "ForegroundUncheckedDisabled",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetForegroundUncheckedDisabled(
        DependencyObject d)
        => (Brush?)d.GetValue(ForegroundUncheckedDisabledProperty);

    public static void SetForegroundUncheckedDisabled(
        DependencyObject d,
        Brush? value)
        => d.SetValue(ForegroundUncheckedDisabledProperty, value);

    public static readonly DependencyProperty BackgroundUncheckedDisabledProperty = DependencyProperty.RegisterAttached(
        "BackgroundUncheckedDisabled",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetBackgroundUncheckedDisabled(
        DependencyObject d)
        => (Brush?)d.GetValue(BackgroundUncheckedDisabledProperty);

    public static void SetBackgroundUncheckedDisabled(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BackgroundUncheckedDisabledProperty, value);

    public static readonly DependencyProperty BorderBrushUncheckedDisabledProperty = DependencyProperty.RegisterAttached(
        "BorderBrushUncheckedDisabled",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetBorderBrushUncheckedDisabled(
        DependencyObject d)
        => (Brush?)d.GetValue(BorderBrushUncheckedDisabledProperty);

    public static void SetBorderBrushUncheckedDisabled(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BorderBrushUncheckedDisabledProperty, value);

    public static readonly DependencyProperty CheckBackgroundFillUncheckedDisabledProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundFillUncheckedDisabled",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetCheckBackgroundFillUncheckedDisabled(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundFillUncheckedDisabledProperty);

    public static void SetCheckBackgroundFillUncheckedDisabled(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundFillUncheckedDisabledProperty, value);

    public static readonly DependencyProperty CheckBackgroundStrokeUncheckedDisabledProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundStrokeUncheckedDisabled",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetCheckBackgroundStrokeUncheckedDisabled(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundStrokeUncheckedDisabledProperty);

    public static void SetCheckBackgroundStrokeUncheckedDisabled(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundStrokeUncheckedDisabledProperty, value);

    public static readonly DependencyProperty CheckGlyphForegroundUncheckedDisabledProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphForegroundUncheckedDisabled",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetCheckGlyphForegroundUncheckedDisabled(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckGlyphForegroundUncheckedDisabledProperty);

    public static void SetCheckGlyphForegroundUncheckedDisabled(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckGlyphForegroundUncheckedDisabledProperty, value);

    public static readonly DependencyProperty CheckGlyphCheckedProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphChecked",
        typeof(object),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(propertyChangedCallback: null));

    public static object? GetCheckGlyphChecked(
        DependencyObject d)
        => d.GetValue(CheckGlyphCheckedProperty);

    public static void SetCheckGlyphChecked(
        DependencyObject d,
        object? value)
        => d.SetValue(CheckGlyphCheckedProperty, value);

    public static readonly DependencyProperty CheckGlyphCheckedTemplateProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphCheckedTemplate",
        typeof(DataTemplate),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(DataTemplate)));

    public static DataTemplate? GetCheckGlyphCheckedTemplate(
        DependencyObject d)
        => (DataTemplate?)d.GetValue(CheckGlyphCheckedTemplateProperty);

    public static void SetCheckGlyphCheckedTemplate(
        DependencyObject d,
        DataTemplate? value)
        => d.SetValue(CheckGlyphCheckedTemplateProperty, value);

    public static readonly DependencyProperty ForegroundCheckedProperty = DependencyProperty.RegisterAttached(
        "ForegroundChecked",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetForegroundChecked(DependencyObject d) => (Brush?)d.GetValue(ForegroundCheckedProperty);

    public static void SetForegroundChecked(DependencyObject d, Brush? value) => d.SetValue(ForegroundCheckedProperty, value);

    public static readonly DependencyProperty BackgroundCheckedProperty = DependencyProperty.RegisterAttached(
        "BackgroundChecked",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetBackgroundChecked(
        DependencyObject d)
        => (Brush?)d.GetValue(BackgroundCheckedProperty);

    public static void SetBackgroundChecked(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BackgroundCheckedProperty, value);

    public static readonly DependencyProperty BorderBrushCheckedProperty = DependencyProperty.RegisterAttached(
        "BorderBrushChecked",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetBorderBrushChecked(
        DependencyObject d)
        => (Brush?)d.GetValue(BorderBrushCheckedProperty);

    public static void SetBorderBrushChecked(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BorderBrushCheckedProperty, value);

    public static readonly DependencyProperty CheckBackgroundFillCheckedProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundFillChecked",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetCheckBackgroundFillChecked(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundFillCheckedProperty);

    public static void SetCheckBackgroundFillChecked(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundFillCheckedProperty, value);

    public static readonly DependencyProperty CheckBackgroundStrokeCheckedProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundStrokeChecked",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetCheckBackgroundStrokeChecked(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundStrokeCheckedProperty);

    public static void SetCheckBackgroundStrokeChecked(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundStrokeCheckedProperty, value);

    public static readonly DependencyProperty CheckGlyphForegroundCheckedProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphForegroundChecked",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetCheckGlyphForegroundChecked(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckGlyphForegroundCheckedProperty);

    public static void SetCheckGlyphForegroundChecked(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckGlyphForegroundCheckedProperty, value);

    public static readonly DependencyProperty ForegroundCheckedMouseOverProperty = DependencyProperty.RegisterAttached(
        "ForegroundCheckedMouseOver",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetForegroundCheckedMouseOver(
        DependencyObject d)
        => (Brush?)d.GetValue(ForegroundCheckedMouseOverProperty);

    public static void SetForegroundCheckedMouseOver(
        DependencyObject d,
        Brush? value)
        => d.SetValue(ForegroundCheckedMouseOverProperty, value);

    public static readonly DependencyProperty BackgroundCheckedMouseOverProperty = DependencyProperty.RegisterAttached(
        "BackgroundCheckedMouseOver",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetBackgroundCheckedMouseOver(
        DependencyObject d)
        => (Brush?)d.GetValue(BackgroundCheckedMouseOverProperty);

    public static void SetBackgroundCheckedMouseOver(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BackgroundCheckedMouseOverProperty, value);

    public static readonly DependencyProperty BorderBrushCheckedMouseOverProperty = DependencyProperty.RegisterAttached(
        "BorderBrushCheckedMouseOver",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetBorderBrushCheckedMouseOver(
        DependencyObject d)
        => (Brush?)d.GetValue(BorderBrushCheckedMouseOverProperty);

    public static void SetBorderBrushCheckedMouseOver(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BorderBrushCheckedMouseOverProperty, value);

    public static readonly DependencyProperty CheckBackgroundFillCheckedMouseOverProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundFillCheckedMouseOver",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetCheckBackgroundFillCheckedMouseOver(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundFillCheckedMouseOverProperty);

    public static void SetCheckBackgroundFillCheckedMouseOver(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundFillCheckedMouseOverProperty, value);

    public static readonly DependencyProperty CheckBackgroundStrokeCheckedMouseOverProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundStrokeCheckedMouseOver",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetCheckBackgroundStrokeCheckedMouseOver(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundStrokeCheckedMouseOverProperty);

    public static void SetCheckBackgroundStrokeCheckedMouseOver(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundStrokeCheckedMouseOverProperty, value);

    public static readonly DependencyProperty CheckGlyphForegroundCheckedMouseOverProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphForegroundCheckedMouseOver",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetCheckGlyphForegroundCheckedMouseOver(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckGlyphForegroundCheckedMouseOverProperty);

    public static void SetCheckGlyphForegroundCheckedMouseOver(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckGlyphForegroundCheckedMouseOverProperty, value);

    public static readonly DependencyProperty ForegroundCheckedPressedProperty = DependencyProperty.RegisterAttached(
        "ForegroundCheckedPressed",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetForegroundCheckedPressed(
        DependencyObject d)
        => (Brush?)d.GetValue(ForegroundCheckedPressedProperty);

    public static void SetForegroundCheckedPressed(
        DependencyObject d,
        Brush? value)
        => d.SetValue(ForegroundCheckedPressedProperty, value);

    public static readonly DependencyProperty BackgroundCheckedPressedProperty = DependencyProperty.RegisterAttached(
        "BackgroundCheckedPressed",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetBackgroundCheckedPressed(
        DependencyObject d)
        => (Brush?)d.GetValue(BackgroundCheckedPressedProperty);

    public static void SetBackgroundCheckedPressed(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BackgroundCheckedPressedProperty, value);

    public static readonly DependencyProperty BorderBrushCheckedPressedProperty = DependencyProperty.RegisterAttached(
        "BorderBrushCheckedPressed",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetBorderBrushCheckedPressed(
        DependencyObject d)
        => (Brush?)d.GetValue(BorderBrushCheckedPressedProperty);

    public static void SetBorderBrushCheckedPressed(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BorderBrushCheckedPressedProperty, value);

    public static readonly DependencyProperty CheckBackgroundFillCheckedPressedProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundFillCheckedPressed",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetCheckBackgroundFillCheckedPressed(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundFillCheckedPressedProperty);

    public static void SetCheckBackgroundFillCheckedPressed(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundFillCheckedPressedProperty, value);

    public static readonly DependencyProperty CheckBackgroundStrokeCheckedPressedProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundStrokeCheckedPressed",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetCheckBackgroundStrokeCheckedPressed(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundStrokeCheckedPressedProperty);

    public static void SetCheckBackgroundStrokeCheckedPressed(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundStrokeCheckedPressedProperty, value);

    public static readonly DependencyProperty CheckGlyphForegroundCheckedPressedProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphForegroundCheckedPressed",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetCheckGlyphForegroundCheckedPressed(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckGlyphForegroundCheckedPressedProperty);

    public static void SetCheckGlyphForegroundCheckedPressed(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckGlyphForegroundCheckedPressedProperty, value);

    public static readonly DependencyProperty ForegroundCheckedDisabledProperty = DependencyProperty.RegisterAttached(
        "ForegroundCheckedDisabled",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetForegroundCheckedDisabled(
        DependencyObject d)
        => (Brush?)d.GetValue(ForegroundCheckedDisabledProperty);

    public static void SetForegroundCheckedDisabled(
        DependencyObject d,
        Brush? value)
        => d.SetValue(ForegroundCheckedDisabledProperty, value);

    public static readonly DependencyProperty BackgroundCheckedDisabledProperty = DependencyProperty.RegisterAttached(
        "BackgroundCheckedDisabled",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetBackgroundCheckedDisabled(
        DependencyObject d)
        => (Brush?)d.GetValue(BackgroundCheckedDisabledProperty);

    public static void SetBackgroundCheckedDisabled(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BackgroundCheckedDisabledProperty, value);

    public static readonly DependencyProperty BorderBrushCheckedDisabledProperty = DependencyProperty.RegisterAttached(
        "BorderBrushCheckedDisabled",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetBorderBrushCheckedDisabled(
        DependencyObject d)
        => (Brush?)d.GetValue(BorderBrushCheckedDisabledProperty);

    public static void SetBorderBrushCheckedDisabled(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BorderBrushCheckedDisabledProperty, value);

    public static readonly DependencyProperty CheckBackgroundFillCheckedDisabledProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundFillCheckedDisabled",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetCheckBackgroundFillCheckedDisabled(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundFillCheckedDisabledProperty);

    public static void SetCheckBackgroundFillCheckedDisabled(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundFillCheckedDisabledProperty, value);

    public static readonly DependencyProperty CheckBackgroundStrokeCheckedDisabledProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundStrokeCheckedDisabled",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetCheckBackgroundStrokeCheckedDisabled(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundStrokeCheckedDisabledProperty);

    public static void SetCheckBackgroundStrokeCheckedDisabled(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundStrokeCheckedDisabledProperty, value);

    public static readonly DependencyProperty CheckGlyphForegroundCheckedDisabledProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphForegroundCheckedDisabled",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetCheckGlyphForegroundCheckedDisabled(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckGlyphForegroundCheckedDisabledProperty);

    public static void SetCheckGlyphForegroundCheckedDisabled(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckGlyphForegroundCheckedDisabledProperty, value);

    public static readonly DependencyProperty CheckGlyphIndeterminateProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphIndeterminate",
        typeof(object),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(propertyChangedCallback: null));

    public static object? GetCheckGlyphIndeterminate(
        DependencyObject d)
        => d.GetValue(CheckGlyphIndeterminateProperty);

    public static void SetCheckGlyphIndeterminate(
        DependencyObject d,
        object? value)
        => d.SetValue(CheckGlyphIndeterminateProperty, value);

    public static readonly DependencyProperty CheckGlyphIndeterminateTemplateProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphIndeterminateTemplate",
        typeof(DataTemplate),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(DataTemplate)));

    public static DataTemplate? GetCheckGlyphIndeterminateTemplate(
        DependencyObject d)
        => (DataTemplate?)d.GetValue(CheckGlyphIndeterminateTemplateProperty);

    public static void SetCheckGlyphIndeterminateTemplate(
        DependencyObject d,
        DataTemplate? value)
        => d.SetValue(CheckGlyphIndeterminateTemplateProperty, value);

    public static readonly DependencyProperty ForegroundIndeterminateProperty = DependencyProperty.RegisterAttached(
        "ForegroundIndeterminate",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetForegroundIndeterminate(
        DependencyObject d)
        => (Brush?)d.GetValue(ForegroundIndeterminateProperty);

    public static void SetForegroundIndeterminate(
        DependencyObject d,
        Brush? value)
        => d.SetValue(ForegroundIndeterminateProperty, value);

    public static readonly DependencyProperty BackgroundIndeterminateProperty = DependencyProperty.RegisterAttached(
        "BackgroundIndeterminate",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetBackgroundIndeterminate(
        DependencyObject d)
        => (Brush?)d.GetValue(BackgroundIndeterminateProperty);

    public static void SetBackgroundIndeterminate(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BackgroundIndeterminateProperty, value);

    public static readonly DependencyProperty BorderBrushIndeterminateProperty = DependencyProperty.RegisterAttached(
        "BorderBrushIndeterminate",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetBorderBrushIndeterminate(
        DependencyObject d)
        => (Brush?)d.GetValue(BorderBrushIndeterminateProperty);

    public static void SetBorderBrushIndeterminate(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BorderBrushIndeterminateProperty, value);

    public static readonly DependencyProperty CheckBackgroundFillIndeterminateProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundFillIndeterminate",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetCheckBackgroundFillIndeterminate(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundFillIndeterminateProperty);

    public static void SetCheckBackgroundFillIndeterminate(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundFillIndeterminateProperty, value);

    public static readonly DependencyProperty CheckBackgroundStrokeIndeterminateProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundStrokeIndeterminate",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetCheckBackgroundStrokeIndeterminate(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundStrokeIndeterminateProperty);

    public static void SetCheckBackgroundStrokeIndeterminate(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundStrokeIndeterminateProperty, value);

    public static readonly DependencyProperty CheckGlyphForegroundIndeterminateProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphForegroundIndeterminate",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetCheckGlyphForegroundIndeterminate(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckGlyphForegroundIndeterminateProperty);

    public static void SetCheckGlyphForegroundIndeterminate(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckGlyphForegroundIndeterminateProperty, value);

    public static readonly DependencyProperty ForegroundIndeterminateMouseOverProperty = DependencyProperty.RegisterAttached(
        "ForegroundIndeterminateMouseOver",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetForegroundIndeterminateMouseOver(
        DependencyObject d)
        => (Brush?)d.GetValue(ForegroundIndeterminateMouseOverProperty);

    public static void SetForegroundIndeterminateMouseOver(
        DependencyObject d,
        Brush? value)
        => d.SetValue(ForegroundIndeterminateMouseOverProperty, value);

    public static readonly DependencyProperty BackgroundIndeterminateMouseOverProperty = DependencyProperty.RegisterAttached(
        "BackgroundIndeterminateMouseOver",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetBackgroundIndeterminateMouseOver(
        DependencyObject d)
        => (Brush?)d.GetValue(BackgroundIndeterminateMouseOverProperty);

    public static void SetBackgroundIndeterminateMouseOver(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BackgroundIndeterminateMouseOverProperty, value);

    public static readonly DependencyProperty BorderBrushIndeterminateMouseOverProperty = DependencyProperty.RegisterAttached(
        "BorderBrushIndeterminateMouseOver",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetBorderBrushIndeterminateMouseOver(
        DependencyObject d)
        => (Brush?)d.GetValue(BorderBrushIndeterminateMouseOverProperty);

    public static void SetBorderBrushIndeterminateMouseOver(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BorderBrushIndeterminateMouseOverProperty, value);

    public static readonly DependencyProperty CheckBackgroundFillIndeterminateMouseOverProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundFillIndeterminateMouseOver",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetCheckBackgroundFillIndeterminateMouseOver(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundFillIndeterminateMouseOverProperty);

    public static void SetCheckBackgroundFillIndeterminateMouseOver(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundFillIndeterminateMouseOverProperty, value);

    public static readonly DependencyProperty CheckBackgroundStrokeIndeterminateMouseOverProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundStrokeIndeterminateMouseOver",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetCheckBackgroundStrokeIndeterminateMouseOver(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundStrokeIndeterminateMouseOverProperty);

    public static void SetCheckBackgroundStrokeIndeterminateMouseOver(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundStrokeIndeterminateMouseOverProperty, value);

    public static readonly DependencyProperty CheckGlyphForegroundIndeterminateMouseOverProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphForegroundIndeterminateMouseOver",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetCheckGlyphForegroundIndeterminateMouseOver(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckGlyphForegroundIndeterminateMouseOverProperty);

    public static void SetCheckGlyphForegroundIndeterminateMouseOver(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckGlyphForegroundIndeterminateMouseOverProperty, value);

    public static readonly DependencyProperty ForegroundIndeterminatePressedProperty = DependencyProperty.RegisterAttached(
        "ForegroundIndeterminatePressed",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetForegroundIndeterminatePressed(
        DependencyObject d)
        => (Brush?)d.GetValue(ForegroundIndeterminatePressedProperty);

    public static void SetForegroundIndeterminatePressed(
        DependencyObject d,
        Brush? value)
        => d.SetValue(ForegroundIndeterminatePressedProperty, value);

    public static readonly DependencyProperty BackgroundIndeterminatePressedProperty = DependencyProperty.RegisterAttached(
        "BackgroundIndeterminatePressed",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetBackgroundIndeterminatePressed(
        DependencyObject d)
        => (Brush?)d.GetValue(BackgroundIndeterminatePressedProperty);

    public static void SetBackgroundIndeterminatePressed(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BackgroundIndeterminatePressedProperty, value);

    public static readonly DependencyProperty BorderBrushIndeterminatePressedProperty = DependencyProperty.RegisterAttached(
        "BorderBrushIndeterminatePressed",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetBorderBrushIndeterminatePressed(
        DependencyObject d)
        => (Brush?)d.GetValue(BorderBrushIndeterminatePressedProperty);

    public static void SetBorderBrushIndeterminatePressed(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BorderBrushIndeterminatePressedProperty, value);

    public static readonly DependencyProperty CheckBackgroundFillIndeterminatePressedProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundFillIndeterminatePressed",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetCheckBackgroundFillIndeterminatePressed(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundFillIndeterminatePressedProperty);

    public static void SetCheckBackgroundFillIndeterminatePressed(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundFillIndeterminatePressedProperty, value);

    public static readonly DependencyProperty CheckBackgroundStrokeIndeterminatePressedProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundStrokeIndeterminatePressed",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetCheckBackgroundStrokeIndeterminatePressed(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundStrokeIndeterminatePressedProperty);

    public static void SetCheckBackgroundStrokeIndeterminatePressed(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundStrokeIndeterminatePressedProperty, value);

    public static readonly DependencyProperty CheckGlyphForegroundIndeterminatePressedProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphForegroundIndeterminatePressed",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetCheckGlyphForegroundIndeterminatePressed(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckGlyphForegroundIndeterminatePressedProperty);

    public static void SetCheckGlyphForegroundIndeterminatePressed(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckGlyphForegroundIndeterminatePressedProperty, value);

    public static readonly DependencyProperty ForegroundIndeterminateDisabledProperty = DependencyProperty.RegisterAttached(
        "ForegroundIndeterminateDisabled",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetForegroundIndeterminateDisabled(
        DependencyObject d)
        => (Brush?)d.GetValue(ForegroundIndeterminateDisabledProperty);

    public static void SetForegroundIndeterminateDisabled(
        DependencyObject d,
        Brush? value)
        => d.SetValue(ForegroundIndeterminateDisabledProperty, value);

    public static readonly DependencyProperty BackgroundIndeterminateDisabledProperty = DependencyProperty.RegisterAttached(
        "BackgroundIndeterminateDisabled",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetBackgroundIndeterminateDisabled(
        DependencyObject d)
        => (Brush?)d.GetValue(BackgroundIndeterminateDisabledProperty);

    public static void SetBackgroundIndeterminateDisabled(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BackgroundIndeterminateDisabledProperty, value);

    public static readonly DependencyProperty BorderBrushIndeterminateDisabledProperty = DependencyProperty.RegisterAttached(
        "BorderBrushIndeterminateDisabled",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetBorderBrushIndeterminateDisabled(
        DependencyObject d)
        => (Brush?)d.GetValue(BorderBrushIndeterminateDisabledProperty);

    public static void SetBorderBrushIndeterminateDisabled(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BorderBrushIndeterminateDisabledProperty, value);

    public static readonly DependencyProperty CheckBackgroundFillIndeterminateDisabledProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundFillIndeterminateDisabled",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetCheckBackgroundFillIndeterminateDisabled(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundFillIndeterminateDisabledProperty);

    public static void SetCheckBackgroundFillIndeterminateDisabled(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundFillIndeterminateDisabledProperty, value);

    public static readonly DependencyProperty CheckBackgroundStrokeIndeterminateDisabledProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundStrokeIndeterminateDisabled",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetCheckBackgroundStrokeIndeterminateDisabled(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundStrokeIndeterminateDisabledProperty);

    public static void SetCheckBackgroundStrokeIndeterminateDisabled(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundStrokeIndeterminateDisabledProperty, value);

    public static readonly DependencyProperty CheckGlyphForegroundIndeterminateDisabledProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphForegroundIndeterminateDisabled",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    public static Brush? GetCheckGlyphForegroundIndeterminateDisabled(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckGlyphForegroundIndeterminateDisabledProperty);

    public static void SetCheckGlyphForegroundIndeterminateDisabled(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckGlyphForegroundIndeterminateDisabledProperty, value);
}
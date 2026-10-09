namespace Atc.Wpf.Helpers;

/// <summary>Provides attached properties for styling a <see cref="CheckBox"/>, such as check size, glyphs and brushes for the unchecked, checked and indeterminate states.</summary>
[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static class CheckBoxHelper
{
    /// <summary>Identifies the CheckSize attached property.</summary>
    public static readonly DependencyProperty CheckSizeProperty = DependencyProperty.RegisterAttached(
        "CheckSize",
        typeof(double),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(18.0));

    /// <summary>Gets the value of the CheckSize attached property.</summary>
    public static double GetCheckSize(DependencyObject d)
        => (double)d.GetValue(CheckSizeProperty);

    /// <summary>Sets the value of the CheckSize attached property.</summary>
    public static void SetCheckSize(
        DependencyObject d,
        double value)
        => d.SetValue(CheckSizeProperty, value);

    /// <summary>Identifies the CheckCornerRadius attached property.</summary>
    public static readonly DependencyProperty CheckCornerRadiusProperty = DependencyProperty.RegisterAttached(
        "CheckCornerRadius",
        typeof(CornerRadius),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(
            new CornerRadius(0),
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Gets the value of the CheckCornerRadius attached property.</summary>
    public static CornerRadius GetCheckCornerRadius(UIElement element)
        => (CornerRadius)element.GetValue(CheckCornerRadiusProperty);

    /// <summary>Sets the value of the CheckCornerRadius attached property.</summary>
    public static void SetCheckCornerRadius(
        UIElement element,
        CornerRadius value)
        => element.SetValue(CheckCornerRadiusProperty, value);

    /// <summary>Identifies the CheckStrokeThickness attached property.</summary>
    public static readonly DependencyProperty CheckStrokeThicknessProperty = DependencyProperty.RegisterAttached(
        "CheckStrokeThickness",
        typeof(double),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(1d));

    /// <summary>Gets the value of the CheckStrokeThickness attached property.</summary>
    public static double GetCheckStrokeThickness(DependencyObject d)
        => (double)d.GetValue(CheckStrokeThicknessProperty);

    /// <summary>Sets the value of the CheckStrokeThickness attached property.</summary>
    public static void SetCheckStrokeThickness(
        DependencyObject d,
        double value)
        => d.SetValue(CheckStrokeThicknessProperty, value);

    /// <summary>Identifies the CheckGlyphUnchecked attached property.</summary>
    public static readonly DependencyProperty CheckGlyphUncheckedProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphUnchecked",
        typeof(object),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(propertyChangedCallback: null));

    /// <summary>Gets the value of the CheckGlyphUnchecked attached property.</summary>
    public static object? GetCheckGlyphUnchecked(DependencyObject d)
        => d.GetValue(CheckGlyphUncheckedProperty);

    /// <summary>Sets the value of the CheckGlyphUnchecked attached property.</summary>
    public static void SetCheckGlyphUnchecked(
        DependencyObject d,
        object? value)
        => d.SetValue(CheckGlyphUncheckedProperty, value);

    /// <summary>Identifies the CheckGlyphUncheckedTemplate attached property.</summary>
    public static readonly DependencyProperty CheckGlyphUncheckedTemplateProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphUncheckedTemplate",
        typeof(DataTemplate),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(DataTemplate)));

    /// <summary>Gets the value of the CheckGlyphUncheckedTemplate attached property.</summary>
    public static DataTemplate? GetCheckGlyphUncheckedTemplate(
        DependencyObject d)
        => (DataTemplate?)d.GetValue(CheckGlyphUncheckedTemplateProperty);

    /// <summary>Sets the value of the CheckGlyphUncheckedTemplate attached property.</summary>
    public static void SetCheckGlyphUncheckedTemplate(
        DependencyObject d,
        DataTemplate? value)
        => d.SetValue(CheckGlyphUncheckedTemplateProperty, value);

    /// <summary>Identifies the ForegroundUnchecked attached property.</summary>
    public static readonly DependencyProperty ForegroundUncheckedProperty = DependencyProperty.RegisterAttached(
        "ForegroundUnchecked",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the ForegroundUnchecked attached property.</summary>
    public static Brush? GetForegroundUnchecked(DependencyObject d)
        => (Brush?)d.GetValue(ForegroundUncheckedProperty);

    /// <summary>Sets the value of the ForegroundUnchecked attached property.</summary>
    public static void SetForegroundUnchecked(
        DependencyObject d,
        Brush? value)
        => d.SetValue(ForegroundUncheckedProperty, value);

    /// <summary>Identifies the BackgroundUnchecked attached property.</summary>
    public static readonly DependencyProperty BackgroundUncheckedProperty = DependencyProperty.RegisterAttached(
        "BackgroundUnchecked",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the BackgroundUnchecked attached property.</summary>
    public static Brush? GetBackgroundUnchecked(DependencyObject d)
        => (Brush?)d.GetValue(BackgroundUncheckedProperty);

    /// <summary>Sets the value of the BackgroundUnchecked attached property.</summary>
    public static void SetBackgroundUnchecked(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BackgroundUncheckedProperty, value);

    /// <summary>Identifies the BorderBrushUnchecked attached property.</summary>
    public static readonly DependencyProperty BorderBrushUncheckedProperty = DependencyProperty.RegisterAttached(
        "BorderBrushUnchecked",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the BorderBrushUnchecked attached property.</summary>
    public static Brush? GetBorderBrushUnchecked(DependencyObject d)
        => (Brush?)d.GetValue(BorderBrushUncheckedProperty);

    /// <summary>Sets the value of the BorderBrushUnchecked attached property.</summary>
    public static void SetBorderBrushUnchecked(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BorderBrushUncheckedProperty, value);

    /// <summary>Identifies the CheckBackgroundFillUnchecked attached property.</summary>
    public static readonly DependencyProperty CheckBackgroundFillUncheckedProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundFillUnchecked",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckBackgroundFillUnchecked attached property.</summary>
    public static Brush? GetCheckBackgroundFillUnchecked(DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundFillUncheckedProperty);

    /// <summary>Sets the value of the CheckBackgroundFillUnchecked attached property.</summary>
    public static void SetCheckBackgroundFillUnchecked(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundFillUncheckedProperty, value);

    /// <summary>Identifies the CheckBackgroundStrokeUnchecked attached property.</summary>
    public static readonly DependencyProperty CheckBackgroundStrokeUncheckedProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundStrokeUnchecked",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckBackgroundStrokeUnchecked attached property.</summary>
    public static Brush? GetCheckBackgroundStrokeUnchecked(DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundStrokeUncheckedProperty);

    /// <summary>Sets the value of the CheckBackgroundStrokeUnchecked attached property.</summary>
    public static void SetCheckBackgroundStrokeUnchecked(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundStrokeUncheckedProperty, value);

    /// <summary>Identifies the CheckGlyphForegroundUnchecked attached property.</summary>
    public static readonly DependencyProperty CheckGlyphForegroundUncheckedProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphForegroundUnchecked",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckGlyphForegroundUnchecked attached property.</summary>
    public static Brush? GetCheckGlyphForegroundUnchecked(DependencyObject d)
        => (Brush?)d.GetValue(CheckGlyphForegroundUncheckedProperty);

    /// <summary>Sets the value of the CheckGlyphForegroundUnchecked attached property.</summary>
    public static void SetCheckGlyphForegroundUnchecked(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckGlyphForegroundUncheckedProperty, value);

    /// <summary>Identifies the ForegroundUncheckedMouseOver attached property.</summary>
    public static readonly DependencyProperty ForegroundUncheckedMouseOverProperty = DependencyProperty.RegisterAttached(
        "ForegroundUncheckedMouseOver",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the ForegroundUncheckedMouseOver attached property.</summary>
    public static Brush? GetForegroundUncheckedMouseOver(DependencyObject d)
        => (Brush?)d.GetValue(ForegroundUncheckedMouseOverProperty);

    /// <summary>Sets the value of the ForegroundUncheckedMouseOver attached property.</summary>
    public static void SetForegroundUncheckedMouseOver(
        DependencyObject d,
        Brush? value)
        => d.SetValue(ForegroundUncheckedMouseOverProperty, value);

    /// <summary>Identifies the BackgroundUncheckedMouseOver attached property.</summary>
    public static readonly DependencyProperty BackgroundUncheckedMouseOverProperty = DependencyProperty.RegisterAttached(
        "BackgroundUncheckedMouseOver",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the BackgroundUncheckedMouseOver attached property.</summary>
    public static Brush? GetBackgroundUncheckedMouseOver(DependencyObject d)
        => (Brush?)d.GetValue(BackgroundUncheckedMouseOverProperty);

    /// <summary>Sets the value of the BackgroundUncheckedMouseOver attached property.</summary>
    public static void SetBackgroundUncheckedMouseOver(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BackgroundUncheckedMouseOverProperty, value);

    /// <summary>Identifies the BorderBrushUncheckedMouseOver attached property.</summary>
    public static readonly DependencyProperty BorderBrushUncheckedMouseOverProperty = DependencyProperty.RegisterAttached(
        "BorderBrushUncheckedMouseOver",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the BorderBrushUncheckedMouseOver attached property.</summary>
    public static Brush? GetBorderBrushUncheckedMouseOver(DependencyObject d)
        => (Brush?)d.GetValue(BorderBrushUncheckedMouseOverProperty);

    /// <summary>Sets the value of the BorderBrushUncheckedMouseOver attached property.</summary>
    public static void SetBorderBrushUncheckedMouseOver(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BorderBrushUncheckedMouseOverProperty, value);

    /// <summary>Identifies the CheckBackgroundFillUncheckedMouseOver attached property.</summary>
    public static readonly DependencyProperty CheckBackgroundFillUncheckedMouseOverProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundFillUncheckedMouseOver",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckBackgroundFillUncheckedMouseOver attached property.</summary>
    public static Brush? GetCheckBackgroundFillUncheckedMouseOver(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundFillUncheckedMouseOverProperty);

    /// <summary>Sets the value of the CheckBackgroundFillUncheckedMouseOver attached property.</summary>
    public static void SetCheckBackgroundFillUncheckedMouseOver(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundFillUncheckedMouseOverProperty, value);

    /// <summary>Identifies the CheckBackgroundStrokeUncheckedMouseOver attached property.</summary>
    public static readonly DependencyProperty CheckBackgroundStrokeUncheckedMouseOverProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundStrokeUncheckedMouseOver",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckBackgroundStrokeUncheckedMouseOver attached property.</summary>
    public static Brush? GetCheckBackgroundStrokeUncheckedMouseOver(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundStrokeUncheckedMouseOverProperty);

    /// <summary>Sets the value of the CheckBackgroundStrokeUncheckedMouseOver attached property.</summary>
    public static void SetCheckBackgroundStrokeUncheckedMouseOver(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundStrokeUncheckedMouseOverProperty, value);

    /// <summary>Identifies the CheckGlyphForegroundUncheckedMouseOver attached property.</summary>
    public static readonly DependencyProperty CheckGlyphForegroundUncheckedMouseOverProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphForegroundUncheckedMouseOver",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckGlyphForegroundUncheckedMouseOver attached property.</summary>
    public static Brush? GetCheckGlyphForegroundUncheckedMouseOver(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckGlyphForegroundUncheckedMouseOverProperty);

    /// <summary>Sets the value of the CheckGlyphForegroundUncheckedMouseOver attached property.</summary>
    public static void SetCheckGlyphForegroundUncheckedMouseOver(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckGlyphForegroundUncheckedMouseOverProperty, value);

    /// <summary>Identifies the ForegroundUncheckedPressed attached property.</summary>
    public static readonly DependencyProperty ForegroundUncheckedPressedProperty = DependencyProperty.RegisterAttached(
        "ForegroundUncheckedPressed",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the ForegroundUncheckedPressed attached property.</summary>
    public static Brush? GetForegroundUncheckedPressed(DependencyObject d)
        => (Brush?)d.GetValue(ForegroundUncheckedPressedProperty);

    /// <summary>Sets the value of the ForegroundUncheckedPressed attached property.</summary>
    public static void SetForegroundUncheckedPressed(
        DependencyObject d,
        Brush? value)
        => d.SetValue(ForegroundUncheckedPressedProperty, value);

    /// <summary>Identifies the BackgroundUncheckedPressed attached property.</summary>
    public static readonly DependencyProperty BackgroundUncheckedPressedProperty = DependencyProperty.RegisterAttached(
        "BackgroundUncheckedPressed",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the BackgroundUncheckedPressed attached property.</summary>
    public static Brush? GetBackgroundUncheckedPressed(DependencyObject d)
        => (Brush?)d.GetValue(BackgroundUncheckedPressedProperty);

    /// <summary>Sets the value of the BackgroundUncheckedPressed attached property.</summary>
    public static void SetBackgroundUncheckedPressed(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BackgroundUncheckedPressedProperty, value);

    /// <summary>Identifies the BorderBrushUncheckedPressed attached property.</summary>
    public static readonly DependencyProperty BorderBrushUncheckedPressedProperty = DependencyProperty.RegisterAttached(
        "BorderBrushUncheckedPressed",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the BorderBrushUncheckedPressed attached property.</summary>
    public static Brush? GetBorderBrushUncheckedPressed(DependencyObject d)
        => (Brush?)d.GetValue(BorderBrushUncheckedPressedProperty);

    /// <summary>Sets the value of the BorderBrushUncheckedPressed attached property.</summary>
    public static void SetBorderBrushUncheckedPressed(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BorderBrushUncheckedPressedProperty, value);

    /// <summary>Identifies the CheckBackgroundFillUncheckedPressed attached property.</summary>
    public static readonly DependencyProperty CheckBackgroundFillUncheckedPressedProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundFillUncheckedPressed",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckBackgroundFillUncheckedPressed attached property.</summary>
    public static Brush? GetCheckBackgroundFillUncheckedPressed(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundFillUncheckedPressedProperty);

    /// <summary>Sets the value of the CheckBackgroundFillUncheckedPressed attached property.</summary>
    public static void SetCheckBackgroundFillUncheckedPressed(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundFillUncheckedPressedProperty, value);

    /// <summary>Identifies the CheckBackgroundStrokeUncheckedPressed attached property.</summary>
    public static readonly DependencyProperty CheckBackgroundStrokeUncheckedPressedProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundStrokeUncheckedPressed",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckBackgroundStrokeUncheckedPressed attached property.</summary>
    public static Brush? GetCheckBackgroundStrokeUncheckedPressed(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundStrokeUncheckedPressedProperty);

    /// <summary>Sets the value of the CheckBackgroundStrokeUncheckedPressed attached property.</summary>
    public static void SetCheckBackgroundStrokeUncheckedPressed(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundStrokeUncheckedPressedProperty, value);

    /// <summary>Identifies the CheckGlyphForegroundUncheckedPressed attached property.</summary>
    public static readonly DependencyProperty CheckGlyphForegroundUncheckedPressedProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphForegroundUncheckedPressed",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckGlyphForegroundUncheckedPressed attached property.</summary>
    public static Brush? GetCheckGlyphForegroundUncheckedPressed(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckGlyphForegroundUncheckedPressedProperty);

    /// <summary>Sets the value of the CheckGlyphForegroundUncheckedPressed attached property.</summary>
    public static void SetCheckGlyphForegroundUncheckedPressed(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckGlyphForegroundUncheckedPressedProperty, value);

    /// <summary>Identifies the ForegroundUncheckedDisabled attached property.</summary>
    public static readonly DependencyProperty ForegroundUncheckedDisabledProperty = DependencyProperty.RegisterAttached(
        "ForegroundUncheckedDisabled",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the ForegroundUncheckedDisabled attached property.</summary>
    public static Brush? GetForegroundUncheckedDisabled(DependencyObject d)
        => (Brush?)d.GetValue(ForegroundUncheckedDisabledProperty);

    /// <summary>Sets the value of the ForegroundUncheckedDisabled attached property.</summary>
    public static void SetForegroundUncheckedDisabled(
        DependencyObject d,
        Brush? value)
        => d.SetValue(ForegroundUncheckedDisabledProperty, value);

    /// <summary>Identifies the BackgroundUncheckedDisabled attached property.</summary>
    public static readonly DependencyProperty BackgroundUncheckedDisabledProperty = DependencyProperty.RegisterAttached(
        "BackgroundUncheckedDisabled",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the BackgroundUncheckedDisabled attached property.</summary>
    public static Brush? GetBackgroundUncheckedDisabled(DependencyObject d)
        => (Brush?)d.GetValue(BackgroundUncheckedDisabledProperty);

    /// <summary>Sets the value of the BackgroundUncheckedDisabled attached property.</summary>
    public static void SetBackgroundUncheckedDisabled(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BackgroundUncheckedDisabledProperty, value);

    /// <summary>Identifies the BorderBrushUncheckedDisabled attached property.</summary>
    public static readonly DependencyProperty BorderBrushUncheckedDisabledProperty = DependencyProperty.RegisterAttached(
        "BorderBrushUncheckedDisabled",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the BorderBrushUncheckedDisabled attached property.</summary>
    public static Brush? GetBorderBrushUncheckedDisabled(DependencyObject d)
        => (Brush?)d.GetValue(BorderBrushUncheckedDisabledProperty);

    /// <summary>Sets the value of the BorderBrushUncheckedDisabled attached property.</summary>
    public static void SetBorderBrushUncheckedDisabled(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BorderBrushUncheckedDisabledProperty, value);

    /// <summary>Identifies the CheckBackgroundFillUncheckedDisabled attached property.</summary>
    public static readonly DependencyProperty CheckBackgroundFillUncheckedDisabledProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundFillUncheckedDisabled",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckBackgroundFillUncheckedDisabled attached property.</summary>
    public static Brush? GetCheckBackgroundFillUncheckedDisabled(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundFillUncheckedDisabledProperty);

    /// <summary>Sets the value of the CheckBackgroundFillUncheckedDisabled attached property.</summary>
    public static void SetCheckBackgroundFillUncheckedDisabled(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundFillUncheckedDisabledProperty, value);

    /// <summary>Identifies the CheckBackgroundStrokeUncheckedDisabled attached property.</summary>
    public static readonly DependencyProperty CheckBackgroundStrokeUncheckedDisabledProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundStrokeUncheckedDisabled",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckBackgroundStrokeUncheckedDisabled attached property.</summary>
    public static Brush? GetCheckBackgroundStrokeUncheckedDisabled(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundStrokeUncheckedDisabledProperty);

    /// <summary>Sets the value of the CheckBackgroundStrokeUncheckedDisabled attached property.</summary>
    public static void SetCheckBackgroundStrokeUncheckedDisabled(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundStrokeUncheckedDisabledProperty, value);

    /// <summary>Identifies the CheckGlyphForegroundUncheckedDisabled attached property.</summary>
    public static readonly DependencyProperty CheckGlyphForegroundUncheckedDisabledProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphForegroundUncheckedDisabled",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckGlyphForegroundUncheckedDisabled attached property.</summary>
    public static Brush? GetCheckGlyphForegroundUncheckedDisabled(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckGlyphForegroundUncheckedDisabledProperty);

    /// <summary>Sets the value of the CheckGlyphForegroundUncheckedDisabled attached property.</summary>
    public static void SetCheckGlyphForegroundUncheckedDisabled(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckGlyphForegroundUncheckedDisabledProperty, value);

    /// <summary>Identifies the CheckGlyphChecked attached property.</summary>
    public static readonly DependencyProperty CheckGlyphCheckedProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphChecked",
        typeof(object),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(propertyChangedCallback: null));

    /// <summary>Gets the value of the CheckGlyphChecked attached property.</summary>
    public static object? GetCheckGlyphChecked(DependencyObject d)
        => d.GetValue(CheckGlyphCheckedProperty);

    /// <summary>Sets the value of the CheckGlyphChecked attached property.</summary>
    public static void SetCheckGlyphChecked(
        DependencyObject d,
        object? value)
        => d.SetValue(CheckGlyphCheckedProperty, value);

    /// <summary>Identifies the CheckGlyphCheckedTemplate attached property.</summary>
    public static readonly DependencyProperty CheckGlyphCheckedTemplateProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphCheckedTemplate",
        typeof(DataTemplate),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(DataTemplate)));

    /// <summary>Gets the value of the CheckGlyphCheckedTemplate attached property.</summary>
    public static DataTemplate? GetCheckGlyphCheckedTemplate(DependencyObject d)
        => (DataTemplate?)d.GetValue(CheckGlyphCheckedTemplateProperty);

    /// <summary>Sets the value of the CheckGlyphCheckedTemplate attached property.</summary>
    public static void SetCheckGlyphCheckedTemplate(
        DependencyObject d,
        DataTemplate? value)
        => d.SetValue(CheckGlyphCheckedTemplateProperty, value);

    /// <summary>Identifies the ForegroundChecked attached property.</summary>
    public static readonly DependencyProperty ForegroundCheckedProperty = DependencyProperty.RegisterAttached(
        "ForegroundChecked",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the ForegroundChecked attached property.</summary>
    public static Brush? GetForegroundChecked(DependencyObject d)
        => (Brush?)d.GetValue(ForegroundCheckedProperty);

    /// <summary>Sets the value of the ForegroundChecked attached property.</summary>
    public static void SetForegroundChecked(
        DependencyObject d,
        Brush? value)
        => d.SetValue(ForegroundCheckedProperty, value);

    /// <summary>Identifies the BackgroundChecked attached property.</summary>
    public static readonly DependencyProperty BackgroundCheckedProperty = DependencyProperty.RegisterAttached(
        "BackgroundChecked",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the BackgroundChecked attached property.</summary>
    public static Brush? GetBackgroundChecked(DependencyObject d)
        => (Brush?)d.GetValue(BackgroundCheckedProperty);

    /// <summary>Sets the value of the BackgroundChecked attached property.</summary>
    public static void SetBackgroundChecked(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BackgroundCheckedProperty, value);

    /// <summary>Identifies the BorderBrushChecked attached property.</summary>
    public static readonly DependencyProperty BorderBrushCheckedProperty = DependencyProperty.RegisterAttached(
        "BorderBrushChecked",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the BorderBrushChecked attached property.</summary>
    public static Brush? GetBorderBrushChecked(DependencyObject d)
        => (Brush?)d.GetValue(BorderBrushCheckedProperty);

    /// <summary>Sets the value of the BorderBrushChecked attached property.</summary>
    public static void SetBorderBrushChecked(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BorderBrushCheckedProperty, value);

    /// <summary>Identifies the CheckBackgroundFillChecked attached property.</summary>
    public static readonly DependencyProperty CheckBackgroundFillCheckedProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundFillChecked",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckBackgroundFillChecked attached property.</summary>
    public static Brush? GetCheckBackgroundFillChecked(DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundFillCheckedProperty);

    /// <summary>Sets the value of the CheckBackgroundFillChecked attached property.</summary>
    public static void SetCheckBackgroundFillChecked(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundFillCheckedProperty, value);

    /// <summary>Identifies the CheckBackgroundStrokeChecked attached property.</summary>
    public static readonly DependencyProperty CheckBackgroundStrokeCheckedProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundStrokeChecked",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckBackgroundStrokeChecked attached property.</summary>
    public static Brush? GetCheckBackgroundStrokeChecked(DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundStrokeCheckedProperty);

    /// <summary>Sets the value of the CheckBackgroundStrokeChecked attached property.</summary>
    public static void SetCheckBackgroundStrokeChecked(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundStrokeCheckedProperty, value);

    /// <summary>Identifies the CheckGlyphForegroundChecked attached property.</summary>
    public static readonly DependencyProperty CheckGlyphForegroundCheckedProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphForegroundChecked",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckGlyphForegroundChecked attached property.</summary>
    public static Brush? GetCheckGlyphForegroundChecked(DependencyObject d)
        => (Brush?)d.GetValue(CheckGlyphForegroundCheckedProperty);

    /// <summary>Sets the value of the CheckGlyphForegroundChecked attached property.</summary>
    public static void SetCheckGlyphForegroundChecked(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckGlyphForegroundCheckedProperty, value);

    /// <summary>Identifies the ForegroundCheckedMouseOver attached property.</summary>
    public static readonly DependencyProperty ForegroundCheckedMouseOverProperty = DependencyProperty.RegisterAttached(
        "ForegroundCheckedMouseOver",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the ForegroundCheckedMouseOver attached property.</summary>
    public static Brush? GetForegroundCheckedMouseOver(DependencyObject d)
        => (Brush?)d.GetValue(ForegroundCheckedMouseOverProperty);

    /// <summary>Sets the value of the ForegroundCheckedMouseOver attached property.</summary>
    public static void SetForegroundCheckedMouseOver(
        DependencyObject d,
        Brush? value)
        => d.SetValue(ForegroundCheckedMouseOverProperty, value);

    /// <summary>Identifies the BackgroundCheckedMouseOver attached property.</summary>
    public static readonly DependencyProperty BackgroundCheckedMouseOverProperty = DependencyProperty.RegisterAttached(
        "BackgroundCheckedMouseOver",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the BackgroundCheckedMouseOver attached property.</summary>
    public static Brush? GetBackgroundCheckedMouseOver(DependencyObject d)
        => (Brush?)d.GetValue(BackgroundCheckedMouseOverProperty);

    /// <summary>Sets the value of the BackgroundCheckedMouseOver attached property.</summary>
    public static void SetBackgroundCheckedMouseOver(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BackgroundCheckedMouseOverProperty, value);

    /// <summary>Identifies the BorderBrushCheckedMouseOver attached property.</summary>
    public static readonly DependencyProperty BorderBrushCheckedMouseOverProperty = DependencyProperty.RegisterAttached(
        "BorderBrushCheckedMouseOver",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the BorderBrushCheckedMouseOver attached property.</summary>
    public static Brush? GetBorderBrushCheckedMouseOver(DependencyObject d)
        => (Brush?)d.GetValue(BorderBrushCheckedMouseOverProperty);

    /// <summary>Sets the value of the BorderBrushCheckedMouseOver attached property.</summary>
    public static void SetBorderBrushCheckedMouseOver(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BorderBrushCheckedMouseOverProperty, value);

    /// <summary>Identifies the CheckBackgroundFillCheckedMouseOver attached property.</summary>
    public static readonly DependencyProperty CheckBackgroundFillCheckedMouseOverProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundFillCheckedMouseOver",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckBackgroundFillCheckedMouseOver attached property.</summary>
    public static Brush? GetCheckBackgroundFillCheckedMouseOver(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundFillCheckedMouseOverProperty);

    /// <summary>Sets the value of the CheckBackgroundFillCheckedMouseOver attached property.</summary>
    public static void SetCheckBackgroundFillCheckedMouseOver(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundFillCheckedMouseOverProperty, value);

    /// <summary>Identifies the CheckBackgroundStrokeCheckedMouseOver attached property.</summary>
    public static readonly DependencyProperty CheckBackgroundStrokeCheckedMouseOverProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundStrokeCheckedMouseOver",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckBackgroundStrokeCheckedMouseOver attached property.</summary>
    public static Brush? GetCheckBackgroundStrokeCheckedMouseOver(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundStrokeCheckedMouseOverProperty);

    /// <summary>Sets the value of the CheckBackgroundStrokeCheckedMouseOver attached property.</summary>
    public static void SetCheckBackgroundStrokeCheckedMouseOver(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundStrokeCheckedMouseOverProperty, value);

    /// <summary>Identifies the CheckGlyphForegroundCheckedMouseOver attached property.</summary>
    public static readonly DependencyProperty CheckGlyphForegroundCheckedMouseOverProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphForegroundCheckedMouseOver",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckGlyphForegroundCheckedMouseOver attached property.</summary>
    public static Brush? GetCheckGlyphForegroundCheckedMouseOver(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckGlyphForegroundCheckedMouseOverProperty);

    /// <summary>Sets the value of the CheckGlyphForegroundCheckedMouseOver attached property.</summary>
    public static void SetCheckGlyphForegroundCheckedMouseOver(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckGlyphForegroundCheckedMouseOverProperty, value);

    /// <summary>Identifies the ForegroundCheckedPressed attached property.</summary>
    public static readonly DependencyProperty ForegroundCheckedPressedProperty = DependencyProperty.RegisterAttached(
        "ForegroundCheckedPressed",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the ForegroundCheckedPressed attached property.</summary>
    public static Brush? GetForegroundCheckedPressed(DependencyObject d)
        => (Brush?)d.GetValue(ForegroundCheckedPressedProperty);

    /// <summary>Sets the value of the ForegroundCheckedPressed attached property.</summary>
    public static void SetForegroundCheckedPressed(
        DependencyObject d,
        Brush? value)
        => d.SetValue(ForegroundCheckedPressedProperty, value);

    /// <summary>Identifies the BackgroundCheckedPressed attached property.</summary>
    public static readonly DependencyProperty BackgroundCheckedPressedProperty = DependencyProperty.RegisterAttached(
        "BackgroundCheckedPressed",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the BackgroundCheckedPressed attached property.</summary>
    public static Brush? GetBackgroundCheckedPressed(DependencyObject d)
        => (Brush?)d.GetValue(BackgroundCheckedPressedProperty);

    /// <summary>Sets the value of the BackgroundCheckedPressed attached property.</summary>
    public static void SetBackgroundCheckedPressed(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BackgroundCheckedPressedProperty, value);

    /// <summary>Identifies the BorderBrushCheckedPressed attached property.</summary>
    public static readonly DependencyProperty BorderBrushCheckedPressedProperty = DependencyProperty.RegisterAttached(
        "BorderBrushCheckedPressed",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the BorderBrushCheckedPressed attached property.</summary>
    public static Brush? GetBorderBrushCheckedPressed(DependencyObject d)
        => (Brush?)d.GetValue(BorderBrushCheckedPressedProperty);

    /// <summary>Sets the value of the BorderBrushCheckedPressed attached property.</summary>
    public static void SetBorderBrushCheckedPressed(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BorderBrushCheckedPressedProperty, value);

    /// <summary>Identifies the CheckBackgroundFillCheckedPressed attached property.</summary>
    public static readonly DependencyProperty CheckBackgroundFillCheckedPressedProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundFillCheckedPressed",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckBackgroundFillCheckedPressed attached property.</summary>
    public static Brush? GetCheckBackgroundFillCheckedPressed(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundFillCheckedPressedProperty);

    /// <summary>Sets the value of the CheckBackgroundFillCheckedPressed attached property.</summary>
    public static void SetCheckBackgroundFillCheckedPressed(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundFillCheckedPressedProperty, value);

    /// <summary>Identifies the CheckBackgroundStrokeCheckedPressed attached property.</summary>
    public static readonly DependencyProperty CheckBackgroundStrokeCheckedPressedProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundStrokeCheckedPressed",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckBackgroundStrokeCheckedPressed attached property.</summary>
    public static Brush? GetCheckBackgroundStrokeCheckedPressed(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundStrokeCheckedPressedProperty);

    /// <summary>Sets the value of the CheckBackgroundStrokeCheckedPressed attached property.</summary>
    public static void SetCheckBackgroundStrokeCheckedPressed(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundStrokeCheckedPressedProperty, value);

    /// <summary>Identifies the CheckGlyphForegroundCheckedPressed attached property.</summary>
    public static readonly DependencyProperty CheckGlyphForegroundCheckedPressedProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphForegroundCheckedPressed",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckGlyphForegroundCheckedPressed attached property.</summary>
    public static Brush? GetCheckGlyphForegroundCheckedPressed(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckGlyphForegroundCheckedPressedProperty);

    /// <summary>Sets the value of the CheckGlyphForegroundCheckedPressed attached property.</summary>
    public static void SetCheckGlyphForegroundCheckedPressed(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckGlyphForegroundCheckedPressedProperty, value);

    /// <summary>Identifies the ForegroundCheckedDisabled attached property.</summary>
    public static readonly DependencyProperty ForegroundCheckedDisabledProperty = DependencyProperty.RegisterAttached(
        "ForegroundCheckedDisabled",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the ForegroundCheckedDisabled attached property.</summary>
    public static Brush? GetForegroundCheckedDisabled(DependencyObject d)
        => (Brush?)d.GetValue(ForegroundCheckedDisabledProperty);

    /// <summary>Sets the value of the ForegroundCheckedDisabled attached property.</summary>
    public static void SetForegroundCheckedDisabled(
        DependencyObject d,
        Brush? value)
        => d.SetValue(ForegroundCheckedDisabledProperty, value);

    /// <summary>Identifies the BackgroundCheckedDisabled attached property.</summary>
    public static readonly DependencyProperty BackgroundCheckedDisabledProperty = DependencyProperty.RegisterAttached(
        "BackgroundCheckedDisabled",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the BackgroundCheckedDisabled attached property.</summary>
    public static Brush? GetBackgroundCheckedDisabled(DependencyObject d)
        => (Brush?)d.GetValue(BackgroundCheckedDisabledProperty);

    /// <summary>Sets the value of the BackgroundCheckedDisabled attached property.</summary>
    public static void SetBackgroundCheckedDisabled(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BackgroundCheckedDisabledProperty, value);

    /// <summary>Identifies the BorderBrushCheckedDisabled attached property.</summary>
    public static readonly DependencyProperty BorderBrushCheckedDisabledProperty = DependencyProperty.RegisterAttached(
        "BorderBrushCheckedDisabled",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the BorderBrushCheckedDisabled attached property.</summary>
    public static Brush? GetBorderBrushCheckedDisabled(DependencyObject d)
        => (Brush?)d.GetValue(BorderBrushCheckedDisabledProperty);

    /// <summary>Sets the value of the BorderBrushCheckedDisabled attached property.</summary>
    public static void SetBorderBrushCheckedDisabled(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BorderBrushCheckedDisabledProperty, value);

    /// <summary>Identifies the CheckBackgroundFillCheckedDisabled attached property.</summary>
    public static readonly DependencyProperty CheckBackgroundFillCheckedDisabledProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundFillCheckedDisabled",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckBackgroundFillCheckedDisabled attached property.</summary>
    public static Brush? GetCheckBackgroundFillCheckedDisabled(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundFillCheckedDisabledProperty);

    /// <summary>Sets the value of the CheckBackgroundFillCheckedDisabled attached property.</summary>
    public static void SetCheckBackgroundFillCheckedDisabled(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundFillCheckedDisabledProperty, value);

    /// <summary>Identifies the CheckBackgroundStrokeCheckedDisabled attached property.</summary>
    public static readonly DependencyProperty CheckBackgroundStrokeCheckedDisabledProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundStrokeCheckedDisabled",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckBackgroundStrokeCheckedDisabled attached property.</summary>
    public static Brush? GetCheckBackgroundStrokeCheckedDisabled(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundStrokeCheckedDisabledProperty);

    /// <summary>Sets the value of the CheckBackgroundStrokeCheckedDisabled attached property.</summary>
    public static void SetCheckBackgroundStrokeCheckedDisabled(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundStrokeCheckedDisabledProperty, value);

    /// <summary>Identifies the CheckGlyphForegroundCheckedDisabled attached property.</summary>
    public static readonly DependencyProperty CheckGlyphForegroundCheckedDisabledProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphForegroundCheckedDisabled",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckGlyphForegroundCheckedDisabled attached property.</summary>
    public static Brush? GetCheckGlyphForegroundCheckedDisabled(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckGlyphForegroundCheckedDisabledProperty);

    /// <summary>Sets the value of the CheckGlyphForegroundCheckedDisabled attached property.</summary>
    public static void SetCheckGlyphForegroundCheckedDisabled(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckGlyphForegroundCheckedDisabledProperty, value);

    /// <summary>Identifies the CheckGlyphIndeterminate attached property.</summary>
    public static readonly DependencyProperty CheckGlyphIndeterminateProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphIndeterminate",
        typeof(object),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(propertyChangedCallback: null));

    /// <summary>Gets the value of the CheckGlyphIndeterminate attached property.</summary>
    public static object? GetCheckGlyphIndeterminate(DependencyObject d)
        => d.GetValue(CheckGlyphIndeterminateProperty);

    /// <summary>Sets the value of the CheckGlyphIndeterminate attached property.</summary>
    public static void SetCheckGlyphIndeterminate(
        DependencyObject d,
        object? value)
        => d.SetValue(CheckGlyphIndeterminateProperty, value);

    /// <summary>Identifies the CheckGlyphIndeterminateTemplate attached property.</summary>
    public static readonly DependencyProperty CheckGlyphIndeterminateTemplateProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphIndeterminateTemplate",
        typeof(DataTemplate),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(DataTemplate)));

    /// <summary>Gets the value of the CheckGlyphIndeterminateTemplate attached property.</summary>
    public static DataTemplate? GetCheckGlyphIndeterminateTemplate(
        DependencyObject d)
        => (DataTemplate?)d.GetValue(CheckGlyphIndeterminateTemplateProperty);

    /// <summary>Sets the value of the CheckGlyphIndeterminateTemplate attached property.</summary>
    public static void SetCheckGlyphIndeterminateTemplate(
        DependencyObject d,
        DataTemplate? value)
        => d.SetValue(CheckGlyphIndeterminateTemplateProperty, value);

    /// <summary>Identifies the ForegroundIndeterminate attached property.</summary>
    public static readonly DependencyProperty ForegroundIndeterminateProperty = DependencyProperty.RegisterAttached(
        "ForegroundIndeterminate",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the ForegroundIndeterminate attached property.</summary>
    public static Brush? GetForegroundIndeterminate(DependencyObject d)
        => (Brush?)d.GetValue(ForegroundIndeterminateProperty);

    /// <summary>Sets the value of the ForegroundIndeterminate attached property.</summary>
    public static void SetForegroundIndeterminate(
        DependencyObject d,
        Brush? value)
        => d.SetValue(ForegroundIndeterminateProperty, value);

    /// <summary>Identifies the BackgroundIndeterminate attached property.</summary>
    public static readonly DependencyProperty BackgroundIndeterminateProperty = DependencyProperty.RegisterAttached(
        "BackgroundIndeterminate",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the BackgroundIndeterminate attached property.</summary>
    public static Brush? GetBackgroundIndeterminate(DependencyObject d)
        => (Brush?)d.GetValue(BackgroundIndeterminateProperty);

    /// <summary>Sets the value of the BackgroundIndeterminate attached property.</summary>
    public static void SetBackgroundIndeterminate(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BackgroundIndeterminateProperty, value);

    /// <summary>Identifies the BorderBrushIndeterminate attached property.</summary>
    public static readonly DependencyProperty BorderBrushIndeterminateProperty = DependencyProperty.RegisterAttached(
        "BorderBrushIndeterminate",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the BorderBrushIndeterminate attached property.</summary>
    public static Brush? GetBorderBrushIndeterminate(DependencyObject d)
        => (Brush?)d.GetValue(BorderBrushIndeterminateProperty);

    /// <summary>Sets the value of the BorderBrushIndeterminate attached property.</summary>
    public static void SetBorderBrushIndeterminate(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BorderBrushIndeterminateProperty, value);

    /// <summary>Identifies the CheckBackgroundFillIndeterminate attached property.</summary>
    public static readonly DependencyProperty CheckBackgroundFillIndeterminateProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundFillIndeterminate",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckBackgroundFillIndeterminate attached property.</summary>
    public static Brush? GetCheckBackgroundFillIndeterminate(DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundFillIndeterminateProperty);

    /// <summary>Sets the value of the CheckBackgroundFillIndeterminate attached property.</summary>
    public static void SetCheckBackgroundFillIndeterminate(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundFillIndeterminateProperty, value);

    /// <summary>Identifies the CheckBackgroundStrokeIndeterminate attached property.</summary>
    public static readonly DependencyProperty CheckBackgroundStrokeIndeterminateProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundStrokeIndeterminate",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckBackgroundStrokeIndeterminate attached property.</summary>
    public static Brush? GetCheckBackgroundStrokeIndeterminate(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundStrokeIndeterminateProperty);

    /// <summary>Sets the value of the CheckBackgroundStrokeIndeterminate attached property.</summary>
    public static void SetCheckBackgroundStrokeIndeterminate(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundStrokeIndeterminateProperty, value);

    /// <summary>Identifies the CheckGlyphForegroundIndeterminate attached property.</summary>
    public static readonly DependencyProperty CheckGlyphForegroundIndeterminateProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphForegroundIndeterminate",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckGlyphForegroundIndeterminate attached property.</summary>
    public static Brush? GetCheckGlyphForegroundIndeterminate(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckGlyphForegroundIndeterminateProperty);

    /// <summary>Sets the value of the CheckGlyphForegroundIndeterminate attached property.</summary>
    public static void SetCheckGlyphForegroundIndeterminate(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckGlyphForegroundIndeterminateProperty, value);

    /// <summary>Identifies the ForegroundIndeterminateMouseOver attached property.</summary>
    public static readonly DependencyProperty ForegroundIndeterminateMouseOverProperty = DependencyProperty.RegisterAttached(
        "ForegroundIndeterminateMouseOver",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the ForegroundIndeterminateMouseOver attached property.</summary>
    public static Brush? GetForegroundIndeterminateMouseOver(DependencyObject d)
        => (Brush?)d.GetValue(ForegroundIndeterminateMouseOverProperty);

    /// <summary>Sets the value of the ForegroundIndeterminateMouseOver attached property.</summary>
    public static void SetForegroundIndeterminateMouseOver(
        DependencyObject d,
        Brush? value)
        => d.SetValue(ForegroundIndeterminateMouseOverProperty, value);

    /// <summary>Identifies the BackgroundIndeterminateMouseOver attached property.</summary>
    public static readonly DependencyProperty BackgroundIndeterminateMouseOverProperty = DependencyProperty.RegisterAttached(
        "BackgroundIndeterminateMouseOver",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the BackgroundIndeterminateMouseOver attached property.</summary>
    public static Brush? GetBackgroundIndeterminateMouseOver(DependencyObject d)
        => (Brush?)d.GetValue(BackgroundIndeterminateMouseOverProperty);

    /// <summary>Sets the value of the BackgroundIndeterminateMouseOver attached property.</summary>
    public static void SetBackgroundIndeterminateMouseOver(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BackgroundIndeterminateMouseOverProperty, value);

    /// <summary>Identifies the BorderBrushIndeterminateMouseOver attached property.</summary>
    public static readonly DependencyProperty BorderBrushIndeterminateMouseOverProperty = DependencyProperty.RegisterAttached(
        "BorderBrushIndeterminateMouseOver",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the BorderBrushIndeterminateMouseOver attached property.</summary>
    public static Brush? GetBorderBrushIndeterminateMouseOver(
        DependencyObject d)
        => (Brush?)d.GetValue(BorderBrushIndeterminateMouseOverProperty);

    /// <summary>Sets the value of the BorderBrushIndeterminateMouseOver attached property.</summary>
    public static void SetBorderBrushIndeterminateMouseOver(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BorderBrushIndeterminateMouseOverProperty, value);

    /// <summary>Identifies the CheckBackgroundFillIndeterminateMouseOver attached property.</summary>
    public static readonly DependencyProperty CheckBackgroundFillIndeterminateMouseOverProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundFillIndeterminateMouseOver",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckBackgroundFillIndeterminateMouseOver attached property.</summary>
    public static Brush? GetCheckBackgroundFillIndeterminateMouseOver(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundFillIndeterminateMouseOverProperty);

    /// <summary>Sets the value of the CheckBackgroundFillIndeterminateMouseOver attached property.</summary>
    public static void SetCheckBackgroundFillIndeterminateMouseOver(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundFillIndeterminateMouseOverProperty, value);

    /// <summary>Identifies the CheckBackgroundStrokeIndeterminateMouseOver attached property.</summary>
    public static readonly DependencyProperty CheckBackgroundStrokeIndeterminateMouseOverProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundStrokeIndeterminateMouseOver",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckBackgroundStrokeIndeterminateMouseOver attached property.</summary>
    public static Brush? GetCheckBackgroundStrokeIndeterminateMouseOver(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundStrokeIndeterminateMouseOverProperty);

    /// <summary>Sets the value of the CheckBackgroundStrokeIndeterminateMouseOver attached property.</summary>
    public static void SetCheckBackgroundStrokeIndeterminateMouseOver(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundStrokeIndeterminateMouseOverProperty, value);

    /// <summary>Identifies the CheckGlyphForegroundIndeterminateMouseOver attached property.</summary>
    public static readonly DependencyProperty CheckGlyphForegroundIndeterminateMouseOverProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphForegroundIndeterminateMouseOver",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckGlyphForegroundIndeterminateMouseOver attached property.</summary>
    public static Brush? GetCheckGlyphForegroundIndeterminateMouseOver(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckGlyphForegroundIndeterminateMouseOverProperty);

    /// <summary>Sets the value of the CheckGlyphForegroundIndeterminateMouseOver attached property.</summary>
    public static void SetCheckGlyphForegroundIndeterminateMouseOver(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckGlyphForegroundIndeterminateMouseOverProperty, value);

    /// <summary>Identifies the ForegroundIndeterminatePressed attached property.</summary>
    public static readonly DependencyProperty ForegroundIndeterminatePressedProperty = DependencyProperty.RegisterAttached(
        "ForegroundIndeterminatePressed",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the ForegroundIndeterminatePressed attached property.</summary>
    public static Brush? GetForegroundIndeterminatePressed(DependencyObject d)
        => (Brush?)d.GetValue(ForegroundIndeterminatePressedProperty);

    /// <summary>Sets the value of the ForegroundIndeterminatePressed attached property.</summary>
    public static void SetForegroundIndeterminatePressed(
        DependencyObject d,
        Brush? value)
        => d.SetValue(ForegroundIndeterminatePressedProperty, value);

    /// <summary>Identifies the BackgroundIndeterminatePressed attached property.</summary>
    public static readonly DependencyProperty BackgroundIndeterminatePressedProperty = DependencyProperty.RegisterAttached(
        "BackgroundIndeterminatePressed",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the BackgroundIndeterminatePressed attached property.</summary>
    public static Brush? GetBackgroundIndeterminatePressed(DependencyObject d)
        => (Brush?)d.GetValue(BackgroundIndeterminatePressedProperty);

    /// <summary>Sets the value of the BackgroundIndeterminatePressed attached property.</summary>
    public static void SetBackgroundIndeterminatePressed(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BackgroundIndeterminatePressedProperty, value);

    /// <summary>Identifies the BorderBrushIndeterminatePressed attached property.</summary>
    public static readonly DependencyProperty BorderBrushIndeterminatePressedProperty = DependencyProperty.RegisterAttached(
        "BorderBrushIndeterminatePressed",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the BorderBrushIndeterminatePressed attached property.</summary>
    public static Brush? GetBorderBrushIndeterminatePressed(DependencyObject d)
        => (Brush?)d.GetValue(BorderBrushIndeterminatePressedProperty);

    /// <summary>Sets the value of the BorderBrushIndeterminatePressed attached property.</summary>
    public static void SetBorderBrushIndeterminatePressed(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BorderBrushIndeterminatePressedProperty, value);

    /// <summary>Identifies the CheckBackgroundFillIndeterminatePressed attached property.</summary>
    public static readonly DependencyProperty CheckBackgroundFillIndeterminatePressedProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundFillIndeterminatePressed",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckBackgroundFillIndeterminatePressed attached property.</summary>
    public static Brush? GetCheckBackgroundFillIndeterminatePressed(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundFillIndeterminatePressedProperty);

    /// <summary>Sets the value of the CheckBackgroundFillIndeterminatePressed attached property.</summary>
    public static void SetCheckBackgroundFillIndeterminatePressed(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundFillIndeterminatePressedProperty, value);

    /// <summary>Identifies the CheckBackgroundStrokeIndeterminatePressed attached property.</summary>
    public static readonly DependencyProperty CheckBackgroundStrokeIndeterminatePressedProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundStrokeIndeterminatePressed",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckBackgroundStrokeIndeterminatePressed attached property.</summary>
    public static Brush? GetCheckBackgroundStrokeIndeterminatePressed(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundStrokeIndeterminatePressedProperty);

    /// <summary>Sets the value of the CheckBackgroundStrokeIndeterminatePressed attached property.</summary>
    public static void SetCheckBackgroundStrokeIndeterminatePressed(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundStrokeIndeterminatePressedProperty, value);

    /// <summary>Identifies the CheckGlyphForegroundIndeterminatePressed attached property.</summary>
    public static readonly DependencyProperty CheckGlyphForegroundIndeterminatePressedProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphForegroundIndeterminatePressed",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckGlyphForegroundIndeterminatePressed attached property.</summary>
    public static Brush? GetCheckGlyphForegroundIndeterminatePressed(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckGlyphForegroundIndeterminatePressedProperty);

    /// <summary>Sets the value of the CheckGlyphForegroundIndeterminatePressed attached property.</summary>
    public static void SetCheckGlyphForegroundIndeterminatePressed(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckGlyphForegroundIndeterminatePressedProperty, value);

    /// <summary>Identifies the ForegroundIndeterminateDisabled attached property.</summary>
    public static readonly DependencyProperty ForegroundIndeterminateDisabledProperty = DependencyProperty.RegisterAttached(
        "ForegroundIndeterminateDisabled",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the ForegroundIndeterminateDisabled attached property.</summary>
    public static Brush? GetForegroundIndeterminateDisabled(DependencyObject d)
        => (Brush?)d.GetValue(ForegroundIndeterminateDisabledProperty);

    /// <summary>Sets the value of the ForegroundIndeterminateDisabled attached property.</summary>
    public static void SetForegroundIndeterminateDisabled(
        DependencyObject d,
        Brush? value)
        => d.SetValue(ForegroundIndeterminateDisabledProperty, value);

    /// <summary>Identifies the BackgroundIndeterminateDisabled attached property.</summary>
    public static readonly DependencyProperty BackgroundIndeterminateDisabledProperty = DependencyProperty.RegisterAttached(
        "BackgroundIndeterminateDisabled",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the BackgroundIndeterminateDisabled attached property.</summary>
    public static Brush? GetBackgroundIndeterminateDisabled(DependencyObject d)
        => (Brush?)d.GetValue(BackgroundIndeterminateDisabledProperty);

    /// <summary>Sets the value of the BackgroundIndeterminateDisabled attached property.</summary>
    public static void SetBackgroundIndeterminateDisabled(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BackgroundIndeterminateDisabledProperty, value);

    /// <summary>Identifies the BorderBrushIndeterminateDisabled attached property.</summary>
    public static readonly DependencyProperty BorderBrushIndeterminateDisabledProperty = DependencyProperty.RegisterAttached(
        "BorderBrushIndeterminateDisabled",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the BorderBrushIndeterminateDisabled attached property.</summary>
    public static Brush? GetBorderBrushIndeterminateDisabled(DependencyObject d)
        => (Brush?)d.GetValue(BorderBrushIndeterminateDisabledProperty);

    /// <summary>Sets the value of the BorderBrushIndeterminateDisabled attached property.</summary>
    public static void SetBorderBrushIndeterminateDisabled(
        DependencyObject d,
        Brush? value)
        => d.SetValue(BorderBrushIndeterminateDisabledProperty, value);

    /// <summary>Identifies the CheckBackgroundFillIndeterminateDisabled attached property.</summary>
    public static readonly DependencyProperty CheckBackgroundFillIndeterminateDisabledProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundFillIndeterminateDisabled",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckBackgroundFillIndeterminateDisabled attached property.</summary>
    public static Brush? GetCheckBackgroundFillIndeterminateDisabled(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundFillIndeterminateDisabledProperty);

    /// <summary>Sets the value of the CheckBackgroundFillIndeterminateDisabled attached property.</summary>
    public static void SetCheckBackgroundFillIndeterminateDisabled(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundFillIndeterminateDisabledProperty, value);

    /// <summary>Identifies the CheckBackgroundStrokeIndeterminateDisabled attached property.</summary>
    public static readonly DependencyProperty CheckBackgroundStrokeIndeterminateDisabledProperty = DependencyProperty.RegisterAttached(
        "CheckBackgroundStrokeIndeterminateDisabled",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckBackgroundStrokeIndeterminateDisabled attached property.</summary>
    public static Brush? GetCheckBackgroundStrokeIndeterminateDisabled(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckBackgroundStrokeIndeterminateDisabledProperty);

    /// <summary>Sets the value of the CheckBackgroundStrokeIndeterminateDisabled attached property.</summary>
    public static void SetCheckBackgroundStrokeIndeterminateDisabled(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckBackgroundStrokeIndeterminateDisabledProperty, value);

    /// <summary>Identifies the CheckGlyphForegroundIndeterminateDisabled attached property.</summary>
    public static readonly DependencyProperty CheckGlyphForegroundIndeterminateDisabledProperty = DependencyProperty.RegisterAttached(
        "CheckGlyphForegroundIndeterminateDisabled",
        typeof(Brush),
        typeof(CheckBoxHelper),
        new FrameworkPropertyMetadata(default(Brush)));

    /// <summary>Gets the value of the CheckGlyphForegroundIndeterminateDisabled attached property.</summary>
    public static Brush? GetCheckGlyphForegroundIndeterminateDisabled(
        DependencyObject d)
        => (Brush?)d.GetValue(CheckGlyphForegroundIndeterminateDisabledProperty);

    /// <summary>Sets the value of the CheckGlyphForegroundIndeterminateDisabled attached property.</summary>
    public static void SetCheckGlyphForegroundIndeterminateDisabled(
        DependencyObject d,
        Brush? value)
        => d.SetValue(CheckGlyphForegroundIndeterminateDisabledProperty, value);
}
#pragma warning disable IDE0130
namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// The per-row state of a <see cref="MultiColumnComboBox"/> pop-up grid, set on each realized
/// <see cref="DataGridRow"/> (the rows show the caller's own objects, so the state cannot live on the items).
/// </summary>
internal static class MultiColumnRowState
{
    /// <summary>
    /// Whether the row's item is ticked.
    /// </summary>
    public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.RegisterAttached(
        "IsChecked",
        typeof(bool),
        typeof(MultiColumnRowState),
        new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Whether the row's item can be ticked or unticked (not disabled and not blocked by the limit).
    /// </summary>
    public static readonly DependencyProperty CanToggleProperty = DependencyProperty.RegisterAttached(
        "CanToggle",
        typeof(bool),
        typeof(MultiColumnRowState),
        new PropertyMetadata(BooleanBoxes.TrueBox));

    /// <summary>
    /// The display value of the row's item (the automation name of its check box).
    /// </summary>
    public static readonly DependencyProperty DisplayTextProperty = DependencyProperty.RegisterAttached(
        "DisplayText",
        typeof(string),
        typeof(MultiColumnRowState),
        new PropertyMetadata(string.Empty));

    public static bool GetIsChecked(DependencyObject element)
        => (bool)element.GetValue(IsCheckedProperty);

    public static void SetIsChecked(
        DependencyObject element,
        bool value)
        => element.SetValue(IsCheckedProperty, BooleanBoxes.Box(value));

    public static bool GetCanToggle(DependencyObject element)
        => (bool)element.GetValue(CanToggleProperty);

    public static void SetCanToggle(
        DependencyObject element,
        bool value)
        => element.SetValue(CanToggleProperty, BooleanBoxes.Box(value));

    public static string GetDisplayText(DependencyObject element)
        => (string)element.GetValue(DisplayTextProperty);

    public static void SetDisplayText(
        DependencyObject element,
        string value)
        => element.SetValue(DisplayTextProperty, value);
}
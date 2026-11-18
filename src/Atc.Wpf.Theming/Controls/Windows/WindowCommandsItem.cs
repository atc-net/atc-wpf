// ReSharper disable InconsistentNaming
namespace Atc.Wpf.Theming.Controls.Windows;

/// <summary>
/// The item container used by <see cref="WindowCommands"/>, with an optional trailing separator.
/// </summary>
[TemplatePart(Name = PART_ContentPresenter, Type = typeof(UIElement))]
[TemplatePart(Name = PART_Separator, Type = typeof(UIElement))]
[DefaultStyleKey]
public sealed partial class WindowCommandsItem : ContentControl
{
    private const string PART_ContentPresenter = "PART_ContentPresenter";
    private const string PART_Separator = "PART_Separator";

    /// <summary>
    /// Gets or sets the value indicating whether to show the separator.
    /// </summary>
    [DependencyProperty(
        DefaultValue = true,
        Flags = FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender)]
    private bool isSeparatorVisible;

    [DependencyProperty(
        IsReadOnly = true,
        GenerateClrAccessors = false)]
    private WindowCommands? parentWindowCommands;

    internal PropertyChangeNotifier? VisibilityPropertyChangeNotifier { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="WindowCommands"/> that owns this item.
    /// </summary>
    public WindowCommands? ParentWindowCommands
    {
        get => (WindowCommands?)GetValue(ParentWindowCommandsProperty);
        set => SetValue(ParentWindowCommandsPropertyKey, value);
    }

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        var windowCommands = ItemsControl.ItemsControlFromItemContainer(this) as WindowCommands;
        SetValue(ParentWindowCommandsPropertyKey, windowCommands);
    }
}
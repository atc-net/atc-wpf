// ReSharper disable InconsistentNaming
namespace Atc.Wpf.Theming.Controls.Windows;

[TemplatePart(Name = PART_ContentPresenter, Type = typeof(UIElement))]
[TemplatePart(Name = PART_Separator, Type = typeof(UIElement))]
public class WindowCommandsItem : ContentControl
{
    private const string PART_ContentPresenter = "PART_ContentPresenter";
    private const string PART_Separator = "PART_Separator";

    internal PropertyChangeNotifier? VisibilityPropertyChangeNotifier { get; set; }

    public static readonly DependencyProperty IsSeparatorVisibleProperty = DependencyProperty.Register(
        nameof(IsSeparatorVisible),
        typeof(bool),
        typeof(WindowCommandsItem),
        new FrameworkPropertyMetadata(
            BooleanBoxes.TrueBox,
            FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// Gets or sets the value indicating whether to show the separator.
    /// </summary>
    public bool IsSeparatorVisible
    {
        get => (bool)GetValue(IsSeparatorVisibleProperty);
        set => SetValue(IsSeparatorVisibleProperty, BooleanBoxes.Box(value));
    }

    private static readonly DependencyPropertyKey ParentWindowCommandsPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(ParentWindowCommands),
        typeof(WindowCommands),
        typeof(WindowCommandsItem),
        new PropertyMetadata(null));

    public static readonly DependencyProperty ParentWindowCommandsProperty = ParentWindowCommandsPropertyKey.DependencyProperty;

    public WindowCommands? ParentWindowCommands
    {
        get => (WindowCommands?)GetValue(ParentWindowCommandsProperty);
        protected set => SetValue(ParentWindowCommandsPropertyKey, value);
    }

    static WindowCommandsItem()
        => DefaultStyleKeyProperty.OverrideMetadata(
            typeof(WindowCommandsItem),
            new FrameworkPropertyMetadata(typeof(WindowCommandsItem)));

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        var windowCommands = ItemsControl.ItemsControlFromItemContainer(this) as WindowCommands;
        SetValue(ParentWindowCommandsPropertyKey, windowCommands);
    }
}
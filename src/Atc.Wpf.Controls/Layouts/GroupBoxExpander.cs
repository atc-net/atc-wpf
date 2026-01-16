namespace Atc.Wpf.Controls.Layouts;

[ContentProperty(nameof(Content))]
public sealed partial class GroupBoxExpander : HeaderedContentControl
{
    [DependencyProperty(
        DefaultValue = true,
        PropertyChangedCallback = nameof(OnIsExpandedChanged),
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal)]
    private bool isExpanded;

    [DependencyProperty(DefaultValue = default(ExpanderButtonLocation))]
    private ExpanderButtonLocation expanderButtonLocation;

    [DependencyProperty]
    private CornerRadius cornerRadius;

    [DependencyProperty]
    private Brush? headerBackground;

    [DependencyProperty]
    private Brush? headerForeground;

    [DependencyProperty(DefaultValue = "new Thickness(4)")]
    private Thickness headerPadding;

    public event RoutedEventHandler? Expanded;

    public event RoutedEventHandler? Collapsed;

    static GroupBoxExpander()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(GroupBoxExpander),
            new FrameworkPropertyMetadata(typeof(GroupBoxExpander)));
    }

    private void OnExpanded()
        => Expanded?.Invoke(this, new RoutedEventArgs());

    private void OnCollapsed()
        => Collapsed?.Invoke(this, new RoutedEventArgs());

    private static void OnIsExpandedChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not GroupBoxExpander groupBoxExpander ||
            e.NewValue is not bool isExpanded)
        {
            return;
        }

        if (isExpanded)
        {
            groupBoxExpander.OnExpanded();
        }
        else
        {
            groupBoxExpander.OnCollapsed();
        }
    }
}
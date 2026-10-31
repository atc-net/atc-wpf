namespace Atc.Wpf.Controls.Layouts;

/// <summary>
/// A pane of a <see cref="DockRegion"/>. Several panes in one region are shown as tabs; a pane can be
/// floated into its own tool window, docked back and closed (and restored) through its <see cref="DockPanelPro"/>.
/// </summary>
public sealed partial class DockPane : HeaderedContentControl
{
    private static readonly DependencyPropertyKey StatePropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(State),
        typeof(DockPaneState),
        typeof(DockPane),
        new FrameworkPropertyMetadata(DockPaneState.Docked));

    /// <summary>
    /// Identifies the <see cref="State"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty StateProperty = StatePropertyKey.DependencyProperty;

    /// <summary>
    /// Gets or sets a unique identifier for this pane, used by <see cref="DockPanelPro.FindPane"/> and layout persistence.
    /// </summary>
    [DependencyProperty]
    private string? paneId;

    /// <summary>
    /// Gets or sets a value indicating whether the pane can be closed (tab close button, header close button, Close command).
    /// </summary>
    [DependencyProperty(DefaultValue = true, PropertyChangedCallback = nameof(OnCapabilityChanged))]
    private bool canClose;

    /// <summary>
    /// Gets or sets a value indicating whether the pane can be floated into its own tool window.
    /// Floating also requires <see cref="DockPanelPro.AllowFloating"/>.
    /// </summary>
    [DependencyProperty(DefaultValue = true, PropertyChangedCallback = nameof(OnCapabilityChanged))]
    private bool canFloat;

    static DockPane()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(DockPane),
            new FrameworkPropertyMetadata(typeof(DockPane)));
        FocusableProperty.OverrideMetadata(
            typeof(DockPane),
            new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));
    }

    /// <summary>
    /// Gets where the pane is shown: docked as a tab, floating in a tool window, or closed.
    /// </summary>
    public DockPaneState State
    {
        get => (DockPaneState)GetValue(StateProperty);
        private set => SetValue(StatePropertyKey, value);
    }

    /// <summary>
    /// Gets a value indicating whether the pane is shown in a floating tool window.
    /// </summary>
    public bool IsFloating => State == DockPaneState.Floating;

    /// <summary>
    /// Gets the region the pane belongs to. A floating or closed pane keeps its region and position,
    /// so it returns there when it is docked again.
    /// </summary>
    public DockRegion? Region { get; internal set; }

    /// <summary>
    /// Gets or sets the bounds (in device-independent units) of the floating tool window. <see cref="Rect.Empty"/>
    /// (the default) places the window over the pane's docked position. The value follows the window while it floats.
    /// </summary>
    public Rect FloatingBounds { get; set; } = Rect.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether a floating window currently hosts the pane.
    /// </summary>
    internal bool IsHostedInWindow { get; set; }

    /// <summary>
    /// Gets the header as plain text, used for window titles and automation names.
    /// </summary>
    internal string HeaderText
        => Header switch
        {
            null => PaneId ?? string.Empty,
            string text => text,
            HeaderedContentControl headered => headered.Header?.ToString() ?? string.Empty,
            ContentControl content => content.Content?.ToString() ?? string.Empty,
            TextBlock textBlock => textBlock.Text,
            _ => Header.ToString() ?? string.Empty,
        };

    internal void SetState(DockPaneState value)
        => State = value;

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer()
        => new DockPaneAutomationPeer(this);

    private static void OnCapabilityChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => CommandManager.InvalidateRequerySuggested();
}
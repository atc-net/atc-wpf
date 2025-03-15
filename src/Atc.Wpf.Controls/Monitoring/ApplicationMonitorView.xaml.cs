namespace Atc.Wpf.Controls.Monitoring;

public partial class ApplicationMonitorView
{
    public ApplicationMonitorView()
    {
        InitializeComponent();

        Messenger.Default.Register<ApplicationMonitorScrollEvent>(this, OnApplicationMonitorScrollEvent);
    }

    public static readonly DependencyProperty ShowToolbarProperty = DependencyProperty.Register(
        nameof(ShowToolbar),
        typeof(bool),
        typeof(ApplicationMonitorView),
        new PropertyMetadata(defaultValue: false));

    public bool ShowToolbar
    {
        get => (bool)GetValue(ShowToolbarProperty);
        set => SetValue(ShowToolbarProperty, value);
    }

    public static readonly DependencyProperty ShowClearInToolbarProperty = DependencyProperty.Register(
        nameof(ShowClearInToolbar),
        typeof(bool),
        typeof(ApplicationMonitorView),
        new PropertyMetadata(defaultValue: false));

    public bool ShowClearInToolbar
    {
        get => (bool)GetValue(ShowClearInToolbarProperty);
        set => SetValue(ShowClearInToolbarProperty, value);
    }

    public static readonly DependencyProperty ShowAutoScrollInToolbarProperty = DependencyProperty.Register(
        nameof(ShowAutoScrollInToolbar),
        typeof(bool),
        typeof(ApplicationMonitorView),
        new PropertyMetadata(defaultValue: false));

    public bool ShowAutoScrollInToolbar
    {
        get => (bool)GetValue(ShowAutoScrollInToolbarProperty);
        set => SetValue(ShowAutoScrollInToolbarProperty, value);
    }

    public static readonly DependencyProperty ShowSearchInToolbarProperty = DependencyProperty.Register(
        nameof(ShowSearchInToolbar),
        typeof(bool),
        typeof(ApplicationMonitorView),
        new PropertyMetadata(defaultValue: false));

    public bool ShowSearchInToolbar
    {
        get => (bool)GetValue(ShowSearchInToolbarProperty);
        set => SetValue(ShowSearchInToolbarProperty, value);
    }

    private void OnApplicationMonitorScrollEvent(
        ApplicationMonitorScrollEvent obj)
    {
        _ = Dispatcher.CurrentDispatcher.BeginInvokeIfRequired(() =>
        {
            if (LvEntries.Items.Count <= 0)
            {
                return;
            }

            LvEntries.ScrollIntoView(
                obj.Direction == ListSortDirection.Ascending
                    ? LvEntries.Items[^1]!
                    : LvEntries.Items[0]!);
        });
    }
}
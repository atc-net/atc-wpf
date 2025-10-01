namespace Atc.Wpf.Controls.Monitoring;

public partial class ApplicationMonitorView
{
    [DependencyProperty(DefaultValue = true)]
    private bool showToolbar;

    [DependencyProperty(DefaultValue = true)]
    private bool showClearInToolbar;

    [DependencyProperty(DefaultValue = true)]
    private bool showAutoScrollInToolbar;

    [DependencyProperty(DefaultValue = true)]
    private bool showSearchInToolbar;

    public ApplicationMonitorView()
    {
        InitializeComponent();

        Messenger.Default.Register<ApplicationMonitorScrollEvent>(this, OnApplicationMonitorScrollEvent);
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
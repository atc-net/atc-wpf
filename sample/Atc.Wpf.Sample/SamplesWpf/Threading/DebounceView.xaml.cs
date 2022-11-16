namespace Atc.Wpf.Sample.SamplesWpf.Threading;

/// <summary>
/// Interaction logic for DebounceView.
/// </summary>
public partial class DebounceView
{
    private readonly DebounceDispatcher debounceTimer = new();

    public DebounceView()
    {
        InitializeComponent();
    }

    [SuppressMessage("AsyncUsage", "AsyncFixer03:Fire-and-forget async-void methods or delegates", Justification = "OK.")]
    private void SearchTextBoxOnKeyup(
        object sender,
        KeyEventArgs e)
    {
        var selectedComboBoxItem = CbDebounce.SelectedValue as ComboBoxItem;

        var s = selectedComboBoxItem?.Content?.ToString();
        if (s is null)
        {
            return;
        }

        var delayMs = int.Parse(s, NumberStyles.Any, GlobalizationConstants.EnglishCultureInfo);

        // Fire after [delayMs]ms after last keypress
        debounceTimer.Debounce(delayMs, async _ =>
        {
            var vm = DataContext as DebounceViewModel;
            if (LcSearch.Content is TextBox searchBox)
            {
                await vm!.Search(searchBox.Text);
            }
        });
    }
}
namespace Atc.Wpf.Sample.SamplesWpf.Threading;

public partial class DebounceView
{
    private readonly DebounceDispatcher debounceTimer = new();

    public DebounceView()
    {
        InitializeComponent();
    }

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

        var delayMs = NumberHelper.ParseToInt(s);

        // Fire after [delayMs]ms after last keypress
        debounceTimer.Debounce(delayMs, _ => ExecuteSearch());
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK")]
    [SuppressMessage("Usage", "MA0134:Observe result of async calls", Justification = "OK")]
    private void ExecuteSearch()
    {
        if (DataContext is not DebounceViewModel vm)
        {
            return;
        }

        if (LcSearch.Content is TextBox searchBox)
        {
            Task.Run(async () =>
            {
                try
                {
                    await vm.Search(searchBox.Text).ConfigureAwait(false);
                }
                catch
                {
                    // Swallow exception
                }
            });
        }
    }
}
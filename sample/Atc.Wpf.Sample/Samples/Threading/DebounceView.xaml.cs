namespace Atc.Wpf.Sample.Samples.Threading;

/// <summary>
/// Interaction logic for DebounceView.
/// </summary>
public partial class DebounceView
{
    private readonly DebounceDispatcher debounceTimer = new ();

    public DebounceView()
    {
        this.InitializeComponent();
    }

    [SuppressMessage("AsyncUsage", "AsyncFixer03:Fire-and-forget async-void methods or delegates", Justification = "OK.")]
    private void SearchTextBoxOnKeyup(object sender, KeyEventArgs e)
    {
        var selectedComboBoxItem = this.CbDebounce.SelectedValue as ComboBoxItem;
        if (selectedComboBoxItem?.Content is null)
        {
            return;
        }

        var s = selectedComboBoxItem.Content.ToString();
        if (s is null)
        {
            return;
        }

        var delayMs = int.Parse(s, NumberStyles.Any, GlobalizationConstants.EnglishCultureInfo);

        // Fire after [delayMs]ms after last keypress
        debounceTimer.Debounce(delayMs, async _ =>
        {
            var vm = this.DataContext as DebounceViewModel;
            await vm!.Search(this.TbSearch.Text);
        });
    }
}
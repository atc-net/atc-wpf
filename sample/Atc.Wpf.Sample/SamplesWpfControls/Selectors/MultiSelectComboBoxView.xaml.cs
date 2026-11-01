namespace Atc.Wpf.Sample.SamplesWpfControls.Selectors;

public partial class MultiSelectComboBoxView
{
    public MultiSelectComboBoxView()
    {
        InitializeComponent();
        DataContext = new MultiSelectComboBoxDemoViewModel();

        InteractiveBox.SelectedKeysChanged += (_, e) => Log(SelectedKeysLog, e);
        ChipsBox.SelectedKeysChanged += (_, e) => Log(ChipsLog, e);
        DeferredBox.SelectedKeysChanged += (_, e) => Log(DeferredLog, e);
    }

    private static void Log(
        TextBlock log,
        ValueChangedEventArgs<IReadOnlyList<string>> e)
    {
        var line = string.Create(
            CultureInfo.InvariantCulture,
            $"SelectedKeysChanged: [{string.Join(", ", e.OldValue)}] -> [{string.Join(", ", e.NewValue)}]");
        log.Text = log.Text.Length == 0
            ? line
            : log.Text + Environment.NewLine + line;
    }
}
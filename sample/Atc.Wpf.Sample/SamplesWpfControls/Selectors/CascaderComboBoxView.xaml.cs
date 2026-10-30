namespace Atc.Wpf.Sample.SamplesWpfControls.Selectors;

public partial class CascaderComboBoxView
{
    public CascaderComboBoxView()
    {
        InitializeComponent();
        DataContext = new CascaderComboBoxDemoViewModel();

        InteractiveBox.SelectedKeysChanged += (_, e) => Log(SelectedKeysLog, e);
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
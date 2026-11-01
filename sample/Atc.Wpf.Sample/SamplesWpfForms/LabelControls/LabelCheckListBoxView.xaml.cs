namespace Atc.Wpf.Sample.SamplesWpfForms.LabelControls;

public partial class LabelCheckListBoxView
{
    public LabelCheckListBoxView()
    {
        InitializeComponent();
        DataContext = new LabelControlDemoViewModel();

        InteractiveLabelCheckListBox.SelectedKeysChanged += (_, e) => Log("SelectedKeysChanged", e.NewValue);
        InteractiveLabelCheckListBox.SelectedKeysChangedInvalid += (_, e) => Log("SelectedKeysChangedInvalid", e.NewValue);
    }

    private void Log(
        string eventName,
        IReadOnlyList<string> keys)
    {
        var line = string.Create(CultureInfo.InvariantCulture, $"{eventName}: [{string.Join(", ", keys)}]");
        EventLog.Text = EventLog.Text.Length == 0
            ? line
            : EventLog.Text + Environment.NewLine + line;
    }
}
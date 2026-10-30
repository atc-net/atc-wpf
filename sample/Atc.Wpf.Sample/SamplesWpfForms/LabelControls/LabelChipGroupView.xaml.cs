namespace Atc.Wpf.Sample.SamplesWpfForms.LabelControls;

public partial class LabelChipGroupView
{
    public LabelChipGroupView()
    {
        InitializeComponent();
        DataContext = new LabelControlDemoViewModel();

        InteractiveLabelChipGroup.SelectedKeysChanged += (_, e) => Log("SelectedKeysChanged", e.NewValue);
        InteractiveLabelChipGroup.SelectedKeysChangedInvalid += (_, e) => Log("SelectedKeysChangedInvalid", e.NewValue);
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
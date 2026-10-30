namespace Atc.Wpf.Sample.SamplesWpfForms.LabelControls;

public partial class LabelTreeSelectComboBoxView
{
    public LabelTreeSelectComboBoxView()
    {
        InitializeComponent();
        DataContext = new LabelControlDemoViewModel();

        // Every control gets its own nodes: a node keeps the check state of the one tree it is in.
        InteractiveLabelTreeSelectComboBox.Nodes = SamplesWpfControls.Selectors.CheckTreeSampleData.CreateRegions();
        RegionsBox.Nodes = SamplesWpfControls.Selectors.CheckTreeSampleData.CreateRegions();
        MandatoryPermissionsBox.Nodes = SamplesWpfControls.Selectors.CheckTreeSampleData.CreatePermissions();
        ChipsBox.Nodes = SamplesWpfControls.Selectors.CheckTreeSampleData.CreateRegions();
        ChipsBox.SelectedKeys = ["dk", "de-berlin"];
        VerticalBox.Nodes = SamplesWpfControls.Selectors.CheckTreeSampleData.CreateRegions();
        LimitBox.Nodes = SamplesWpfControls.Selectors.CheckTreeSampleData.CreateRegions();
        InlineTreeLabel.Content = new CheckTreeView
        {
            Height = 220,
            CheckedKeysStrategy = CheckedKeysStrategy.Parents,
            Nodes = SamplesWpfControls.Selectors.CheckTreeSampleData.CreatePermissions(),
        };

        InteractiveLabelTreeSelectComboBox.SelectedKeysChanged += (_, e) => Log("SelectedKeysChanged", e.NewValue);
        InteractiveLabelTreeSelectComboBox.SelectedKeysChangedInvalid += (_, e) => Log("SelectedKeysChangedInvalid", e.NewValue);
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
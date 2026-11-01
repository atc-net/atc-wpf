namespace Atc.Wpf.Sample.SamplesWpfForms.LabelControls;

public partial class LabelCascaderComboBoxView
{
    public LabelCascaderComboBoxView()
    {
        InitializeComponent();
        DataContext = new LabelControlDemoViewModel();

        // Every control gets its own nodes: a node keeps the check state of the one tree it is in.
        InteractiveLabelCascaderComboBox.Nodes = SamplesWpfControls.Selectors.CascaderSampleData.CreateRegions();
        RegionsBox.Nodes = SamplesWpfControls.Selectors.CascaderSampleData.CreateRegions();
        MandatoryBox.Nodes = SamplesWpfControls.Selectors.CascaderSampleData.CreateRegions();
        ChipsBox.Nodes = SamplesWpfControls.Selectors.CascaderSampleData.CreateRegions();
        ChipsBox.SelectedKeys = ["dk-jut-aar", "se-sve"];
        VerticalBox.Nodes = SamplesWpfControls.Selectors.CascaderSampleData.CreateProducts();
        LimitBox.Nodes = SamplesWpfControls.Selectors.CascaderSampleData.CreateRegions();

        InteractiveLabelCascaderComboBox.SelectedKeysChanged += (_, e) => Log("SelectedKeysChanged", e.NewValue);
        InteractiveLabelCascaderComboBox.SelectedKeysChangedInvalid += (_, e) => Log("SelectedKeysChangedInvalid", e.NewValue);
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
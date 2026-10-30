namespace Atc.Wpf.Sample.SamplesWpfForms.LabelControls;

public partial class LabelMultiColumnComboBoxView
{
    public LabelMultiColumnComboBoxView()
    {
        InitializeComponent();
        DataContext = new LabelControlDemoViewModel();

        var customers = SamplesWpfControls.Selectors.MultiColumnSampleData.CreateCustomers();
        InteractiveLabelMultiColumnComboBox.ItemsSource = customers;
        CustomersBox.ItemsSource = customers;
        MandatoryBox.ItemsSource = customers;
        ChipsBox.ItemsSource = customers;
        ChipsBox.SelectedKeys = ["1001", "1004"];
        LimitBox.ItemsSource = customers;

        InteractiveLabelMultiColumnComboBox.SelectedKeysChanged += (_, e) => Log("SelectedKeysChanged", e.NewValue);
        InteractiveLabelMultiColumnComboBox.SelectedKeysChangedInvalid += (_, e) => Log("SelectedKeysChangedInvalid", e.NewValue);
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
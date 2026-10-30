namespace Atc.Wpf.Sample.SamplesWpfControls.Selectors;

[SuppressMessage("Design", "MA0016:Prefer using collection abstraction instead of implementation", Justification = "Bound to the MultiColumnComboBox dependency properties of these types.")]
public partial class MultiColumnComboBoxDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Commit Mode", "Behavior", 1)]
    [ObservableProperty]
    private MultiSelectCommitMode commitMode = MultiSelectCommitMode.Immediate;

    [PropertyDisplay("Keep Selected On Top", "Behavior", 2)]
    [ObservableProperty]
    private bool keepSelectedOnTop;

    [PropertyDisplay("Is Enabled", "Behavior", 3)]
    [ObservableProperty]
    private bool isControlEnabled = true;

    [PropertyDisplay("Display Mode", "Layout", 1)]
    [ObservableProperty]
    private MultiSelectDisplayMode displayMode = MultiSelectDisplayMode.Chips;

    [PropertyDisplay("Is Filter Visible", "Layout", 2)]
    [ObservableProperty]
    private bool isFilterVisible = true;

    public MultiColumnComboBoxDemoViewModel()
    {
        Customers = MultiColumnSampleData.CreateCustomers();
        CustomerKeys = [];
        Parts = MultiColumnSampleData.CreateParts(10_000);
        PartKeys = ["P-00042"];
        DeferredCustomers = MultiColumnSampleData.CreateCustomers();
        DeferredCustomerKeys = ["1002", "1005"];
        Users = MultiColumnSampleData.CreateUsers();
        UserKeys = ["cla"];
        LimitCustomers = MultiColumnSampleData.CreateCustomers();
        LimitCustomerKeys = [];
    }

    public IReadOnlyList<SampleCustomer> Customers { get; }

    public ObservableCollection<string> CustomerKeys { get; set; }

    public IReadOnlyList<SamplePart> Parts { get; }

    public ObservableCollection<string> PartKeys { get; set; }

    public IReadOnlyList<SampleCustomer> DeferredCustomers { get; }

    public ObservableCollection<string> DeferredCustomerKeys { get; set; }

    public IReadOnlyList<SampleUser> Users { get; }

    public ObservableCollection<string> UserKeys { get; set; }

    public IReadOnlyList<SampleCustomer> LimitCustomers { get; }

    public ObservableCollection<string> LimitCustomerKeys { get; set; }
}
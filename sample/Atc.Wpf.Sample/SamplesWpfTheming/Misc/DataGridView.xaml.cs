// ReSharper disable StringLiteralTypo
namespace Atc.Wpf.Sample.SamplesWpfTheming.Misc;

public partial class DataGridView
{
    public DataGridView()
    {
        InitializeComponent();

        DataContext = this;

        PopulateSampleData();

        DoubleClickCommand = new RelayCommand(OnDoubleClick);
    }

    public ObservableCollectionEx<Person> Items { get; set; } = [];

    public Person? SelectedItem { get; set; }

    public ICommand DoubleClickCommand { get; }

    private void PopulateSampleData()
    {
        Items.Add(
            new Person(
                "John",
                "Doe",
                42,
                Colors.DarkCyan,
                new Address(
                    "123 Main St",
                    "Anytown, Califonia",
                    "CA 12345",
                    new CultureInfo("en-US"))));
        Items.Add(
            new Person(
                "Juan",
                "Doe",
                38,
                Colors.DarkCyan,
                new Address(
                    "123 Main St",
                    "Anytown, Califonia",
                    "CA 12345",
                    new CultureInfo("en-US"))));
        Items.Add(
            new Person(
                "Karren",
                "Koe",
                21,
                Colors.DarkCyan,
                new Address(
                    "My Street 21",
                    "Anytown, New York",
                    "NY 1111",
                    new CultureInfo("en-US"))));
        Items.Add(
            new Person(
                "Noelle",
                "Barr",
                1,
                Colors.DarkCyan,
                new Address(
                    "My Street 1",
                    "Unknown",
                    "Abc123",
                    new CultureInfo("en-US"))));
        Items.Add(
            new Person(
                "Batman",
                "Forever",
                1,
                Colors.DarkCyan,
                new Address(
                    "My Street 1",
                    "Unknown",
                    "Abc123",
                    new CultureInfo("en-US"))));
    }

    private void OnDoubleClick()
    {
        if (SelectedItem is null)
        {
            return;
        }

        _ = MessageBox.Show(
            $"Double-clicked on: {SelectedItem.FirstName} {SelectedItem.LastName}",
            "Double Click",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }
}
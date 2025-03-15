// ReSharper disable StringLiteralTypo
namespace Atc.Wpf.Sample.SamplesWpfTheming.Misc;

public partial class ListViewView
{
    public ListViewView()
    {
        InitializeComponent();

        DataContext = this;

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

    public ObservableCollectionEx<Person> Items { get; set; } = new();

    public ObservableCollection<Person> SelectedItems { get; set; } = new();
}
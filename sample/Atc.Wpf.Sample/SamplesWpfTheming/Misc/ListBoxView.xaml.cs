// ReSharper disable StringLiteralTypo
namespace Atc.Wpf.Sample.SamplesWpfTheming.Misc;

/// <summary>
/// Interaction logic for ListBoxView.
/// </summary>
public partial class ListBoxView
{
    public ListBoxView()
    {
        InitializeComponent();

        DataContext = this;

        Items.Add(
            new Person(
                "John",
                "Doe",
                42,
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
                new Address(
                    "My Street 1",
                    "Unknown",
                    "Abc123",
                    new CultureInfo("en-US"))));
    }

    public ObservableCollectionEx<Person> Items { get; set; } = new();
}
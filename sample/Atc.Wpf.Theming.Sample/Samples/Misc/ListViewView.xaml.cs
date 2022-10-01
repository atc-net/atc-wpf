// ReSharper disable StringLiteralTypo
namespace Atc.Wpf.Theming.Sample.Samples.Misc;

/// <summary>
/// Interaction logic for ListViewView.
/// </summary>
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
                "Karren ",
                "Koe",
                21,
                new Address(
                    "My Street 21",
                    "Anytown, New York",
                    "NY 1111",
                    new CultureInfo("en-US"))));
    }

    public ObservableCollectionEx<Person> Items { get; set; } = new();

    public ObservableCollection<Person> SelectedItems { get; set; } = new();
}
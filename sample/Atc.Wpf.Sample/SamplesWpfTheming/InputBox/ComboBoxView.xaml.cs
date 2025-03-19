namespace Atc.Wpf.Sample.SamplesWpfTheming.InputBox;

public partial class ComboBoxView
{
    public ComboBoxView()
    {
        InitializeComponent();

        DataContext = this;

        Items = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "Key1", "Value1" },
            { "Key2", "Value2" },
            { "Key3", "Value3" },
        };

        Items1000 = new Dictionary<string, string>(StringComparer.Ordinal);
        for (var i = 0; i < 1000; i++)
        {
            Items1000.Add($"Item{i}", $"Item{i}");
        }
    }

    public IDictionary<string, string> Items { get; set; }

    public IDictionary<string, string> Items1000 { get; set; }

    public string SelectedKey { get; set; } = string.Empty;
}
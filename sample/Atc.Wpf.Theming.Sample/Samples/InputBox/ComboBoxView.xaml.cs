namespace Atc.Wpf.Theming.Sample.Samples.InputBox;

/// <summary>
/// Interaction logic for ComboBoxView.
/// </summary>
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
    }

    public IDictionary<string, string> Items { get; set; }

    public string SelectedKey { get; set; } = string.Empty;
}
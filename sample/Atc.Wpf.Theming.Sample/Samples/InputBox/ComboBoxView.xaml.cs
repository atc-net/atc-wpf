namespace Atc.Wpf.Theming.Sample.Samples.InputBox;

/// <summary>
/// Interaction logic for ComboBoxView.
/// </summary>
[SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "OK.")]
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
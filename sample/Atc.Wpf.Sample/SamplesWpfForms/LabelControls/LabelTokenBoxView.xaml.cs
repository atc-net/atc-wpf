namespace Atc.Wpf.Sample.SamplesWpfForms.LabelControls;

public partial class LabelTokenBoxView
{
    public LabelTokenBoxView()
    {
        InitializeComponent();
        DataContext = new LabelControlDemoViewModel();

        InteractiveLabelTokenBox.SelectedKeysChanged += (_, e) => Log("SelectedKeysChanged", e.NewValue);
        InteractiveLabelTokenBox.SelectedKeysChangedInvalid += (_, e) => Log("SelectedKeysChangedInvalid", e.NewValue);
    }

    /// <summary>
    /// Gets a new set of known tags; each field gets its own, as created tags are added to it.
    /// </summary>
    [SuppressMessage("Design", "MA0016:Prefer using collection abstraction instead of implementation", Justification = "Bound to the Items dependency property of this type.")]
    public static Dictionary<string, string> KnownTags
        => new(StringComparer.Ordinal)
        {
            ["urgent"] = "urgent",
            ["bug"] = "bug",
            ["feature"] = "feature",
            ["documentation"] = "documentation",
        };

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
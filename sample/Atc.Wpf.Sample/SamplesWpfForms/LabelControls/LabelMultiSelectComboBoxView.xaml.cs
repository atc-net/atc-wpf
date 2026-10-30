namespace Atc.Wpf.Sample.SamplesWpfForms.LabelControls;

public partial class LabelMultiSelectComboBoxView
{
    /// <summary>
    /// Gets the group of each <see cref="DayOfWeek"/> key: weekdays and weekend.
    /// </summary>
    [SuppressMessage("Design", "MA0016:Prefer using collection abstraction instead of implementation", Justification = "Bound to the ItemGroups dependency property of this type.")]
    public static Dictionary<string, string> WeekdayGroups { get; } = Enum.GetNames<DayOfWeek>()
        .ToDictionary(
            x => x,
            x => x is nameof(DayOfWeek.Saturday) or nameof(DayOfWeek.Sunday) ? "Weekend" : "Weekdays",
            StringComparer.Ordinal);

    public LabelMultiSelectComboBoxView()
    {
        InitializeComponent();
        DataContext = new LabelControlDemoViewModel();

        InteractiveLabelMultiSelectComboBox.SelectedKeysChanged += (_, e) => Log("SelectedKeysChanged", e.NewValue);
        InteractiveLabelMultiSelectComboBox.SelectedKeysChangedInvalid += (_, e) => Log("SelectedKeysChangedInvalid", e.NewValue);
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
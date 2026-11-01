namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

public partial class ChipGroupView
{
    public ChipGroupView()
    {
        InitializeComponent();
        DataContext = new ChipGroupDemoViewModel();

        LogLevelGroup.ItemIcons = new Dictionary<string, object>(StringComparer.Ordinal)
        {
            ["error"] = CreateIcon(""),
            ["warning"] = CreateIcon(""),
            ["info"] = CreateIcon(""),
            ["debug"] = CreateIcon(""),
        };

        InteractiveBox.SelectedKeysChanged += (_, e) =>
        {
            var line = string.Create(
                CultureInfo.InvariantCulture,
                $"SelectedKeysChanged: [{string.Join(", ", e.OldValue)}] -> [{string.Join(", ", e.NewValue)}]");
            SelectedKeysLog.Text = SelectedKeysLog.Text.Length == 0
                ? line
                : SelectedKeysLog.Text + Environment.NewLine + line;
        };
    }

    private static TextBlock CreateIcon(string glyph)
        => new()
        {
            Text = glyph,
            FontFamily = new FontFamily("Segoe MDL2 Assets"),
            VerticalAlignment = VerticalAlignment.Center,
        };
}
namespace Atc.Wpf.Sample.SamplesWpfControls.Selectors;

public partial class CheckListBoxView
{
    public CheckListBoxView()
    {
        InitializeComponent();
        DataContext = new CheckListBoxDemoViewModel();

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
}
namespace Atc.Wpf.Sample.SamplesWpfControls.Layouts;

public partial class DockPanelProView
{
    private string? savedLayout;

    public DockPanelProView()
    {
        InitializeComponent();
        MainDockPanel.AddHandler(
            DockRegion.ActivePaneChangedEvent,
            new RoutedPropertyChangedEventHandler<DockPane?>(MainDockPanel_ActivePaneChanged));
    }

    private void SaveLayoutButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        savedLayout = MainDockPanel.SaveLayout();
        Log("Layout saved");
    }

    private void LoadLayoutButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(savedLayout))
        {
            Log("No layout saved yet");
            return;
        }

        MainDockPanel.LoadLayout(savedLayout);
        Log("Layout loaded");
    }

    private void ResetLayoutButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        MainDockPanel.ResetLayout();
        Log("Layout reset");
    }

    private void RestoreClosedButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        foreach (var pane in MainDockPanel.GetPanes().Where(x => x.State == DockPaneState.Closed))
        {
            MainDockPanel.RestorePane(pane);
        }
    }

    private void RedockAllButton_Click(
        object sender,
        RoutedEventArgs e)
        => MainDockPanel.RedockAllPanes();

    private void MainDockPanel_PaneStateChanged(
        object? sender,
        DockPaneStateChangedEventArgs e)
        => Log($"{e.Pane.Header}: {e.OldState} -> {e.NewState}");

    private void MainDockPanel_ActivePaneChanged(
        object sender,
        RoutedPropertyChangedEventArgs<DockPane?> e)
    {
        if (e.NewValue is not null)
        {
            Log($"Active: {e.NewValue.Header}");
        }
    }

    private void Log(string message)
    {
        var lines = (DockLog.Text + Environment.NewLine + message)
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .TakeLast(20);
        DockLog.Text = string.Join(Environment.NewLine, lines);
    }
}
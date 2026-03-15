namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

public partial class AlertDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Severity", "Appearance", 1)]
    [ObservableProperty]
    private AlertSeverity severity = AlertSeverity.Info;

    [PropertyDisplay("Variant", "Appearance", 2)]
    [ObservableProperty]
    private AlertVariant variant = AlertVariant.Filled;

    [PropertyDisplay("Is Dismissible", "Behavior", 1)]
    [ObservableProperty]
    private bool isDismissible;

    [PropertyDisplay("Is Dense", "Behavior", 2)]
    [ObservableProperty]
    private bool isDense;

    [PropertyDisplay("Is Pulsing", "Animation", 1)]
    [ObservableProperty]
    private bool isPulsing;

    [PropertyDisplay("Pulse Duration (s)", "Animation", 2)]
    [PropertyRange(0.3, 5.0, 0.1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double pulseDurationSeconds = 1.5;

    [PropertyDisplay("Pulse Opacity", "Animation", 3)]
    [PropertyRange(0.1, 1.0, 0.05)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double pulseOpacity = 0.65;

    [PropertyDisplay("Show Icon", "Appearance", 3)]
    [ObservableProperty]
    private bool showIcon = true;

    [PropertyDisplay("Content", "Content", 1)]
    [ObservableProperty]
    private string content = "This is an alert message.";

    [PropertyDisplay("Title", "Content", 2)]
    [ObservableProperty]
    private string title = "Alert";

    public TimeSpan PulseDuration => TimeSpan.FromSeconds(PulseDurationSeconds);
}
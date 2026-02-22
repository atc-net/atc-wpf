namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

public class AlertDemoViewModel : ViewModelBase
{
    private AlertSeverity severity = AlertSeverity.Info;
    private AlertVariant variant = AlertVariant.Filled;
    private bool isDismissible;
    private bool isDense;
    private bool isPulsing;
    private double pulseDurationSeconds = 1.5;
    private double pulseOpacity = 0.65;
    private bool showIcon = true;
    private string content = "This is an alert message.";
    private string title = "Alert";

    [PropertyDisplay("Severity", "Appearance", 1)]
    public AlertSeverity Severity
    {
        get => severity;
        set => Set(ref severity, value);
    }

    [PropertyDisplay("Variant", "Appearance", 2)]
    public AlertVariant Variant
    {
        get => variant;
        set => Set(ref variant, value);
    }

    [PropertyDisplay("Is Dismissible", "Behavior", 1)]
    public bool IsDismissible
    {
        get => isDismissible;
        set => Set(ref isDismissible, value);
    }

    [PropertyDisplay("Is Dense", "Behavior", 2)]
    public bool IsDense
    {
        get => isDense;
        set => Set(ref isDense, value);
    }

    [PropertyDisplay("Is Pulsing", "Animation", 1)]
    public bool IsPulsing
    {
        get => isPulsing;
        set => Set(ref isPulsing, value);
    }

    [PropertyDisplay("Pulse Duration (s)", "Animation", 2)]
    [PropertyRange(0.3, 5.0, 0.1)]
    [PropertyEditorHint(EditorHint.Slider)]
    public double PulseDurationSeconds
    {
        get => pulseDurationSeconds;
        set => Set(ref pulseDurationSeconds, value);
    }

    [PropertyDisplay("Pulse Opacity", "Animation", 3)]
    [PropertyRange(0.1, 1.0, 0.05)]
    [PropertyEditorHint(EditorHint.Slider)]
    public double PulseOpacity
    {
        get => pulseOpacity;
        set => Set(ref pulseOpacity, value);
    }

    [PropertyDisplay("Show Icon", "Appearance", 3)]
    public bool ShowIcon
    {
        get => showIcon;
        set => Set(ref showIcon, value);
    }

    [PropertyDisplay("Content", "Content", 1)]
    public string Content
    {
        get => content;
        set => Set(ref content, value);
    }

    [PropertyDisplay("Title", "Content", 2)]
    public string Title
    {
        get => title;
        set => Set(ref title, value);
    }

    public TimeSpan PulseDuration => TimeSpan.FromSeconds(PulseDurationSeconds);
}
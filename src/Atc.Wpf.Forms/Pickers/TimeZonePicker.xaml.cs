// ReSharper disable UnusedParameter.Local
namespace Atc.Wpf.Forms.Pickers;

[SuppressMessage("Naming", "CA1721:Property names should not match get methods", Justification = "OK.")]
public partial class TimeZonePicker
{
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<TimeZoneInfo?>))]
    private static readonly RoutedEvent valueChanged;

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value),
        typeof(TimeZoneInfo),
        typeof(TimeZonePicker),
        new FrameworkPropertyMetadata(
            defaultValue: null,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            OnValuePropertyChanged));

    public TimeZoneInfo? Value
    {
        get => (TimeZoneInfo?)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    [DependencyProperty(DefaultValue = "")]
    private string watermarkText;

    public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
        nameof(ItemTemplate),
        typeof(DataTemplate),
        typeof(TimeZonePicker),
        new PropertyMetadata(defaultValue: null, OnItemTemplateChanged));

    public DataTemplate? ItemTemplate
    {
        get => (DataTemplate?)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    public static readonly DependencyProperty ResolvedItemTemplateProperty = DependencyProperty.Register(
        nameof(ResolvedItemTemplate),
        typeof(DataTemplate),
        typeof(TimeZonePicker),
        new PropertyMetadata(defaultValue: null));

    public DataTemplate? ResolvedItemTemplate
    {
        get => (DataTemplate?)GetValue(ResolvedItemTemplateProperty);
        private set => SetValue(ResolvedItemTemplateProperty, value);
    }

    public TimeZonePicker()
    {
        InitializeComponent();

        TimeZones = TimeZoneInfo.GetSystemTimeZones();
        ApplyResolvedItemTemplate();
    }

    public IReadOnlyCollection<TimeZoneInfo> TimeZones { get; }

    protected override AutomationPeer OnCreateAutomationPeer()
        => new TimeZonePickerAutomationPeer(this);

    private static void OnItemTemplateChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is TimeZonePicker picker)
        {
            picker.ApplyResolvedItemTemplate();
        }
    }

    private void ApplyResolvedItemTemplate()
        => ResolvedItemTemplate = ItemTemplate ?? (DataTemplate)Resources["DefaultItemTemplate"];

    private static void OnValuePropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue == e.NewValue)
        {
            return;
        }

        ((TimeZonePicker)d).RaiseEvent(
            new RoutedPropertyChangedEventArgs<TimeZoneInfo?>(
                (TimeZoneInfo?)e.OldValue,
                (TimeZoneInfo?)e.NewValue,
                ValueChangedEvent));
    }
}
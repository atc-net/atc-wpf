// ReSharper disable UnusedParameter.Local
namespace Atc.Wpf.Forms.Pickers;

[SuppressMessage("Naming", "CA1721:Property names should not match get methods", Justification = "OK.")]
public partial class TimeZonePicker
{
    /// <summary>
    /// Occurs when the selected time zone changes.
    /// </summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<TimeZoneInfo?>))]
    private static readonly RoutedEvent valueChanged;

    /// <summary>
    /// Gets or sets the selected time zone; binds two-way by default.
    /// </summary>
    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnValuePropertyChanged))]
    private TimeZoneInfo? value;

    [DependencyProperty(DefaultValue = "")]
    private string watermarkText;

    /// <summary>
    /// Gets or sets a custom template for the time zone items; when <see langword="null"/>, the built-in template is used.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnItemTemplateChanged))]
    private DataTemplate? itemTemplate;

    /// <summary>
    /// Gets the item template in use: <see cref="ItemTemplate"/> if set, otherwise the built-in template.
    /// </summary>
    [DependencyProperty(SetterAccessModifier = PropertyAccessModifier.Private)]
    private DataTemplate? resolvedItemTemplate;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeZonePicker"/> class.
    /// </summary>
    public TimeZonePicker()
    {
        InitializeComponent();

        TimeZones = TimeZoneInfo.GetSystemTimeZones();
        ApplyResolvedItemTemplate();
    }

    /// <summary>
    /// Gets the time zones available on the local system.
    /// </summary>
    public IReadOnlyCollection<TimeZoneInfo> TimeZones { get; }

    /// <inheritdoc />
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
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
    /// Identifies the <see cref="Value"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value),
        typeof(TimeZoneInfo),
        typeof(TimeZonePicker),
        new FrameworkPropertyMetadata(
            defaultValue: null,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            OnValuePropertyChanged));

    /// <summary>
    /// Gets or sets the selected time zone; binds two-way by default.
    /// </summary>
    public TimeZoneInfo? Value
    {
        get => (TimeZoneInfo?)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    [DependencyProperty(DefaultValue = "")]
    private string watermarkText;

    /// <summary>
    /// Identifies the <see cref="ItemTemplate"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
        nameof(ItemTemplate),
        typeof(DataTemplate),
        typeof(TimeZonePicker),
        new PropertyMetadata(defaultValue: null, OnItemTemplateChanged));

    /// <summary>
    /// Gets or sets a custom template for the time zone items; when <see langword="null"/>, the built-in template is used.
    /// </summary>
    public DataTemplate? ItemTemplate
    {
        get => (DataTemplate?)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="ResolvedItemTemplate"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ResolvedItemTemplateProperty = DependencyProperty.Register(
        nameof(ResolvedItemTemplate),
        typeof(DataTemplate),
        typeof(TimeZonePicker),
        new PropertyMetadata(defaultValue: null));

    /// <summary>
    /// Gets the item template in use: <see cref="ItemTemplate"/> if set, otherwise the built-in template.
    /// </summary>
    public DataTemplate? ResolvedItemTemplate
    {
        get => (DataTemplate?)GetValue(ResolvedItemTemplateProperty);
        private set => SetValue(ResolvedItemTemplateProperty, value);
    }

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
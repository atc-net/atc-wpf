namespace Atc.Wpf.Controls.BaseControls;

/// <summary>
/// Interaction logic for ClockPanelPicker.
/// </summary>
public partial class ClockPanelPicker : INotifyPropertyChanged
{
    private IDictionary<string, string> hours = new Dictionary<string, string>(StringComparer.Ordinal);
    private IDictionary<string, string> minutes = new Dictionary<string, string>(StringComparer.Ordinal);
    private string? selectedKeyHour;
    private string? selectedKeyMinute;

    public event PropertyChangedEventHandler? PropertyChanged;

    public static readonly RoutedEvent SelectedClockChangedEvent = EventManager.RegisterRoutedEvent(
        nameof(SelectedClockChanged),
        RoutingStrategy.Direct,
        typeof(EventHandler<RoutedEventArgs>),
        typeof(ClockPanelPicker));

    public event EventHandler<RoutedEventArgs> SelectedClockChanged
    {
        add => AddHandler(SelectedClockChangedEvent, value);
        remove => RemoveHandler(SelectedClockChangedEvent, value);
    }

    public static readonly DependencyProperty SelectedDateTimeProperty = DependencyProperty.Register(
        nameof(SelectedDateTime),
        typeof(DateTime?),
        typeof(ClockPanelPicker),
        new PropertyMetadata(default(DateTime?)));

    public DateTime? SelectedDateTime
    {
        get => (DateTime?)GetValue(SelectedDateTimeProperty);
        set => SetValue(SelectedDateTimeProperty, value);
    }

    public ClockPanelPicker()
    {
        InitializeComponent();
        DataContext = this;

        for (var i = 0; i < 24; i++)
        {
            Hours.Add(i.ToString(GlobalizationConstants.EnglishCultureInfo), i.ToString(GlobalizationConstants.EnglishCultureInfo));
        }

        for (var i = 0; i < 60; i++)
        {
            Minutes.Add(i.ToString(GlobalizationConstants.EnglishCultureInfo), i.ToString(GlobalizationConstants.EnglishCultureInfo));
        }

        SelectedKeyHour = "0";
        SelectedKeyMinute = "0";
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public IDictionary<string, string> Hours
    {
        get => hours;
        set
        {
            hours = value;
            OnPropertyChanged();
        }
    }

    public IDictionary<string, string> Minutes
    {
        get => minutes;
        set
        {
            minutes = value;
            OnPropertyChanged();
        }
    }

    public string? SelectedKeyHour
    {
        get => selectedKeyHour;
        set
        {
            selectedKeyHour = value;
            OnPropertyChanged();

            SetSelectedDate();
        }
    }

    public string? SelectedKeyMinute
    {
        get => selectedKeyMinute;
        set
        {
            selectedKeyMinute = value;
            OnPropertyChanged();

            SetSelectedDate();
        }
    }

    private void SetSelectedDate()
    {
        var hour = 0;
        var minute = 0;
        if (SelectedKeyHour is not null)
        {
            hour = NumberHelper.ParseToInt(SelectedKeyHour);
        }

        if (SelectedKeyMinute is not null)
        {
            minute = NumberHelper.ParseToInt(SelectedKeyMinute);
        }

        if (SelectedDateTime is null)
        {
            var today = DateTime.Today;
            SelectedDateTime = new DateTime(today.Year, today.Month, today.Day, hour, minute, 0, DateTimeKind.Local);
        }
        else
        {
            SelectedDateTime = new DateTime(SelectedDateTime.Value.Year, SelectedDateTime.Value.Month, SelectedDateTime.Value.Day, hour, minute, 0, DateTimeKind.Local);
        }

        RaiseSelectedClockChangedEvent();
    }

    private void RaiseSelectedClockChangedEvent()
        => RaiseEvent(new RoutedEventArgs(SelectedClockChangedEvent, this));
}
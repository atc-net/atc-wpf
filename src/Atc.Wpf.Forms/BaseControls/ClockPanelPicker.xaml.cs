// ReSharper disable InconsistentNaming
namespace Atc.Wpf.Forms.BaseControls;

public partial class ClockPanelPicker : INotifyPropertyChanged
{
    private IDictionary<string, string> hours = new Dictionary<string, string>(StringComparer.Ordinal);
    private IDictionary<string, string> minutes = new Dictionary<string, string>(StringComparer.Ordinal);
    private string? selectedKeyHour;
    private string? selectedKeyMinute;

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Occurs when the selected hour or minute changes.
    /// </summary>
    [RoutedEvent(
        RoutingStrategy.Direct,
        HandlerType = typeof(EventHandler<RoutedEventArgs>))]
    private static readonly RoutedEvent selectedClockChanged;

    [DependencyProperty]
    private DateTime? selectedDateTime;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClockPanelPicker"/> class.
    /// </summary>
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

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    protected virtual void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Gets or sets the hour items (0-23) shown in the hour drop-down, keyed by value.
    /// </summary>
    public IDictionary<string, string> Hours
    {
        get => hours;
        set
        {
            hours = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the minute items (0-59) shown in the minute drop-down, keyed by value.
    /// </summary>
    public IDictionary<string, string> Minutes
    {
        get => minutes;
        set
        {
            minutes = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the key of the selected hour; setting it updates <c>SelectedDateTime</c>.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the key of the selected minute; setting it updates <c>SelectedDateTime</c>.
    /// </summary>
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
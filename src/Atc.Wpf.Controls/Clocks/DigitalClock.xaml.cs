namespace Atc.Wpf.Controls.Clocks;

/// <summary>
/// A digital clock built from <see cref="SevenSegment"/> digits that shows hours, minutes and (optionally)
/// seconds, with blinking colons and a glow. By default it follows the current local time while it is loaded;
/// turn <see cref="FollowCurrentTime"/> off to show <see cref="Time"/> instead.
/// </summary>
public partial class DigitalClock
{
    private const string DefaultNumberBrushKey = "AtcApps.Brushes.Accent";

    /// <summary>
    /// The time of day shown by the clock (wrapped into 0:00:00 - 23:59:59).
    /// Updated every second while <see cref="FollowCurrentTime"/> is on and the clock is loaded.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnTimeChanged))]
    private TimeSpan time;

    /// <summary>
    /// Whether the clock shows the current local time, updated every second while it is loaded.
    /// Turn it off to show <see cref="Time"/> (bound or set with <see cref="SetTime"/>).
    /// </summary>
    [DependencyProperty(
        DefaultValue = true,
        PropertyChangedCallback = nameof(OnFollowCurrentTimeChanged))]
    private bool followCurrentTime;

    /// <summary>
    /// The brush used for the digits (the theme accent brush by default).
    /// </summary>
    [DependencyProperty]
    private Brush? numberColor;

    /// <summary>
    /// The blur radius of the glow drawn behind the digits (0 turns the glow off).
    /// </summary>
    [DependencyProperty(DefaultValue = 5)]
    private int numberBlurRadius;

    /// <summary>
    /// The brush used for the colons (the theme accent brush by default).
    /// </summary>
    [DependencyProperty]
    private Brush? colonColor;

    /// <summary>
    /// Whether the colons fade in and out each second.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool animateColon;

    /// <summary>
    /// Whether the colons between hours, minutes and seconds are shown.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool showColon;

    /// <summary>
    /// Whether a change of digit fades the segments instead of switching them at once.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool playAnimation;

    /// <summary>
    /// Whether the seconds are shown.
    /// </summary>
    [DependencyProperty(
        DefaultValue = true,
        GeneratePartialHooks = true)]
    private bool showSeconds;

    /// <summary>
    /// Initializes a new instance of the <see cref="DigitalClock"/> class.
    /// </summary>
    public DigitalClock()
    {
        InitializeComponent();

        SetResourceReference(NumberColorProperty, DefaultNumberBrushKey);
        SetResourceReference(ColonColorProperty, DefaultNumberBrushKey);

        Ticker = new ClockTicker(Dispatcher, OnTick);
        Ticker.SetEnabled(FollowCurrentTime);

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;

        UpdateDigits();
    }

    internal ClockTicker Ticker { get; }

    internal bool IsTimerRunning => Ticker.IsRunning;

    /// <summary>
    /// Sets the time of day shown by the clock.
    /// While <see cref="FollowCurrentTime"/> is on, the next tick replaces it with the current time.
    /// </summary>
    /// <param name="timeSpan">The time of day.</param>
    public void SetTime(TimeSpan timeSpan)
        => SetCurrentValue(TimeProperty, timeSpan);

    /// <summary>
    /// Gets the colours and options of the clock as config data (for example to save them in settings).
    /// </summary>
    /// <returns>The config data.</returns>
    public DigitalClockConfigData GetConfigData()
        => new()
        {
            Colors =
            {
                Background = ClockHelper.ToHex(Background),
                Number = ClockHelper.ToHex(NumberColor),
                Colon = ClockHelper.ToHex(ColonColor),
            },
            NumberBlurRadius = NumberBlurRadius,
            AnimateColon = AnimateColon,
            PlayAnimation = PlayAnimation,
            ShowSeconds = ShowSeconds,
        };

    /// <summary>
    /// Applies colours and options from config data. An empty colour restores the theme brush.
    /// </summary>
    /// <param name="configData">The config data.</param>
    public void SetByConfigData(DigitalClockConfigData configData)
    {
        ArgumentNullException.ThrowIfNull(configData);
        if (configData.Colors is null)
        {
            throw new ArgumentException("The config data has no colors.", nameof(configData));
        }

        ClockHelper.SetBrushFromHex(this, BackgroundProperty, configData.Colors.Background, themeResourceKey: null);
        ClockHelper.SetBrushFromHex(this, NumberColorProperty, configData.Colors.Number, DefaultNumberBrushKey);
        ClockHelper.SetBrushFromHex(this, ColonColorProperty, configData.Colors.Colon, DefaultNumberBrushKey);
        SetCurrentValue(NumberBlurRadiusProperty, configData.NumberBlurRadius);
        SetCurrentValue(AnimateColonProperty, configData.AnimateColon);
        SetCurrentValue(PlayAnimationProperty, configData.PlayAnimation);
        SetCurrentValue(ShowSecondsProperty, configData.ShowSeconds);
    }

    private static void OnTimeChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var clock = (DigitalClock)d;
        clock.UpdateDigits();
        clock.HourB.PulseColon();
        clock.MinuteB.PulseColon();
    }

    partial void OnShowSecondsChanged(
        bool oldValue,
        bool newValue)
        => UpdateDigits();

    private static void OnFollowCurrentTimeChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var clock = (DigitalClock)d;
        var follow = (bool)e.NewValue;
        clock.Ticker.SetEnabled(follow);
        if (!follow)
        {
            // Show the bound time again instead of the last ticked one.
            clock.GetBindingExpression(TimeProperty)?.UpdateTarget();
        }
    }

    private static void SetDigits(
        SevenSegment tens,
        SevenSegment ones,
        int value)
    {
        tens.SetCurrentValue(SevenSegment.DigitProperty, value / 10);
        ones.SetCurrentValue(SevenSegment.DigitProperty, value % 10);
    }

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
        => Ticker.SetLoaded(value: true);

    private void OnUnloaded(
        object sender,
        RoutedEventArgs e)
        => Ticker.SetLoaded(value: false);

    private void OnTick(TimeSpan timeOfDay)
        => SetCurrentValue(TimeProperty, timeOfDay);

    private void UpdateDigits()
    {
        var timeOfDay = ClockHelper.ToTimeOfDay(Time);
        SetDigits(HourA, HourB, timeOfDay.Hours);
        SetDigits(MinuteA, MinuteB, timeOfDay.Minutes);
        SetDigits(SecondA, SecondB, timeOfDay.Seconds);
    }
}
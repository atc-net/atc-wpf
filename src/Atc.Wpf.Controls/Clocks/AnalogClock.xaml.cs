namespace Atc.Wpf.Controls.Clocks;

/// <summary>
/// An analog clock face with hour, minute and second hands, hour and minute indicators, numbers and an AM/PM
/// indicator. By default it follows the current local time while it is loaded; turn
/// <see cref="FollowCurrentTime"/> off to show <see cref="Time"/> instead.
/// </summary>
public partial class AnalogClock
{
    private const string ForegroundBrushKey = "AtcApps.Brushes.ThemeForeground";
    private const string BackgroundBrushKey = "AtcApps.Brushes.ThemeBackground";
    private const string AccentBrushKey = "AtcApps.Brushes.Accent";
    private const double FaceCenter = 500;
    private const double IndicatorTop = 40;
    private const double NumberTop = 134;
    private const double NumberHeight = 56;

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
    /// The brush of the clock face (the theme background brush by default).
    /// </summary>
    [DependencyProperty]
    private Brush? clockBackgroundColor;

    /// <summary>
    /// The brush of the ring around the clock face (the theme foreground brush by default).
    /// </summary>
    [DependencyProperty]
    private Brush? clockBorderColor;

    /// <summary>
    /// The brush of the dot in the centre of the clock (the theme foreground brush by default).
    /// </summary>
    [DependencyProperty]
    private Brush? clockCenterDotColor;

    /// <summary>
    /// The brush of the twelve hour indicators (the theme foreground brush by default).
    /// </summary>
    [DependencyProperty]
    private Brush? hourIndicatorColor;

    /// <summary>
    /// The brush of the minute indicators (the theme foreground brush by default).
    /// </summary>
    [DependencyProperty]
    private Brush? minuteIndicatorColor;

    /// <summary>
    /// The brush of the numbers 1-12 (the theme foreground brush by default).
    /// </summary>
    [DependencyProperty]
    private Brush? numberColor;

    /// <summary>
    /// The brush of the hour hand (the theme foreground brush by default).
    /// </summary>
    [DependencyProperty]
    private Brush? hourHandColor;

    /// <summary>
    /// The brush of the minute hand (the theme foreground brush by default).
    /// </summary>
    [DependencyProperty]
    private Brush? minuteHandColor;

    /// <summary>
    /// The brush of the second hand (the theme accent brush by default).
    /// </summary>
    [DependencyProperty]
    private Brush? secondHandColor;

    /// <summary>
    /// The brush of the AM/PM indicator (the theme foreground brush by default).
    /// </summary>
    [DependencyProperty]
    private Brush? amPmColor;

    /// <summary>
    /// Whether the AM/PM indicator is shown.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool showAmPm;

    /// <summary>
    /// Whether the second hand is shown.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool showSeconds;

    /// <summary>
    /// Whether the numbers 1-12 are shown.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool showNumbers;

    /// <summary>
    /// Whether the shown time is before noon. Set by the clock from <see cref="Time"/>.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool isInAm;

    /// <summary>
    /// Initializes a new instance of the <see cref="AnalogClock"/> class.
    /// </summary>
    public AnalogClock()
    {
        InitializeComponent();

        SetThemeBrushes();
        CreateIndicators();
        CreateNumbers();

        Ticker = new ClockTicker(Dispatcher, OnTick);
        Ticker.SetEnabled(FollowCurrentTime);

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;

        UpdateHands();
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
    public AnalogClockConfigData GetConfigData()
        => new()
        {
            Colors =
            {
                Background = ClockHelper.ToHex(Background),
                ClockBackground = ClockHelper.ToHex(ClockBackgroundColor),
                ClockBorder = ClockHelper.ToHex(ClockBorderColor),
                ClockCenterDot = ClockHelper.ToHex(ClockCenterDotColor),
                HourIndicator = ClockHelper.ToHex(HourIndicatorColor),
                MinuteIndicator = ClockHelper.ToHex(MinuteIndicatorColor),
                Number = ClockHelper.ToHex(NumberColor),
                HourHand = ClockHelper.ToHex(HourHandColor),
                MinuteHand = ClockHelper.ToHex(MinuteHandColor),
                SecondHand = ClockHelper.ToHex(SecondHandColor),
                AmPm = ClockHelper.ToHex(AmPmColor),
            },
            ShowAmPm = ShowAmPm,
            ShowSeconds = ShowSeconds,
            ShowNumbers = ShowNumbers,
        };

    /// <summary>
    /// Applies colours and options from config data. An empty colour restores the theme brush.
    /// </summary>
    /// <param name="configData">The config data.</param>
    public void SetByConfigData(AnalogClockConfigData configData)
    {
        ArgumentNullException.ThrowIfNull(configData);
        if (configData.Colors is null)
        {
            throw new ArgumentException("The config data has no colors.", nameof(configData));
        }

        var colors = configData.Colors;
        ClockHelper.SetBrushFromHex(this, BackgroundProperty, colors.Background, themeResourceKey: null);
        ClockHelper.SetBrushFromHex(this, ClockBackgroundColorProperty, colors.ClockBackground, BackgroundBrushKey);
        ClockHelper.SetBrushFromHex(this, ClockBorderColorProperty, colors.ClockBorder, ForegroundBrushKey);
        ClockHelper.SetBrushFromHex(this, ClockCenterDotColorProperty, colors.ClockCenterDot, ForegroundBrushKey);
        ClockHelper.SetBrushFromHex(this, HourIndicatorColorProperty, colors.HourIndicator, ForegroundBrushKey);
        ClockHelper.SetBrushFromHex(this, MinuteIndicatorColorProperty, colors.MinuteIndicator, ForegroundBrushKey);
        ClockHelper.SetBrushFromHex(this, NumberColorProperty, colors.Number, ForegroundBrushKey);
        ClockHelper.SetBrushFromHex(this, HourHandColorProperty, colors.HourHand, ForegroundBrushKey);
        ClockHelper.SetBrushFromHex(this, MinuteHandColorProperty, colors.MinuteHand, ForegroundBrushKey);
        ClockHelper.SetBrushFromHex(this, SecondHandColorProperty, colors.SecondHand, AccentBrushKey);
        ClockHelper.SetBrushFromHex(this, AmPmColorProperty, colors.AmPm, ForegroundBrushKey);
        SetCurrentValue(ShowAmPmProperty, configData.ShowAmPm);
        SetCurrentValue(ShowSecondsProperty, configData.ShowSeconds);
        SetCurrentValue(ShowNumbersProperty, configData.ShowNumbers);
    }

    /// <summary>
    /// Gets the angle (degrees clockwise from 12) of the hour hand; it moves on with the minutes.
    /// </summary>
    internal static double GetHourAngle(TimeSpan time)
    {
        var timeOfDay = ClockHelper.ToTimeOfDay(time);
        return ((timeOfDay.Hours % 12) * 30) + (timeOfDay.Minutes * 0.5);
    }

    /// <summary>
    /// Gets the angle (degrees clockwise from 12) of the minute hand; it moves on with the seconds.
    /// </summary>
    internal static double GetMinuteAngle(TimeSpan time)
    {
        var timeOfDay = ClockHelper.ToTimeOfDay(time);
        return (timeOfDay.Minutes * 6) + (timeOfDay.Seconds * 0.1);
    }

    /// <summary>
    /// Gets the angle (degrees clockwise from 12) of the second hand.
    /// </summary>
    internal static double GetSecondAngle(TimeSpan time)
        => ClockHelper.ToTimeOfDay(time).Seconds * 6;

    private static void OnTimeChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((AnalogClock)d).UpdateHands();

    private static void OnFollowCurrentTimeChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var clock = (AnalogClock)d;
        var follow = (bool)e.NewValue;
        clock.Ticker.SetEnabled(follow);
        if (!follow)
        {
            // Show the bound time again instead of the last ticked one.
            clock.GetBindingExpression(TimeProperty)?.UpdateTarget();
        }
    }

    private static string GetDesignator(
        string designator,
        string fallback)
        => string.IsNullOrWhiteSpace(designator)
            ? fallback
            : designator;

    private void SetThemeBrushes()
    {
        SetResourceReference(ClockBackgroundColorProperty, BackgroundBrushKey);
        SetResourceReference(ClockBorderColorProperty, ForegroundBrushKey);
        SetResourceReference(ClockCenterDotColorProperty, ForegroundBrushKey);
        SetResourceReference(HourIndicatorColorProperty, ForegroundBrushKey);
        SetResourceReference(MinuteIndicatorColorProperty, ForegroundBrushKey);
        SetResourceReference(NumberColorProperty, ForegroundBrushKey);
        SetResourceReference(HourHandColorProperty, ForegroundBrushKey);
        SetResourceReference(MinuteHandColorProperty, ForegroundBrushKey);
        SetResourceReference(SecondHandColorProperty, AccentBrushKey);
        SetResourceReference(AmPmColorProperty, ForegroundBrushKey);
    }

    private void CreateIndicators()
    {
        for (var minute = 0; minute < 60; minute++)
        {
            var isHour = minute % 5 == 0;
            var width = isHour ? 20 : 10;
            var height = isHour ? 80 : 30;
            var indicator = new Rectangle
            {
                Width = width,
                Height = height,
                Margin = new Thickness(0, IndicatorTop, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                StrokeThickness = 0,
                RenderTransformOrigin = new Point(0.5, (FaceCenter - IndicatorTop) / height),
                RenderTransform = new RotateTransform(minute * 6),
            };

            indicator.SetBinding(
                Shape.FillProperty,
                new Binding(isHour ? nameof(HourIndicatorColor) : nameof(MinuteIndicatorColor)) { Source = this });

            if (isHour)
            {
                HourIndicators.Children.Add(indicator);
            }
            else
            {
                MinuteIndicators.Children.Add(indicator);
            }
        }
    }

    private void CreateNumbers()
    {
        for (var number = 1; number <= 12; number++)
        {
            var angle = number * 30;
            var text = new TextBlock
            {
                Text = number.ToString(CultureInfo.InvariantCulture),
                FontSize = 48,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                RenderTransformOrigin = new Point(0.5, 0.5),
                RenderTransform = new RotateTransform(-angle),
            };

            text.SetBinding(
                TextBlock.ForegroundProperty,
                new Binding(nameof(NumberColor)) { Source = this });

            var holder = new Grid
            {
                Width = 80,
                Height = NumberHeight,
                Margin = new Thickness(0, NumberTop, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                RenderTransformOrigin = new Point(0.5, (FaceCenter - NumberTop) / NumberHeight),
                RenderTransform = new RotateTransform(angle),
            };

            holder.Children.Add(text);
            Numbers.Children.Add(holder);
        }
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

    private void UpdateHands()
    {
        var timeOfDay = ClockHelper.ToTimeOfDay(Time);
        HourHandRotation.SetCurrentValue(RotateTransform.AngleProperty, GetHourAngle(timeOfDay));
        MinuteHandRotation.SetCurrentValue(RotateTransform.AngleProperty, GetMinuteAngle(timeOfDay));
        SecondHandRotation.SetCurrentValue(RotateTransform.AngleProperty, GetSecondAngle(timeOfDay));

        var isAm = timeOfDay.Hours < 12;
        SetCurrentValue(IsInAmProperty, isAm);

        var format = CultureInfo.CurrentCulture.DateTimeFormat;
        var invariant = CultureInfo.InvariantCulture.DateTimeFormat;
        var designator = isAm
            ? GetDesignator(format.AMDesignator, invariant.AMDesignator)
            : GetDesignator(format.PMDesignator, invariant.PMDesignator);
        AmPmText.SetCurrentValue(TextBlock.TextProperty, designator);
    }
}
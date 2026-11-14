namespace Atc.Wpf.Controls.Tests.Clocks;

[Collection(nameof(PackResourceTestGroup))]
public sealed class DigitalClockTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void Constructor_WithoutApplicationResources_FollowsTheCurrentTimeButDoesNotTick()
    {
        var sut = new DigitalClock();

        Assert.True(sut.FollowCurrentTime);
        Assert.False(sut.IsTimerRunning);
        Assert.Equal("000000", GetDigits(sut));
    }

    [StaFact]
    public void Time_IsShownAsHoursMinutesAndSeconds()
    {
        var sut = new DigitalClock { FollowCurrentTime = false };

        sut.Time = new TimeSpan(12, 34, 56);

        Assert.Equal("123456", GetDigits(sut));
    }

    [StaFact]
    public void Time_OutsideADay_IsWrappedIntoATimeOfDay()
    {
        var sut = new DigitalClock { FollowCurrentTime = false };

        sut.SetTime(new TimeSpan(1, 1, 2, 3));

        Assert.Equal("010203", GetDigits(sut));

        sut.SetTime(TimeSpan.FromSeconds(-1));

        Assert.Equal("235959", GetDigits(sut));
    }

    [StaFact]
    public void Loaded_StartsTheTimerWithTheCurrentTime_AndUnloaded_StopsIt()
    {
        var sut = new DigitalClock();
        sut.Ticker.Now = () => new DateTime(2026, 1, 2, 9, 8, 7, DateTimeKind.Local);

        sut.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));

        Assert.True(sut.IsTimerRunning);
        Assert.Equal(new TimeSpan(9, 8, 7), sut.Time);
        Assert.Equal("090807", GetDigits(sut));

        sut.RaiseEvent(new RoutedEventArgs(FrameworkElement.UnloadedEvent));

        Assert.False(sut.IsTimerRunning);
    }

    [StaFact]
    public void FollowCurrentTime_Off_StopsTheTimerWhileLoaded()
    {
        var sut = new DigitalClock();
        sut.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));

        sut.FollowCurrentTime = false;

        Assert.False(sut.IsTimerRunning);

        sut.FollowCurrentTime = true;

        Assert.True(sut.IsTimerRunning);

        sut.RaiseEvent(new RoutedEventArgs(FrameworkElement.UnloadedEvent));
    }

    [StaFact]
    public void FollowCurrentTime_Off_ShowsTheBoundTimeAgain()
    {
        var sut = new DigitalClock();
        sut.SetBinding(DigitalClock.TimeProperty, new Binding { Source = new TimeSpan(10, 20, 30) });
        sut.Ticker.Now = () => new DateTime(2026, 1, 2, 9, 8, 7, DateTimeKind.Local);
        sut.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));

        Assert.Equal("090807", GetDigits(sut));

        sut.FollowCurrentTime = false;

        Assert.Equal("102030", GetDigits(sut));

        sut.RaiseEvent(new RoutedEventArgs(FrameworkElement.UnloadedEvent));
    }

    [StaFact]
    public void Loaded_WithoutFollowCurrentTime_DoesNotStartTheTimer()
    {
        var sut = new DigitalClock { FollowCurrentTime = false };

        sut.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));

        Assert.False(sut.IsTimerRunning);
    }

    [StaFact]
    public void Settings_ArePassedToTheDigits()
    {
        var sut = new DigitalClock
        {
            NumberColor = Brushes.Red,
            ColonColor = Brushes.Blue,
            NumberBlurRadius = 12,
            ShowSeconds = false,
            ShowColon = true,
            PlayAnimation = false,
            AnimateColon = false,
            FollowCurrentTime = false,
        };
        sut.Measure(new Size(400, 100));
        sut.Arrange(new Rect(0, 0, 400, 100));

        Assert.Same(sut, BindingOperations.GetBindingExpression(sut.HourA, SevenSegment.NumberColorProperty)?.ResolvedSource);
        Assert.Same(Brushes.Red, sut.HourA.NumberColor);
        Assert.Same(Brushes.Blue, sut.HourB.ColonColor);
        Assert.Equal(12, sut.SecondB.BlurRadius);
        Assert.False(sut.MinuteA.PlayAnimation);
        Assert.False(sut.HourB.AnimateColon);
        Assert.True(sut.HourB.ShowColon);
        Assert.False(sut.MinuteB.ShowColon);
        Assert.Equal(Visibility.Collapsed, sut.SecondA.Visibility);
    }

    [StaFact]
    public void ConfigData_RoundTrips()
    {
        var source = new DigitalClock
        {
            NumberColor = new SolidColorBrush(Colors.Red),
            ColonColor = new SolidColorBrush(Colors.Blue),
            NumberBlurRadius = 12,
            ShowSeconds = false,
            AnimateColon = false,
            PlayAnimation = false,
        };

        var data = source.GetConfigData();
        var target = new DigitalClock();
        target.SetByConfigData(data);

        Assert.Equal("#FFFF0000", data.Colors.Number);
        Assert.Null(data.Colors.Background);
        Assert.Equal(Colors.Blue, Assert.IsType<SolidColorBrush>(target.ColonColor).Color);
        Assert.Equal(12, target.NumberBlurRadius);
        Assert.False(target.ShowSeconds);
        Assert.False(target.AnimateColon);
        Assert.False(target.PlayAnimation);
    }

    private static string GetDigits(DigitalClock clock)
        => string.Concat(
            new[] { clock.HourA, clock.HourB, clock.MinuteA, clock.MinuteB, clock.SecondA, clock.SecondB }
                .Select(s => s.Digit?.ToString(CultureInfo.InvariantCulture) ?? " "));
}
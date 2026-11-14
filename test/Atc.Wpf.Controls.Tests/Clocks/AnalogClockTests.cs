namespace Atc.Wpf.Controls.Tests.Clocks;

[Collection(nameof(PackResourceTestGroup))]
public sealed class AnalogClockTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [Theory]
    [InlineData(0, 0, 0, 0, 0, 0)]
    [InlineData(3, 0, 0, 90, 0, 0)]
    [InlineData(15, 30, 0, 105, 180, 0)]
    [InlineData(23, 59, 45, 359.5, 358.5, 270)]
    public void Angles_FollowTheTime(
        int hours,
        int minutes,
        int seconds,
        double hourAngle,
        double minuteAngle,
        double secondAngle)
    {
        var time = new TimeSpan(hours, minutes, seconds);

        Assert.Equal(hourAngle, AnalogClock.GetHourAngle(time), 6);
        Assert.Equal(minuteAngle, AnalogClock.GetMinuteAngle(time), 6);
        Assert.Equal(secondAngle, AnalogClock.GetSecondAngle(time), 6);
    }

    [StaFact]
    public void Constructor_WithoutApplicationResources_BuildsTheFace()
    {
        var sut = new AnalogClock();

        Assert.True(sut.FollowCurrentTime);
        Assert.False(sut.IsTimerRunning);
        Assert.Equal(12, sut.HourIndicators.Children.Count);
        Assert.Equal(48, sut.MinuteIndicators.Children.Count);
        Assert.Equal(12, sut.Numbers.Children.Count);
    }

    [StaFact]
    public void Time_TurnsTheHandsAndSetsAmPm()
    {
        var sut = new AnalogClock { FollowCurrentTime = false };

        sut.Time = new TimeSpan(15, 30, 10);

        Assert.Equal(105, sut.HourHandRotation.Angle, 6);
        Assert.Equal(181, sut.MinuteHandRotation.Angle, 6);
        Assert.Equal(60, sut.SecondHandRotation.Angle, 6);
        Assert.False(sut.IsInAm);
        Assert.False(string.IsNullOrEmpty(sut.AmPmText.Text));

        sut.SetTime(new TimeSpan(9, 0, 0));

        Assert.True(sut.IsInAm);
        Assert.Equal(270, sut.HourHandRotation.Angle, 6);
    }

    [StaFact]
    public void Loaded_StartsTheTimerWithTheCurrentTime_AndUnloaded_StopsIt()
    {
        var sut = new AnalogClock();
        sut.Ticker.Now = () => new DateTime(2026, 1, 2, 6, 0, 0, DateTimeKind.Local);

        sut.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));

        Assert.True(sut.IsTimerRunning);
        Assert.Equal(new TimeSpan(6, 0, 0), sut.Time);
        Assert.Equal(180, sut.HourHandRotation.Angle, 6);

        sut.RaiseEvent(new RoutedEventArgs(FrameworkElement.UnloadedEvent));

        Assert.False(sut.IsTimerRunning);
    }

    [StaFact]
    public void Loaded_Twice_KeepsOneRunningTimer_AndUnloaded_StopsIt()
    {
        var sut = new AnalogClock();

        sut.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));
        sut.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));
        sut.RaiseEvent(new RoutedEventArgs(FrameworkElement.UnloadedEvent));

        Assert.False(sut.IsTimerRunning);
    }

    [StaFact]
    public void ConfigData_RoundTrips_AndAnEmptyColorRestoresTheThemeBrush()
    {
        var source = new AnalogClock
        {
            SecondHandColor = new SolidColorBrush(Colors.Red),
            ShowNumbers = false,
            ShowAmPm = false,
        };

        var data = source.GetConfigData();
        var target = new AnalogClock();
        target.SetByConfigData(data);

        Assert.Equal("#FFFF0000", data.Colors.SecondHand);
        Assert.Equal(Colors.Red, Assert.IsType<SolidColorBrush>(target.SecondHandColor).Color);
        Assert.False(target.ShowNumbers);
        Assert.False(target.ShowAmPm);
        Assert.True(target.ShowSeconds);

        target.SetByConfigData(new AnalogClockConfigData());

        Assert.Equal(
            "ResourceReferenceExpression",
            target.ReadLocalValue(AnalogClock.SecondHandColorProperty).GetType().Name);
    }
}
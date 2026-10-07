namespace Atc.Wpf.Hardware.Services.Internal;

internal static class JustConnectedTimer
{
    public static void TransitionToAvailableAfter(
        Func<DeviceState> stateGetter,
        Action<DeviceState> stateSetter,
        TimeSpan duration)
    {
        if (duration <= TimeSpan.Zero)
        {
            stateSetter(DeviceState.Available);
            return;
        }

        var timer = new DispatcherTimer
        {
            Interval = duration,
        };

        timer.Tick += OnTick;
        timer.Start();

        void OnTick(
            object? sender,
            EventArgs e)
        {
            timer.Tick -= OnTick;
            timer.Stop();

            // The device may have been unplugged (or otherwise changed state) while the timer ran.
            if (stateGetter() is DeviceState.JustConnected)
            {
                stateSetter(DeviceState.Available);
            }
        }
    }
}
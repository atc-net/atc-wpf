namespace Atc.Wpf.Tests.Helpers;

/// <summary>
/// <see cref="SliderHelper.EnableMouseWheelProperty"/> lets the mouse wheel step a slider's value by its
/// small or large change, kept inside the slider's range.
/// </summary>
public sealed class SliderHelperTests
{
    [StaFact]
    public void MouseWheel_IsIgnoredByDefault()
    {
        var slider = CreateSlider(value: 50);

        var handled = Wheel(slider, delta: 120);

        Assert.Equal(50, slider.Value);
        Assert.False(handled);
    }

    [StaTheory]
    [InlineData(120, 52)]
    [InlineData(-120, 48)]
    public void MouseHover_WheelStepsBySmallChange(
        int delta,
        double expected)
    {
        var slider = CreateSlider(value: 50);
        SliderHelper.SetEnableMouseWheel(slider, MouseWheelState.MouseHover);

        var handled = Wheel(slider, delta);

        Assert.Equal(expected, slider.Value);
        Assert.True(handled);
    }

    [StaFact]
    public void ChangeValueByLargeChange_WheelStepsByLargeChange()
    {
        var slider = CreateSlider(value: 50);
        SliderHelper.SetEnableMouseWheel(slider, MouseWheelState.MouseHover);
        SliderHelper.SetChangeValueBy(slider, MouseWheelChangeState.LargeChange);

        Wheel(slider, delta: 120);

        Assert.Equal(60, slider.Value);
    }

    [StaTheory]
    [InlineData(99, 120, 100)]
    [InlineData(1, -120, 0)]
    public void Wheel_StopsAtTheEndsOfTheRange(
        double value,
        int delta,
        double expected)
    {
        var slider = CreateSlider(value);
        SliderHelper.SetEnableMouseWheel(slider, MouseWheelState.MouseHover);

        Wheel(slider, delta);

        Assert.Equal(expected, slider.Value);
    }

    [StaFact]
    public void ControlFocused_WheelIsIgnoredWhileTheSliderHasNoFocus()
    {
        var slider = CreateSlider(value: 50);
        SliderHelper.SetEnableMouseWheel(slider, MouseWheelState.ControlFocused);

        var handled = Wheel(slider, delta: 120);

        Assert.Equal(50, slider.Value);
        Assert.False(handled);
    }

    [StaFact]
    public void TurnedOffAgain_WheelIsIgnored()
    {
        var slider = CreateSlider(value: 50);
        SliderHelper.SetEnableMouseWheel(slider, MouseWheelState.MouseHover);
        SliderHelper.SetEnableMouseWheel(slider, MouseWheelState.None);

        Wheel(slider, delta: 120);

        Assert.Equal(50, slider.Value);
    }

    [StaFact]
    public void SwitchedBetweenModes_OneWheelNotchStepsOnce()
    {
        var slider = CreateSlider(value: 50);
        SliderHelper.SetEnableMouseWheel(slider, MouseWheelState.MouseHover);
        SliderHelper.SetEnableMouseWheel(slider, MouseWheelState.ControlFocused);
        SliderHelper.SetEnableMouseWheel(slider, MouseWheelState.MouseHover);

        Wheel(slider, delta: 120);

        Assert.Equal(52, slider.Value);
    }

    [StaFact]
    public void Wheel_KeepsAOneWayBindingOnValue()
    {
        var source = new SliderSource { Number = 50 };
        var slider = CreateSlider(value: 0);
        slider.SetBinding(RangeBase.ValueProperty, new Binding(nameof(SliderSource.Number)) { Source = source, Mode = BindingMode.OneWay });
        SliderHelper.SetEnableMouseWheel(slider, MouseWheelState.MouseHover);

        Wheel(slider, delta: 120);
        source.Number = 10;

        Assert.Equal(10, slider.Value);
    }

    private static Slider CreateSlider(double value)
        => new()
        {
            Minimum = 0,
            Maximum = 100,
            SmallChange = 2,
            LargeChange = 10,
            Value = value,
        };

    private static bool Wheel(
        Slider slider,
        int delta)
    {
        var args = new MouseWheelEventArgs(Mouse.PrimaryDevice, Environment.TickCount, delta)
        {
            RoutedEvent = UIElement.PreviewMouseWheelEvent,
        };

        slider.RaiseEvent(args);
        return args.Handled;
    }

    private sealed class SliderSource : INotifyPropertyChanged
    {
        private double number;

        public event PropertyChangedEventHandler? PropertyChanged;

        public double Number
        {
            get => number;
            set
            {
                number = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Number)));
            }
        }
    }
}
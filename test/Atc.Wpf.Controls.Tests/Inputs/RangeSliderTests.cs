namespace Atc.Wpf.Controls.Tests.Inputs;

public sealed class RangeSliderTests
{
    private const double TrackWidth = 220;

    private const string PartsTemplate = """
        <ControlTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                         xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                         TargetType="Control">
            <Canvas x:Name="PART_Track" Height="20">
                <Border x:Name="PART_RangeHighlight" Height="4" />
                <Thumb x:Name="PART_LowerThumb" Width="20" Height="20" Focusable="True" />
                <Thumb x:Name="PART_UpperThumb" Width="20" Height="20" Focusable="True" />
            </Canvas>
        </ControlTemplate>
        """;

    [StaFact]
    public void Defaults_SelectTheWholeRange()
    {
        var sut = new RangeSlider();

        Assert.Equal(0, sut.Minimum);
        Assert.Equal(100, sut.Maximum);
        Assert.Equal(0, sut.RangeStart);
        Assert.Equal(100, sut.RangeEnd);
    }

    [StaTheory]
    [InlineData(-5, 0)]
    [InlineData(150, 100)]
    public void RangeStart_OutsideTheBounds_IsKeptInside(
        double value,
        double expected)
    {
        var sut = new RangeSlider { RangeStart = value };

        Assert.Equal(expected, sut.RangeStart);
    }

    [StaFact]
    public void RangeEnd_AboveMaximum_IsKeptAtMaximum()
    {
        var sut = new RangeSlider { RangeEnd = 150 };

        Assert.Equal(100, sut.RangeEnd);
    }

    [StaFact]
    public void RangeEnd_BelowRangeStart_IsKeptAtRangeStart()
    {
        var sut = new RangeSlider { RangeStart = 40 };

        sut.RangeEnd = 30;

        Assert.Equal(40, sut.RangeEnd);
    }

    [StaFact]
    public void Minimum_RaisedAboveRangeStart_PullsTheRangeStartUp()
    {
        var sut = new RangeSlider { RangeStart = 10, RangeEnd = 90 };

        sut.Minimum = 20;

        Assert.Equal(20, sut.RangeStart);
        Assert.Equal(90, sut.RangeEnd);
    }

    [StaFact]
    public void Maximum_LoweredBelowRangeEnd_PullsTheRangeEndDown()
    {
        var sut = new RangeSlider { RangeStart = 10, RangeEnd = 90 };

        sut.Maximum = 50;

        Assert.Equal(10, sut.RangeStart);
        Assert.Equal(50, sut.RangeEnd);
    }

    // XAML sets attributes in the order they are written, and bindings deliver their values in any order:
    // a range set before its bounds must survive.
    [StaFact]
    public void RangeSetBeforeItsBounds_KeepsTheRange()
    {
        var sut = new RangeSlider
        {
            RangeEnd = 250,
            RangeStart = 220,
            Minimum = 200,
            Maximum = 300,
        };

        Assert.Equal(220, sut.RangeStart);
        Assert.Equal(250, sut.RangeEnd);
    }

    // Minimum above the default Maximum (100) used to bounce RangeStart between the two bounds until the
    // stack overflowed, which ends the process.
    [StaFact]
    public void MinimumSetAboveTheDefaultMaximum_DoesNotCrashAndKeepsTheRangeInside()
    {
        var sut = new RangeSlider
        {
            Minimum = 200,
            Maximum = 300,
        };

        Assert.Equal(200, sut.Minimum);
        Assert.Equal(300, sut.Maximum);
        Assert.InRange(sut.RangeStart, 200, 300);
        Assert.InRange(sut.RangeEnd, sut.RangeStart, 300);
    }

    [StaFact]
    public void MaximumBelowMinimum_IsKeptAtMinimum()
    {
        var sut = new RangeSlider { Minimum = 50 };

        sut.Maximum = 10;

        Assert.Equal(50, sut.Maximum);
        Assert.Equal(50, sut.RangeStart);
        Assert.Equal(50, sut.RangeEnd);
    }

    [StaFact]
    public void TwoWayBinding_ReceivesTheValueKeptInsideTheBounds()
    {
        var source = new RangeSource { End = 150 };
        var sut = new RangeSlider();
        sut.SetBinding(RangeSlider.RangeEndProperty, new Binding(nameof(RangeSource.End)) { Source = source });
        Flush();

        Assert.Equal(100, sut.RangeEnd);
        Assert.Equal(100, source.End);
    }

    [StaFact]
    public void TwoWayBindings_RangeBoundBeforeItsBounds_KeepsTheSourceRange()
    {
        var source = new RangeSource { Min = 200, Max = 300, Start = 220, End = 250 };
        var sut = new RangeSlider();
        sut.SetBinding(RangeSlider.RangeStartProperty, new Binding(nameof(RangeSource.Start)) { Source = source });
        sut.SetBinding(RangeSlider.RangeEndProperty, new Binding(nameof(RangeSource.End)) { Source = source });
        sut.SetBinding(RangeSlider.MinimumProperty, new Binding(nameof(RangeSource.Min)) { Source = source });
        sut.SetBinding(RangeSlider.MaximumProperty, new Binding(nameof(RangeSource.Max)) { Source = source });
        Flush();

        Assert.Equal(220, sut.RangeStart);
        Assert.Equal(250, sut.RangeEnd);
        Assert.Equal(220, source.Start);
        Assert.Equal(250, source.End);
    }

    [StaFact]
    public void ChangingTheRange_RaisesRangeChanged()
    {
        var sut = new RangeSlider();
        var raised = 0;
        sut.RangeChanged += (_, _) => raised++;

        sut.RangeStart = 10;
        sut.RangeEnd = 60;

        Assert.Equal(2, raised);
    }

    [StaFact]
    public void Rendered_PlacesTheThumbsAndHighlightByValue()
    {
        var sut = new RangeSlider { RangeStart = 25, RangeEnd = 75, ThumbSize = 20 };

        var window = Show(sut);
        try
        {
            var travel = Part<FrameworkElement>(sut, "PART_Track").ActualWidth - sut.ThumbSize;
            Assert.Equal(0.25 * travel, Canvas.GetLeft(Part<Thumb>(sut, "PART_LowerThumb")), 3);
            Assert.Equal(0.75 * travel, Canvas.GetLeft(Part<Thumb>(sut, "PART_UpperThumb")), 3);
            Assert.Equal(0.5 * travel, Part<FrameworkElement>(sut, "PART_RangeHighlight").Width, 3);
            Assert.Equal((0.25 * travel) + 10, Canvas.GetLeft(Part<FrameworkElement>(sut, "PART_RangeHighlight")), 3);
        }
        finally
        {
            window.Close();
        }
    }

    [StaFact]
    public void DraggingTheLowerThumb_MovesRangeStartBySnappedSteps()
    {
        var sut = new RangeSlider { RangeStart = 20, RangeEnd = 80, Step = 5, ThumbSize = 20 };

        var window = Show(sut);
        try
        {
            var travel = Part<FrameworkElement>(sut, "PART_Track").ActualWidth - sut.ThumbSize;

            // 12 % of the travel is 12 units; snapped to the step of 5 that is 10.
            Drag(Part<Thumb>(sut, "PART_LowerThumb"), 0.12 * travel);

            Assert.Equal(30, sut.RangeStart);
        }
        finally
        {
            window.Close();
        }
    }

    [StaFact]
    public void DraggingTheLowerThumbPastTheUpperThumb_StopsAtRangeEnd()
    {
        var sut = new RangeSlider { RangeStart = 20, RangeEnd = 40 };

        var window = Show(sut);
        try
        {
            Drag(Part<Thumb>(sut, "PART_LowerThumb"), TrackWidth);

            Assert.Equal(40, sut.RangeStart);
            Assert.Equal(40, sut.RangeEnd);
        }
        finally
        {
            window.Close();
        }
    }

    [StaFact]
    public void DraggingTheUpperThumbBelowTheLowerThumb_StopsAtRangeStart()
    {
        var sut = new RangeSlider { RangeStart = 20, RangeEnd = 40 };

        var window = Show(sut);
        try
        {
            Drag(Part<Thumb>(sut, "PART_UpperThumb"), -TrackWidth);

            Assert.Equal(20, sut.RangeEnd);
        }
        finally
        {
            window.Close();
        }
    }

    [StaTheory]
    [InlineData("PART_LowerThumb", Key.Right, 21, 80)]
    [InlineData("PART_LowerThumb", Key.Left, 19, 80)]
    [InlineData("PART_LowerThumb", Key.Home, 0, 80)]
    [InlineData("PART_LowerThumb", Key.End, 80, 80)]
    [InlineData("PART_UpperThumb", Key.Up, 20, 81)]
    [InlineData("PART_UpperThumb", Key.Down, 20, 79)]
    [InlineData("PART_UpperThumb", Key.Home, 20, 20)]
    [InlineData("PART_UpperThumb", Key.End, 20, 100)]
    public void Keyboard_MovesTheFocusedThumb(
        string thumb,
        Key key,
        double expectedStart,
        double expectedEnd)
    {
        var sut = new RangeSlider { RangeStart = 20, RangeEnd = 80 };

        var window = Show(sut);
        try
        {
            Assert.True(Part<Thumb>(sut, thumb).Focus());

            KeyPress.Send(sut, key);

            Assert.Equal(expectedStart, sut.RangeStart);
            Assert.Equal(expectedEnd, sut.RangeEnd);
        }
        finally
        {
            window.Close();
        }
    }

    [StaFact]
    public void Keyboard_RightToLeft_RightArrowMovesTowardsTheMinimum()
    {
        var sut = new RangeSlider { RangeStart = 20, RangeEnd = 80, FlowDirection = FlowDirection.RightToLeft };

        var window = Show(sut);
        try
        {
            Assert.True(Part<Thumb>(sut, "PART_LowerThumb").Focus());

            KeyPress.Send(sut, Key.Right);

            Assert.Equal(19, sut.RangeStart);
        }
        finally
        {
            window.Close();
        }
    }

    private static void Drag(
        Thumb thumb,
        double horizontalChange)
        => thumb.RaiseEvent(new DragDeltaEventArgs(horizontalChange, 0) { RoutedEvent = Thumb.DragDeltaEvent });

    private static Window Show(RangeSlider sut)
    {
        // The shipped template takes its thumb brushes from the application's theme resources (StaticResource),
        // which a unit test cannot provide; the logic under test only needs the four template parts.
        sut.Template = (ControlTemplate)XamlReader.Parse(PartsTemplate);
        sut.Width = TrackWidth;

        var window = new Window
        {
            Content = sut,
            Width = 400,
            Height = 200,
            Left = -10000,
            Top = -10000,
            ShowInTaskbar = false,
            ShowActivated = false,
            WindowStyle = WindowStyle.None,
        };

        window.Show();
        Flush();
        return window;
    }

    private static T Part<T>(
        Control control,
        string name)
        where T : class
        => Assert.IsType<T>(control.Template.FindName(name, control), exactMatch: false);

    private static void Flush()
        => Dispatcher.CurrentDispatcher.Invoke(() => { }, DispatcherPriority.ContextIdle);

    private sealed class RangeSource : INotifyPropertyChanged
    {
        private double min;
        private double max = 100;
        private double start;
        private double end;

        public event PropertyChangedEventHandler? PropertyChanged;

        public double Min
        {
            get => min;
            set => Set(ref min, value);
        }

        public double Max
        {
            get => max;
            set => Set(ref max, value);
        }

        public double Start
        {
            get => start;
            set => Set(ref start, value);
        }

        public double End
        {
            get => end;
            set => Set(ref end, value);
        }

        private void Set(
            ref double field,
            double value,
            [CallerMemberName] string propertyName = "")
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
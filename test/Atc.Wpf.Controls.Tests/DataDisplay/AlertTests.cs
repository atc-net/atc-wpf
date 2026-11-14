namespace Atc.Wpf.Controls.Tests.DataDisplay;

[Collection(nameof(PackResourceTestGroup))]
public sealed class AlertTests
{
    private const string Dictionary = "DataDisplay/Alert.xaml";

    [StaTheory]
    [InlineData(AlertVariant.Filled)]
    [InlineData(AlertVariant.Outlined)]
    [InlineData(AlertVariant.Text)]
    public void EverySeverity_HasItsOwnColors(AlertVariant variant)
    {
        var looks = new List<string>();
        foreach (var severity in Enum.GetValues<AlertSeverity>())
        {
            var alert = new Alert { Severity = severity, Variant = variant, Content = "Message" };
            var window = Show(alert);
            try
            {
                var border = ThemedHost.Part<Border>(alert, "PART_Border");
                looks.Add($"{Describe(border.Background)}|{Describe(border.BorderBrush)}|{Describe(alert.Foreground)}");
            }
            finally
            {
                window.Close();
            }
        }

        Assert.Equal(looks.Count, looks.Distinct(StringComparer.Ordinal).Count());
    }

    [StaTheory]
    [InlineData(AlertVariant.Filled, AlertSeverity.Error)]
    [InlineData(AlertVariant.Outlined, AlertSeverity.Success)]
    [InlineData(AlertVariant.Text, AlertSeverity.Warning)]
    public void OverrideBrushes_WinOverTheSeverityColors(
        AlertVariant variant,
        AlertSeverity severity)
    {
        var background = new SolidColorBrush(Colors.Olive);
        var foreground = new SolidColorBrush(Colors.Navy);
        var border = new SolidColorBrush(Colors.Maroon);
        var alert = new Alert
        {
            Severity = severity,
            Variant = variant,
            AlertBackground = background,
            AlertForeground = foreground,
            AlertBorderBrush = border,
        };

        var window = Show(alert);
        try
        {
            var partBorder = ThemedHost.Part<Border>(alert, "PART_Border");
            Assert.Same(background, partBorder.Background);
            Assert.Same(border, partBorder.BorderBrush);
            Assert.Same(foreground, alert.Foreground);
        }
        finally
        {
            window.Close();
        }
    }

    [StaFact]
    public void CloseButton_IsHiddenUnlessDismissible()
    {
        var alert = new Alert();

        var window = Show(alert);
        try
        {
            var button = ThemedHost.Part<Button>(alert, "PART_CloseButton");
            Assert.Equal(Visibility.Collapsed, button.Visibility);

            alert.IsDismissible = true;
            ThemedHost.Pump();

            Assert.Equal(Visibility.Visible, button.Visibility);
        }
        finally
        {
            window.Close();
        }
    }

    [StaFact]
    public void ClickingClose_RaisesCloseClickOnce()
    {
        var alert = new Alert { IsDismissible = true };
        var closed = 0;
        alert.CloseClick += (_, _) => closed++;

        var window = Show(alert);
        try
        {
            var button = ThemedHost.Part<Button>(alert, "PART_CloseButton");
            var args = new RoutedEventArgs(ButtonBase.ClickEvent);

            button.RaiseEvent(args);

            Assert.Equal(1, closed);
            Assert.True(args.Handled);
        }
        finally
        {
            window.Close();
        }
    }

    [StaFact]
    public void CustomIcon_ReplacesTheDefaultIcon_AndShowIconFalseHidesBoth()
    {
        var alert = new Alert { Icon = "!" };

        var window = Show(alert);
        try
        {
            Assert.Equal(Visibility.Visible, ThemedHost.Part<UIElement>(alert, "PART_Icon").Visibility);
            Assert.Equal(Visibility.Collapsed, ThemedHost.Part<UIElement>(alert, "PART_DefaultIcon").Visibility);

            alert.ShowIcon = false;
            ThemedHost.Pump();

            Assert.Equal(Visibility.Collapsed, ThemedHost.Part<UIElement>(alert, "PART_Icon").Visibility);
            Assert.Equal(Visibility.Collapsed, ThemedHost.Part<UIElement>(alert, "PART_DefaultIcon").Visibility);
        }
        finally
        {
            window.Close();
        }
    }

    [StaFact]
    public void Pulsing_AnimatesTheBorderOpacity_UntilTurnedOff()
    {
        var alert = new Alert { IsPulsing = true };

        var window = Show(alert);
        try
        {
            var border = ThemedHost.Part<Border>(alert, "PART_Border");
            Assert.True(border.HasAnimatedProperties);

            alert.IsPulsing = false;

            Assert.False(border.HasAnimatedProperties);
            Assert.Equal(1.0, border.Opacity);
        }
        finally
        {
            window.Close();
        }
    }

    // A pulse repeats forever; an alert taken off screen (e.g. dismissed) must not keep its animation clock
    // running, and it pulses again when it is shown again.
    [StaFact]
    public void PulsingAlert_StopsWhenUnloaded_AndResumesWhenLoadedAgain()
    {
        var alert = new Alert { IsPulsing = true };
        var host = new ContentControl { Content = alert };

        var window = Show(host);
        try
        {
            var border = ThemedHost.Part<Border>(alert, "PART_Border");
            Assert.True(border.HasAnimatedProperties);

            host.Content = null;
            ThemedHost.Pump();
            Assert.False(border.HasAnimatedProperties);

            host.Content = alert;
            ThemedHost.Pump();
            Assert.True(ThemedHost.Part<Border>(alert, "PART_Border").HasAnimatedProperties);
            Assert.True(alert.IsPulsing);
        }
        finally
        {
            window.Close();
        }
    }

    private static string Describe(Brush? brush)
        => brush switch
        {
            SolidColorBrush solid => solid.Color.ToString(CultureInfo.InvariantCulture),
            null => "none",
            _ => brush.GetType().Name,
        };

    private static Window Show(FrameworkElement content)
        => ThemedHost.Show(content, Dictionary, (typeof(Alert), "AtcApps.Styles.Alert"));
}
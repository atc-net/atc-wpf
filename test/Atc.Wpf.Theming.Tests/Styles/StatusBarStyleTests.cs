namespace Atc.Wpf.Theming.Tests.Styles;

/// <summary>
/// The themed StatusBar takes its background from the title bar of a NiceWindow, and must still work in a plain
/// Window: no binding errors for the missing NiceWindow ancestor and a theme background behind its light text.
/// </summary>
public sealed class StatusBarStyleTests
{
    private const int PathErrorId = 40;

    [StaFact]
    public void StatusBar_InAPlainWindow_HasAThemeBackgroundAndNoBindingErrors()
    {
        var resources = ThemedWindow.CreateResources();
        var statusBar = new StatusBar { Style = (Style)resources["AtcApps.Styles.StatusBar"] };
        statusBar.Items.Add(new StatusBarItem { Content = "Ready" });

        using var trace = BindingTraceCapture.Start();
        var canary = new TextBlock();
        canary.SetBinding(TextBlock.TextProperty, new Binding("DoesNotExist") { Source = new object() });

        var panel = new StackPanel();
        panel.Children.Add(statusBar);
        panel.Children.Add(canary);

        var window = ThemedWindow.Show(resources, panel);
        try
        {
            Assert.Contains(trace.Events, e => e.Id == PathErrorId && e.Message.Contains("DoesNotExist", StringComparison.Ordinal));

            var errors = trace.Events
                .Where(e => e.Message.Contains("NiceWindow", StringComparison.Ordinal) ||
                            e.Message.Contains("StatusBar", StringComparison.Ordinal))
                .Select(e => $"{e.Id}: {e.Message}")
                .ToList();
            Assert.True(errors.Count == 0, "The StatusBar style wrote binding errors:" + Environment.NewLine + string.Join(Environment.NewLine, errors));

            var background = Assert.IsType<SolidColorBrush>(statusBar.Background, exactMatch: false);
            Assert.NotEqual(0, background.Color.A);
        }
        finally
        {
            window.Close();
        }
    }

    [StaFact]
    public void IsNiceWindowValueConverter_PlainWindowOrNull_IsFalse()
    {
        var converter = IsNiceWindowValueConverter.Instance;

        Assert.False((bool)converter.Convert(new Window(), typeof(bool), parameter: null, CultureInfo.InvariantCulture));
        Assert.False((bool)converter.Convert(value: null, typeof(bool), parameter: null, CultureInfo.InvariantCulture));
    }
}
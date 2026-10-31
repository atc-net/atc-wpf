namespace Atc.Wpf.Theming.Tests.Styles;

/// <summary>
/// The themed TabItem must show where the keyboard focus is (WCAG 2.4.7): its focus visual is a themed
/// outline instead of none.
/// </summary>
public sealed class TabItemStyleTests
{
    [StaFact]
    public void TabItem_HasAThemedFocusVisual()
    {
        var resources = ThemedWindow.CreateResources();
        resources.MergedDictionaries.Add(new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/Atc.Wpf.Theming;component/Styles/BaseControls/TabControl.xaml", UriKind.Absolute),
        });

        // The TabItem template itself is not applied: its ContentControlEx pulls in Generic.xaml, which
        // registers the ATC themes at the process-wide ThemeManager.
        var tabItemStyle = (Style)resources["AtcApps.Styles.TabItem"];
        var focusVisualStyle = tabItemStyle.Setters
            .OfType<Setter>()
            .Where(s => s.Property == FrameworkElement.FocusVisualStyleProperty)
            .Select(s => s.Value)
            .OfType<Style>()
            .SingleOrDefault();
        Assert.NotNull(focusVisualStyle);

        var template = focusVisualStyle.Setters
            .OfType<Setter>()
            .Where(s => s.Property == Control.TemplateProperty)
            .Select(s => s.Value)
            .OfType<ControlTemplate>()
            .Single();

        var outlineHost = new Control { Template = template };
        var host = new Border { Child = outlineHost, Resources = resources };
        host.Measure(new Size(50, 20));
        host.Arrange(new Rect(0, 0, 50, 20));

        var outline = Assert.IsType<System.Windows.Shapes.Rectangle>(VisualTreeHelper.GetChild(outlineHost, 0));
        var stroke = Assert.IsType<SolidColorBrush>(outline.Stroke, exactMatch: false);
        Assert.Equal(
            ((SolidColorBrush)resources["AtcApps.Brushes.SystemControlFocusVisualPrimary"]).Color,
            stroke.Color);
    }
}
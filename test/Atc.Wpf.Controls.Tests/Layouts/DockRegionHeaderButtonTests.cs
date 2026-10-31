namespace Atc.Wpf.Controls.Tests.Layouts;

[Collection(nameof(PackResourceTestGroup))]
public sealed class DockRegionHeaderButtonTests : IDisposable
{
    public DockRegionHeaderButtonTests()
    {
        // pack://application URIs need the Application type initialized.
        _ = typeof(Atc.Wpf.MarkupExtensions.StaticResourceExtension).Assembly;
        RuntimeHelpers.RunClassConstructor(typeof(Application).TypeHandle);
    }

    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void Header_buttons_follow_the_header_text_colour_on_a_dark_header()
    {
        var (panel, bottom, _) = DockPanelProPaneTests.CreatePanel();
        bottom.HeaderBackground = new SolidColorBrush(Color.FromRgb(0x20, 0x20, 0x20));
        var host = DockPanelProFloatingWindowTests.Show(panel);
        try
        {
            host.UpdateLayout();

            var floatButton = (Button)bottom.Template.FindName("PART_FloatButton", bottom);
            var closeButton = (Button)bottom.Template.FindName("PART_CloseButton", bottom);
            var headerText = (Brush)((FrameworkElement)floatButton.Parent).GetValue(TextElement.ForegroundProperty);

            Assert.NotEqual(Colors.Black, ((SolidColorBrush)headerText).Color);
            Assert.Equal(((SolidColorBrush)headerText).Color, ((SolidColorBrush)floatButton.Foreground).Color);
            Assert.Equal(((SolidColorBrush)headerText).Color, ((SolidColorBrush)closeButton.Foreground).Color);
        }
        finally
        {
            host.Close();
        }
    }
}
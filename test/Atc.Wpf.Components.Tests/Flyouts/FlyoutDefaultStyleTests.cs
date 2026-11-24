namespace Atc.Wpf.Components.Tests.Flyouts;

/// <summary>
/// A <see cref="Flyout"/> and its <see cref="FlyoutHost"/> get their template from the assembly's theme
/// dictionary, so they are drawn even when the application merges no Atc.Wpf.Components styles.
/// </summary>
public sealed class FlyoutDefaultStyleTests
{
    [StaFact]
    public void Flyout_WithoutAppResources_HasATemplate()
    {
        // Arrange
        // pack://application URIs (used by the theme dictionary) need the Application type initialized.
        RuntimeHelpers.RunClassConstructor(typeof(Application).TypeHandle);

        var host = new FlyoutHost();
        var service = new FlyoutService(host);
        var window = new Window
        {
            Content = host,
            Width = 400,
            Height = 300,
            Left = -10000,
            Top = -10000,
            ShowInTaskbar = false,
            ShowActivated = false,
            WindowStyle = WindowStyle.None,
        };

        try
        {
            window.Show();

            // Act
            _ = service.ShowAsync("Header", new TextBlock { Text = "Content" });
            FlyoutWindow.Pump();

            // Assert
            var flyout = Assert.IsType<Flyout>(host.Items[0]);
            Assert.NotNull(host.Template);
            Assert.NotNull(flyout.Template);
            Assert.NotNull(flyout.Template.FindName("PART_FlyoutPanel", flyout));

            service.CloseAllFlyouts();
        }
        finally
        {
            window.Close();
            window.Dispatcher.InvokeShutdown();
        }
    }
}
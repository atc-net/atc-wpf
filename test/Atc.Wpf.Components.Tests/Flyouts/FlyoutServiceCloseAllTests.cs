namespace Atc.Wpf.Components.Tests.Flyouts;

/// <summary>
/// Same contract as <see cref="FlyoutHostCloseAllTests"/>, for the service: a templated flyout only leaves the
/// service's open stack when its close animation has completed on the dispatcher, so CloseAllFlyouts must not
/// wait for the stack to empty.
/// </summary>
public sealed class FlyoutServiceCloseAllTests
{
    private const string FlyoutTemplateXaml = """
        <ControlTemplate
            xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
            xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
            TargetType="ContentControl">
            <Grid>
                <Border x:Name="PART_Overlay" />
                <Border x:Name="PART_FlyoutPanel">
                    <ContentPresenter />
                </Border>
            </Grid>
        </ControlTemplate>
        """;

    [Fact]
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Any failure on the worker thread is handed back to the test thread and asserted.")]
    public void CloseAllFlyouts_WhileCloseAnimationsAreRunning_ReturnsWithoutHanging()
    {
        Exception? failure = null;
        var completed = false;

        // Run on a dedicated STA thread so a hang fails this test instead of blocking the test run.
        var thread = new Thread(() =>
        {
            try
            {
                var panel = new Grid();
                var service = new FlyoutService(panel);
                _ = service.ShowAsync("First", new TextBlock());
                _ = service.ShowAsync("Second", new TextBlock());

                var flyouts = panel.Children.OfType<Flyout>().ToArray();
                Assert.Equal(2, flyouts.Length);
                foreach (var flyout in flyouts)
                {
                    flyout.Template = (ControlTemplate)System.Windows.Markup.XamlReader.Parse(FlyoutTemplateXaml);
                    flyout.ApplyTemplate();
                }

                service.CloseAllFlyouts();

                Assert.All(flyouts, flyout => Assert.False(flyout.IsOpen));
                completed = true;
            }
            catch (Exception ex)
            {
                failure = ex;
            }
        })
        {
            IsBackground = true,
        };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();

        var finished = thread.Join(TimeSpan.FromSeconds(5));

        Assert.True(finished, "CloseAllFlyouts did not return within 5 seconds.");
        Assert.Null(failure);
        Assert.True(completed);
    }
}
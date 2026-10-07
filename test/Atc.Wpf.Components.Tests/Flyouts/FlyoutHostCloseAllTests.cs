namespace Atc.Wpf.Components.Tests.Flyouts;

/// <summary>
/// With a template, closing a flyout animates and the flyout only leaves the host's open stack when the
/// animation completes - which needs the dispatcher. CloseAllFlyouts must therefore not wait for that
/// synchronously.
/// </summary>
public sealed class FlyoutHostCloseAllTests
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
                var host = new FlyoutHost();
                var first = CreateTemplatedFlyout();
                var second = CreateTemplatedFlyout();
                host.OpenFlyout(first);
                host.OpenFlyout(second);

                host.CloseAllFlyouts();

                Assert.False(first.IsOpen);
                Assert.False(second.IsOpen);
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

    private static Flyout CreateTemplatedFlyout()
    {
        var flyout = new Flyout
        {
            Template = (ControlTemplate)System.Windows.Markup.XamlReader.Parse(FlyoutTemplateXaml),
        };
        flyout.ApplyTemplate();
        return flyout;
    }
}
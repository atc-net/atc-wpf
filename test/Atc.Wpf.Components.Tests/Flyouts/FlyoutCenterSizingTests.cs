namespace Atc.Wpf.Components.Tests.Flyouts;

/// <summary>
/// A centred flyout can size to its content (<c>Height = double.NaN</c>) while staying inside its host; fixed
/// heights are unchanged. <see cref="FlyoutService.IsHostConfigured"/> tells whether the service can show at all.
/// </summary>
[Collection(nameof(PackResourceTestGroup))]
public sealed class FlyoutCenterSizingTests
{
    [StaFact]
    public void CenterFlyout_WithAutomaticHeight_IsAsTallAsItsContent()
    {
        using var sut = FlyoutWindow.Show();
        var service = new FlyoutService(sut.Host);
        var options = CenterOptions(height: double.NaN);

        _ = service.ShowAsync("Short", new Border { Height = 60 }, options);
        FlyoutWindow.Pump();

        var panel = Panel(sut.Host);
        Assert.InRange(panel.ActualHeight, 60, 250);
        Assert.Equal(VerticalAlignment.Center, panel.VerticalAlignment);
        service.CloseAllFlyouts();
    }

    [StaFact]
    public void CenterFlyout_WithAutomaticHeight_StaysInsideTheHostWhenTheContentIsTaller()
    {
        using var sut = FlyoutWindow.Show(height: 400);
        var service = new FlyoutService(sut.Host);
        var options = CenterOptions(height: double.NaN);
        options.MaxHeight = double.PositiveInfinity;

        _ = service.ShowAsync("Long", new Border { Height = 2000 }, options);
        FlyoutWindow.Pump();

        Assert.True(
            Panel(sut.Host).ActualHeight <= sut.Host.ActualHeight,
            $"The flyout ({Panel(sut.Host).ActualHeight}) is taller than its host ({sut.Host.ActualHeight}).");
        service.CloseAllFlyouts();
    }

    [StaFact]
    public void CenterFlyout_WithAFixedHeight_KeepsIt()
    {
        using var sut = FlyoutWindow.Show();
        var service = new FlyoutService(sut.Host);

        _ = service.ShowAsync("Fixed", new Border { Height = 60 }, CenterOptions(height: 320));
        FlyoutWindow.Pump();

        Assert.Equal(320, Panel(sut.Host).ActualHeight, 1);
        service.CloseAllFlyouts();
    }

    [StaFact]
    public void CenterFlyout_WithRoundedCorners_RoundsTheTopCornersOfItsHeader()
    {
        // The header has its own background; with square corners it paints over the panel's rounded top corners.
        using var sut = FlyoutWindow.Show();
        var service = new FlyoutService(sut.Host);
        var options = CenterOptions(height: double.NaN);
        options.CornerRadius = new CornerRadius(8);

        _ = service.ShowAsync("Rounded", new Border { Height = 60 }, options);
        FlyoutWindow.Pump();

        var flyout = Assert.IsType<Flyout>(sut.Host.Items[^1]);
        var header = Assert.IsType<Border>(flyout.Template.FindName("HeaderBorder", flyout));
        Assert.Equal(new CornerRadius(8, 8, 0, 0), header.CornerRadius);
        service.CloseAllFlyouts();
    }

    [StaFact]
    public void ClosingAFlyout_GivesTheFocusBackToTheElementThatHadIt()
    {
        using var sut = FlyoutWindow.Show();
        var service = new FlyoutService(sut.Host);
        Assert.True(sut.PageButton.Focus());
        var content = new TextBox();

        _ = service.ShowAsync("Dialog", content, CenterOptions(height: double.NaN));
        FlyoutWindow.Pump();
        content.Focus();
        service.CloseTopFlyout();

        Assert.True(FlyoutWindow.WaitUntil(() => sut.Host.Items.Count == 0), "The flyout did not close.");
        Assert.True(sut.PageButton.IsKeyboardFocused);
    }

    [StaFact]
    public void IsHostConfigured_IsFalseUntilAHostIsSet()
    {
        var service = new FlyoutService();
        Assert.False(service.IsHostConfigured);

        service.SetFlyoutHost(new FlyoutHost());
        Assert.True(service.IsHostConfigured);

        var panelService = new FlyoutService();
        panelService.SetHostPanel(new Grid());
        Assert.True(panelService.IsHostConfigured);
    }

    private static FlyoutOptions CenterOptions(double height)
    {
        var options = FlyoutOptions.Center;
        options.Height = height;
        options.AnimationDuration = 0;
        return options;
    }

    private static Border Panel(FlyoutHost host)
    {
        var flyout = Assert.IsType<Flyout>(host.Items[^1]);
        return Assert.IsType<Border>(flyout.Template.FindName("PART_FlyoutPanel", flyout));
    }
}
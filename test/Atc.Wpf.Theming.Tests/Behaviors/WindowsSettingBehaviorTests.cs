namespace Atc.Wpf.Theming.Tests.Behaviors;

public sealed class WindowsSettingBehaviorTests
{
    [Fact]
    public void UpgradeSettingsIfRequired_UpgradeRequested_UpgradesOnceAndSaves()
    {
        var settings = new FakeSettings { UpgradeSettings = true };

        WindowsSettingBehavior.UpgradeSettingsIfRequired(settings);

        Assert.Equal(1, settings.UpgradeCalls);
        Assert.False(settings.UpgradeSettings);
        Assert.Equal(1, settings.SaveCalls);
    }

    [Fact]
    public void UpgradeSettingsIfRequired_AlreadyUpgraded_DoesNothing()
    {
        var settings = new FakeSettings { UpgradeSettings = false };

        WindowsSettingBehavior.UpgradeSettingsIfRequired(settings);

        Assert.Equal(0, settings.UpgradeCalls);
        Assert.Equal(0, settings.SaveCalls);
    }

    [Fact]
    public void UpgradeSettingsIfRequired_UpgradeThrows_DoesNotPropagate()
    {
        var settings = new FakeSettings
        {
            UpgradeSettings = true,
            ThrowOnUpgrade = true,
        };

        var exception = Record.Exception(() => WindowsSettingBehavior.UpgradeSettingsIfRequired(settings));

        Assert.Null(exception);
        Assert.Equal(1, settings.UpgradeCalls);
    }

    [Fact]
    public void IsPlacementOnScreen_InsideTheScreen_IsTrue()
    {
        var monitors = Monitors(new Rect(0, 0, 1920, 1080));

        Assert.True(WindowsSettingBehavior.IsPlacementOnScreen(new Rect(100, 100, 800, 600), monitors));
    }

    [Fact]
    public void IsPlacementOnScreen_PartlyOffTheScreen_IsTrue()
    {
        var monitors = Monitors(new Rect(0, 0, 1920, 1080));

        Assert.True(WindowsSettingBehavior.IsPlacementOnScreen(new Rect(1700, 900, 800, 600), monitors));
    }

    [Fact]
    public void IsPlacementOnScreen_OnASecondaryMonitorLeftOfThePrimary_IsTrue()
    {
        var monitors = Monitors(
            new Rect(0, 0, 1920, 1080),
            new Rect(-1920, 0, 1920, 1080));

        Assert.True(WindowsSettingBehavior.IsPlacementOnScreen(new Rect(-1800, 100, 800, 600), monitors));
    }

    // A window last closed on a monitor that is no longer attached.
    [Fact]
    public void IsPlacementOnScreen_EntirelyOffTheScreen_IsFalse()
    {
        var monitors = Monitors(new Rect(0, 0, 1920, 1080));

        Assert.False(WindowsSettingBehavior.IsPlacementOnScreen(new Rect(2000, 100, 800, 600), monitors));
        Assert.False(WindowsSettingBehavior.IsPlacementOnScreen(new Rect(-900, 100, 800, 600), monitors));
        Assert.False(WindowsSettingBehavior.IsPlacementOnScreen(new Rect(100, 1200, 800, 600), monitors));
    }

    [Fact]
    public void IsPlacementOnScreen_OnlyTouchingTheScreenEdge_IsFalse()
    {
        var monitors = Monitors(new Rect(0, 0, 1920, 1080));

        Assert.False(WindowsSettingBehavior.IsPlacementOnScreen(new Rect(1920, 100, 800, 600), monitors));
    }

    [Fact]
    public void IsPlacementOnScreen_EmptyPlacement_IsFalse()
    {
        var monitors = Monitors(new Rect(0, 0, 1920, 1080));

        Assert.False(WindowsSettingBehavior.IsPlacementOnScreen(Rect.Empty, monitors));
        Assert.False(WindowsSettingBehavior.IsPlacementOnScreen(new Rect(100, 100, 0, 0), monitors));
    }

    // Two monitors offset vertically leave an area inside their bounding box that no monitor covers.
    [Fact]
    public void IsPlacementOnScreen_InTheGapBetweenOffsetMonitors_IsFalse()
    {
        var monitors = Monitors(
            new Rect(0, 0, 1920, 1080),
            new Rect(1920, 600, 1920, 1080));

        Assert.False(WindowsSettingBehavior.IsPlacementOnScreen(new Rect(2100, 50, 800, 400), monitors));
    }

    [Fact]
    public void IsPlacementOnScreen_TitleBarAboveEveryMonitor_IsFalse()
    {
        var monitors = Monitors(
            new Rect(0, 0, 1920, 1080),
            new Rect(1920, 0, 1920, 1080));

        Assert.False(WindowsSettingBehavior.IsPlacementOnScreen(new Rect(100, -300, 800, 600), monitors));
    }

    [Fact]
    public void IsPlacementOnScreen_OnlyASliverOfTheTitleBarVisible_IsFalse()
    {
        var monitors = Monitors(new Rect(0, 0, 1920, 1080));

        Assert.False(WindowsSettingBehavior.IsPlacementOnScreen(new Rect(1915, 100, 800, 600), monitors));
        Assert.False(WindowsSettingBehavior.IsPlacementOnScreen(new Rect(100, 1075, 800, 600), monitors));
    }

    // With the taskbar at the top, workspace y = 0 is screen y = 40.
    [Fact]
    public void IsPlacementOnScreen_UsesWorkspaceCoordinates()
    {
        var monitors = new[]
        {
            new MonitorWorkArea(new Rect(0, 0, 1920, 1080), new Rect(0, 40, 1920, 1040)),
        };

        Assert.True(WindowsSettingBehavior.IsPlacementOnScreen(new Rect(100, 0, 800, 600), monitors));
        Assert.False(WindowsSettingBehavior.IsPlacementOnScreen(new Rect(100, 1030, 800, 600), monitors));
    }

    private static MonitorWorkArea[] Monitors(params Rect[] bounds)
        => bounds
            .Select(b => new MonitorWorkArea(b, b))
            .ToArray();

    private sealed class FakeSettings : IWindowPlacementSettings
    {
        public WindowPlacementSetting? Placement { get; set; }

        public bool UpgradeSettings { get; set; }

        public bool ThrowOnUpgrade { get; set; }

        public int UpgradeCalls { get; private set; }

        public int SaveCalls { get; private set; }

        public void Reload()
        {
        }

        public void Upgrade()
        {
            UpgradeCalls++;
            if (ThrowOnUpgrade)
            {
                throw new InvalidOperationException("Upgrade failed");
            }
        }

        public void Save()
            => SaveCalls++;
    }
}
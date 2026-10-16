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
        var screen = new Rect(0, 0, 1920, 1080);

        Assert.True(WindowsSettingBehavior.IsPlacementOnScreen(new Rect(100, 100, 800, 600), screen));
    }

    [Fact]
    public void IsPlacementOnScreen_PartlyOffTheScreen_IsTrue()
    {
        var screen = new Rect(0, 0, 1920, 1080);

        Assert.True(WindowsSettingBehavior.IsPlacementOnScreen(new Rect(1700, 900, 800, 600), screen));
    }

    [Fact]
    public void IsPlacementOnScreen_OnASecondaryMonitorLeftOfThePrimary_IsTrue()
    {
        var screen = new Rect(-1920, 0, 3840, 1080);

        Assert.True(WindowsSettingBehavior.IsPlacementOnScreen(new Rect(-1800, 100, 800, 600), screen));
    }

    // A window last closed on a monitor that is no longer attached.
    [Fact]
    public void IsPlacementOnScreen_EntirelyOffTheScreen_IsFalse()
    {
        var screen = new Rect(0, 0, 1920, 1080);

        Assert.False(WindowsSettingBehavior.IsPlacementOnScreen(new Rect(2000, 100, 800, 600), screen));
        Assert.False(WindowsSettingBehavior.IsPlacementOnScreen(new Rect(-900, 100, 800, 600), screen));
        Assert.False(WindowsSettingBehavior.IsPlacementOnScreen(new Rect(100, 1200, 800, 600), screen));
    }

    [Fact]
    public void IsPlacementOnScreen_OnlyTouchingTheScreenEdge_IsFalse()
    {
        var screen = new Rect(0, 0, 1920, 1080);

        Assert.False(WindowsSettingBehavior.IsPlacementOnScreen(new Rect(1920, 100, 800, 600), screen));
    }

    [Fact]
    public void IsPlacementOnScreen_EmptyPlacement_IsFalse()
    {
        var screen = new Rect(0, 0, 1920, 1080);

        Assert.False(WindowsSettingBehavior.IsPlacementOnScreen(Rect.Empty, screen));
        Assert.False(WindowsSettingBehavior.IsPlacementOnScreen(new Rect(100, 100, 0, 0), screen));
    }

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
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
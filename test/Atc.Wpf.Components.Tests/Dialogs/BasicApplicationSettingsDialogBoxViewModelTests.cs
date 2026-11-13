namespace Atc.Wpf.Components.Tests.Dialogs;

public sealed class BasicApplicationSettingsDialogBoxViewModelTests
{
    // Apps installed under Program Files cannot write next to the exe; the save must report that, not throw.
    [Fact]
    public void TryWriteCustomSettings_WithAMissingDirectory_ReturnsTheError()
    {
        var path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString("N"), "appsettings.custom.json");

        var error = BasicApplicationSettingsDialogBoxViewModel.TryWriteCustomSettings(path, "{}");

        Assert.IsAssignableFrom<System.IO.IOException>(error);
        Assert.False(System.IO.File.Exists(path));
    }
}
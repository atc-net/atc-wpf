namespace Atc.Wpf.Controls.Tests.Buttons;

/// <summary>
/// These controls listen to the process-wide ThemeManager.ThemeChanged event. The subscription must not
/// keep an instance alive once it has left the visual tree (or was never added to it).
/// </summary>
public sealed class ThemeChangedSubscriptionLeakTests
{
    [StaTheory]
    [InlineData(typeof(Atc.Wpf.Controls.Buttons.AuthenticationButton))]
    [InlineData(typeof(Atc.Wpf.Controls.Buttons.ConnectivityButton))]
    [InlineData(typeof(Atc.Wpf.Controls.Inputs.RichTextBoxEx))]
    public void WhileLoaded_IsKeptAliveByThemeSubscription(Type controlType)
    {
        // Guards against "fixing" the leak by never listening for theme changes at all.
        var reference = ControlLeakProbe.CreateLoaded(controlType);

        Assert.False(ControlLeakProbe.IsCollected(reference));
    }

    [StaTheory]
    [InlineData(typeof(Atc.Wpf.Controls.Buttons.AuthenticationButton))]
    [InlineData(typeof(Atc.Wpf.Controls.Buttons.ConnectivityButton))]
    [InlineData(typeof(Atc.Wpf.Controls.Inputs.RichTextBoxEx))]
    public void Constructed_NeverLoaded_CanBeGarbageCollected(Type controlType)
    {
        var reference = ControlLeakProbe.CreateUnloaded(controlType);

        Assert.True(ControlLeakProbe.IsCollected(reference));
    }

    [StaTheory]
    [InlineData(typeof(Atc.Wpf.Controls.Buttons.AuthenticationButton))]
    [InlineData(typeof(Atc.Wpf.Controls.Buttons.ConnectivityButton))]
    [InlineData(typeof(Atc.Wpf.Controls.Inputs.RichTextBoxEx))]
    public void LoadedThenUnloaded_CanBeGarbageCollected(Type controlType)
    {
        var reference = ControlLeakProbe.CreateLoadedThenUnloaded(controlType);

        Assert.True(ControlLeakProbe.IsCollected(reference));
    }
}
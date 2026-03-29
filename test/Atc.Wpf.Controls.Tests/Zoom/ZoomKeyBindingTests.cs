namespace Atc.Wpf.Controls.Tests.Zoom;

public sealed class ZoomKeyBindingTests
{
    [Fact]
    public void Constructor_SetsProperties()
    {
        var binding = new ZoomKeyBinding(Key.F, ModifierKeys.Control, ZoomActionType.ZoomFit);

        Assert.Equal(Key.F, binding.Key);
        Assert.Equal(ModifierKeys.Control, binding.Modifiers);
        Assert.Equal(ZoomActionType.ZoomFit, binding.Action);
    }

    [Fact]
    public void CreateDefaults_ReturnsNonEmptyList()
    {
        var defaults = ZoomKeyBinding.CreateDefaults();

        Assert.NotEmpty(defaults);
    }

    [Fact]
    public void CreateDefaults_ContainsZoomIn()
    {
        var defaults = ZoomKeyBinding.CreateDefaults();

        Assert.Contains(
            defaults,
            b => b.Action == ZoomActionType.ZoomIn);
    }

    [Fact]
    public void CreateDefaults_ContainsZoomOut()
    {
        var defaults = ZoomKeyBinding.CreateDefaults();

        Assert.Contains(
            defaults,
            b => b.Action == ZoomActionType.ZoomOut);
    }

    [Fact]
    public void CreateDefaults_ContainsZoomFill()
    {
        var defaults = ZoomKeyBinding.CreateDefaults();

        Assert.Contains(
            defaults,
            b => b.Action == ZoomActionType.ZoomFill &&
                 b.Modifiers == (ModifierKeys.Control | ModifierKeys.Alt));
    }

    [Fact]
    public void CreateDefaults_ContainsZoom100Percent()
    {
        var defaults = ZoomKeyBinding.CreateDefaults();

        Assert.Contains(
            defaults,
            b => b.Action == ZoomActionType.Zoom100Percent);
    }
}
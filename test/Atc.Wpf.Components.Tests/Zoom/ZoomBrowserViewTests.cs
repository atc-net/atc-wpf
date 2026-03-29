namespace Atc.Wpf.Components.Tests.Zoom;

public sealed class ZoomBrowserViewTests
{
    [StaFact]
    public void ShowToolbar_DefaultValue_IsTrue()
    {
        var metadata = ZoomBrowserView.ShowToolbarProperty.DefaultMetadata;

        Assert.True((bool)metadata.DefaultValue);
    }

    [StaFact]
    public void ShowMiniMap_DefaultValue_IsTrue()
    {
        var metadata = ZoomBrowserView.ShowMiniMapProperty.DefaultMetadata;

        Assert.True((bool)metadata.DefaultValue);
    }

    [StaFact]
    public void ShowStatusBar_DefaultValue_IsTrue()
    {
        var metadata = ZoomBrowserView.ShowStatusBarProperty.DefaultMetadata;

        Assert.True((bool)metadata.DefaultValue);
    }

    [StaFact]
    public void ZoomContent_DefaultValue_IsNull()
    {
        var metadata = ZoomBrowserView.ZoomContentProperty.DefaultMetadata;

        Assert.Null(metadata.DefaultValue);
    }
}
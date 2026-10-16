namespace Atc.Wpf.Tests.MarkupExtensions;

public sealed class MarkupExtensionManagerTests
{
    // Each [StaFact] runs on its own thread, so targets another test left in the shared manager belong to a
    // different (finished) thread and cannot be updated from this one.
    public MarkupExtensionManagerTests()
        => SpawningTextExtension.Manager.ActiveExtensions.Clear();

    [StaFact]
    public void UpdateAllTargets_tolerates_extensions_registered_during_the_update()
    {
        var textBlock = (TextBlock)XamlReader.Parse(
            """
            <TextBlock
                xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                xmlns:test="clr-namespace:Atc.Wpf.Tests.XUnitTestTypes;assembly=Atc.Wpf.Tests"
                Text="{test:SpawningTextExtension}" />
            """);
        var countBefore = SpawningTextExtension.Manager.ActiveExtensions.Count;

        var exception = Record.Exception(SpawningTextExtension.Manager.UpdateAllTargets);

        Assert.Null(exception);
        Assert.Equal("spawned", textBlock.Text);
        Assert.Equal(countBefore + 1, SpawningTextExtension.Manager.ActiveExtensions.Count);
    }

    [StaFact]
    public void CleanupInactiveExtensions_keeps_extensions_whose_target_is_alive()
    {
        var textBlock = (TextBlock)XamlReader.Parse(
            """
            <TextBlock
                xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                xmlns:test="clr-namespace:Atc.Wpf.Tests.XUnitTestTypes;assembly=Atc.Wpf.Tests"
                Text="{test:SpawningTextExtension}" />
            """);
        var countBefore = SpawningTextExtension.Manager.ActiveExtensions.Count;

        SpawningTextExtension.Manager.CleanupInactiveExtensions();

        Assert.Equal(countBefore, SpawningTextExtension.Manager.ActiveExtensions.Count);
        GC.KeepAlive(textBlock);
    }
}
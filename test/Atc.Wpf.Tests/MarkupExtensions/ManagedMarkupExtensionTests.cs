namespace Atc.Wpf.Tests.MarkupExtensions;

public sealed class ManagedMarkupExtensionTests
{
    // Flyout.xaml sets ToolTip through a Resx extension in a style setter; a culture change then updated the
    // sealed setter and threw "After a 'SetterBase' is in use (sealed), it cannot be modified".
    [StaFact]
    public void UpdateAllTargets_ExtensionInASealedStyleSetter_DoesNotThrow()
    {
        var style = (Style)XamlReader.Parse(
            """
            <Style
                xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                xmlns:test="clr-namespace:Atc.Wpf.Tests.XUnitTestTypes;assembly=Atc.Wpf.Tests"
                TargetType="ToolTip">
                <Setter Property="Content" Value="{test:TestTextExtension}" />
            </Style>
            """);
        style.Seal();

        var exception = Record.Exception(TestTextExtension.Manager.UpdateAllTargets);

        Assert.Null(exception);
    }

    [StaFact]
    public void UpdateAllTargets_ExtensionOnAnElement_UpdatesTheElement()
    {
        TestTextExtension.Text = "Before";
        var textBlock = (TextBlock)XamlReader.Parse(
            """
            <TextBlock
                xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                xmlns:test="clr-namespace:Atc.Wpf.Tests.XUnitTestTypes;assembly=Atc.Wpf.Tests"
                Text="{test:TestTextExtension}" />
            """);

        TestTextExtension.Text = "After";
        TestTextExtension.Manager.UpdateAllTargets();

        Assert.Equal("After", textBlock.Text);
    }
}
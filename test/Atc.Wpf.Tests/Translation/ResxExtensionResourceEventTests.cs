namespace Atc.Wpf.Tests.Translation;

public sealed class ResxExtensionResourceEventTests
{
    [StaFact]
    public void Resource_handler_can_supply_the_value()
    {
        var textBlock = new TextBlock();
        var sut = new ResxExtension("Atc.Wpf.Tests.NoSuchResources", "Greeting")
        {
            Prefix = "[",
            Suffix = "]",
        };

        try
        {
            sut.Resource += (_, e) => e.Result = e.Key + " from handler";

            var actual = sut.ProvideValue(CreateServiceProvider(textBlock, TextBlock.TextProperty));

            Assert.Equal("[Greeting from handler]", actual);
        }
        finally
        {
            ResxExtension.MarkupManager.ActiveExtensions.Remove(sut);
        }
    }

    [StaFact]
    public void Resource_handler_receives_the_resx_name_and_key_and_falls_back_when_it_sets_no_result()
    {
        var textBlock = new TextBlock();
        var sut = new ResxExtension("Atc.Wpf.Tests.NoSuchResources", "Greeting");
        ResourceEventArgs? received = null;

        try
        {
            sut.Resource += (_, e) => received = e;

            var actual = sut.ProvideValue(CreateServiceProvider(textBlock, TextBlock.TextProperty));

            Assert.NotNull(received);
            Assert.Equal("Atc.Wpf.Tests.NoSuchResources", received.ResxName);
            Assert.Equal("Greeting", received.Key);
            Assert.Null(received.Result);
            Assert.Equal("Greeting", actual);
        }
        finally
        {
            ResxExtension.MarkupManager.ActiveExtensions.Remove(sut);
        }
    }

    private static IServiceProvider CreateServiceProvider(
        DependencyObject target,
        DependencyProperty property)
    {
        var provideValueTarget = Substitute.For<IProvideValueTarget>();
        provideValueTarget.TargetObject.Returns(target);
        provideValueTarget.TargetProperty.Returns(property);

        var serviceProvider = Substitute.For<IServiceProvider>();
        serviceProvider.GetService(typeof(IProvideValueTarget)).Returns(provideValueTarget);
        return serviceProvider;
    }
}
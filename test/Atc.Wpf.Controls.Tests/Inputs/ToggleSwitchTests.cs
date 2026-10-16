namespace Atc.Wpf.Controls.Tests.Inputs;

public sealed class ToggleSwitchTests : IDisposable
{
    private const string TemplateXaml = """
        <ControlTemplate
            xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
            xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
            xmlns:inputs="clr-namespace:Atc.Wpf.Controls.Inputs;assembly=Atc.Wpf.Controls"
            TargetType="inputs:ToggleSwitch">
            <StackPanel>
                <ContentPresenter x:Name="HeaderContentPresenter" ContentSource="Header" />
                <ContentPresenter x:Name="ContentPresenter" />
                <ContentPresenter x:Name="OffContentPresenter" Content="{TemplateBinding OffContent}" />
                <ContentPresenter x:Name="OnContentPresenter" Content="{TemplateBinding OnContent}" />
            </StackPanel>
        </ControlTemplate>
        """;

    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaTheory]
    [InlineData(null, Visibility.Collapsed)]
    [InlineData("", Visibility.Collapsed)]
    [InlineData("Header", Visibility.Visible)]
    public void Header_Text_SetsTheHeaderPresenterVisibility(
        string? header,
        Visibility expected)
    {
        var sut = CreateTemplated();

        sut.Header = header;

        Assert.Equal(expected, FindPresenter(sut, "HeaderContentPresenter").Visibility);
    }

    [StaFact]
    public void Header_Element_ShowsTheHeaderPresenter()
    {
        var sut = CreateTemplated();

        sut.Header = new Border();

        Assert.Equal(Visibility.Visible, FindPresenter(sut, "HeaderContentPresenter").Visibility);
    }

    [StaTheory]
    [InlineData(null)]
    [InlineData("")]
    public void Content_Empty_ShowsTheOnAndOffContentInsteadOfTheContent(
        string? content)
    {
        var sut = CreateTemplated();

        sut.Content = content;

        Assert.Equal(Visibility.Collapsed, FindPresenter(sut, "ContentPresenter").Visibility);
        Assert.Equal(Visibility.Visible, FindPresenter(sut, "OffContentPresenter").Visibility);
        Assert.Equal(Visibility.Visible, FindPresenter(sut, "OnContentPresenter").Visibility);
    }

    [StaFact]
    public void Content_Text_ShowsTheContentInsteadOfTheOnAndOffContent()
    {
        var sut = CreateTemplated();

        sut.Content = "Enable feature";

        Assert.Equal(Visibility.Visible, FindPresenter(sut, "ContentPresenter").Visibility);
        Assert.Equal(Visibility.Collapsed, FindPresenter(sut, "OffContentPresenter").Visibility);
        Assert.Equal(Visibility.Collapsed, FindPresenter(sut, "OnContentPresenter").Visibility);
    }

    private static ToggleSwitch CreateTemplated()
    {
        var toggleSwitch = new ToggleSwitch
        {
            Template = (ControlTemplate)XamlReader.Parse(TemplateXaml),
        };

        toggleSwitch.ApplyTemplate();
        return toggleSwitch;
    }

    private static ContentPresenter FindPresenter(
        ToggleSwitch toggleSwitch,
        string name)
        => (ContentPresenter)toggleSwitch.Template.FindName(name, toggleSwitch);
}
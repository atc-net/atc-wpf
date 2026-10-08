namespace Atc.Wpf.Controls.Tests;

/// <summary>
/// Photos and flags must not be mirrored in a right-to-left layout.
/// </summary>
public sealed class RightToLeftImageTests : IDisposable
{
    public RightToLeftImageTests()
    {
        // pack://application URIs need the Application type initialized.
        RuntimeHelpers.RunClassConstructor(typeof(Application).TypeHandle);
    }

    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void Avatar_ImageIsNotMirrored()
    {
        var resources = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/Atc.Wpf.Controls;component/DataDisplay/Avatar.xaml"),
        };
        var avatar = new Avatar { Style = (Style)resources["AtcApps.Styles.Avatar"], ImageSource = BitmapSource.Create(1, 1, 96, 96, PixelFormats.Bgra32, null, new byte[4], 4) };
        _ = new Border { FlowDirection = FlowDirection.RightToLeft, Child = avatar };

        avatar.Measure(new Size(100, 100));
        avatar.Arrange(new Rect(0, 0, 100, 100));

        var image = (Image)avatar.Template.FindName("PART_Image", avatar);
        Assert.Equal(FlowDirection.RightToLeft, avatar.FlowDirection);
        Assert.Equal(FlowDirection.RightToLeft, ((FrameworkElement)avatar.Template.FindName("PART_Initials", avatar)).FlowDirection);
        Assert.Equal(FlowDirection.LeftToRight, image.FlowDirection);
    }

    [StaFact]
    public void CountrySelector_FlagIsNotMirrored()
        => AssertFlagIsNotMirrored(new CountrySelector(), "CbCountries");

    [StaFact]
    public void LanguageSelector_FlagIsNotMirrored()
        => AssertFlagIsNotMirrored(new LanguageSelector(), "CbLanguages");

    private static void AssertFlagIsNotMirrored(
        UserControl selector,
        string comboBoxName)
    {
        var comboBox = (ComboBox)selector.FindName(comboBoxName);
        var item = (Panel)comboBox.ItemTemplate.LoadContent();

        _ = new Border { FlowDirection = FlowDirection.RightToLeft, Child = item };

        var flag = item.Children.OfType<Image>().Single();
        Assert.Equal(FlowDirection.RightToLeft, item.FlowDirection);
        Assert.Equal(FlowDirection.LeftToRight, flag.FlowDirection);
    }
}
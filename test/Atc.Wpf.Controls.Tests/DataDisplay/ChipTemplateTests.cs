namespace Atc.Wpf.Controls.Tests.DataDisplay;

[Collection(nameof(PackResourceTestGroup))]
public sealed class ChipTemplateTests : IDisposable
{
    public ChipTemplateTests()
    {
        // pack://application URIs need the Application type initialized.
        RuntimeHelpers.RunClassConstructor(typeof(Application).TypeHandle);
    }

    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void Check_mark_follows_the_Filter_variant_unless_ShowCheckMark_is_set()
    {
        var filter = Realize(new Chip { Variant = ChipVariant.Filter, IsSelected = true });
        var plain = Realize(new Chip { IsSelected = true });
        var forced = Realize(new Chip { IsSelected = true, ShowCheckMark = true });
        var hidden = Realize(new Chip { Variant = ChipVariant.Filter, IsSelected = true, ShowCheckMark = false });
        var unselected = Realize(new Chip { ShowCheckMark = true });

        Assert.Equal(Visibility.Visible, CheckMark(filter).Visibility);
        Assert.Equal(Visibility.Collapsed, CheckMark(plain).Visibility);
        Assert.Equal(Visibility.Visible, CheckMark(forced).Visibility);
        Assert.Equal(Visibility.Collapsed, CheckMark(hidden).Visibility);
        Assert.Equal(Visibility.Collapsed, CheckMark(unselected).Visibility);
    }

    [StaFact]
    public void Selected_border_uses_SelectedBorderBrush_when_set()
    {
        var accent = Brushes.Red;
        var selectedBackground = Brushes.Blue;
        var withBrush = Realize(new Chip { IsSelected = true, SelectedBackground = selectedBackground, SelectedBorderBrush = accent });
        var withoutBrush = Realize(new Chip { IsSelected = true, SelectedBackground = selectedBackground });

        Assert.Same(accent, Border(withBrush).BorderBrush);
        Assert.Same(selectedBackground, Border(withoutBrush).BorderBrush);
    }

    private static Chip Realize(Chip chip)
    {
        var dictionary = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/Atc.Wpf.Controls;component/DataDisplay/Chip.xaml", UriKind.Absolute),
        };
        chip.Style = (Style)dictionary["AtcApps.Styles.Chip"];
        chip.ApplyTemplate();
        chip.Measure(new Size(200, 50));
        chip.Arrange(new Rect(0, 0, 200, 50));
        chip.UpdateLayout();
        return chip;
    }

    private static System.Windows.Shapes.Path CheckMark(Chip chip)
        => (System.Windows.Shapes.Path)chip.Template.FindName("PART_Checkmark", chip);

    private static Border Border(Chip chip)
        => (Border)chip.Template.FindName("PART_Border", chip);
}
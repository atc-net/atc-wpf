namespace Atc.Wpf.Controls.Tests.DataDisplay;

[Collection(nameof(PackResourceTestGroup))]
public sealed class SegmentedTemplateTests : IDisposable
{
    public SegmentedTemplateTests()
    {
        // pack://application URIs need the Application type initialized.
        RuntimeHelpers.RunClassConstructor(typeof(Application).TypeHandle);
    }

    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void Vertical_stacks_the_segments_in_rows_with_separators_between()
    {
        var sut = Realize(Create(), x => x.Orientation = Orientation.Vertical);
        var host = (Grid)sut.Template.FindName("PART_ItemsHost", sut);

        Assert.Equal(5, host.RowDefinitions.Count);
        Assert.Empty(host.ColumnDefinitions);
        Assert.Equal(0, Grid.GetRow(sut.Items[0]));
        Assert.Equal(4, Grid.GetRow(sut.Items[2]));
        Assert.Equal(0, Grid.GetColumn(sut.Items[2]));
        Assert.True(sut.Items[2].TranslatePoint(default, sut).Y > sut.Items[0].TranslatePoint(default, sut).Y);
    }

    [StaFact]
    public void Separate_pills_drop_the_outer_shape_and_give_each_segment_a_border()
    {
        var sut = Realize(Create(), x => x.IsJoined = false);
        var outer = (Border)sut.Template.FindName("PART_Border", sut);
        var host = (Grid)sut.Template.FindName("PART_ItemsHost", sut);

        Assert.Equal(new Thickness(0), outer.BorderThickness);
        Assert.Equal(new Thickness(1), sut.Items[0].BorderThickness);
        Assert.Equal(3, host.Children.Count);
        Assert.Equal(4d, host.ColumnDefinitions[1].Width.Value);
    }

    [StaFact]
    public void Icon_only_hides_the_text_when_there_is_an_icon()
    {
        var sut = Create();
        sut.Items[0].Icon = "A";
        sut = Realize(sut, x => x.IconOnly = true);

        Assert.Equal(Visibility.Collapsed, Content(sut.Items[0]).Visibility);
        Assert.Equal(Visibility.Visible, Content(sut.Items[1]).Visibility);
    }

    [StaFact]
    public void Size_changes_font_size_and_padding_like_chip()
    {
        var small = Realize(Create(), x => x.Size = SegmentedSize.Small);
        var large = Realize(Create(), x => x.Size = SegmentedSize.Large);
        var medium = Realize(Create(), _ => { });

        Assert.Equal(11d, small.Items[0].FontSize);
        Assert.Equal(new Thickness(8, 4, 8, 4), small.Items[0].Padding);
        Assert.Equal(13d, medium.Items[0].FontSize);
        Assert.Equal(new Thickness(12, 6, 12, 6), medium.Items[0].Padding);
        Assert.Equal(15d, large.Items[0].FontSize);
        Assert.Equal(new Thickness(16, 8, 16, 8), large.Items[0].Padding);
    }

    private static Segmented Create()
    {
        var sut = new Segmented { SelectionMode = SegmentedSelectionMode.Multiple };
        sut.Items.Add(new SegmentedItem { Key = "a", Content = "Alpha" });
        sut.Items.Add(new SegmentedItem { Key = "b", Content = "Beta" });
        sut.Items.Add(new SegmentedItem { Key = "c", Content = "Gamma" });
        return sut;
    }

    private static Segmented Realize(
        Segmented sut,
        Action<Segmented> configure)
    {
        var dictionary = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/Atc.Wpf.Controls;component/DataDisplay/Segmented.xaml", UriKind.Absolute),
        };
        sut.Style = (Style)dictionary["AtcApps.Styles.Segmented"];
        var itemStyle = (Style)dictionary["AtcApps.Styles.SegmentedItem"];
        foreach (var item in sut.Items)
        {
            item.Style = itemStyle;
        }

        configure(sut);
        sut.ApplyTemplate();
        sut.Measure(new Size(400, 400));
        sut.Arrange(new Rect(0, 0, 400, 400));
        sut.UpdateLayout();
        return sut;
    }

    private static ContentPresenter Content(SegmentedItem item)
        => (ContentPresenter)item.Template.FindName("PART_Content", item);
}
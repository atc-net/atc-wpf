namespace Atc.Wpf.Theming.Tests.ValueConverters;

public sealed class TreeViewMarginValueConverterTests
{
    private readonly IValueConverter converter = new TreeViewMarginValueConverter();

    [Fact]
    public void Instance_Is_Not_Null()
        => Assert.NotNull(TreeViewMarginValueConverter.Instance);

    [Fact]
    public void Convert_Null_ReturnsZeroThickness()
        => Assert.Equal(
            new Thickness(0),
            converter.Convert(
                value: null,
                targetType: typeof(Thickness),
                parameter: null,
                culture: CultureInfo.InvariantCulture));

    [Fact]
    public void Convert_NonTreeViewItem_ReturnsZeroThickness()
        => Assert.Equal(
            new Thickness(0),
            converter.Convert(
                value: "NotATreeViewItem",
                targetType: typeof(Thickness),
                parameter: null,
                culture: CultureInfo.InvariantCulture));

    [StaFact]
    public void Convert_DepthZero_ReturnsZeroLeftThickness()
    {
        var instance = new TreeViewMarginValueConverter { Length = 12 };
        var item = new System.Windows.Controls.TreeViewItem();

        var result = (Thickness)instance.Convert(
            value: item,
            targetType: typeof(Thickness),
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        // Depth 0 (no parent) → left margin = 12 * 0 = 0.
        Assert.Equal(new Thickness(0, 0, 0, 0), result);
    }

    [Fact]
    public void Length_Property_Defaults_To_Zero()
        => Assert.Equal(
            0,
            new TreeViewMarginValueConverter().Length);

    [Fact]
    public void ConvertBack_ReturnsUnsetValue()
        => Assert.Equal(
            DependencyProperty.UnsetValue,
            converter.ConvertBack(
                value: new Thickness(4),
                targetType: typeof(TreeViewItem),
                parameter: null,
                culture: CultureInfo.InvariantCulture));
}
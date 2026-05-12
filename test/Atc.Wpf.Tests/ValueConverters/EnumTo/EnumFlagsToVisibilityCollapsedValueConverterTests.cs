// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Tests.ValueConverters;

public sealed class EnumFlagsToVisibilityCollapsedValueConverterTests
{
    [Flags]
    public enum TestPermissions
    {
        None = 0,
        Read = 1,
        Write = 2,
        Admin = 4,
    }

    private readonly IValueConverter converter = new EnumFlagsToVisibilityCollapsedValueConverter();

    [Fact]
    public void Instance_Is_Not_Null()
        => Assert.NotNull(EnumFlagsToVisibilityCollapsedValueConverter.Instance);

    [Fact]
    public void Convert_ValueHasFlag_ReturnsCollapsed()
        => Assert.Equal(
            Visibility.Collapsed,
            converter.Convert(
                value: TestPermissions.Read | TestPermissions.Write,
                targetType: null,
                parameter: TestPermissions.Read,
                culture: null));

    [Fact]
    public void Convert_ValueMissingFlag_ReturnsVisible()
        => Assert.Equal(
            Visibility.Visible,
            converter.Convert(
                value: TestPermissions.Read,
                targetType: null,
                parameter: TestPermissions.Admin,
                culture: null));

    [Fact]
    public void Convert_NullValue_ReturnsVisible()
        => Assert.Equal(
            Visibility.Visible,
            converter.Convert(
                value: null,
                targetType: null,
                parameter: TestPermissions.Read,
                culture: null));

    [Fact]
    public void Convert_NullParameter_ReturnsVisible()
        => Assert.Equal(
            Visibility.Visible,
            converter.Convert(
                value: TestPermissions.Read,
                targetType: null,
                parameter: null,
                culture: null));

    [Fact]
    public void ConvertBack_ThrowsNotSupportedException()
        => Assert.Throws<NotSupportedException>(() =>
            converter.ConvertBack(
                Visibility.Collapsed,
                targetType: null,
                parameter: null,
                culture: null));
}
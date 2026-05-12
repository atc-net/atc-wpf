// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Tests.ValueConverters;

public sealed class EnumFlagsToBoolValueConverterTests
{
    [Flags]
    public enum TestPermissions
    {
        None = 0,
        Read = 1,
        Write = 2,
        Execute = 4,
        Admin = 8,
    }

    private readonly IValueConverter converter = new EnumFlagsToBoolValueConverter();

    [Fact]
    public void Instance_Is_Not_Null()
        => Assert.NotNull(EnumFlagsToBoolValueConverter.Instance);

    [Fact]
    public void Convert_ValueHasSingleFlag_ReturnsTrue()
        => Assert.True(
            (bool)converter.Convert(
                value: TestPermissions.Read | TestPermissions.Write,
                targetType: null,
                parameter: TestPermissions.Read,
                culture: null));

    [Fact]
    public void Convert_ValueMissingFlag_ReturnsFalse()
        => Assert.False(
            (bool)converter.Convert(
                value: TestPermissions.Read,
                targetType: null,
                parameter: TestPermissions.Admin,
                culture: null));

    [Fact]
    public void Convert_ValueHasCombinedFlags_ReturnsTrue()
        => Assert.True(
            (bool)converter.Convert(
                value: TestPermissions.Read | TestPermissions.Write | TestPermissions.Admin,
                targetType: null,
                parameter: TestPermissions.Read | TestPermissions.Write,
                culture: null));

    [Fact]
    public void Convert_ValueMissingOneOfCombinedFlags_ReturnsFalse()
        => Assert.False(
            (bool)converter.Convert(
                value: TestPermissions.Read,
                targetType: null,
                parameter: TestPermissions.Read | TestPermissions.Write,
                culture: null));

    [Fact]
    public void Convert_StringParameter_SingleFlag_ReturnsTrue()
        => Assert.True(
            (bool)converter.Convert(
                value: TestPermissions.Read | TestPermissions.Write,
                targetType: null,
                parameter: "Read",
                culture: null));

    [Fact]
    public void Convert_StringParameter_CombinedFlags_ReturnsTrue()
        => Assert.True(
            (bool)converter.Convert(
                value: TestPermissions.Read | TestPermissions.Write | TestPermissions.Execute,
                targetType: null,
                parameter: "Read,Write",
                culture: null));

    [Fact]
    public void Convert_StringParameter_CaseInsensitive_ReturnsTrue()
        => Assert.True(
            (bool)converter.Convert(
                value: TestPermissions.Read,
                targetType: null,
                parameter: "read",
                culture: null));

    [Fact]
    public void Convert_InvalidStringParameter_ReturnsFalse()
        => Assert.False(
            (bool)converter.Convert(
                value: TestPermissions.Read,
                targetType: null,
                parameter: "NotAFlag",
                culture: null));

    [Fact]
    public void Convert_NullValue_ReturnsFalse()
        => Assert.False(
            (bool)converter.Convert(
                value: null,
                targetType: null,
                parameter: TestPermissions.Read,
                culture: null));

    [Fact]
    public void Convert_NullParameter_ReturnsFalse()
        => Assert.False(
            (bool)converter.Convert(
                value: TestPermissions.Read,
                targetType: null,
                parameter: null,
                culture: null));

    [Fact]
    public void Convert_NonEnumValue_ReturnsFalse()
        => Assert.False(
            (bool)converter.Convert(
                value: "NotAnEnum",
                targetType: null,
                parameter: TestPermissions.Read,
                culture: null));

    [Fact]
    public void ConvertBack_ThrowsNotSupportedException()
        => Assert.Throws<NotSupportedException>(() =>
            converter.ConvertBack(
                true,
                targetType: null,
                parameter: null,
                culture: null));
}
namespace Atc.Wpf.Forms.Tests.Internal.Helpers;

public sealed class MultiSelectPropertyHelperTests
{
    [Flags]
    [SuppressMessage("Design", "CA1028:Enum Storage should be Int32", Justification = "Tests the signed byte storage path.")]
    [SuppressMessage("Usage", "CA2217:Do not mark enums with FlagsAttribute", Justification = "Tests the sign bit.")]
    [SuppressMessage("Major Code Smell", "S4070:Non-flags enums should not be marked with \"FlagsAttribute\"", Justification = "Tests the sign bit.")]
    public enum SignedBits : sbyte
    {
        None = 0,
        Low = 1,
        High = sbyte.MinValue,
        All = Low | High,
    }

    [Fact]
    public void GetEnumItems_SignedBits_KeepsTheSignBitAndSkipsZeroAndCombinedValues()
    {
        var actual = MultiSelectPropertyHelper.GetEnumItems(typeof(SignedBits));

        Assert.Equal(2, actual.Count);
        Assert.Contains("Low", actual.Keys, StringComparer.Ordinal);
        Assert.Contains("High", actual.Keys, StringComparer.Ordinal);
    }

    [Fact]
    public void GetSelectedKeys_CombinedFlags_ReturnsTheSingleFlags()
    {
        var actual = MultiSelectPropertyHelper.GetSelectedKeys(SignedBits.All);

        Assert.Equal(2, actual.Count);
        Assert.Contains("Low", actual, StringComparer.Ordinal);
        Assert.Contains("High", actual, StringComparer.Ordinal);
    }

    [Theory]
    [InlineData(typeof(SignedBits))]
    [InlineData(typeof(SignedBits?))]
    public void CombineFlags_Keys_ReturnsTheCombinedValue(Type flagsType)
    {
        Assert.Equal(SignedBits.All, MultiSelectPropertyHelper.CombineFlags(flagsType, ["Low", "High"]));
        Assert.Equal(SignedBits.None, MultiSelectPropertyHelper.CombineFlags(flagsType, []));
        Assert.Equal(SignedBits.Low, MultiSelectPropertyHelper.CombineFlags(flagsType, ["Low", "Unknown"]));
    }

    [Theory]
    [InlineData(typeof(List<string>), typeof(string))]
    [InlineData(typeof(string[]), typeof(string))]
    [InlineData(typeof(IReadOnlyList<DayOfWeek>), typeof(DayOfWeek))]
    [InlineData(typeof(HashSet<DayOfWeek>), typeof(DayOfWeek))]
    [InlineData(typeof(ISet<string>), typeof(string))]
    public void TryGetCollectionElementType_SupportedCollection_ReturnsTheElementType(
        Type type,
        Type expected)
    {
        Assert.True(MultiSelectPropertyHelper.TryGetCollectionElementType(type, out var actual));
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(typeof(string))]
    [InlineData(typeof(List<int>))]
    [InlineData(typeof(List<DayOfWeek?>))]
    [InlineData(typeof(Dictionary<string, string>))]
    [InlineData(typeof(string[,]))]
    [InlineData(typeof(System.Collections.ObjectModel.Collection<string>))]
    public void TryGetCollectionElementType_UnsupportedType_ReturnsFalse(
        Type type)
        => Assert.False(MultiSelectPropertyHelper.TryGetCollectionElementType(type, out _));
}
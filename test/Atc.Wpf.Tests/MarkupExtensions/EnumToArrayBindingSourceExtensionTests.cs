namespace Atc.Wpf.Tests.MarkupExtensions;

public sealed class EnumToArrayBindingSourceExtensionTests
{
    [Fact]
    public void ProvideValue_for_a_plain_enum_returns_each_value_once()
    {
        var actual = (Array)new EnumToArrayBindingSourceExtension(typeof(DayOfWeek)).ProvideValue(serviceProvider: null!);

        Assert.NotEmpty(actual);
        Assert.Equal(actual.Length, actual.Cast<object>().Distinct().Count());
        Assert.DoesNotContain(null, actual.Cast<object?>());
    }

    [Fact]
    public void ProvideValue_for_a_nullable_enum_starts_with_a_null_slot_followed_by_each_value_once()
    {
        var plain = (Array)new EnumToArrayBindingSourceExtension(typeof(DayOfWeek)).ProvideValue(serviceProvider: null!);

        var actual = (Array)new EnumToArrayBindingSourceExtension(typeof(DayOfWeek?)).ProvideValue(serviceProvider: null!);

        Assert.Equal(plain.Length + 1, actual.Length);
        Assert.Null(actual.GetValue(0));
        Assert.Equal(plain.Cast<object>(), actual.Cast<object?>().Skip(1));
    }

    [Fact]
    public void ProvideValue_for_a_nullable_enum_does_not_repeat_the_first_member()
    {
        var actual = (Array)new EnumToArrayBindingSourceExtension(typeof(DayOfWeek?)).ProvideValue(serviceProvider: null!);

        var values = actual.Cast<object?>().Where(v => v is not null).ToList();

        Assert.Equal(values.Count, values.Distinct().Count());
    }

    [Fact]
    public void ProvideValue_throws_when_no_enum_type_is_given()
    {
        var sut = new EnumToArrayBindingSourceExtension();

        Assert.Throws<InvalidOperationException>(() => sut.ProvideValue(serviceProvider: null!));
    }
}
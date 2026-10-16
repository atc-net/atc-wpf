namespace Atc.Wpf.Forms.Tests.Internal.Helpers;

public sealed class DateTimeTextBoxHelperTests
{
    [Fact]
    public void GetSelectedDateAsText_Short_UsesThePickerCultureForPatternAndSeparators()
    {
        var threadCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("da-DK");

            var actual = DateTimeTextBoxHelper.GetSelectedDateAsText(
                new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Unspecified),
                DatePickerFormat.Short,
                CultureInfo.GetCultureInfo("en-US"));

            Assert.Equal("1/2/2026", actual);
        }
        finally
        {
            CultureInfo.CurrentCulture = threadCulture;
        }
    }

    [Fact]
    public void GetSelectedDateAsText_Short_RoundTripsThroughThePickerCultureParser()
    {
        var threadCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("da-DK");
            var pickerCulture = CultureInfo.GetCultureInfo("en-US");
            var expected = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Unspecified);

            var text = DateTimeTextBoxHelper.GetSelectedDateAsText(expected, DatePickerFormat.Short, pickerCulture);
            var parsed = DateTimeTextBoxHelper.TryParseUsingSpecificCulture(DatePickerFormat.Short, text, pickerCulture, out var actual);

            Assert.True(parsed);
            Assert.Equal(expected, actual);
        }
        finally
        {
            CultureInfo.CurrentCulture = threadCulture;
        }
    }
}
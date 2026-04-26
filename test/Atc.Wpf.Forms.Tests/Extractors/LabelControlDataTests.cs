namespace Atc.Wpf.Forms.Tests.Extractors;

public sealed class LabelControlDataTests
{
    [Fact]
    public void Constructor_assigns_required_DataType_and_LabelText()
    {
        var sut = new LabelControlData(typeof(int), "Age");

        Assert.Equal(typeof(int), sut.DataType);
        Assert.Equal("Age", sut.LabelText);
    }

    [Fact]
    public void Optional_properties_default_to_null_or_false()
    {
        var sut = new LabelControlData(typeof(string), "Name");

        Assert.Null(sut.WatermarkText);
        Assert.Null(sut.Value);
        Assert.Null(sut.Value2);
        Assert.False(sut.IsReadOnly);
        Assert.False(sut.IsMandatory);
        Assert.Null(sut.Minimum);
        Assert.Null(sut.Maximum);
        Assert.Null(sut.MinimumDateTime);
        Assert.Null(sut.MaximumDateTime);
        Assert.Null(sut.RegexPattern);
    }

    [Fact]
    public void Mutable_properties_round_trip_assignments()
    {
        var sut = new LabelControlData(typeof(decimal), "Amount")
        {
            WatermarkText = "0.00",
            Value = 42.5m,
            Value2 = "x",
            IsReadOnly = true,
            IsMandatory = true,
            Minimum = 0m,
            Maximum = 99m,
            MinimumDateTime = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            MaximumDateTime = new DateTime(2030, 12, 31, 0, 0, 0, DateTimeKind.Utc),
            RegexPattern = @"^\d+(\.\d{1,2})?$",
        };

        Assert.Equal("0.00", sut.WatermarkText);
        Assert.Equal(42.5m, sut.Value);
        Assert.Equal("x", sut.Value2);
        Assert.True(sut.IsReadOnly);
        Assert.True(sut.IsMandatory);
        Assert.Equal(0m, sut.Minimum);
        Assert.Equal(99m, sut.Maximum);
        Assert.Equal(new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc), sut.MinimumDateTime);
        Assert.Equal(new DateTime(2030, 12, 31, 0, 0, 0, DateTimeKind.Utc), sut.MaximumDateTime);
        Assert.Equal(@"^\d+(\.\d{1,2})?$", sut.RegexPattern);
    }

    [Fact]
    public void ToString_includes_DataType_and_LabelText()
    {
        var sut = new LabelControlData(typeof(int), "Age");

        var actual = sut.ToString();

        Assert.Contains(nameof(LabelControlData.DataType), actual, StringComparison.Ordinal);
        Assert.Contains("Age", actual, StringComparison.Ordinal);
        Assert.Contains("Int32", actual, StringComparison.Ordinal);
    }
}
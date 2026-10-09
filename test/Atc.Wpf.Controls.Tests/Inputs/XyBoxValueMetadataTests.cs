namespace Atc.Wpf.Controls.Tests.Inputs;

public sealed class XyBoxValueMetadataTests
{
    public static TheoryData<DependencyProperty, Type, UpdateSourceTrigger> ValueProperties
        => new()
        {
            { DecimalXyBox.ValueXProperty, typeof(DecimalXyBox), UpdateSourceTrigger.LostFocus },
            { DecimalXyBox.ValueYProperty, typeof(DecimalXyBox), UpdateSourceTrigger.LostFocus },
            { IntegerXyBox.ValueXProperty, typeof(IntegerXyBox), UpdateSourceTrigger.LostFocus },
            { IntegerXyBox.ValueYProperty, typeof(IntegerXyBox), UpdateSourceTrigger.LostFocus },
            { PixelSizeBox.ValueWidthProperty, typeof(PixelSizeBox), UpdateSourceTrigger.PropertyChanged },
            { PixelSizeBox.ValueHeightProperty, typeof(PixelSizeBox), UpdateSourceTrigger.PropertyChanged },
        };

    [Theory]
    [MemberData(nameof(ValueProperties))]
    public void ValueProperty_HasTheExpectedMetadata(
        DependencyProperty property,
        Type ownerType,
        UpdateSourceTrigger expectedTrigger)
    {
        ArgumentNullException.ThrowIfNull(property);

        var metadata = Assert.IsType<FrameworkPropertyMetadata>(property.GetMetadata(ownerType));

        Assert.True(metadata.BindsTwoWayByDefault);
        Assert.True(metadata.Journal);
        Assert.True(metadata.IsAnimationProhibited);
        Assert.Equal(expectedTrigger, metadata.DefaultUpdateSourceTrigger);
        Assert.Null(metadata.PropertyChangedCallback);
    }
}
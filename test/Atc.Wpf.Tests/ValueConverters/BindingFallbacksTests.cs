// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Tests.ValueConverters;

[Collection("Sequential")]
public sealed class BindingFallbacksTests
{
    [Fact]
    public void DefaultColor_Is_DeepPink()
        => Assert.Equal(Colors.DeepPink, BindingFallbacks.DefaultColor);

    [Fact]
    public void Color_Defaults_To_DefaultColor()
    {
        try
        {
            BindingFallbacks.Reset();
            Assert.Equal(BindingFallbacks.DefaultColor, BindingFallbacks.Color);
        }
        finally
        {
            BindingFallbacks.Reset();
        }
    }

    [Fact]
    public void Brush_Is_Frozen()
        => Assert.True(BindingFallbacks.Brush.IsFrozen);

    [Fact]
    public void Brush_MatchesColor()
    {
        try
        {
            BindingFallbacks.Reset();
            Assert.Equal(BindingFallbacks.Color, BindingFallbacks.Brush.Color);
        }
        finally
        {
            BindingFallbacks.Reset();
        }
    }

    [Fact]
    public void Brush_RebuildsWhenColorChanges()
    {
        var before = BindingFallbacks.Brush;
        try
        {
            BindingFallbacks.Color = Colors.HotPink;
            var after = BindingFallbacks.Brush;

            Assert.NotEqual(before.Color, after.Color);
            Assert.Equal(Colors.HotPink, after.Color);
            Assert.True(after.IsFrozen);
        }
        finally
        {
            BindingFallbacks.Reset();
        }
    }

    [Fact]
    public void Reset_Restores_DefaultColor()
    {
        BindingFallbacks.Color = Colors.Black;
        BindingFallbacks.Reset();
        Assert.Equal(BindingFallbacks.DefaultColor, BindingFallbacks.Color);
    }

    [Fact]
    public void Override_PropagatesToBrushToColorConverter()
    {
        try
        {
            BindingFallbacks.Color = Colors.HotPink;

            var actual = new BrushToColorValueConverter().Convert(
                value: null,
                targetType: typeof(Color),
                parameter: null,
                culture: CultureInfo.InvariantCulture);

            Assert.Equal(Colors.HotPink, actual);
        }
        finally
        {
            BindingFallbacks.Reset();
        }
    }

    [Fact]
    public void Override_PropagatesToColorToBrushConverter()
    {
        try
        {
            BindingFallbacks.Color = Colors.HotPink;

            var actual = new ColorToBrushValueConverter().Convert(
                value: null,
                targetType: typeof(SolidColorBrush),
                parameter: null,
                culture: CultureInfo.InvariantCulture);

            var brush = Assert.IsType<SolidColorBrush>(actual);
            Assert.Equal(Colors.HotPink, brush.Color);
        }
        finally
        {
            BindingFallbacks.Reset();
        }
    }

    [Fact]
    public void LogLevel_FallbackColor_Delegates_To_BindingFallbacks_Get()
    {
        try
        {
            BindingFallbacks.Color = Colors.HotPink;
            Assert.Equal(Colors.HotPink, LogLevelToColorValueConverter.FallbackColor);
        }
        finally
        {
            BindingFallbacks.Reset();
        }
    }

    [Fact]
    public void LogLevel_FallbackColor_Delegates_To_BindingFallbacks_Set()
    {
        try
        {
            LogLevelToColorValueConverter.FallbackColor = Colors.HotPink;
            Assert.Equal(Colors.HotPink, BindingFallbacks.Color);
        }
        finally
        {
            BindingFallbacks.Reset();
        }
    }
}
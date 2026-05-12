namespace Atc.Wpf.Tests.ValueConverters;

[Collection("Sequential")]
public sealed class BackgroundToForegroundValueConverterTests
{
    [Fact]
    public void Convert_returns_white_brush_for_dark_background()
    {
        var darkBackground = new SolidColorBrush(Colors.Black);

        var actual = BackgroundToForegroundValueConverter.Instance.Convert(
            darkBackground,
            typeof(SolidColorBrush),
            parameter: null,
            CultureInfo.InvariantCulture);

        var brush = Assert.IsType<SolidColorBrush>(actual);
        Assert.Equal(Colors.White, brush.Color);
    }

    [Fact]
    public void Convert_returns_black_brush_for_light_background()
    {
        var lightBackground = new SolidColorBrush(Colors.White);

        var actual = BackgroundToForegroundValueConverter.Instance.Convert(
            lightBackground,
            typeof(SolidColorBrush),
            parameter: null,
            CultureInfo.InvariantCulture);

        var brush = Assert.IsType<SolidColorBrush>(actual);
        Assert.Equal(Colors.Black, brush.Color);
    }

    [Fact]
    public void Convert_returns_frozen_brush_so_it_can_be_safely_shared()
    {
        var actual = BackgroundToForegroundValueConverter.Instance.Convert(
            new SolidColorBrush(Colors.Red),
            typeof(SolidColorBrush),
            parameter: null,
            CultureInfo.InvariantCulture);

        var brush = Assert.IsType<SolidColorBrush>(actual);
        Assert.True(brush.IsFrozen);
    }

    [Fact]
    public void Convert_returns_white_brush_for_non_brush_input()
    {
        var actual = BackgroundToForegroundValueConverter.Instance.Convert(
            "not a brush",
            typeof(SolidColorBrush),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Same(Brushes.White, actual);
    }

    [Fact]
    public void ConvertBack_returns_DependencyProperty_UnsetValue()
    {
        var actual = BackgroundToForegroundValueConverter.Instance.ConvertBack(
            Brushes.White,
            typeof(SolidColorBrush),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Same(DependencyProperty.UnsetValue, actual);
    }

    [Fact]
    public void Convert_multi_returns_explicit_title_brush_when_provided()
    {
        var background = new SolidColorBrush(Colors.Black);
        var explicitTitle = new SolidColorBrush(Colors.Yellow);

        var actual = BackgroundToForegroundValueConverter.Instance.Convert(
            new object[] { background, explicitTitle },
            typeof(SolidColorBrush),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Same(explicitTitle, actual);
    }

    [Fact]
    public void Convert_multi_falls_back_to_ideal_text_color_when_title_brush_is_null()
    {
        var background = new SolidColorBrush(Colors.Black);

        var actual = BackgroundToForegroundValueConverter.Instance.Convert(
            new object?[] { background, null }!,
            typeof(SolidColorBrush),
            parameter: null,
            CultureInfo.InvariantCulture);

        var brush = Assert.IsType<SolidColorBrush>(actual);
        Assert.Equal(Colors.White, brush.Color);
    }

    [Fact]
    public void ConvertBack_multi_returns_one_UnsetValue_per_targetType()
    {
        var actual = BackgroundToForegroundValueConverter.Instance.ConvertBack(
            Brushes.White,
            [typeof(SolidColorBrush), typeof(SolidColorBrush)],
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.NotNull(actual);
        Assert.Equal(2, actual.Length);
        Assert.All(actual, x => Assert.Same(DependencyProperty.UnsetValue, x));
    }

    [Fact]
    public void ConvertBack_multi_returns_empty_array_when_targetTypes_is_empty()
    {
        var actual = BackgroundToForegroundValueConverter.Instance.ConvertBack(
            Brushes.White,
            [],
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.NotNull(actual);
        Assert.Empty(actual);
    }

    [Fact]
    public void Defaults_Match_BuiltInValues()
    {
        try
        {
            BackgroundToForegroundValueConverter.ResetToDefaults();

            Assert.Equal(86, BackgroundToForegroundValueConverter.LuminanceThreshold);
            Assert.Equal(Colors.Black, BackgroundToForegroundValueConverter.DarkForegroundColor);
            Assert.Equal(Colors.White, BackgroundToForegroundValueConverter.LightForegroundColor);
        }
        finally
        {
            BackgroundToForegroundValueConverter.ResetToDefaults();
        }
    }

    [Fact]
    public void Convert_RespectsOverriddenDarkForeground()
    {
        try
        {
            BackgroundToForegroundValueConverter.DarkForegroundColor = Colors.DarkBlue;

            var result = (SolidColorBrush)BackgroundToForegroundValueConverter.Instance.Convert(
                new SolidColorBrush(Colors.White),
                typeof(SolidColorBrush),
                parameter: null,
                CultureInfo.InvariantCulture)!;

            Assert.Equal(Colors.DarkBlue, result.Color);
        }
        finally
        {
            BackgroundToForegroundValueConverter.ResetToDefaults();
        }
    }

    [Fact]
    public void Convert_RespectsOverriddenLightForeground()
    {
        try
        {
            BackgroundToForegroundValueConverter.LightForegroundColor = Colors.LightYellow;

            var result = (SolidColorBrush)BackgroundToForegroundValueConverter.Instance.Convert(
                new SolidColorBrush(Colors.Black),
                typeof(SolidColorBrush),
                parameter: null,
                CultureInfo.InvariantCulture)!;

            Assert.Equal(Colors.LightYellow, result.Color);
        }
        finally
        {
            BackgroundToForegroundValueConverter.ResetToDefaults();
        }
    }

    [Fact]
    public void Convert_RespectsOverriddenLuminanceThreshold()
    {
        var midGrey = new SolidColorBrush(Color.FromRgb(150, 150, 150));
        try
        {
            BackgroundToForegroundValueConverter.ResetToDefaults();
            var defaultThresholdResult = (SolidColorBrush)BackgroundToForegroundValueConverter.Instance.Convert(
                midGrey,
                typeof(SolidColorBrush),
                parameter: null,
                CultureInfo.InvariantCulture)!;

            BackgroundToForegroundValueConverter.LuminanceThreshold = 200;
            var raisedThresholdResult = (SolidColorBrush)BackgroundToForegroundValueConverter.Instance.Convert(
                midGrey,
                typeof(SolidColorBrush),
                parameter: null,
                CultureInfo.InvariantCulture)!;

            Assert.NotEqual(defaultThresholdResult.Color, raisedThresholdResult.Color);
        }
        finally
        {
            BackgroundToForegroundValueConverter.ResetToDefaults();
        }
    }

    [Fact]
    public void ResetToDefaults_RestoresBuiltInValues()
    {
        BackgroundToForegroundValueConverter.LuminanceThreshold = 200;
        BackgroundToForegroundValueConverter.DarkForegroundColor = Colors.Red;
        BackgroundToForegroundValueConverter.LightForegroundColor = Colors.Green;

        BackgroundToForegroundValueConverter.ResetToDefaults();

        Assert.Equal(BackgroundToForegroundValueConverter.DefaultLuminanceThreshold, BackgroundToForegroundValueConverter.LuminanceThreshold);
        Assert.Equal(BackgroundToForegroundValueConverter.DefaultDarkForegroundColor, BackgroundToForegroundValueConverter.DarkForegroundColor);
        Assert.Equal(BackgroundToForegroundValueConverter.DefaultLightForegroundColor, BackgroundToForegroundValueConverter.LightForegroundColor);
    }
}
namespace Atc.Wpf.Tests.ValueConverters;

public sealed class WindowResizeModeMinMaxButtonVisibilityMultiValueConverterTests
{
    private static readonly WindowResizeModeMinMaxButtonVisibilityMultiValueConverter Sut
        = WindowResizeModeMinMaxButtonVisibilityMultiValueConverter.Instance;

    [Fact]
    public void Convert_returns_Visible_when_values_is_null_as_a_safe_fallback()
    {
        var actual = Sut.Convert(
            values: null,
            typeof(Visibility),
            WindowResizeModeButtonType.Min,
            CultureInfo.InvariantCulture);

        Assert.Equal(Visibility.Visible, actual);
    }

    [Fact]
    public void Convert_returns_Visible_when_parameter_is_not_a_button_type()
    {
        var actual = Sut.Convert(
            new object[] { true, false, ResizeMode.CanResize },
            typeof(Visibility),
            parameter: "not-a-button-type",
            CultureInfo.InvariantCulture);

        Assert.Equal(Visibility.Visible, actual);
    }

    [Theory]
    [InlineData(true, false, Visibility.Visible)]
    [InlineData(false, false, Visibility.Collapsed)]
    [InlineData(true, true, Visibility.Collapsed)]
    [InlineData(false, true, Visibility.Collapsed)]
    public void Convert_close_button_obeys_showButton_and_useNoneStyle(
        bool showButton,
        bool useNoneWindowStyle,
        Visibility expected)
    {
        var actual = Sut.Convert(
            new object[] { showButton, useNoneWindowStyle, ResizeMode.CanResize },
            typeof(Visibility),
            WindowResizeModeButtonType.Close,
            CultureInfo.InvariantCulture);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(WindowResizeModeButtonType.Min)]
    [InlineData(WindowResizeModeButtonType.Max)]
    public void Convert_min_max_buttons_collapse_when_resize_mode_is_NoResize(
        WindowResizeModeButtonType which)
    {
        var actual = Sut.Convert(
            new object[] { true, false, ResizeMode.NoResize },
            typeof(Visibility),
            which,
            CultureInfo.InvariantCulture);

        Assert.Equal(Visibility.Collapsed, actual);
    }

    [Fact]
    public void Convert_max_button_collapses_when_resize_mode_is_CanMinimize()
    {
        var actual = Sut.Convert(
            new object[] { true, false, ResizeMode.CanMinimize },
            typeof(Visibility),
            WindowResizeModeButtonType.Max,
            CultureInfo.InvariantCulture);

        Assert.Equal(Visibility.Collapsed, actual);
    }

    [Fact]
    public void Convert_min_button_is_visible_when_resize_mode_is_CanMinimize()
    {
        var actual = Sut.Convert(
            new object[] { true, false, ResizeMode.CanMinimize },
            typeof(Visibility),
            WindowResizeModeButtonType.Min,
            CultureInfo.InvariantCulture);

        Assert.Equal(Visibility.Visible, actual);
    }

    [Theory]
    [InlineData(ResizeMode.CanResize)]
    [InlineData(ResizeMode.CanResizeWithGrip)]
    public void Convert_min_button_is_visible_when_resize_mode_supports_resize(
        ResizeMode resizeMode)
    {
        var actual = Sut.Convert(
            new object[] { true, false, resizeMode },
            typeof(Visibility),
            WindowResizeModeButtonType.Min,
            CultureInfo.InvariantCulture);

        Assert.Equal(Visibility.Visible, actual);
    }

    [Fact]
    public void ConvertBack_returns_one_UnsetValue_per_targetType()
    {
        var actual = Sut.ConvertBack(
            Visibility.Visible,
            [typeof(bool), typeof(bool), typeof(ResizeMode)],
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Equal(3, actual.Length);
        Assert.All(actual, x => Assert.Same(DependencyProperty.UnsetValue, x));
    }
}
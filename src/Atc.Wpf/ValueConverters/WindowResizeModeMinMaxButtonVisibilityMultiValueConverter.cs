namespace Atc.Wpf.ValueConverters;

/// <summary>
/// MultiValueConverter: show-button flag, use-none-window-style flag and <see cref="ResizeMode"/> To the <see cref="Visibility"/> of the window button given by the <see cref="WindowResizeModeButtonType"/> parameter.
/// </summary>
public sealed class WindowResizeModeMinMaxButtonVisibilityMultiValueConverter : IMultiValueConverter
{
    /// <summary>
    /// Gets a static default instance of <see cref="WindowResizeModeMinMaxButtonVisibilityMultiValueConverter"/>.
    /// </summary>
    public static readonly WindowResizeModeMinMaxButtonVisibilityMultiValueConverter Instance = new();

    /// <inheritdoc />
    public object Convert(
        object[]? values,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (values is null || parameter is not WindowResizeModeButtonType whichButton)
        {
            return Visibility.Visible;
        }

        var showButton = (values.ElementAtOrDefault(0) as bool?)
            .GetValueOrDefault(defaultValue: true);
        var useNoneWindowStyle = (values.ElementAtOrDefault(1) as bool?)
            .GetValueOrDefault(defaultValue: false);

        if (whichButton == WindowResizeModeButtonType.Close)
        {
            return useNoneWindowStyle || !showButton
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        var windowResizeMode = (values.ElementAtOrDefault(2) as ResizeMode?)
            .GetValueOrDefault(ResizeMode.CanResize);

        switch (windowResizeMode)
        {
            case ResizeMode.NoResize:
                return Visibility.Collapsed;
            case ResizeMode.CanMinimize:
                if (whichButton == WindowResizeModeButtonType.Min)
                {
                    return useNoneWindowStyle || !showButton
                        ? Visibility.Collapsed
                        : Visibility.Visible;
                }

                return Visibility.Collapsed;
            case ResizeMode.CanResize:
            case ResizeMode.CanResizeWithGrip:
            default:
                return useNoneWindowStyle || !showButton
                    ? Visibility.Collapsed
                    : Visibility.Visible;
        }
    }

    /// <inheritdoc />
    public object[] ConvertBack(
        object? value,
        Type[] targetTypes,
        object? parameter,
        CultureInfo culture)
        => targetTypes
            .Select(_ => DependencyProperty.UnsetValue)
            .ToArray();
}
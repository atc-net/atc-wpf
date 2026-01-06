namespace Atc.Wpf.ValueConverters;

/// <summary>
/// Base class for value converters that can be used as markup extensions.
/// Provides exception handling that logs errors and returns <see cref="DependencyProperty.UnsetValue"/>.
/// </summary>
[SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Converter exceptions should not crash the application.")]
[MarkupExtensionReturnType(typeof(IValueConverter))]
public abstract class MarkupValueConverterBase : MarkupExtension, IValueConverter
{
    /// <inheritdoc />
    public override object ProvideValue(IServiceProvider serviceProvider)
        => this;

    /// <summary>
    /// Converts a value. Override this method to implement conversion logic.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
    protected abstract object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture);

    /// <summary>
    /// Converts a value back. Override this method to implement reverse conversion logic.
    /// </summary>
    /// <param name="value">The value to convert back.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
    protected abstract object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture);

    /// <inheritdoc />
    object? IValueConverter.Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        try
        {
            return Convert(value, targetType, parameter, culture);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[{GetType().Name}] Convert failed: {ex.Message}");
            return DependencyProperty.UnsetValue;
        }
    }

    /// <inheritdoc />
    object? IValueConverter.ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        try
        {
            return ConvertBack(value, targetType, parameter, culture);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[{GetType().Name}] ConvertBack failed: {ex.Message}");
            return DependencyProperty.UnsetValue;
        }
    }
}
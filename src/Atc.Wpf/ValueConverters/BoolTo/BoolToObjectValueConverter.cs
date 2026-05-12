// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Bool To Object — returns <see cref="TrueValue"/> when value is
/// <see langword="true"/>, otherwise <see cref="FalseValue"/>.
/// </summary>
/// <remarks>
/// Unlike most converters in this library, this type is <b>stateful</b> — it has
/// per-instance configurable values. Use it as a <c>StaticResource</c> rather than via a
/// shared <c>Instance</c>. Lets you pick any two values (string, Brush, Color, double, etc.)
/// without writing a one-off converter.
/// </remarks>
/// <example>
/// <code>
/// &lt;UserControl.Resources&gt;
///     &lt;atcValueConverters:BoolToObjectValueConverter x:Key="StatusText"
///         TrueValue="Online" FalseValue="Offline" /&gt;
/// &lt;/UserControl.Resources&gt;
///
/// &lt;TextBlock Text="{Binding IsConnected, Converter={StaticResource StatusText}}" /&gt;
/// </code>
/// </example>
[ValueConversion(typeof(bool), typeof(object))]
public sealed class BoolToObjectValueConverter : DependencyObject, IValueConverter
{
    /// <summary>Identifies the <see cref="TrueValue"/> dependency property.</summary>
    public static readonly DependencyProperty TrueValueProperty = DependencyProperty.Register(
        nameof(TrueValue),
        typeof(object),
        typeof(BoolToObjectValueConverter),
        new PropertyMetadata(defaultValue: null));

    /// <summary>Identifies the <see cref="FalseValue"/> dependency property.</summary>
    public static readonly DependencyProperty FalseValueProperty = DependencyProperty.Register(
        nameof(FalseValue),
        typeof(object),
        typeof(BoolToObjectValueConverter),
        new PropertyMetadata(defaultValue: null));

    /// <summary>Value returned when the bound source is <see langword="true"/>.</summary>
    public object? TrueValue
    {
        get => GetValue(TrueValueProperty);
        set => SetValue(TrueValueProperty, value);
    }

    /// <summary>Value returned when the bound source is <see langword="false"/> or any other value.</summary>
    public object? FalseValue
    {
        get => GetValue(FalseValueProperty);
        set => SetValue(FalseValueProperty, value);
    }

    /// <inheritdoc />
    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value is true
            ? TrueValue
            : FalseValue;

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}
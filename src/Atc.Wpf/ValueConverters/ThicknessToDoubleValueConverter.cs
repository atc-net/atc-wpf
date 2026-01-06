namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Thickness To Double (extracts one side).
/// </summary>
/// <remarks>
/// <para>Supports two-way binding when <see cref="TakeThicknessSide"/> is set.</para>
/// <para>Convert: Thickness → double (specified side)</para>
/// <para>ConvertBack: double → Thickness (applies to specified side, others are 0)</para>
/// </remarks>
[ValueConversion(typeof(Thickness), typeof(double), ParameterType = typeof(ThicknessSideType))]
public sealed class ThicknessToDoubleValueConverter : IValueConverter
{
    public static readonly ThicknessToDoubleValueConverter Instance = new();

    /// <summary>
    /// Gets or sets which side of the Thickness to extract/apply.
    /// </summary>
    public ThicknessSideType TakeThicknessSide { get; set; } = ThicknessSideType.None;

    /// <inheritdoc />
    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not Thickness thickness)
        {
            return 0d;
        }

        var takeThicknessSide = GetThicknessSide(parameter);

        return takeThicknessSide switch
        {
            ThicknessSideType.Left => thickness.Left,
            ThicknessSideType.Top => thickness.Top,
            ThicknessSideType.Right => thickness.Right,
            ThicknessSideType.Bottom => thickness.Bottom,
            _ => 0d,
        };
    }

    /// <inheritdoc />
    public object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not double doubleValue)
        {
            return default(Thickness);
        }

        var takeThicknessSide = GetThicknessSide(parameter);

        return takeThicknessSide switch
        {
            ThicknessSideType.Left => new Thickness(doubleValue, 0, 0, 0),
            ThicknessSideType.Top => new Thickness(0, doubleValue, 0, 0),
            ThicknessSideType.Right => new Thickness(0, 0, doubleValue, 0),
            ThicknessSideType.Bottom => new Thickness(0, 0, 0, doubleValue),
            _ => default(Thickness),
        };
    }

    private ThicknessSideType GetThicknessSide(object? parameter)
        => parameter is ThicknessSideType sideType
            ? sideType
            : TakeThicknessSide;
}
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Thickness To Double (extracts one side).
/// </summary>
/// <remarks>
/// <para>Supports two-way binding when <see cref="TakeThicknessSide"/> is set.</para>
/// <para>Convert: Thickness → double (specified side)</para>
/// <para>ConvertBack: double → Thickness (applies to specified side, others are 0)</para>
/// </remarks>
[ValueConversion(typeof(Thickness), typeof(double), ParameterType = typeof(LeftTopRightBottomType))]
public sealed class ThicknessToDoubleValueConverter : IValueConverter
{
    public static readonly ThicknessToDoubleValueConverter Instance = new();

    /// <summary>
    /// Gets or sets which side of the Thickness to extract/apply.
    /// </summary>
    public LeftTopRightBottomType TakeThicknessSide { get; set; } = LeftTopRightBottomType.None;

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
            LeftTopRightBottomType.Left => thickness.Left,
            LeftTopRightBottomType.Top => thickness.Top,
            LeftTopRightBottomType.Right => thickness.Right,
            LeftTopRightBottomType.Bottom => thickness.Bottom,
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
            LeftTopRightBottomType.Left => new Thickness(doubleValue, 0, 0, 0),
            LeftTopRightBottomType.Top => new Thickness(0, doubleValue, 0, 0),
            LeftTopRightBottomType.Right => new Thickness(0, 0, doubleValue, 0),
            LeftTopRightBottomType.Bottom => new Thickness(0, 0, 0, doubleValue),
            _ => default(Thickness),
        };
    }

    private LeftTopRightBottomType GetThicknessSide(object? parameter)
        => parameter is LeftTopRightBottomType sideType
            ? sideType
            : TakeThicknessSide;
}
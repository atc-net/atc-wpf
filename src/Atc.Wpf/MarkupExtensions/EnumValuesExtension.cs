namespace Atc.Wpf.MarkupExtensions;

/// <summary>
/// Markup Extension: EnumValuesExtension.
/// </summary>
/// <remarks>
/// Produces an <see cref="Enum"/>[] for use as a <c>ConverterParameter</c> with converters that
/// support multi-value enum matching (e.g. <see cref="EnumToVisibilityVisibleValueConverter"/>).
/// <para>
/// Two usage modes:
/// <list type="bullet">
///   <item><description><b>Positional</b> — pass up to 8 enum literals via <c>{x:Static}</c> for full compile-time type safety.</description></item>
///   <item><description><b>Type + Values</b> — set the <see cref="Type"/> and a comma-separated <see cref="Values"/> string (parse-time validated).</description></item>
/// </list>
/// </para>
/// </remarks>
/// <example>
///     <![CDATA[
///     <!-- Positional / type-safe -->
///     ConverterParameter="{atc:EnumValues {x:Static sys:DayOfWeek.Monday}, {x:Static sys:DayOfWeek.Tuesday}}"
///
///     <!-- Type + Values string -->
///     ConverterParameter="{atc:EnumValues Type={x:Type sys:DayOfWeek}, Values='Monday,Tuesday'}"
///     ]]>
/// </example>
[MarkupExtensionReturnType(typeof(Enum[]))]
[SuppressMessage("Blocker Code Smell", "S3427:Method overloads with default parameter values should not overlap ", Justification = "WPF markup needs overload constructors in order to work.")]
public sealed class EnumValuesExtension : MarkupExtension
{
    private readonly Enum[]? positionalValues;
    private Type? enumType;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumValuesExtension"/> class.
    /// </summary>
    public EnumValuesExtension()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumValuesExtension"/> class.
    /// </summary>
    public EnumValuesExtension(Enum v1)
        => positionalValues = [v1];

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumValuesExtension"/> class.
    /// </summary>
    public EnumValuesExtension(
        Enum v1,
        Enum v2)
        => positionalValues = [v1, v2];

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumValuesExtension"/> class.
    /// </summary>
    public EnumValuesExtension(
        Enum v1,
        Enum v2,
        Enum v3)
        => positionalValues = [v1, v2, v3];

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumValuesExtension"/> class.
    /// </summary>
    public EnumValuesExtension(
        Enum v1,
        Enum v2,
        Enum v3,
        Enum v4)
        => positionalValues = [v1, v2, v3, v4];

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumValuesExtension"/> class.
    /// </summary>
    public EnumValuesExtension(
        Enum v1,
        Enum v2,
        Enum v3,
        Enum v4,
        Enum v5)
        => positionalValues = [v1, v2, v3, v4, v5];

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumValuesExtension"/> class.
    /// </summary>
    public EnumValuesExtension(
        Enum v1,
        Enum v2,
        Enum v3,
        Enum v4,
        Enum v5,
        Enum v6)
        => positionalValues = [v1, v2, v3, v4, v5, v6];

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumValuesExtension"/> class.
    /// </summary>
    public EnumValuesExtension(
        Enum v1,
        Enum v2,
        Enum v3,
        Enum v4,
        Enum v5,
        Enum v6,
        Enum v7)
        => positionalValues = [v1, v2, v3, v4, v5, v6, v7];

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumValuesExtension"/> class.
    /// </summary>
    public EnumValuesExtension(
        Enum v1,
        Enum v2,
        Enum v3,
        Enum v4,
        Enum v5,
        Enum v6,
        Enum v7,
        Enum v8)
        => positionalValues = [v1, v2, v3, v4, v5, v6, v7, v8];

    /// <summary>
    /// Gets or sets the enum <see cref="System.Type"/> used when parsing <see cref="Values"/>.
    /// </summary>
    /// <exception cref="UnexpectedTypeException">Thrown when the assigned type is not an enum type.</exception>
    public Type? Type
    {
        get => enumType;
        set
        {
            if (value == enumType)
            {
                return;
            }

            if (value is not null)
            {
                var underlying = Nullable.GetUnderlyingType(value) ?? value;
                if (!underlying.IsEnum)
                {
                    throw new UnexpectedTypeException($"Type {underlying.FullName} is not a enumerated type");
                }
            }

            enumType = value;
        }
    }

    /// <summary>
    /// Gets or sets a comma-separated list of enum member names (parsed against <see cref="Type"/>).
    /// </summary>
    public string? Values { get; set; }

    /// <inheritdoc />
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (positionalValues is not null)
        {
            return positionalValues;
        }

        if (enumType is null || string.IsNullOrEmpty(Values))
        {
            throw new InvalidOperationException(
                "EnumValuesExtension requires either positional enum values or both 'Type' and 'Values' to be set.");
        }

        var actualEnumType = Nullable.GetUnderlyingType(enumType) ?? enumType;
        var parts = Values.Split(',');
        var result = new Enum[parts.Length];
        for (var i = 0; i < parts.Length; i++)
        {
            result[i] = (Enum)Enum.Parse(actualEnumType, parts[i].Trim(), ignoreCase: true);
        }

        return result;
    }
}
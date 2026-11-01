namespace Atc.Wpf.Forms.Internal.Helpers;

/// <summary>
/// Maps model properties that hold several values ([Flags] enums and collections of enums or strings) to and from
/// the selection keys of the multi-select label controls.
/// </summary>
internal static class MultiSelectPropertyHelper
{
    private static readonly Type[] SupportedCollectionDefinitions =
    [
        typeof(List<>),
        typeof(IList<>),
        typeof(ICollection<>),
        typeof(IEnumerable<>),
        typeof(IReadOnlyList<>),
        typeof(IReadOnlyCollection<>),
        typeof(HashSet<>),
        typeof(ISet<>),
    ];

    /// <summary>
    /// Whether the type (or the type wrapped by <see cref="Nullable{T}"/>) is an enum marked with <see cref="FlagsAttribute"/>.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns><see langword="true"/> for a [Flags] enum; otherwise <see langword="false"/>.</returns>
    public static bool IsFlagsEnum(Type? type)
    {
        var enumType = type is null
            ? null
            : Nullable.GetUnderlyingType(type) ?? type;

        return enumType is { IsEnum: true } &&
               enumType.IsDefined(typeof(FlagsAttribute), inherit: false);
    }

    /// <summary>
    /// Gets the element type of a collection the form can edit and write back: an array, <see cref="List{T}"/>,
    /// <see cref="HashSet{T}"/>, or one of the collection interfaces they implement, holding enum values or strings.
    /// </summary>
    /// <param name="type">The property type.</param>
    /// <param name="elementType">The enum or <see cref="string"/> element type when the type is supported.</param>
    /// <returns><see langword="true"/> when the type is a supported collection; otherwise <see langword="false"/>.</returns>
    public static bool TryGetCollectionElementType(
        Type type,
        [NotNullWhen(true)] out Type? elementType)
    {
        ArgumentNullException.ThrowIfNull(type);

        elementType = null;
        Type? candidate = null;
        if (type.IsArray && type.GetArrayRank() == 1)
        {
            candidate = type.GetElementType();
        }
        else if (type.IsGenericType &&
                 SupportedCollectionDefinitions.Contains(type.GetGenericTypeDefinition()))
        {
            candidate = type.GetGenericArguments()[0];
        }

        if (candidate is null ||
            !(candidate.IsEnum || candidate == typeof(string)))
        {
            return false;
        }

        elementType = candidate;
        return true;
    }

    /// <summary>
    /// Gets the items of a multi-select control for an enum type: for a [Flags] enum, the single-bit values (no zero and
    /// no combined values); otherwise every value except those named None, Unknown and Default.
    /// </summary>
    /// <param name="enumType">The enum type.</param>
    /// <returns>The items, keyed by the enum value name, with the description as display value.</returns>
    public static Dictionary<string, string> GetEnumItems(Type enumType)
    {
        ArgumentNullException.ThrowIfNull(enumType);

        var isFlags = IsFlagsEnum(enumType);
        var items = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (Enum enumValue in enumType.GetEnumValues())
        {
            var key = enumValue.ToString();
            if (isFlags)
            {
                if (!IsSingleFlag(enumValue))
                {
                    continue;
                }
            }
            else if (key.Equals("None", StringComparison.Ordinal) ||
                     key.Equals("Unknown", StringComparison.Ordinal) ||
                     key.Equals("Default", StringComparison.Ordinal))
            {
                continue;
            }

            items.TryAdd(
                key,
                enumValue
                    .GetDescription()
                    .NormalizePascalCase());
        }

        return items;
    }

    /// <summary>
    /// Gets the selection keys for a property value: the single flags set in a [Flags] enum value, or the distinct
    /// non-null values of a collection.
    /// </summary>
    /// <param name="value">The property value; <see langword="null"/> gives no keys.</param>
    /// <returns>The selection keys.</returns>
    public static IList<string> GetSelectedKeys(object? value)
    {
        switch (value)
        {
            case null:
                return [];
            case Enum enumValue when IsFlagsEnum(enumValue.GetType()):
            {
                var bits = ToBits(enumValue);
                return enumValue
                    .GetType()
                    .GetEnumValues()
                    .Cast<Enum>()
                    .Where(x => IsSingleFlag(x) && (bits & ToBits(x)) != 0)
                    .Select(x => x.ToString())
                    .Distinct(StringComparer.Ordinal)
                    .ToList();
            }

            case IEnumerable enumerable and not string:
                return enumerable
                    .Cast<object?>()
                    .Where(x => x is not null)
                    .Select(x => x!.ToString()!)
                    .Distinct(StringComparer.Ordinal)
                    .ToList();
            default:
                return [];
        }
    }

    /// <summary>
    /// Combines selection keys into a value of a [Flags] enum; no keys give the zero value.
    /// </summary>
    /// <param name="flagsType">The [Flags] enum type, or a <see cref="Nullable{T}"/> of it.</param>
    /// <param name="keys">The names of the flags to set; names that are not values of the enum are ignored.</param>
    /// <returns>The boxed enum value.</returns>
    public static object CombineFlags(
        Type flagsType,
        IEnumerable<string>? keys)
    {
        ArgumentNullException.ThrowIfNull(flagsType);

        var enumType = Nullable.GetUnderlyingType(flagsType) ?? flagsType;
        ulong bits = 0;
        foreach (var key in keys ?? [])
        {
            if (Enum.TryParse(enumType, key, ignoreCase: false, out var parsed) &&
                parsed is Enum enumValue)
            {
                bits |= ToBits(enumValue);
            }
        }

        return Enum.ToObject(enumType, bits);
    }

    /// <summary>
    /// Gets the maximum number of selected values from a <c>[MaxLength]</c> or <c>[Length]</c> attribute on the property.
    /// </summary>
    /// <param name="propertyInfo">The property.</param>
    /// <returns>The maximum, or <see langword="null"/> when there is no attribute with a maximum of 1 or more.</returns>
    public static int? GetMaxSelectionCount(PropertyInfo propertyInfo)
    {
        ArgumentNullException.ThrowIfNull(propertyInfo);

        var maximum = propertyInfo.GetCustomAttribute<MaxLengthAttribute>()?.Length ??
                      propertyInfo.GetCustomAttribute<LengthAttribute>()?.MaximumLength;

        return maximum is >= 1
            ? maximum
            : null;
    }

    private static bool IsSingleFlag(Enum enumValue)
    {
        var bits = ToBits(enumValue);
        return bits != 0 && (bits & (bits - 1)) == 0;
    }

    private static ulong ToBits(Enum enumValue)
        => Type.GetTypeCode(Enum.GetUnderlyingType(enumValue.GetType())) switch
        {
            TypeCode.SByte => unchecked((byte)Convert.ToSByte(enumValue, CultureInfo.InvariantCulture)),
            TypeCode.Int16 => unchecked((ushort)Convert.ToInt16(enumValue, CultureInfo.InvariantCulture)),
            TypeCode.Int32 => unchecked((uint)Convert.ToInt32(enumValue, CultureInfo.InvariantCulture)),
            TypeCode.Int64 => unchecked((ulong)Convert.ToInt64(enumValue, CultureInfo.InvariantCulture)),
            _ => Convert.ToUInt64(enumValue, CultureInfo.InvariantCulture),
        };
}
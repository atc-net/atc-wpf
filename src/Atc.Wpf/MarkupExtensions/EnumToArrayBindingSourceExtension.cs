namespace Atc.Wpf.MarkupExtensions;

/// <summary>
/// Markup Extension: EnumToArrayBindingSourceExtension.
/// </summary>
/// <seealso cref="MarkupExtension" />
/// <example>
///     <![CDATA[<ComboBox ItemsSource="{Binding Source={local:EnumToArrayBindingSourceExtension {x:Type local:TheEnum}}}"/>]]>
/// </example>
// ReSharper disable RedundantOverload.Global
[SuppressMessage("Blocker Code Smell", "S3427:Method overloads with default parameter values should not overlap ", Justification = "WPF markup need to have overload constructors in order to work.")]
public class EnumToArrayBindingSourceExtension : MarkupExtension
{
    private Type? enumType;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumToArrayBindingSourceExtension"/> class.
    /// </summary>
    public EnumToArrayBindingSourceExtension()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumToArrayBindingSourceExtension"/> class.
    /// </summary>
    /// <param name="enumType">Type of the enum.</param>
    public EnumToArrayBindingSourceExtension(
        Type enumType)
        : this(
            enumType,
            DropDownFirstItemType.None,
            includeDefault: false,
            useDescriptionAttribute: true,
            SortDirectionType.None)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumToArrayBindingSourceExtension"/> class.
    /// </summary>
    /// <param name="enumType">Type of the enum.</param>
    /// <param name="firstItemType">Type of the first item.</param>
    public EnumToArrayBindingSourceExtension(
        Type enumType,
        DropDownFirstItemType firstItemType = DropDownFirstItemType.None)
        : this(
            enumType,
            firstItemType,
            includeDefault: false,
            useDescriptionAttribute: true,
            SortDirectionType.None)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumToArrayBindingSourceExtension"/> class.
    /// </summary>
    /// <param name="enumType">Type of the enum.</param>
    /// <param name="firstItemType">First type of the item.</param>
    /// <param name="includeDefault">if set to <see langword="true" /> [include default].</param>
    /// <param name="useDescriptionAttribute">if set to <see langword="true" /> [use description attribute].</param>
    public EnumToArrayBindingSourceExtension(
        Type enumType,
        DropDownFirstItemType firstItemType = DropDownFirstItemType.None,
        bool includeDefault = false,
        bool useDescriptionAttribute = true)
        : this(
            enumType,
            firstItemType,
            includeDefault,
            useDescriptionAttribute,
            SortDirectionType.None)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumToArrayBindingSourceExtension"/> class.
    /// </summary>
    /// <param name="enumType">Type of the enum.</param>
    /// <param name="firstItemType">First type of the item.</param>
    /// <param name="includeDefault">if set to <see langword="true" /> [include default].</param>
    /// <param name="useDescriptionAttribute">if set to <see langword="true" /> [use description attribute].</param>
    /// <param name="sortDirectionType">Type of the sort direction.</param>
    public EnumToArrayBindingSourceExtension(
        Type enumType,
        DropDownFirstItemType firstItemType = DropDownFirstItemType.None,
        bool includeDefault = false,
        bool useDescriptionAttribute = true,
        SortDirectionType sortDirectionType = SortDirectionType.None)
    {
        EnumType = enumType;
        FirstItemType = firstItemType;
        IncludeDefault = includeDefault;
        UseDescriptionAttribute = useDescriptionAttribute;
        SortDirectionType = sortDirectionType;
    }

    /// <summary>
    /// Gets or sets the type of the enum.
    /// </summary>
    /// <value>
    /// The type of the enum.
    /// </value>
    /// <exception cref="ArgumentException">Type must be for an Enum.</exception>
    public Type? EnumType
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
                var type = Nullable.GetUnderlyingType(value) ?? value;
                if (!type.IsEnum)
                {
                    throw new UnexpectedTypeException($"Type {type.FullName} is not a enumerated type");
                }
            }

            enumType = value;
        }
    }

    /// <summary>
    /// Gets or sets the first type of the item.
    /// </summary>
    /// <value>
    /// The first type of the item.
    /// </value>
    public DropDownFirstItemType FirstItemType { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [include default].
    /// </summary>
    /// <value>
    ///   <see langword="true" /> if [include default]; otherwise, <see langword="false" />.
    /// </value>
    public bool IncludeDefault { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [use description attribute].
    /// </summary>
    /// <value>
    /// <see langword="true" /> if [use description attribute]; otherwise, <see langword="false" />.
    /// </value>
    public bool UseDescriptionAttribute { get; set; }

    /// <summary>
    /// Gets or sets the type of the sort direction.
    /// </summary>
    /// <value>
    /// The type of the sort direction.
    /// </value>
    public SortDirectionType SortDirectionType { get; set; }

    /// <summary>
    /// Provides the value.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <exception cref="InvalidOperationException">The EnumType must be specified.</exception>
    public override object ProvideValue(
        IServiceProvider serviceProvider)
        => GetEnumValues();

    private Array GetEnumValues()
    {
        if (enumType is null)
        {
            throw new InvalidOperationException("The EnumType must be specified.");
        }

        var actualEnumType = Nullable.GetUnderlyingType(enumType) ?? enumType;
        var enumValues = EnumHelper.ConvertEnumToArray(
            actualEnumType,
            FirstItemType,
            UseDescriptionAttribute,
            IncludeDefault,
            SortDirectionType);

        if (actualEnumType == enumType)
        {
            return enumValues;
        }

        var tempArray = Array.CreateInstance(actualEnumType, enumValues.Length + 1);
        enumValues.CopyTo(tempArray, 1);
        return tempArray;
    }
}
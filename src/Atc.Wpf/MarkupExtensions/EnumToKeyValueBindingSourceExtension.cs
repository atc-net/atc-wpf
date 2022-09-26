namespace Atc.Wpf.MarkupExtensions;

/// <summary>
/// Markup Extension: EnumBindingSource.
/// </summary>
/// <seealso cref="MarkupExtension" />
/// <example>
///     <![CDATA[<ComboBox ItemsSource="{Binding Source={local:EnumBindingSource {x:Type local:TheEnum}}}"/>]]>
/// </example>
[SuppressMessage("Blocker Code Smell", "S3427:Method overloads with default parameter values should not overlap ", Justification = "WPF markup need to have overload constructors in order to work.")]
public class EnumToKeyValueBindingSourceExtension : MarkupExtension
{
    private Type? enumType;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumToKeyValueBindingSourceExtension"/> class.
    /// </summary>
    public EnumToKeyValueBindingSourceExtension()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumToKeyValueBindingSourceExtension"/> class.
    /// </summary>
    /// <param name="enumType">Type of the enum.</param>
    public EnumToKeyValueBindingSourceExtension(
        Type enumType)
        : this(enumType, DropDownFirstItemType.None, false, true, SortDirectionType.None)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumToKeyValueBindingSourceExtension"/> class.
    /// </summary>
    /// <param name="enumType">Type of the enum.</param>
    /// <param name="firstItemType">Type of the first item.</param>
    public EnumToKeyValueBindingSourceExtension(
        Type enumType,
        DropDownFirstItemType firstItemType = DropDownFirstItemType.None)
        : this(enumType, firstItemType, false, true, SortDirectionType.None)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumToKeyValueBindingSourceExtension"/> class.
    /// </summary>
    /// <param name="enumType">Type of the enum.</param>
    /// <param name="firstItemType">First type of the item.</param>
    /// <param name="includeDefault">if set to <c>true</c> [include default].</param>
    /// <param name="useDescriptionAttribute">if set to <c>true</c> [use description attribute].</param>
    public EnumToKeyValueBindingSourceExtension(
        Type enumType,
        DropDownFirstItemType firstItemType = DropDownFirstItemType.None,
        bool includeDefault = false,
        bool useDescriptionAttribute = true)
        : this(enumType, firstItemType, includeDefault, useDescriptionAttribute, SortDirectionType.None)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumToKeyValueBindingSourceExtension"/> class.
    /// </summary>
    /// <param name="enumType">Type of the enum.</param>
    /// <param name="firstItemType">First type of the item.</param>
    /// <param name="includeDefault">if set to <c>true</c> [include default].</param>
    /// <param name="useDescriptionAttribute">if set to <c>true</c> [use description attribute].</param>
    /// <param name="sortDirectionType">Type of the sort direction.</param>
    public EnumToKeyValueBindingSourceExtension(
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
    ///   <c>true</c> if [include default]; otherwise, <c>false</c>.
    /// </value>
    public bool IncludeDefault { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [use description attribute].
    /// </summary>
    /// <value>
    /// <c>true</c> if [use description attribute]; otherwise, <c>false</c>.
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
    /// Gets or sets a value indicating whether [key as string].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [key as string]; otherwise, <c>false</c>.
    /// </value>
    public bool KeyAsString { get; set; }

    /// <summary>
    /// Provides the value.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <exception cref="InvalidOperationException">The EnumType must be specified.</exception>
    public override object ProvideValue(
        IServiceProvider serviceProvider)
        => GetEnumValues();

    private object GetEnumValues()
    {
        if (enumType is null)
        {
            throw new InvalidOperationException("The EnumType must be specified.");
        }

        var actualEnumType = Nullable.GetUnderlyingType(enumType) ?? enumType;
        if (KeyAsString)
        {
            var enumValuesAsString = EnumHelper.ConvertEnumToDictionaryWithStringKey(
                actualEnumType,
                FirstItemType,
                UseDescriptionAttribute,
                IncludeDefault,
                SortDirectionType);
            return enumValuesAsString;
        }

        var enumValues = EnumHelper.ConvertEnumToDictionary(
            actualEnumType,
            FirstItemType,
            UseDescriptionAttribute,
            IncludeDefault,
            SortDirectionType);
        return enumValues;
    }
}
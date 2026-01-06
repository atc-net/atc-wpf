// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
namespace Atc.Wpf.ValidationRules;

/// <summary>
/// A WPF <see cref="ValidationRule"/> that validates a string input
/// based on a set of constraints defined in <see cref="StringAttribute"/>.
/// </summary>
/// <remarks>
/// This rule bridges your reusable <see cref="StringAttribute"/> logic with WPF binding validation.
/// Consumers can configure constraints either:
/// <list type="bullet">
///   <item><description>
/// By setting individual properties (e.g. <see cref="Required"/>, <see cref="MinLength"/>).
/// </description></item>
///   <item><description>
/// Or by supplying a <see cref="StringAttribute"/> instance directly as content.
/// Thanks to <see cref="ContentPropertyAttribute"/>, the <see cref="Constraints"/> property
/// is the implicit content property in XAML.
/// </description></item>
/// </list>
/// </remarks>
[ContentProperty(nameof(Constraints))]
public class StringValidationRule : ValidationRule
{
    /// <summary>
    /// Gets or sets the underlying <see cref="StringAttribute"/> that defines all validation rules.
    /// This property is also the implicit content property in XAML.
    /// </summary>
    public StringAttribute Constraints { get; set; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether the value is required.
    /// </summary>
    public bool Required
    {
        get => Constraints.Required;
        set => Constraints.Required = value;
    }

    /// <summary>
    /// Gets or sets the minimum allowed string length.
    /// </summary>
    public uint MinLength
    {
        get => Constraints.MinLength;
        set => Constraints.MinLength = value;
    }

    /// <summary>
    /// Gets or sets the maximum allowed string length.
    /// </summary>
    public uint MaxLength
    {
        get => Constraints.MaxLength;
        set => Constraints.MaxLength = value;
    }

    /// <summary>
    /// XAML-friendly proxy for <see cref="StringAttribute.InvalidCharacters"/>.
    /// Example: <c>InvalidCharacters=":/\?*|"</c>
    /// </summary>
    public string InvalidCharacters
    {
        get => new(Constraints.InvalidCharacters);
        set => Constraints.InvalidCharacters = value?.ToCharArray() ?? [];
    }

    /// <summary>
    /// XAML-friendly proxy for <see cref="StringAttribute.InvalidPrefixStrings"/>.
    /// Uses comma-separated values.
    /// Example: <c>InvalidPrefixStrings="tmp_,test_,_hidden"</c>
    /// </summary>
    public string InvalidPrefixStrings
    {
        get => string.Join(",", Constraints.InvalidPrefixStrings);
        set => Constraints.InvalidPrefixStrings = SplitCsv(value);
    }

    /// <summary>
    /// Gets or sets the regular expression pattern used to validate the string.
    /// An empty string disables regex validation.
    /// </summary>
    public string RegularExpression
    {
        get => Constraints.RegularExpression;
        set => Constraints.RegularExpression = value;
    }

    /// <inheritdoc />
    public override System.Windows.Controls.ValidationResult Validate(
        object? value,
        CultureInfo cultureInfo)
        => StringAttribute.TryIsValid(
            value?.ToString() ?? string.Empty,
            Constraints,
            out var errorMessage)
            ? System.Windows.Controls.ValidationResult.ValidResult
            : new System.Windows.Controls.ValidationResult(false, errorMessage);

    private static string[] SplitCsv(string? csv)
        => string.IsNullOrWhiteSpace(csv)
            ? []
            : csv
                .Split(',')
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .ToArray();
}
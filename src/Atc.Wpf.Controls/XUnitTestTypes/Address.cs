namespace Atc.Wpf.Controls.XUnitTestTypes;

public sealed class Address
{
    [Required]
    [MinLength(2)]
    [Description("Hello street")]
    public string? StreetName { get; set; }

    [Required]
    [MinLength(2)]
    [MaxLength(16)]
    public string? CityName { get; set; }

    [Required]
    [RegularExpression("^\\d{4}$")]
    public string? PostalCode { get; set; }

    [Required]
    public CultureInfo? Country { get; set; }

    public override string ToString()
        => $"{nameof(StreetName)}: {StreetName}, {nameof(CityName)}: {CityName}, {nameof(PostalCode)}: {PostalCode}, {nameof(Country)}: ({Country})";
}
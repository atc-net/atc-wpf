#pragma warning disable SA1402
// ReSharper disable ConvertToPrimaryConstructor
namespace Atc.Wpf.Sample.Models;

public record Address
{
    public Address(
        string streetName,
        string cityName,
        string postalCode,
        CultureInfo country)
    {
        StreetName = streetName;
        CityName = cityName;
        PostalCode = postalCode;
        Country = country;
    }

    [Required]
    [MinLength(2)]
    public string StreetName { get; init; }

    [Required]
    [MinLength(2)]
    [MaxLength(16)]
    public string CityName { get; init; }

    [Required]
    [RegularExpression("^\\d{4}$")]
    public string PostalCode { get; init; }

    [Required]
    public CultureInfo? Country { get; init; }

    [Required]
    public Point2D? LocationCoordinates { get; init; }
}

public record Person
{
    public Person(
        string firstName,
        string lastName,
        int age,
        Color? favoriteColor,
        Address address)
    {
        FirstName = firstName;
        LastName = lastName;
        Age = age;
        if (favoriteColor.HasValue)
        {
            FavoriteColor = favoriteColor.Value;
        }

        MyAddress = address;
    }

    [Required]
    [MinLength(2)]
    public string FirstName { get; init; }

    [Required]
    [MinLength(2)]
    public string LastName { get; init; }

    [Range(1, 99)]
    public int Age { get; init; }

    public Color FavoriteColor { get; init; }

    public CultureInfo? PreferredLanguage { get; init; }

    public Address MyAddress { get; init; }
}

public record Account
{
    public Account(
        string accountNumber,
        Person primaryContactPerson)
    {
        AccountNumber = accountNumber;
        PrimaryContactPerson = primaryContactPerson;
    }

    public Guid Id { get; } = Guid.NewGuid();

    [Required]
    [MinLength(2)]
    public string AccountNumber { get; init; }

    public Person PrimaryContactPerson { get; init; }

    public DayOfWeek FirstDayOfWeek { get; init; } = DayOfWeek.Monday;
}
#pragma warning restore SA1402
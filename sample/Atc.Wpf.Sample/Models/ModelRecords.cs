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
        Color favoriteColor,
        Address myAddress)
    {
        FirstName = firstName;
        LastName = lastName;
        Age = age;
        FavoriteColor = favoriteColor;
        MyAddress = myAddress;
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
        DateTime createdDate,
        Person primaryContactPerson)
    {
        AccountNumber = accountNumber;
        CreatedDate = createdDate;
        PrimaryContactPerson = primaryContactPerson;
    }

    public Guid Id { get; } = Guid.NewGuid();

    [Required]
    [MinLength(2)]
    public string AccountNumber { get; init; }

    [Required]
    public DateTime CreatedDate { get; init; }

    public Person PrimaryContactPerson { get; init; }

    public DayOfWeek FirstDayOfWeek { get; init; } = DayOfWeek.Monday;
}

public record BrushInfo(
    string Key,
    string DisplayName,
    SolidColorBrush Brush);
#pragma warning restore SA1402
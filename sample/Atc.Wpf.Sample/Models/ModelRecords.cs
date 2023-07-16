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

    public CultureInfo Country { get; init; }
}

public record Person
{
    public Person(
        string firstName,
        string lastName,
        int age,
        Address address)
    {
        FirstName = firstName;
        LastName = lastName;
        Age = age;
        Address = address;
    }

    [Required]
    [MinLength(2)]
    public string FirstName { get; init; }

    [Required]
    [MinLength(2)]
    public string LastName { get; init; }

    [Range(1, 99)]
    public int Age { get; init; }

    public Address Address { get; init; }
}
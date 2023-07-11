namespace Atc.Wpf.Sample.Models;

public record Address(
    [Required, MinLength(2)] string StreetName,
    [Required, MinLength(2), MaxLength(16)] string CityName,
    [Required, RegularExpression("^\\d{4}$")] string PostalCode,
    CultureInfo Country);

public record Person(
    [Required, MinLength(2)] string FirstName,
    [Required, MinLength(2)] string LastName,
    [Range(1, 99)] int Age,
    Address? Address);
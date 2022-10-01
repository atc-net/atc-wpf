namespace Atc.Wpf.Theming.Sample.Models;

public record Address(
    string StreetName,
    string CityName,
    string PostalCode,
    CultureInfo Country);

public record Person(
    string FirstName,
    string LastName,
    int Age,
    Address? Address);
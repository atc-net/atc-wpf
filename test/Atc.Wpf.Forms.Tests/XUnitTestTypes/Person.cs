namespace Atc.Wpf.Forms.Tests.XUnitTestTypes;

public sealed class Person
{
    [Required]
    [MinLength(2)]
    public string? FirstName { get; set; }

    [Required]
    [MinLength(2)]
    public string? LastName { get; set; }

    [Required]
    [Range(1, 99)]
    public int? Age { get; set; }

    [Required]
    public Address? Address { get; set; }

    public override string ToString()
        => $"{nameof(FirstName)}: {FirstName}, {nameof(LastName)}: {LastName}, {nameof(Age)}: {Age}, {nameof(Address)}: ({Address})";
}
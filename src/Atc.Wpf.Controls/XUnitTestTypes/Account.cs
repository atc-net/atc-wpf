namespace Atc.Wpf.Controls.XUnitTestTypes;

public class Account
{
    public Guid Id { get; } = Guid.NewGuid();

    [Required]
    [MinLength(2)]
    public string AccountNumber { get; set; } = string.Empty;

    public Person? PrimaryContactPerson { get; set; }

    public override string ToString()
        => $"{nameof(Id)}: {Id}, {nameof(AccountNumber)}: {AccountNumber}, {nameof(PrimaryContactPerson)}: {PrimaryContactPerson}";
}
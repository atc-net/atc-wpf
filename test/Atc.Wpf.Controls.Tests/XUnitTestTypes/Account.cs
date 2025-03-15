namespace Atc.Wpf.Controls.Tests.XUnitTestTypes;

public sealed class Account
{
    public Guid Id { get; } = Guid.NewGuid();

    [Required]
    [MinLength(2)]
    public string AccountNumber { get; set; } = string.Empty;

    [Required]
    public DateTime CreatedDate { get; init; } = DateTime.Now.AddYears(-2).AddDays(3);

    public Person? PrimaryContactPerson { get; set; }

    public DayOfWeek FirstDayOfWeek { get; init; } = DayOfWeek.Monday;

    public override string ToString()
        => $"{nameof(Id)}: {Id}, {nameof(AccountNumber)}: {AccountNumber}, {nameof(PrimaryContactPerson)}: {PrimaryContactPerson}, {nameof(FirstDayOfWeek)}: {FirstDayOfWeek}";
}
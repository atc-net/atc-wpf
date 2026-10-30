namespace Atc.Wpf.Sample.SamplesWpfControls.Selectors;

/// <summary>
/// A customer row for the <see cref="MultiColumnComboBox"/> samples.
/// </summary>
public sealed class SampleCustomer
{
    public SampleCustomer(
        string number,
        string name,
        string city,
        string country,
        string email,
        bool isActive)
    {
        Number = number;
        Name = name;
        City = city;
        Country = country;
        Contact = new SampleContact(email);
        IsActive = isActive;
    }

    public string Number { get; }

    public string Name { get; }

    public string City { get; }

    public string Country { get; }

    public SampleContact Contact { get; }

    public bool IsActive { get; }
}
namespace Atc.Wpf.Sample.SamplesWpfForms.PropertyEditing;

/// <summary>
/// Nested address object for demonstrating complex property editing.
/// </summary>
public class SampleAddress : INotifyPropertyChanged
{
    private string street = string.Empty;
    private string city = string.Empty;
    private string zipCode = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    [DisplayName("Street Address")]
    [Description("Street name and number")]
    public string Street
    {
        get => street;
        set
        {
            street = value;
            OnPropertyChanged(nameof(Street));
        }
    }

    [Description("City name")]
    public string City
    {
        get => city;
        set
        {
            city = value;
            OnPropertyChanged(nameof(City));
        }
    }

    [DisplayName("ZIP Code")]
    [Description("Postal code")]
    [MaxLength(10)]
    public string ZipCode
    {
        get => zipCode;
        set
        {
            zipCode = value;
            OnPropertyChanged(nameof(ZipCode));
        }
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
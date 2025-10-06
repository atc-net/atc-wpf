namespace Atc.Wpf.Sample.SamplesWpfControls.LabelControls;

public sealed class LabelMixBindingsViewModel : ViewModelBase
{
    private bool fieldBool1;
    private string fieldSelectedKey1 = string.Empty;
    private string fieldSelectedKey2 = string.Empty;
    private string fieldText1 = string.Empty;
    private string fieldText2 = string.Empty;
    private string fieldText3 = string.Empty;
    private string fieldSelectedKey3 = string.Empty;
    private Uri? fieldUri1;

    public LabelMixBindingsViewModel()
    {
        FieldItems2.Add("Key1", "Value 1");
        FieldItems2.Add("Key2", "Value 2");
        FieldItems2.Add("Key3", "Value 3");
        FieldItems2.Add("Key4", "Value 4");

        FieldItems2.CollectionChanged += FieldItems2OnCollectionChanged;
    }

    public IRelayCommand AddCommand => new RelayCommand(AddCommandHandler);

    public IRelayCommand GoCommand => new RelayCommand(GoCommandHandler, () => CanExecuteGo);

    public bool FieldBool1
    {
        get => fieldBool1;
        set
        {
            fieldBool1 = value;
            RaisePropertyChanged();
        }
    }

    public string FieldSelectedKey1
    {
        get => fieldSelectedKey1;
        set
        {
            fieldSelectedKey1 = value;
            RaisePropertyChanged();
        }
    }

    public string FieldText1
    {
        get => fieldText1;
        set
        {
            fieldText1 = value;
            RaisePropertyChanged();
        }
    }

    public string FieldText2
    {
        get => fieldText2;
        set
        {
            fieldText2 = value;
            RaisePropertyChanged();
        }
    }

    public string FieldText3
    {
        get => fieldText3;
        set
        {
            fieldText3 = value;
            RaisePropertyChanged();
        }
    }

    public string FieldSelectedKey2
    {
        get => fieldSelectedKey2;
        set
        {
            fieldSelectedKey2 = value;
            RaisePropertyChanged();
        }
    }

    public ObservableDictionary<string, string> FieldItems2 { get; set; } = new();

    public string FieldSelectedKey3
    {
        get => fieldSelectedKey3;
        set
        {
            fieldSelectedKey3 = value;
            RaisePropertyChanged();
        }
    }

    public Uri? FieldUri1
    {
        get => fieldUri1;
        set
        {
            fieldUri1 = value;
            RaisePropertyChanged();
        }
    }

    public bool CanExecuteGo
        => !string.IsNullOrWhiteSpace(FieldSelectedKey1) &&
           !string.IsNullOrWhiteSpace(FieldSelectedKey2) &&
           !string.IsNullOrWhiteSpace(FieldText1) &&
           !string.IsNullOrWhiteSpace(FieldText2) &&
           !string.IsNullOrWhiteSpace(FieldText3) &&
           !string.IsNullOrWhiteSpace(FieldSelectedKey3);

    private void FieldItems2OnCollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e)
        => RaisePropertyChanged(nameof(FieldItems2));

    private void AddCommandHandler()
    {
        var dateTimeNow = DateTime.Now;
        FieldItems2.Add($"Key{dateTimeNow.Ticks}", $"Value {dateTimeNow.Ticks}");
    }

    private void GoCommandHandler()
    {
        MessageBox.Show("Go", "Hello", MessageBoxButton.OK);
    }
}
namespace Atc.Wpf.Controls.Sample.Samples.LabelControls;

public class LabelMixBindingsViewModel : ViewModelBase
{
    private bool fieldBool1;
    private string fieldSelectedKey1 = string.Empty;
    private string fieldText1 = string.Empty;
    private string fieldText2 = string.Empty;
    private string fieldText3 = string.Empty;
    private string fieldSelectedKey2 = string.Empty;

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

    public bool CanExecuteGo
        => !string.IsNullOrWhiteSpace(FieldSelectedKey1) &&
           !string.IsNullOrWhiteSpace(FieldText1) &&
           !string.IsNullOrWhiteSpace(FieldText2) &&
           !string.IsNullOrWhiteSpace(FieldText3) &&
           !string.IsNullOrWhiteSpace(FieldSelectedKey2);

    private void GoCommandHandler()
    {
        MessageBox.Show("Go", "Hello", MessageBoxButton.OK);
    }
}
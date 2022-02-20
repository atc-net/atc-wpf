namespace Atc.Wpf.Sample.Samples.Commands;

public class RelayCommandViewModel : ViewModelBase
{
    private bool isTestEnabled;

    public ICommand Test1Command => new RelayCommand(this.Test1CommandHandler);

    public ICommand Test2Command => new RelayCommand(this.Test2CommandHandler, () => this.IsTestEnabled);

    public bool IsTestEnabled
    {
        get => this.isTestEnabled;
        set
        {
            this.isTestEnabled = value;
            this.RaisePropertyChanged();
        }
    }

    private void Test1CommandHandler()
    {
        _ = MessageBox.Show("Test1-command is hit", "Hallo", MessageBoxButton.OK);
    }

    private void Test2CommandHandler()
    {
        _ = MessageBox.Show("Test2-command is hit", "Hallo", MessageBoxButton.OK);
    }
}
namespace Atc.Wpf.Sample.Samples.Commands;

public class RelayCommandViewModel : ViewModelBase
{
    private bool isTestEnabled;

    public IRelayCommand Test1Command => new RelayCommand(this.Test1CommandHandler);

    public IRelayCommand Test2Command => new RelayCommand(this.Test2CommandHandler, () => this.IsTestEnabled);

    public IRelayCommand<string> Test3Command => new RelayCommand<string>(this.Test3CommandHandler);

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

    private void Test3CommandHandler(string obj)
    {
        _ = MessageBox.Show("Test3-command is hit", $"CommandParameter: {obj}", MessageBoxButton.OK);
    }
}
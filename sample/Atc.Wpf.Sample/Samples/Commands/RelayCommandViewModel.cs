namespace Atc.Wpf.Sample.Samples.Commands;

public class RelayCommandViewModel : ViewModelBase
{
    private bool isTestEnabled;

    public IRelayCommand Test1Command => new RelayCommand(Test1CommandHandler);

    public IRelayCommand Test2Command => new RelayCommand(Test2CommandHandler, () => IsTestEnabled);

    public IRelayCommand<string> Test3Command => new RelayCommand<string>(Test3CommandHandler);

    public bool IsTestEnabled
    {
        get => isTestEnabled;
        set
        {
            isTestEnabled = value;
            RaisePropertyChanged();
        }
    }

    private void Test1CommandHandler()
    {
        _ = MessageBox.Show("Test1-command is hit", "Hello", MessageBoxButton.OK);
    }

    private void Test2CommandHandler()
    {
        _ = MessageBox.Show("Test2-command is hit", "Hello", MessageBoxButton.OK);
    }

    private void Test3CommandHandler(string obj)
    {
        _ = MessageBox.Show("Test3-command is hit", $"CommandParameter: {obj}", MessageBoxButton.OK);
    }
}
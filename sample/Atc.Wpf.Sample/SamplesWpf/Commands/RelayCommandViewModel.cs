namespace Atc.Wpf.Sample.SamplesWpf.Commands;

public sealed class RelayCommandViewModel : ViewModelBase
{
    private bool isTestEnabled;

    public IRelayCommand Test1Command => new RelayCommand(Test1CommandHandler);

    public IRelayCommand Test2Command => new RelayCommand(Test2CommandHandler, () => IsTestEnabled);

    public IRelayCommand<string> Test3Command => new RelayCommand<string>(Test3CommandHandler);

    public IRelayCommand<string> Test4Command => new RelayCommand<string>(Test4CommandHandler, CanTest4CommandHandler);

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

    private bool CanTest4CommandHandler(string obj)
    {
        return IsTestEnabled;
    }

    private void Test4CommandHandler(string obj)
    {
        _ = MessageBox.Show("Test4-command is hit", $"CommandParameter: {obj}", MessageBoxButton.OK);
    }
}
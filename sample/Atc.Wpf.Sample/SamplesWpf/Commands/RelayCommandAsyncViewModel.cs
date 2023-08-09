namespace Atc.Wpf.Sample.SamplesWpf.Commands;

public class RelayCommandAsyncViewModel : ViewModelBase
{
    private bool isTestEnabled;

    public IRelayCommandAsync Test1Command => new RelayCommandAsync(Test1CommandHandler);

    public IRelayCommandAsync Test2Command => new RelayCommandAsync(Test2CommandHandler, () => IsTestEnabled);

    public IRelayCommandAsync<string> Test3Command => new RelayCommandAsync<string>(Test3CommandHandler);

    public bool IsTestEnabled
    {
        get => isTestEnabled;
        set
        {
            isTestEnabled = value;
            RaisePropertyChanged();
        }
    }

    private async Task Test1CommandHandler()
    {
        await Task.Delay(1000, CancellationToken.None).ConfigureAwait(false);
        _ = MessageBox.Show("Test1-command is hit", "Hello", MessageBoxButton.OK);
    }

    private async Task Test2CommandHandler()
    {
        await Task.Delay(1000, CancellationToken.None).ConfigureAwait(false);
        _ = MessageBox.Show("Test2-command is hit", "Hello", MessageBoxButton.OK);
    }

    private async Task Test3CommandHandler(string obj)
    {
        await Task.Delay(1000, CancellationToken.None).ConfigureAwait(false);
        _ = MessageBox.Show("Test3-command is hit", $"CommandParameter: {obj}", MessageBoxButton.OK);
    }
}
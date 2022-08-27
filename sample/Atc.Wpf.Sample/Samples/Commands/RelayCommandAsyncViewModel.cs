namespace Atc.Wpf.Sample.Samples.Commands;

public class RelayCommandAsyncViewModel : ViewModelBase
{
    private bool isTestEnabled;

    public IRelayCommandAsync Test1Command => new RelayCommandAsync(this.Test1CommandHandler);

    public IRelayCommandAsync Test2Command => new RelayCommandAsync(this.Test2CommandHandler, () => this.IsTestEnabled);

    public IRelayCommandAsync<string> Test3Command => new RelayCommandAsync<string>(this.Test3CommandHandler);

    public bool IsTestEnabled
    {
        get => this.isTestEnabled;
        set
        {
            this.isTestEnabled = value;
            this.RaisePropertyChanged();
        }
    }

    private async Task Test1CommandHandler()
    {
        await Task.Delay(1000, CancellationToken.None);
        _ = MessageBox.Show("Test1-command is hit", "Hallo", MessageBoxButton.OK);
    }

    private async Task Test2CommandHandler()
    {
        await Task.Delay(1000, CancellationToken.None);
        _ = MessageBox.Show("Test2-command is hit", "Hallo", MessageBoxButton.OK);
    }

    private async Task Test3CommandHandler(string obj)
    {
        await Task.Delay(1000, CancellationToken.None);
        _ = MessageBox.Show("Test3-command is hit", $"CommandParameter: {obj}", MessageBoxButton.OK);
    }
}
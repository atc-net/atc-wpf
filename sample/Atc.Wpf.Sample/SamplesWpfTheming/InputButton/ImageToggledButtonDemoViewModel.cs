namespace Atc.Wpf.Sample.SamplesWpfTheming.InputButton;

public partial class ImageToggledButtonDemoViewModel : ViewModelBase
{
    private bool isBusy;

    [PropertyDisplay("Is Primary", "State", 2)]
    [ObservableProperty]
    private bool isPrimary;

    [PropertyDisplay("Is Connected", "State", 3)]
    [ObservableProperty]
    private bool isConnected;

    [PropertyDisplay("Is Busy", "State", 1)]
    public new bool IsBusy
    {
        get => isBusy;
        set => Set(ref isBusy, value);
    }

    [RelayCommand]
    private async Task Connect()
    {
        IsBusy = true;

        await Task
            .Delay(2_000)
            .ConfigureAwait(false);

        await Application.Current.Dispatcher
            .InvokeAsyncIfRequired(() =>
            {
                IsConnected = true;
                IsBusy = false;
            })
            .ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task Disconnect()
    {
        IsBusy = true;

        await Task
            .Delay(500)
            .ConfigureAwait(false);

        await Application.Current.Dispatcher
            .InvokeAsyncIfRequired(() =>
            {
                IsConnected = false;
                IsBusy = false;
            })
            .ConfigureAwait(false);
    }
}

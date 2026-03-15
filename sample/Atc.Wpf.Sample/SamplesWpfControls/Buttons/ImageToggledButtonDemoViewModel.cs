namespace Atc.Wpf.Sample.SamplesWpfControls.Buttons;

public partial class ImageToggledButtonDemoViewModel : ViewModelBase
{
    private bool isBusy;

    [PropertyDisplay("Is Connected", "State", 2)]
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
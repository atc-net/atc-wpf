namespace Atc.Wpf.Sample.SamplesWpfTheming.InputButton;

public partial class ImageToggledButton
{
    public ImageToggledButton()
    {
        InitializeComponent();

        DataContext = this;
    }

    [DependencyProperty]
    private bool isBusy;

    [DependencyProperty]
    private bool isConnected;

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
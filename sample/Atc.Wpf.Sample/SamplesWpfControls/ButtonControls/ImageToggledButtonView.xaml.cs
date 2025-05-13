namespace Atc.Wpf.Sample.SamplesWpfControls.ButtonControls;

public partial class ImageToggledButtonView
{
    public ImageToggledButtonView()
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
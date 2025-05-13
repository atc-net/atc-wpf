namespace Atc.Wpf.Sample.SamplesWpfControls.ButtonControls;

public partial class AuthenticationButtonView
{
    public AuthenticationButtonView()
    {
        InitializeComponent();

        DataContext = this;
    }

    [DependencyProperty]
    private bool isBusy;

    [DependencyProperty]
    private bool isAuthenticated;

    [RelayCommand]
    private async Task Login()
    {
        IsBusy = true;

        await Task
            .Delay(2_000)
            .ConfigureAwait(false);

        await Application.Current.Dispatcher
            .InvokeAsyncIfRequired(() =>
            {
                IsAuthenticated = true;
                IsBusy = false;
            })
            .ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task Logout()
    {
        IsBusy = true;

        await Task
            .Delay(500)
            .ConfigureAwait(false);

        await Application.Current.Dispatcher
            .InvokeAsyncIfRequired(() =>
            {
                IsAuthenticated = false;
                IsBusy = false;
            })
            .ConfigureAwait(false);
    }
}
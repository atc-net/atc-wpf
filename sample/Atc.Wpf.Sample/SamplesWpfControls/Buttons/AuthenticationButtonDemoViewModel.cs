namespace Atc.Wpf.Sample.SamplesWpfControls.Buttons;

public partial class AuthenticationButtonDemoViewModel : ViewModelBase
{
    private bool isBusy;

    [PropertyDisplay("Is Authenticated", "State", 2)]
    [ObservableProperty]
    private bool isAuthenticated;

    [PropertyDisplay("Is Busy", "State", 1)]
    public new bool IsBusy
    {
        get => isBusy;
        set => Set(ref isBusy, value);
    }

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
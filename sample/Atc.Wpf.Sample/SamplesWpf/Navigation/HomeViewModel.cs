namespace Atc.Wpf.Sample.SamplesWpf.Navigation;

public sealed class HomeViewModel : ViewModelBase, INavigationAware
{
    private string message = "Welcome to the Navigation Demo!";

    public string Message
    {
        get => message;
        set
        {
            message = value;
            RaisePropertyChanged();
        }
    }

    public void OnNavigatedTo(NavigationParameters? parameters)
    {
        Message = "Welcome to the Navigation Demo! (OnNavigatedTo called)";
    }

    public void OnNavigatedFrom()
    {
        // Clean up if needed
    }
}

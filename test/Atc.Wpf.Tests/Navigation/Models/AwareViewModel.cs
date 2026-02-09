namespace Atc.Wpf.Tests.Navigation.Models;

internal sealed class AwareViewModel : INavigationAware
{
    public bool NavigatedToCalled { get; private set; }

    public bool NavigatedFromCalled { get; private set; }

    public void OnNavigatedTo(NavigationParameters? parameters)
        => NavigatedToCalled = true;

    public void OnNavigatedFrom()
        => NavigatedFromCalled = true;
}
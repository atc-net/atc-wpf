namespace Atc.Wpf.Tests.Navigation.Models;

internal sealed class GuardedViewModel : INavigationGuard
{
    public bool AllowNavigation { get; set; } = true;

    public bool CanNavigateAway()
        => AllowNavigation;

    public Task<bool> CanNavigateAwayAsync(
        CancellationToken cancellationToken = default)
        => Task.FromResult(AllowNavigation);
}
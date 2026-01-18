namespace Atc.Wpf.Sample.SamplesWpf.Navigation;

public sealed class DetailsViewModel : ViewModelBase, INavigationAware
{
    private int itemId;
    private string itemName = string.Empty;

    public int ItemId
    {
        get => itemId;
        set
        {
            itemId = value;
            RaisePropertyChanged();
        }
    }

    public string ItemName
    {
        get => itemName;
        set
        {
            itemName = value;
            RaisePropertyChanged();
        }
    }

    public void OnNavigatedTo(NavigationParameters? parameters)
    {
        if (parameters is null)
        {
            return;
        }

        ItemId = parameters.GetValueOrDefault("ItemId", 0);
        ItemName = parameters.GetValueOrDefault("ItemName", "Unknown") ?? "Unknown";
    }

    public void OnNavigatedFrom()
    {
        // Clean up if needed
    }
}
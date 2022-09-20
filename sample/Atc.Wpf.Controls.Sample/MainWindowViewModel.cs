namespace Atc.Wpf.Controls.Sample;

public class MainWindowViewModel : MainWindowViewModelBase, IMainWindowViewModel
{
    private string selectedKeyTheme = "Light";
    private string selectedKeyAccentColor = "Blue";

    public MainWindowViewModel()
    {
        Themes = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var item in ThemeManager.Current.BaseColors)
        {
            Themes.Add(item, item);
        }

        AccentColors = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var item in ThemeManager.Current.ColorSchemes)
        {
            AccentColors.Add(item, item);
        }
    }

    public IDictionary<string, string> Themes { get; }

    public IDictionary<string, string> AccentColors { get; }

    public string SelectedKeyTheme
    {
        get => selectedKeyTheme;
        set
        {
            selectedKeyTheme = value;
            RaisePropertyChanged();

            ThemeManager.Current.ChangeTheme(Application.Current, SelectedKeyTheme, SelectedKeyAccentColor);
        }
    }

    public string SelectedKeyAccentColor
    {
        get => selectedKeyAccentColor;
        set
        {
            selectedKeyAccentColor = value;
            RaisePropertyChanged();

            ThemeManager.Current.ChangeTheme(Application.Current, SelectedKeyTheme, SelectedKeyAccentColor);
        }
    }

    public void UpdateSelectedView(
        SampleTreeViewItem? sampleTreeViewItem)
    {
        var samplePath = sampleTreeViewItem?.SamplePath;
        var header = sampleTreeViewItem?.Header?.ToString();
        Messenger.Default.Send(new SampleItemMessage(header, samplePath));
    }
}
namespace Atc.Wpf.Sample.SamplesWpfTheming.Theme;

public partial class WindowsThemeSyncView : INotifyPropertyChanged
{
    public WindowsThemeSyncView()
    {
        InitializeComponent();
        DataContext = this;

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool FollowAppMode
    {
        get => WindowsThemeSync.Mode.HasFlag(WindowsThemeSyncMode.AppMode);
        set => SetSyncPart(WindowsThemeSyncMode.AppMode, value);
    }

    public bool FollowAccent
    {
        get => WindowsThemeSync.Mode.HasFlag(WindowsThemeSyncMode.Accent);
        set => SetSyncPart(WindowsThemeSyncMode.Accent, value);
    }

    public string WindowsAppMode
        => WindowsThemeSync.IsWindowsAppModeLight
            ? "Light"
            : "Dark";

    public string WindowsAccent
        => WindowsThemeSync.WindowsAccentColor?.ToString(GlobalizationConstants.EnglishCultureInfo) ?? "Unknown";

    public Brush WindowsAccentBrush
        => WindowsThemeSync.WindowsAccentColor is { } color
            ? new SolidColorBrush(color)
            : Brushes.Transparent;

    public string CurrentTheme
        => ThemeManager.Current.DetectTheme(Application.Current)?.Name ?? string.Empty;

    private static void SetSyncPart(
        WindowsThemeSyncMode part,
        bool follow)
        => WindowsThemeSync.Mode = follow
            ? WindowsThemeSync.Mode | part
            : WindowsThemeSync.Mode & ~part;

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        WindowsThemeSync.ModeChanged += OnModeChanged;
        ThemeManager.Current.ThemeChanged += OnThemeChanged;
        RaiseAll();
    }

    private void OnUnloaded(
        object sender,
        RoutedEventArgs e)
    {
        WindowsThemeSync.ModeChanged -= OnModeChanged;
        ThemeManager.Current.ThemeChanged -= OnThemeChanged;
    }

    private void OnModeChanged(
        object? sender,
        EventArgs e)
        => RaiseAll();

    private void OnThemeChanged(
        object? sender,
        ThemeChangedEventArgs e)
        => RaiseAll();

    private void RaiseAll()
    {
        OnPropertyChanged(nameof(FollowAppMode));
        OnPropertyChanged(nameof(FollowAccent));
        OnPropertyChanged(nameof(WindowsAppMode));
        OnPropertyChanged(nameof(WindowsAccent));
        OnPropertyChanged(nameof(WindowsAccentBrush));
        OnPropertyChanged(nameof(CurrentTheme));
    }

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
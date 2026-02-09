namespace Atc.Wpf.Sample.SamplesWpf.Hotkeys;

public sealed partial class HotkeyServiceViewModel : ViewModelBase, IDisposable
{
    private readonly IHotkeyService hotkeyService = new HotkeyService();
    private readonly List<IHotkeyRegistration> activeRegistrations = [];

    private string statusText = "Not listening";
    private ObservableCollection<IHotkeyRegistration> registeredHotkeys = [];
    private ObservableCollection<string> eventLog = [];

    public HotkeyServiceViewModel()
    {
        hotkeyService.HotkeyPressed += OnHotkeyPressed;
        hotkeyService.ConflictDetected += OnConflictDetected;
    }

    public string StatusText
    {
        get => statusText;
        set
        {
            statusText = value;
            RaisePropertyChanged();
        }
    }

    public bool IsListening => hotkeyService.IsListening;

    public ObservableCollection<IHotkeyRegistration> RegisteredHotkeys
    {
        get => registeredHotkeys;
        set
        {
            registeredHotkeys = value;
            RaisePropertyChanged();
        }
    }

    public ObservableCollection<string> EventLog
    {
        get => eventLog;
        set
        {
            eventLog = value;
            RaisePropertyChanged();
        }
    }

    [RelayCommand]
    private void ToggleListening()
    {
        if (hotkeyService.IsListening)
        {
            hotkeyService.StopListening();
            StatusText = "Stopped listening";
            EventLog.Insert(0, $"[{DateTime.Now:HH:mm:ss}] Listening stopped");
        }
        else
        {
            var window = Application.Current.MainWindow;
            if (window is null)
            {
                StatusText = "No main window available";
                return;
            }

            hotkeyService.StartListening(window);
            StatusText = "Listening for hotkeys";
            EventLog.Insert(0, $"[{DateTime.Now:HH:mm:ss}] Listening started");
        }

        RaisePropertyChanged(nameof(IsListening));
    }

    [RelayCommand]
    private void RegisterGlobalHotkey()
    {
        var reg = hotkeyService.Register(
            ModifierKeys.Control | ModifierKeys.Shift,
            Key.H,
            _ => { },
            "Ctrl+Shift+H (Global)",
            HotkeyScope.Global);

        activeRegistrations.Add(reg);
        RefreshRegistrations();
        StatusText = "Registered global hotkey: Ctrl+Shift+H";
        EventLog.Insert(0, $"[{DateTime.Now:HH:mm:ss}] Registered Ctrl+Shift+H (Global)");
    }

    [RelayCommand]
    private void RegisterLocalHotkey()
    {
        var reg = hotkeyService.Register(
            ModifierKeys.Control,
            Key.G,
            _ => { },
            "Ctrl+G (Local)",
            HotkeyScope.Local);

        activeRegistrations.Add(reg);
        RefreshRegistrations();
        StatusText = "Registered local hotkey: Ctrl+G";
        EventLog.Insert(0, $"[{DateTime.Now:HH:mm:ss}] Registered Ctrl+G (Local)");
    }

    [RelayCommand]
    private void RegisterChord()
    {
        var reg = hotkeyService.RegisterChord(
            ModifierKeys.Control,
            Key.K,
            ModifierKeys.Control,
            Key.T,
            _ => { },
            "Ctrl+K, Ctrl+T (Chord)");

        activeRegistrations.Add(reg);
        RefreshRegistrations();
        StatusText = "Registered chord: Ctrl+K, Ctrl+T";
        EventLog.Insert(0, $"[{DateTime.Now:HH:mm:ss}] Registered chord Ctrl+K, Ctrl+T");
    }

    [RelayCommand]
    private void TriggerConflict()
    {
        var reg = hotkeyService.Register(
            ModifierKeys.Control | ModifierKeys.Shift,
            Key.H,
            _ => { },
            "Ctrl+Shift+H (Duplicate)",
            HotkeyScope.Global);

        activeRegistrations.Add(reg);
        RefreshRegistrations();
        StatusText = "Registered duplicate Ctrl+Shift+H — check log for conflict event";
    }

    [RelayCommand]
    private void UnregisterAll()
    {
        foreach (var reg in activeRegistrations)
        {
            hotkeyService.Unregister(reg);
        }

        activeRegistrations.Clear();
        RefreshRegistrations();
        StatusText = "All hotkeys unregistered";
        EventLog.Insert(0, $"[{DateTime.Now:HH:mm:ss}] All hotkeys unregistered");
    }

    [RelayCommand]
    private void SaveBindings()
    {
        var filePath = Path.Combine(Path.GetTempPath(), "atc-wpf-hotkeys.json");
        hotkeyService.SaveBindings(filePath);
        StatusText = $"Bindings saved to {filePath}";
        EventLog.Insert(0, $"[{DateTime.Now:HH:mm:ss}] Saved {hotkeyService.Registrations.Count} binding(s) to {filePath}");
    }

    [RelayCommand]
    private void LoadBindings()
    {
        var filePath = Path.Combine(Path.GetTempPath(), "atc-wpf-hotkeys.json");
        if (!File.Exists(filePath))
        {
            StatusText = "No saved bindings file found";
            EventLog.Insert(0, $"[{DateTime.Now:HH:mm:ss}] No file found at {filePath}");
            return;
        }

        hotkeyService.LoadBindings(filePath);
        RefreshRegistrations();
        StatusText = $"Bindings loaded from {filePath}";
        EventLog.Insert(0, $"[{DateTime.Now:HH:mm:ss}] Loaded bindings from {filePath} — {hotkeyService.Registrations.Count} total registration(s)");
    }

    [RelayCommand]
    private void ClearLog()
    {
        EventLog.Clear();
    }

    public void Dispose()
    {
        hotkeyService.HotkeyPressed -= OnHotkeyPressed;
        hotkeyService.ConflictDetected -= OnConflictDetected;
        hotkeyService.Dispose();
    }

    private void OnHotkeyPressed(
        object? sender,
        HotkeyPressedEventArgs e)
    {
        var desc = e.Registration.Description ?? $"Id={e.Registration.Id}";
        EventLog.Insert(0, $"[{DateTime.Now:HH:mm:ss}] Hotkey pressed: {desc}");
    }

    private void OnConflictDetected(
        object? sender,
        HotkeyConflictEventArgs e)
    {
        var existingDesc = e.Existing.Description ?? $"Id={e.Existing.Id}";
        var requestedDesc = e.Requested.Description ?? $"Id={e.Requested.Id}";
        EventLog.Insert(0, $"[{DateTime.Now:HH:mm:ss}] Conflict: '{requestedDesc}' conflicts with '{existingDesc}'");
    }

    private void RefreshRegistrations()
    {
        RegisteredHotkeys = new ObservableCollection<IHotkeyRegistration>(hotkeyService.Registrations);
    }
}
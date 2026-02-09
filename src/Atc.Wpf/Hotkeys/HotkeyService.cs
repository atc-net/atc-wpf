namespace Atc.Wpf.Hotkeys;

/// <summary>
/// Provides keyboard shortcut management with support for global (system-wide)
/// and local (window-scoped) hotkeys, two-stroke chords, and conflict detection.
/// </summary>
public sealed class HotkeyService : IHotkeyService
{
    private const int WmHotkey = 0x0312;
    private static readonly TimeSpan ChordTimeout = TimeSpan.FromMilliseconds(1500);
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() },
    };

    private readonly Dispatcher dispatcher;
    private readonly Lock registrationLock = new();
    private readonly List<HotkeyRegistration> registrations = [];

    private int nextId;

    [SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Disposed in DestroyListener() via StopListening().")]
    private HwndSource? hwndSource;
    private Window? targetWindow;
    private bool disposed;

    // Chord state
    private HotkeyRegistration? pendingChordFirstStroke;
    private DispatcherTimer? chordTimer;

    public HotkeyService(Dispatcher? dispatcher = null)
    {
        this.dispatcher = dispatcher
            ?? Application.Current?.Dispatcher
            ?? Dispatcher.CurrentDispatcher;
    }

    public event EventHandler<HotkeyPressedEventArgs>? HotkeyPressed;

    public event EventHandler<HotkeyConflictEventArgs>? ConflictDetected;

    public IReadOnlyList<IHotkeyRegistration> Registrations
    {
        get
        {
            lock (registrationLock)
            {
                return registrations.ToList().AsReadOnly();
            }
        }
    }

    public bool IsListening => hwndSource is not null;

    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "Registration logic with conflict detection and scope handling.")]
    public IHotkeyRegistration Register(
        ModifierKeys modifiers,
        Key key,
        Action<HotkeyPressedEventArgs> callback,
        string? description = null,
        HotkeyScope scope = HotkeyScope.Local)
    {
        ArgumentNullException.ThrowIfNull(callback);

        var id = Interlocked.Increment(ref nextId);

        var registration = new HotkeyRegistration(
            id,
            modifiers,
            key,
            callback,
            description,
            scope,
            chord: null,
            unregisterCallback: r => Unregister(r));

        lock (registrationLock)
        {
            var existing = registrations.Find(r =>
                r.Chord is null &&
                r.Modifiers == modifiers &&
                r.Key == key &&
                r.Scope == scope);

            if (existing is not null)
            {
                ConflictDetected?.Invoke(this, new HotkeyConflictEventArgs(existing, registration));
            }

            registrations.Add(registration);
        }

        if (scope == HotkeyScope.Global && hwndSource is not null)
        {
            RegisterGlobalHotkey(registration);
        }
        else if (scope == HotkeyScope.Local && targetWindow is not null)
        {
            RegisterLocalHotkey(registration);
        }

        return registration;
    }

    public IHotkeyRegistration RegisterChord(
        ModifierKeys firstModifiers,
        Key firstKey,
        ModifierKeys secondModifiers,
        Key secondKey,
        Action<HotkeyPressedEventArgs> callback,
        string? description = null)
    {
        ArgumentNullException.ThrowIfNull(callback);

        var id = Interlocked.Increment(ref nextId);

        var chord = new HotkeyChord(firstModifiers, firstKey, secondModifiers, secondKey);

        var registration = new HotkeyRegistration(
            id,
            firstModifiers,
            firstKey,
            callback,
            description,
            HotkeyScope.Local,
            chord,
            unregisterCallback: r => Unregister(r));

        lock (registrationLock)
        {
            var existing = registrations.Find(r =>
                r.Chord is not null &&
                r.Chord.Equals(chord));

            if (existing is not null)
            {
                ConflictDetected?.Invoke(this, new HotkeyConflictEventArgs(existing, registration));
            }

            registrations.Add(registration);
        }

        return registration;
    }

    public void Unregister(IHotkeyRegistration registration)
    {
        ArgumentNullException.ThrowIfNull(registration);

        if (registration is not HotkeyRegistration reg)
        {
            return;
        }

        lock (registrationLock)
        {
            if (!registrations.Remove(reg))
            {
                return;
            }
        }

        if (reg.Scope == HotkeyScope.Global && hwndSource is not null)
        {
            UnregisterGlobalHotkey(reg);
        }
        else if (reg.Scope == HotkeyScope.Local && targetWindow is not null)
        {
            RemoveLocalHotkey(reg);
        }
    }

    public void StartListening(Window window)
    {
        ArgumentNullException.ThrowIfNull(window);

        if (hwndSource is not null)
        {
            return;
        }

        targetWindow = window;

        if (dispatcher.CheckAccess())
        {
            CreateListener();
        }
        else
        {
            dispatcher.Invoke(CreateListener);
        }
    }

    public void StopListening()
    {
        if (hwndSource is null)
        {
            return;
        }

        if (dispatcher.CheckAccess())
        {
            DestroyListener();
        }
        else
        {
            dispatcher.Invoke(DestroyListener);
        }
    }

    public void SaveBindings(string filePath)
    {
        ArgumentException.ThrowIfNullOrEmpty(filePath);

        List<HotkeyBindingDefinition> definitions;
        lock (registrationLock)
        {
            definitions = registrations
                .Select(r =>
                {
                    var def = new HotkeyBindingDefinition
                    {
                        Modifiers = r.Modifiers,
                        Key = r.Key,
                        Description = r.Description,
                        Scope = r.Scope,
                    };

                    if (r.Chord is not null)
                    {
                        def.SecondModifiers = r.Chord.SecondModifiers;
                        def.SecondKey = r.Chord.SecondKey;
                    }

                    return def;
                })
                .ToList();
        }

        var json = JsonSerializer.Serialize(definitions, JsonOptions);
        File.WriteAllText(filePath, json);
    }

    public void LoadBindings(string filePath)
    {
        ArgumentException.ThrowIfNullOrEmpty(filePath);

        var json = File.ReadAllText(filePath);
        var definitions = JsonSerializer.Deserialize<List<HotkeyBindingDefinition>>(
            json,
            JsonOptions);

        if (definitions is null)
        {
            return;
        }

        foreach (var def in definitions)
        {
            if (def.SecondModifiers.HasValue && def.SecondKey.HasValue)
            {
                RegisterChord(
                    def.Modifiers,
                    def.Key,
                    def.SecondModifiers.Value,
                    def.SecondKey.Value,
                    static _ => { },
                    def.Description);
            }
            else
            {
                Register(
                    def.Modifiers,
                    def.Key,
                    static _ => { },
                    def.Description,
                    def.Scope);
            }
        }
    }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        CancelChordPending();
        StopListening();

        lock (registrationLock)
        {
            registrations.Clear();
        }
    }

    private static uint GetWin32Modifiers(ModifierKeys modifiers)
    {
        uint result = 0;

        if (modifiers.HasFlag(ModifierKeys.Alt))
        {
            result |= 0x0001; // MOD_ALT
        }

        if (modifiers.HasFlag(ModifierKeys.Control))
        {
            result |= 0x0002; // MOD_CONTROL
        }

        if (modifiers.HasFlag(ModifierKeys.Shift))
        {
            result |= 0x0004; // MOD_SHIFT
        }

        if (modifiers.HasFlag(ModifierKeys.Windows))
        {
            result |= 0x0008; // MOD_WIN
        }

        return result;
    }

    private void CreateListener()
    {
        var parameters = new HwndSourceParameters("HotkeyListener")
        {
            ParentWindow = new IntPtr(-3), // HWND_MESSAGE
            Width = 0,
            Height = 0,
        };

        hwndSource = new HwndSource(parameters);
        hwndSource.AddHook(WndProc);

        // Register existing global hotkeys
        List<HotkeyRegistration> globalRegistrations;
        lock (registrationLock)
        {
            globalRegistrations = registrations
                .Where(r => r.Scope == HotkeyScope.Global && r.Chord is null)
                .ToList();
        }

        foreach (var reg in globalRegistrations)
        {
            RegisterGlobalHotkey(reg);
        }

        // Register existing local hotkeys
        List<HotkeyRegistration> localRegistrations;
        lock (registrationLock)
        {
            localRegistrations = registrations
                .Where(r => r.Scope == HotkeyScope.Local && r.Chord is null)
                .ToList();
        }

        foreach (var reg in localRegistrations)
        {
            RegisterLocalHotkey(reg);
        }

        // Hook PreviewKeyDown for chord detection
        if (targetWindow is not null)
        {
            targetWindow.PreviewKeyDown += OnWindowPreviewKeyDown;
        }
    }

    private void DestroyListener()
    {
        if (targetWindow is not null)
        {
            targetWindow.PreviewKeyDown -= OnWindowPreviewKeyDown;
        }

        CancelChordPending();

        // Unregister all global hotkeys
        List<HotkeyRegistration> globalRegistrations;
        lock (registrationLock)
        {
            globalRegistrations = registrations
                .Where(r => r.Scope == HotkeyScope.Global && r.Chord is null)
                .ToList();
        }

        foreach (var reg in globalRegistrations)
        {
            UnregisterGlobalHotkey(reg);
        }

        // Remove local input bindings
        List<HotkeyRegistration> localRegistrations;
        lock (registrationLock)
        {
            localRegistrations = registrations
                .Where(r => r.Scope == HotkeyScope.Local && r.Chord is null)
                .ToList();
        }

        foreach (var reg in localRegistrations)
        {
            RemoveLocalHotkey(reg);
        }

        if (hwndSource is not null)
        {
            hwndSource.RemoveHook(WndProc);
            hwndSource.Dispose();
            hwndSource = null;
        }

        targetWindow = null;
    }

    private void RegisterGlobalHotkey(HotkeyRegistration registration)
    {
        if (hwndSource is null)
        {
            return;
        }

        var vk = (uint)KeyInterop.VirtualKeyFromKey(registration.Key);
        var mods = GetWin32Modifiers(registration.Modifiers);

        NativeMethods.RegisterHotKey(hwndSource.Handle, registration.Id, mods, vk);
    }

    private void UnregisterGlobalHotkey(HotkeyRegistration registration)
    {
        if (hwndSource is null)
        {
            return;
        }

        NativeMethods.UnregisterHotKey(hwndSource.Handle, registration.Id);
    }

    private void RegisterLocalHotkey(HotkeyRegistration registration)
    {
        if (targetWindow is null)
        {
            return;
        }

        var gesture = new KeyGesture(registration.Key, registration.Modifiers);
        var command = new HotkeyRoutedCommand(registration, this);
        var binding = new InputBinding(command, gesture) { CommandParameter = registration };
        targetWindow.InputBindings.Add(binding);
    }

    private void RemoveLocalHotkey(HotkeyRegistration registration)
    {
        if (targetWindow is null)
        {
            return;
        }

        for (var i = targetWindow.InputBindings.Count - 1; i >= 0; i--)
        {
            if (targetWindow.InputBindings[i].CommandParameter is HotkeyRegistration reg &&
                reg.Id == registration.Id)
            {
                targetWindow.InputBindings.RemoveAt(i);
            }
        }
    }

    private IntPtr WndProc(
        IntPtr hwnd,
        int msg,
        IntPtr wParam,
        IntPtr lParam,
        ref bool handled)
    {
        if (msg != WmHotkey)
        {
            return IntPtr.Zero;
        }

        handled = true;

        var id = wParam.ToInt32();

        HotkeyRegistration? registration;
        lock (registrationLock)
        {
            registration = registrations.Find(r => r.Id == id);
        }

        if (registration is not null)
        {
            InvokeCallback(registration);
        }

        return IntPtr.Zero;
    }

    private void OnWindowPreviewKeyDown(
        object sender,
        KeyEventArgs e)
    {
        var key = e.Key == Key.System ? e.SystemKey : e.Key;
        var modifiers = Keyboard.Modifiers;

        if (pendingChordFirstStroke is not null)
        {
            HandleChordSecondStroke(key, modifiers, e);
            return;
        }

        // Check if this key combo matches the first stroke of any chord
        List<HotkeyRegistration> chordRegistrations;
        lock (registrationLock)
        {
            chordRegistrations = registrations
                .Where(r =>
                    r.Chord is not null &&
                    r.Chord.FirstModifiers == modifiers &&
                    r.Chord.FirstKey == key)
                .ToList();
        }

        if (chordRegistrations.Count > 0)
        {
            pendingChordFirstStroke = chordRegistrations[0];
            StartChordTimer();
            e.Handled = true;
        }
    }

    private void HandleChordSecondStroke(
        Key key,
        ModifierKeys modifiers,
        KeyEventArgs e)
    {
        CancelChordPending();

        List<HotkeyRegistration> matches;
        lock (registrationLock)
        {
            matches = registrations
                .Where(r =>
                    r.Chord is not null &&
                    r.Chord.FirstModifiers == pendingChordFirstStroke!.Chord!.FirstModifiers &&
                    r.Chord.FirstKey == pendingChordFirstStroke.Chord.FirstKey &&
                    r.Chord.SecondModifiers == modifiers &&
                    r.Chord.SecondKey == key)
                .ToList();
        }

        pendingChordFirstStroke = null;

        foreach (var match in matches)
        {
            InvokeCallback(match);
            e.Handled = true;
        }
    }

    private void StartChordTimer()
    {
        chordTimer = new DispatcherTimer(DispatcherPriority.Normal, dispatcher)
        {
            Interval = ChordTimeout,
        };

        chordTimer.Tick += OnChordTimeout;
        chordTimer.Start();
    }

    private void OnChordTimeout(
        object? sender,
        EventArgs e)
    {
        CancelChordPending();
        pendingChordFirstStroke = null;
    }

    private void CancelChordPending()
    {
        if (chordTimer is null)
        {
            return;
        }

        chordTimer.Stop();
        chordTimer.Tick -= OnChordTimeout;
        chordTimer = null;
    }

    internal void InvokeCallback(HotkeyRegistration registration)
    {
        var args = new HotkeyPressedEventArgs(registration);
        registration.Callback(args);
        HotkeyPressed?.Invoke(this, args);
    }

    /// <summary>
    /// An ICommand implementation used internally for local hotkey InputBindings.
    /// </summary>
    private sealed class HotkeyRoutedCommand(
        HotkeyRegistration registration,
        HotkeyService service) : ICommand
    {
        public event EventHandler? CanExecuteChanged
        {
            add
            {
                // Intentionally empty — CanExecute is always true.
            }

            remove
            {
                // Intentionally empty — CanExecute is always true.
            }
        }

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            service.InvokeCallback(registration);
        }
    }
}
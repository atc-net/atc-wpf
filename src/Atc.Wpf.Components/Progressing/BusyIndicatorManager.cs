namespace Atc.Wpf.Components.Progressing;

/// <summary>
/// Manages the registration and activation of <see cref="BusyOverlay"/> controls.
/// Use the <see cref="RegionNameProperty"/> attached property on a BusyOverlay to register it as a named region.
/// </summary>
public static class BusyIndicatorManager
{
    private static readonly ConcurrentDictionary<string, ConcurrentBag<WeakReference<BusyOverlay>>> Regions = new(StringComparer.Ordinal);

    private static readonly ConcurrentDictionary<Guid, BusySession> ActiveSessions = new();

    /// <summary>
    /// Attached property to register a BusyOverlay as a named region.
    /// </summary>
    public static readonly DependencyProperty RegionNameProperty =
        DependencyProperty.RegisterAttached(
            "RegionName",
            typeof(string),
            typeof(BusyIndicatorManager),
            new PropertyMetadata(
                defaultValue: null,
                propertyChangedCallback: OnRegionNameChanged));

    public static string? GetRegionName(DependencyObject obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        return (string?)obj.GetValue(RegionNameProperty);
    }

    public static void SetRegionName(
        DependencyObject obj,
        string? value)
    {
        ArgumentNullException.ThrowIfNull(obj);
        obj.SetValue(RegionNameProperty, value);
    }

    internal static BusyToken ShowBusy(
        Dispatcher dispatcher,
        string message,
        string regionName,
        bool allowCancellation)
    {
        var token = new BusyToken { RegionName = regionName };
        var cts = allowCancellation ? new CancellationTokenSource() : null;
        var session = new BusySession(cts);

        ActiveSessions[token.Id] = session;

        if (!dispatcher.CheckAccess())
        {
            _ = dispatcher.BeginInvoke(
                new Action(() => ApplyBusy(
                    token,
                    message,
                    allowCancellation,
                    cts)));
            return token;
        }

        ApplyBusy(
            token,
            message,
            allowCancellation,
            cts);
        return token;
    }

    internal static void HideBusy(
        Dispatcher dispatcher,
        BusyToken token)
    {
        if (!ActiveSessions.TryRemove(token.Id, out var session))
        {
            return;
        }

        session.Cts?.Dispose();

        if (!dispatcher.CheckAccess())
        {
            _ = dispatcher.BeginInvoke(
                new Action(() => ClearOverlays(token.RegionName)));
            return;
        }

        ClearOverlays(token.RegionName);
    }

    internal static void HideAllBusy(
        Dispatcher dispatcher,
        string regionName)
    {
        var tokensToRemove = ActiveSessions
            .Where(kv => string.Equals(kv.Value.RegionName, regionName, StringComparison.Ordinal))
            .Select(kv => kv.Key)
            .ToArray();

        foreach (var id in tokensToRemove)
        {
            if (ActiveSessions.TryRemove(id, out var session))
            {
                session.Cts?.Dispose();
            }
        }

        if (!dispatcher.CheckAccess())
        {
            _ = dispatcher.BeginInvoke(
                new Action(() => ClearOverlays(regionName)));
            return;
        }

        ClearOverlays(regionName);
    }

    internal static void ReportProgress(
        Dispatcher dispatcher,
        BusyToken token,
        BusyInfo info)
    {
        if (!ActiveSessions.ContainsKey(token.Id))
        {
            return;
        }

        if (!dispatcher.CheckAccess())
        {
            _ = dispatcher.BeginInvoke(
                new Action(() => UpdateOverlayContent(
                    token.RegionName,
                    info.DisplayText)));
            return;
        }

        UpdateOverlayContent(
            token.RegionName,
            info.DisplayText);
    }

    internal static CancellationToken GetCancellationToken(BusyToken token)
    {
        if (ActiveSessions.TryGetValue(token.Id, out var session) && session.Cts is not null)
        {
            return session.Cts.Token;
        }

        return CancellationToken.None;
    }

    internal static void MarkCancelled(BusyToken token)
        => token.IsCancelled = true;

    private static void OnRegionNameChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not BusyOverlay overlay)
        {
            return;
        }

        if (e.OldValue is string oldName)
        {
            RemoveOverlay(
                oldName,
                overlay);
        }

        if (e.NewValue is string newName)
        {
            overlay.Loaded += OnOverlayLoaded;
            overlay.Unloaded += OnOverlayUnloaded;

            if (overlay.IsLoaded)
            {
                AddOverlay(
                    newName,
                    overlay);
            }
        }
    }

    private static void OnOverlayLoaded(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is BusyOverlay overlay)
        {
            var name = GetRegionName(overlay) ?? string.Empty;
            AddOverlay(
                name,
                overlay);
        }
    }

    private static void OnOverlayUnloaded(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is BusyOverlay overlay)
        {
            var name = GetRegionName(overlay) ?? string.Empty;
            RemoveOverlay(
                name,
                overlay);
        }
    }

    private static void AddOverlay(
        string regionName,
        BusyOverlay overlay)
    {
        var bag = Regions.GetOrAdd(regionName, _ => []);
        bag.Add(new WeakReference<BusyOverlay>(overlay));
    }

    private static void RemoveOverlay(
        string regionName,
        BusyOverlay overlay)
    {
        if (!Regions.TryGetValue(regionName, out var bag))
        {
            return;
        }

        var newBag = new ConcurrentBag<WeakReference<BusyOverlay>>(
            bag.Where(wr => wr.TryGetTarget(out var target) && !ReferenceEquals(target, overlay)));
        Regions[regionName] = newBag;
    }

    private static void ApplyBusy(
        BusyToken token,
        string message,
        bool allowCancellation,
        CancellationTokenSource? cts)
    {
        if (ActiveSessions.TryGetValue(token.Id, out var session))
        {
            session.RegionName = token.RegionName;
        }

        foreach (var overlay in GetOverlays(token.RegionName))
        {
            overlay.BusyContent = string.IsNullOrEmpty(message)
                ? null
                : message;

            if (allowCancellation && cts is not null)
            {
                var cancelButton = new Button
                {
                    Content = Miscellaneous.Cancel,
                    MinWidth = 80,
                    Margin = new Thickness(0, 8, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Center,
                };

                cancelButton.Click += (_, _) =>
                {
                    token.IsCancelled = true;
                    cts.Cancel();
                };

                overlay.BusyContentAfter = cancelButton;
            }

            overlay.IsBusy = true;
        }
    }

    private static void ClearOverlays(string regionName)
    {
        foreach (var overlay in GetOverlays(regionName))
        {
            overlay.IsBusy = false;
            overlay.BusyContent = null;
            overlay.BusyContentAfter = null;
        }
    }

    private static void UpdateOverlayContent(
        string regionName,
        string displayText)
    {
        foreach (var overlay in GetOverlays(regionName))
        {
            overlay.BusyContent = displayText;
        }
    }

    private static IEnumerable<BusyOverlay> GetOverlays(string regionName)
    {
        if (Regions.TryGetValue(regionName, out var bag))
        {
            foreach (var wr in bag)
            {
                if (wr.TryGetTarget(out var overlay))
                {
                    yield return overlay;
                }
            }

            yield break;
        }

        if (!string.IsNullOrEmpty(regionName) && Regions.TryGetValue(string.Empty, out var defaultBag))
        {
            foreach (var wr in defaultBag)
            {
                if (wr.TryGetTarget(out var overlay))
                {
                    yield return overlay;
                }
            }
        }
    }

    private sealed class BusySession
    {
        public BusySession(CancellationTokenSource? cts)
        {
            Cts = cts;
        }

        public CancellationTokenSource? Cts { get; }

        public string RegionName { get; set; } = string.Empty;
    }
}
namespace Atc.Wpf.Components.Progressing;

/// <summary>
/// Manages the registration and activation of <see cref="BusyOverlay"/> controls.
/// Use the <see cref="RegionNameProperty"/> attached property on a BusyOverlay to register it as a named region.
/// </summary>
public static class BusyIndicatorManager
{
    private static readonly ConcurrentDictionary<string, ConcurrentBag<WeakReference<BusyOverlay>>> Regions = new(StringComparer.Ordinal);

    private static readonly ConcurrentDictionary<Guid, BusySession> ActiveSessions = new();

    private static long sessionSequence;

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

    /// <summary>
    /// Gets the region name the specified <see cref="BusyOverlay"/> is registered under.
    /// </summary>
    public static string? GetRegionName(DependencyObject obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        return (string?)obj.GetValue(RegionNameProperty);
    }

    /// <summary>
    /// Sets the region name that registers the specified <see cref="BusyOverlay"/> as a named region.
    /// </summary>
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
        var session = new BusySession(
            token,
            message,
            cts,
            Interlocked.Increment(ref sessionSequence))
        {
            RegionName = regionName,
        };

        ActiveSessions[token.Id] = session;

        if (!dispatcher.CheckAccess())
        {
            _ = dispatcher.BeginInvoke(
                new Action(() => ApplyBusy(token)));
            return token;
        }

        ApplyBusy(token);
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

        if (IsRegionBusy(token.RegionName))
        {
            // Another session still owns the region, so its overlay stays up, showing the newest
            // remaining session's message and Cancel button instead of the hidden session's.
            if (!dispatcher.CheckAccess())
            {
                _ = dispatcher.BeginInvoke(
                    new Action(() => ReapplyNewestSession(token.RegionName)));
                return;
            }

            ReapplyNewestSession(token.RegionName);
            return;
        }

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

    private static void ApplyBusy(BusyToken token)
    {
        if (!ActiveSessions.TryGetValue(token.Id, out var session))
        {
            // Hidden before this queued apply ran; showing now would leave the overlay stuck.
            return;
        }

        ApplySession(session);
    }

    private static void ReapplyNewestSession(string regionName)
    {
        var newest = ActiveSessions.Values
            .Where(s => string.Equals(s.RegionName, regionName, StringComparison.Ordinal))
            .MaxBy(s => s.Sequence);

        if (newest is not null)
        {
            ApplySession(newest);
        }
    }

    private static void ApplySession(BusySession session)
    {
        var token = session.Token;
        var cts = session.Cts;

        foreach (var overlay in GetOverlays(session.RegionName))
        {
            overlay.BusyContent = string.IsNullOrEmpty(session.Message)
                ? null
                : session.Message;

            if (cts is not null)
            {
                var cancelButton = new Button
                {
                    Content = Word.Cancel,
                    MinWidth = 80,
                    Margin = new Thickness(0, 8, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Center,
                };

                cancelButton.Click += (_, _) => CancelSession(token, cts);

                overlay.BusyContentAfter = cancelButton;
            }
            else
            {
                // A Cancel button left by an earlier session must not outlive it.
                overlay.BusyContentAfter = null;
            }

            overlay.IsBusy = true;
        }
    }

    private static void CancelSession(
        BusyToken token,
        CancellationTokenSource cts)
    {
        if (!ActiveSessions.ContainsKey(token.Id))
        {
            // The session ended (and disposed its source) after the button was shown.
            return;
        }

        try
        {
            token.IsCancelled = true;
            cts.Cancel();
        }
        catch (ObjectDisposedException)
        {
            // The session ended on another thread between the check and the cancel.
        }
    }

    private static bool IsRegionBusy(string regionName)
        => ActiveSessions.Values.Any(s => string.Equals(s.RegionName, regionName, StringComparison.Ordinal));

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
        public BusySession(
            BusyToken token,
            string message,
            CancellationTokenSource? cts,
            long sequence)
        {
            Token = token;
            Message = message;
            Cts = cts;
            Sequence = sequence;
        }

        public BusyToken Token { get; }

        public string Message { get; }

        public CancellationTokenSource? Cts { get; }

        public long Sequence { get; }

        public string RegionName { get; set; } = string.Empty;
    }
}
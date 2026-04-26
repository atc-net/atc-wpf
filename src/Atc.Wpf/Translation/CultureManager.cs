// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
namespace Atc.Wpf.Translation;

/// <summary>
/// Provides the ability to change the UICulture for WPF Windows and controls
/// dynamically.
/// </summary>
/// <remarks>
/// XAML elements that use the <see cref="ResxExtension" /> are automatically
/// updated when the <see cref="UiCulture" /> property is changed.
/// </remarks>
public static class CultureManager
{
    private static readonly Lock HandlerLock = new();
    private static readonly List<WeakSubscription> HandlerSubscriptions = [];

    /// <summary>
    /// Current UICulture of the application.
    /// </summary>
    private static CultureInfo uiCulture = Thread.CurrentThread.CurrentUICulture;

    /// <summary>
    /// Should the <see cref="Thread.CurrentCulture" /> be changed when the
    /// <see cref="UiCulture" /> changes.
    /// </summary>
    private static bool synchronizeThreadCulture = true;

    /// <summary>
    /// Raised when the UI culture changes. Subscribers are stored as weak references
    /// so that long-lived static subscriptions do not root WPF controls — there is no
    /// need to call <c>-=</c> in a control's <c>Unloaded</c> handler. Lambda subscribers
    /// with closures must keep the delegate referenced themselves, since the closure
    /// is the delegate's only strong root in the standard <c>+=</c> pattern.
    /// </summary>
    public static event EventHandler<UiCultureEventArgs>? UiCultureChanged
    {
        add
        {
            if (value is null)
            {
                return;
            }

            lock (HandlerLock)
            {
                HandlerSubscriptions.RemoveAll(static s => !s.IsAlive);
                HandlerSubscriptions.Add(new WeakSubscription(value));
            }
        }

        remove
        {
            if (value is null)
            {
                return;
            }

            lock (HandlerLock)
            {
                for (var i = HandlerSubscriptions.Count - 1; i >= 0; i--)
                {
                    if (HandlerSubscriptions[i].Matches(value))
                    {
                        HandlerSubscriptions.RemoveAt(i);
                        return;
                    }
                }
            }
        }
    }

    private static void RaiseUiCultureChanged(UiCultureEventArgs args)
    {
        WeakSubscription[] snapshot;
        lock (HandlerLock)
        {
            var live = new List<WeakSubscription>(HandlerSubscriptions.Count);
            for (var i = HandlerSubscriptions.Count - 1; i >= 0; i--)
            {
                if (HandlerSubscriptions[i].IsAlive)
                {
                    live.Add(HandlerSubscriptions[i]);
                }
                else
                {
                    HandlerSubscriptions.RemoveAt(i);
                }
            }

            snapshot = [.. live];
        }

        foreach (var subscription in snapshot)
        {
            subscription.Invoke(sender: null, args);
        }
    }

    /// <summary>
    /// Gets or sets the UI culture.
    /// </summary>
    /// <value>
    /// The UI culture.
    /// </value>
    public static CultureInfo UiCulture
    {
        get => uiCulture;

        set
        {
            if (value is null)
            {
                return;
            }

            var oldUiCulture = new CultureInfo(uiCulture.LCID);
            uiCulture = value;

            CultureInfo.DefaultThreadCurrentUICulture = value;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.DefaultThreadCurrentUICulture;
            if (SynchronizeThreadCulture)
            {
                SetBackendCulture(value);
            }

            UiCultureExtension.UpdateAllTargets();
            ResxExtension.UpdateAllTargets();

            RaiseUiCultureChanged(
                new UiCultureEventArgs(
                    oldUiCulture,
                    value));
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [synchronize thread culture].
    /// </summary>
    /// <value>
    ///   <see langword="true" /> if [synchronize thread culture]; otherwise, <see langword="false" />.
    /// </value>
    public static bool SynchronizeThreadCulture
    {
        get => synchronizeThreadCulture;

        set
        {
            synchronizeThreadCulture = value;
            if (value)
            {
                SetBackendCulture(UiCulture);
            }
        }
    }

    /// <summary>
    /// Set the UI culture to the given culture and the backend culture to 'en-US'.
    /// </summary>
    /// <param name="uiCultureInfo">The UI culture to set.</param>
    /// <param name="synchronizeThreadCultures">The synchronizeThreadCultures to set.</param>
    public static void SetCultures(
        CultureInfo uiCultureInfo,
        bool synchronizeThreadCultures)
        => SetCultures(
            uiCultureInfo,
            GlobalizationConstants.EnglishCultureInfo,
            synchronizeThreadCultures);

    /// <summary>
    /// Set the cultures to the given cultures.
    /// </summary>
    /// <param name="backendCultureInfo">The backend culture to set.</param>
    /// <param name="uiCultureInfo">The UI culture to set.</param>
    /// <param name="synchronizeThreadCultures">The synchronizeThreadCultures to set.</param>
    public static void SetCultures(
        CultureInfo backendCultureInfo,
        CultureInfo uiCultureInfo,
        bool synchronizeThreadCultures = false)
    {
        ArgumentNullException.ThrowIfNull(backendCultureInfo);

        SynchronizeThreadCulture = synchronizeThreadCultures;
        SetBackendCulture(backendCultureInfo);
        UiCulture = uiCultureInfo;
    }

    /// <summary>
    /// Set the cultures to the given cultures.
    /// </summary>
    /// <param name="cultureName">The cultureName to set.</param>
    /// <param name="synchronizeThreadCultures">The synchronizeThreadCultures to set.</param>
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK")]
    public static void SetCultures(
        string cultureName,
        bool synchronizeThreadCultures = false)
    {
        ArgumentNullException.ThrowIfNull(cultureName);

        SynchronizeThreadCulture = synchronizeThreadCultures;

        try
        {
            SetCultures(
                GlobalizationConstants.EnglishCultureInfo,
                cultureName is null ? GlobalizationConstants.EnglishCultureInfo : new CultureInfo(cultureName));
        }
        catch
        {
            SetCultures(
                GlobalizationConstants.EnglishCultureInfo,
                GlobalizationConstants.EnglishCultureInfo);
        }
    }

    /// <summary>
    /// Set the UI culture to the given culture.
    /// </summary>
    /// <param name="uiCultureInfo">The UI culture to set.</param>
    public static void SetUiCulture(CultureInfo uiCultureInfo)
    {
        ArgumentNullException.ThrowIfNull(uiCultureInfo);

        UiCulture = uiCultureInfo;
    }

    /// <summary>
    /// Set the backend culture to the given culture.
    /// </summary>
    /// <param name="backendCultureInfo">The backend culture to set.</param>
    /// <remarks>If the culture is neutral then creates a specific culture.</remarks>
    public static void SetBackendCulture(CultureInfo backendCultureInfo)
    {
        ArgumentNullException.ThrowIfNull(backendCultureInfo);

        CultureInfo.DefaultThreadCurrentCulture = backendCultureInfo.IsNeutralCulture
            ? CultureInfo.CreateSpecificCulture(backendCultureInfo.Name)
            : backendCultureInfo;

        Thread.CurrentThread.CurrentCulture = CultureInfo.DefaultThreadCurrentCulture;
    }
}
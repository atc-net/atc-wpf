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
    private static readonly WeakEventList UiCultureChangedHandlers = new();

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
    /// Raised when the UI culture changes.
    /// </summary>
    /// <remarks>
    /// The event never keeps a subscriber alive, so there is no need to call <c>-=</c> in a control's
    /// <c>Unloaded</c> handler:
    /// <list type="bullet">
    /// <item><description>An instance-method handler, or a lambda that uses <c>this</c>, lives as long as that object and ends when it is collected.</description></item>
    /// <item><description>A lambda that captures only locals has no owner to follow, so it lives until it is removed with <c>-=</c>; keep the delegate in a field to remove it.</description></item>
    /// <item><description>A subscriber that belongs to another UI thread is called on that thread.</description></item>
    /// </list>
    /// </remarks>
    public static event EventHandler<UiCultureEventArgs>? UiCultureChanged
    {
        add => UiCultureChangedHandlers.Add(value);
        remove => UiCultureChangedHandlers.Remove(value);
    }

    private static void RaiseUiCultureChanged(UiCultureEventArgs args)
        => UiCultureChangedHandlers.Raise(sender: null, args);

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

            // Custom and supplemental locales share LCID 4096 and cannot be re-created from it, so copy the instance.
            var oldUiCulture = (CultureInfo)uiCulture.Clone();
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
            GlobalizationConstants.EnglishCultureInfo,
            uiCultureInfo,
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

        CultureInfo uiCultureInfo;
        try
        {
            uiCultureInfo = new CultureInfo(cultureName);
        }
        catch
        {
            TraceOnce.Warn(
                $"culture:{cultureName}",
                $"CultureManager: '{cultureName}' is not a valid culture name; using English instead.");
            uiCultureInfo = GlobalizationConstants.EnglishCultureInfo;
        }

        SetCultures(
            GlobalizationConstants.EnglishCultureInfo,
            uiCultureInfo,
            synchronizeThreadCultures);
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
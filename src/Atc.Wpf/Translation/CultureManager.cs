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
    /// <summary>
    /// Current UICulture of the application.
    /// </summary>
    private static CultureInfo uiCulture = Thread.CurrentThread.CurrentUICulture;

    /// <summary>
    /// Should the <see cref="Thread.CurrentCulture" /> be changed when the
    /// <see cref="UiCulture" /> changes.
    /// </summary>
    private static bool synchronizeThreadCulture = true;

    public static event EventHandler<UiCultureEventArgs>? UiCultureChanged;

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
            Thread.CurrentThread.CurrentUICulture = value;
            if (SynchronizeThreadCulture)
            {
                SetThreadCulture(value);
            }

            UiCultureExtension.UpdateAllTargets();
            ResxExtension.UpdateAllTargets();

            UiCultureChanged?.Invoke(
                sender: null,
                new UiCultureEventArgs(
                    oldUiCulture,
                    value));
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [synchronize thread culture].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [synchronize thread culture]; otherwise, <c>false</c>.
    /// </value>
    public static bool SynchronizeThreadCulture
    {
        get => synchronizeThreadCulture;

        set
        {
            synchronizeThreadCulture = value;
            if (value)
            {
                SetThreadCulture(UiCulture);
            }
        }
    }

    /// <summary>
    /// Set the thread culture to the given culture.
    /// </summary>
    /// <param name="value">The culture to set.</param>
    /// <remarks>If the culture is neutral then creates a specific culture.</remarks>
    private static void SetThreadCulture(
        CultureInfo value)
    {
        Thread.CurrentThread.CurrentCulture = value.IsNeutralCulture
            ? CultureInfo.CreateSpecificCulture(value.Name)
            : value;
    }
}
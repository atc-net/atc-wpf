namespace Atc.Wpf.Controls.Helpers;

/// <summary>
/// Raises a value's <c>*LostFocus</c> event once per edit: when focus leaves an editor inside the owner
/// and the value differs from when the editor got focus.
/// </summary>
/// <typeparam name="T">The value type.</typeparam>
/// <remarks>
/// Listens to the focus events that bubble up to the owner, so focus moving between two editors in the owner
/// (for example from the X box to the Y box) ends the edit of the first. A value set from code while no editor
/// has focus raises nothing.
/// </remarks>
internal sealed class LostFocusValueTracker<T>
{
    private readonly Func<T> getValue;
    private readonly Action<T, T> onEdited;
    private T valueOnFocus;

    /// <summary>
    /// Initializes a new instance of the <see cref="LostFocusValueTracker{T}"/> class.
    /// </summary>
    /// <param name="owner">The control whose editors are tracked.</param>
    /// <param name="getValue">Gets the current value.</param>
    /// <param name="onEdited">Called with the old and new value when an edit that changed the value ends.</param>
    public LostFocusValueTracker(
        UIElement owner,
        Func<T> getValue,
        Action<T, T> onEdited)
    {
        this.getValue = getValue;
        this.onEdited = onEdited;
        valueOnFocus = getValue();

        owner.AddHandler(UIElement.GotFocusEvent, new RoutedEventHandler(OnGotFocus), handledEventsToo: true);
        owner.AddHandler(UIElement.LostFocusEvent, new RoutedEventHandler(OnLostFocus), handledEventsToo: true);
    }

    private void OnGotFocus(
        object sender,
        RoutedEventArgs e)
        => valueOnFocus = getValue();

    private void OnLostFocus(
        object sender,
        RoutedEventArgs e)
    {
        var oldValue = valueOnFocus;
        var newValue = getValue();
        valueOnFocus = newValue;

        if (!EqualityComparer<T>.Default.Equals(oldValue, newValue))
        {
            onEdited(oldValue, newValue);
        }
    }
}
// ReSharper disable once CheckNamespace
namespace System.Windows;

/// <summary>
/// Extension methods for Adorner.
/// </summary>
public static class AdornerExtensions
{
    /// <summary>
    /// Tries the add adorner.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="uiElement">The UI element.</param>
    /// <param name="adorner">The adorner.</param>
    public static void TryAddAdorner<T>(
        this UIElement uiElement,
        Adorner adorner)
        where T : Adorner
    {
        ArgumentNullException.ThrowIfNull(uiElement);
        ArgumentNullException.ThrowIfNull(adorner);

        var adornerLayer = AdornerLayer.GetAdornerLayer(uiElement);
        if (adornerLayer is not null && !adornerLayer.ContainsAdorner<T>(uiElement))
        {
            adornerLayer.Add(adorner);
        }
    }

    /// <summary>
    /// Tries the remove adorners.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="uiElement">The UI element.</param>
    public static void TryRemoveAdorners<T>(
        this UIElement uiElement)
        where T : Adorner
    {
        ArgumentNullException.ThrowIfNull(uiElement);

        var adornerLayer = AdornerLayer.GetAdornerLayer(uiElement);
        adornerLayer?.RemoveAdorners<T>(uiElement);
    }

    /// <summary>
    /// Determines whether the specified adorner layer contains adorner.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="adornerLayer">The adorner layer.</param>
    /// <param name="uiElement">The UI element.</param>
    /// <returns>
    ///   <see langword="true" /> if the specified adorner layer contains adorner; otherwise, <see langword="false" />.
    /// </returns>
    public static bool ContainsAdorner<T>(
        this AdornerLayer adornerLayer,
        UIElement uiElement)
        where T : Adorner
    {
        ArgumentNullException.ThrowIfNull(adornerLayer);
        ArgumentNullException.ThrowIfNull(uiElement);

        var adorners = adornerLayer.GetAdorners(uiElement);
        if (adorners is null)
        {
            return false;
        }

        for (var i = adorners.Length - 1; i >= 0; i--)
        {
            if (adorners[i] is T)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Removes the adorners.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="adornerLayer">The adorner layer.</param>
    /// <param name="uiElement">The UI element.</param>
    public static void RemoveAdorners<T>(
        this AdornerLayer adornerLayer,
        UIElement uiElement)
        where T : Adorner
    {
        ArgumentNullException.ThrowIfNull(adornerLayer);
        ArgumentNullException.ThrowIfNull(uiElement);

        var adorners = adornerLayer.GetAdorners(uiElement);
        if (adorners is null)
        {
            return;
        }

        for (var i = adorners.Length - 1; i >= 0; i--)
        {
            if (adorners[i] is T)
            {
                adornerLayer.Remove(adorners[i]);
            }
        }
    }

    /// <summary>
    /// Removes all adorners.
    /// </summary>
    /// <param name="adornerLayer">The adorner layer.</param>
    /// <param name="uiElement">The UI element.</param>
    public static void RemoveAllAdorners(
        this AdornerLayer adornerLayer,
        UIElement uiElement)
    {
        ArgumentNullException.ThrowIfNull(adornerLayer);
        ArgumentNullException.ThrowIfNull(uiElement);

        var adorners = adornerLayer.GetAdorners(uiElement);
        if (adorners is null)
        {
            return;
        }

        foreach (var toRemove in adorners)
        {
            adornerLayer.Remove(toRemove);
        }
    }
}
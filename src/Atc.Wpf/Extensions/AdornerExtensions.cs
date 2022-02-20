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
    public static void TryAddAdorner<T>(this UIElement uiElement, Adorner adorner)
        where T : Adorner
    {
        if (uiElement is null)
        {
            throw new ArgumentNullException(nameof(uiElement));
        }

        if (adorner is null)
        {
            throw new ArgumentNullException(nameof(adorner));
        }

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
    public static void TryRemoveAdorners<T>(this UIElement uiElement)
        where T : Adorner
    {
        if (uiElement is null)
        {
            throw new ArgumentNullException(nameof(uiElement));
        }

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
    ///   <c>true</c> if the specified adorner layer contains adorner; otherwise, <c>false</c>.
    /// </returns>
    public static bool ContainsAdorner<T>(this AdornerLayer adornerLayer, UIElement uiElement)
        where T : Adorner
    {
        if (adornerLayer is null)
        {
            throw new ArgumentNullException(nameof(adornerLayer));
        }

        if (uiElement is null)
        {
            throw new ArgumentNullException(nameof(uiElement));
        }

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
    public static void RemoveAdorners<T>(this AdornerLayer adornerLayer, UIElement uiElement)
        where T : Adorner
    {
        if (adornerLayer is null)
        {
            throw new ArgumentNullException(nameof(adornerLayer));
        }

        if (uiElement is null)
        {
            throw new ArgumentNullException(nameof(uiElement));
        }

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
    public static void RemoveAllAdorners(this AdornerLayer adornerLayer, UIElement uiElement)
    {
        if (adornerLayer is null)
        {
            throw new ArgumentNullException(nameof(adornerLayer));
        }

        if (uiElement is null)
        {
            throw new ArgumentNullException(nameof(uiElement));
        }

        var adorners = adornerLayer.GetAdorners(uiElement);
        if (adorners is null)
        {
            return;
        }

        foreach (Adorner toRemove in adorners)
        {
            adornerLayer.Remove(toRemove);
        }
    }
}
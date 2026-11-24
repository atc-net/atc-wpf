namespace Atc.Wpf.Components.Tests.TestSupport;

/// <summary>
/// Keeps the flyouts under an element without a template, so they open and close at once without animations.
/// </summary>
/// <remarks>
/// Flyouts get a default template from the assembly's theme dictionary. Tests of ordering, tracking and
/// cancellation use this to stay independent of animation timing; the templated, animated path is covered by
/// the tests that use <see cref="FlyoutWindow"/>.
/// </remarks>
internal static class UntemplatedFlyouts
{
    public static T Apply<T>(T element)
        where T : FrameworkElement
    {
        var style = new Style(typeof(Flyout));
        style.Setters.Add(new Setter(Control.TemplateProperty, value: null));
        element.Resources[typeof(Flyout)] = style;
        return element;
    }
}
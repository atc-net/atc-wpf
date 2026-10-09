// ReSharper disable CheckNamespace
namespace Atc.Wpf.Forms;

/// <summary>
/// Extension methods for <see cref="LabelContent"/>.
/// </summary>
public static class LabelContentExtensions
{
    /// <summary>
    /// Gets the view model used as the data context of the hosted content.
    /// </summary>
    /// <typeparam name="T">The view model type.</typeparam>
    /// <param name="labelContent">The label content control.</param>
    /// <returns>The view model, or <see langword="null"/> if the content is not a framework element or its data context is not of type <typeparamref name="T"/>.</returns>
    public static T? GetViewModel<T>(this LabelContent labelContent)
        where T : ViewModelBase
    {
        ArgumentNullException.ThrowIfNull(labelContent);

        return labelContent.Content is FrameworkElement frameworkElement
            ? frameworkElement.DataContext as T
            : null;
    }
}
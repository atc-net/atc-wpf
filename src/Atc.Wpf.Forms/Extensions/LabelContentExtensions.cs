// ReSharper disable CheckNamespace
namespace Atc.Wpf.Forms;

public static class LabelContentExtensions
{
    public static T? GetViewModel<T>(this LabelContent labelContent)
        where T : ViewModelBase
    {
        ArgumentNullException.ThrowIfNull(labelContent);

        return labelContent.Content is FrameworkElement frameworkElement
            ? frameworkElement.DataContext as T
            : null;
    }
}
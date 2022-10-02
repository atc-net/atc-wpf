// ReSharper disable SuggestBaseTypeForParameter
namespace Atc.Wpf.Theming.Controls;

[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "OK.")]
[SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "OK.")]
public class GridViewHeaderRowPresenterEx : GridViewHeaderRowPresenter
{
    protected override void OnVisualChildrenChanged(
        DependencyObject visualAdded,
        DependencyObject visualRemoved)
    {
        base.OnVisualChildrenChanged(visualAdded, visualRemoved);

        if (visualAdded is not Separator addedIndicator)
        {
            return;
        }

        var visibilityPropertyChangeNotifier = new PropertyChangeNotifier(addedIndicator, IsVisibleProperty);
        visibilityPropertyChangeNotifier.ValueChanged += OnValueChanged;
    }

    private void OnValueChanged(
        object? sender,
        EventArgs e)
    {
        if (sender is not Separator { IsVisible: true } indicator)
        {
            return;
        }

        var border = indicator.FindChild<Border>();
        if (border is null)
        {
            return;
        }

        var itemsControl = FindItemsControlThroughTemplatedParent(this);
        if (itemsControl is null)
        {
            return;
        }

        var brush = ItemHelper.GetGridViewHeaderIndicatorBrush(itemsControl) ?? Brushes.Navy;
        border.SetValue(Border.BackgroundProperty, brush);
    }

    private static ItemsControl? FindItemsControlThroughTemplatedParent(
        GridViewHeaderRowPresenter presenter)
    {
        var frameworkElement = presenter.TemplatedParent as FrameworkElement;
        ItemsControl? itemsControl = null;

        while (frameworkElement is not null)
        {
            itemsControl = frameworkElement as ItemsControl;
            if (itemsControl is not null)
            {
                break;
            }

            frameworkElement = frameworkElement.TemplatedParent as FrameworkElement;
        }

        return itemsControl;
    }
}
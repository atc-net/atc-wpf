namespace Atc.Wpf.Controls.Behaviors;

public class AutoScrollListViewBehavior : Behavior<ListView>
{
    public bool ScrollToBottom { get; set; } = true;

    protected override void OnAttached()
    {
        base.OnAttached();
        var items = AssociatedObject.Items;
        ((INotifyCollectionChanged)items).CollectionChanged += OnCollectionChanged;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        var items = AssociatedObject.Items;
        ((INotifyCollectionChanged)items).CollectionChanged -= OnCollectionChanged;
    }

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        var listView = AssociatedObject;
        if (listView.Items.Count <= 1)
        {
            return;
        }

        var targetItem = ScrollToBottom
            ? listView.Items[^1]!
            : listView.Items[0]!;

        listView.ScrollIntoView(targetItem);
    }
}
namespace Atc.Wpf.Controls.Behaviors;

public sealed class AutoScrollListViewBehavior : Behavior<ListView>
{
    public static readonly DependencyProperty ScrollDirectionProperty =
        DependencyProperty.Register(
            nameof(ScrollDirection),
            typeof(ScrollDirectionType),
            typeof(AutoScrollListViewBehavior),
            new PropertyMetadata(ScrollDirectionType.Bottom));

    public ScrollDirectionType ScrollDirection
    {
        get => (ScrollDirectionType)GetValue(ScrollDirectionProperty);
        set => SetValue(ScrollDirectionProperty, value);
    }

    public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.Register(
            nameof(IsEnabled),
            typeof(bool),
            typeof(AutoScrollListViewBehavior),
            new PropertyMetadata(BooleanBoxes.FalseBox));

    public bool IsEnabled
    {
        get => (bool)GetValue(IsEnabledProperty);
        set => SetValue(IsEnabledProperty, value);
    }

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

    private void OnCollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e)
    {
        if (!IsEnabled)
        {
            return;
        }

        var listView = AssociatedObject;
        if (listView.Items.Count <= 1)
        {
            return;
        }

        var targetItem = ScrollDirection == ScrollDirectionType.Bottom
            ? listView.Items[^1]!
            : listView.Items[0]!;

        listView.ScrollIntoView(targetItem);
    }
}
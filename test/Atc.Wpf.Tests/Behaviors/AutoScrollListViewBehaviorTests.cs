namespace Atc.Wpf.Tests.Behaviors;

public sealed class AutoScrollListViewBehaviorTests
{
    private const int ItemCount = 1_000;

    [StaFact]
    public void ManyAdds_ScrollOnceAfterTheBurst_ToTheLastItem()
    {
        // Arrange
        var items = new ObservableCollection<string>();
        var listView = new ListView { ItemsSource = items };
        var behavior = new AutoScrollListViewBehavior { IsEnabled = true };
        var window = ShowInWindow(listView);
        try
        {
            Interaction.GetBehaviors(listView).Add(behavior);
            var scrollViewer = FindScrollViewer(listView);
            var scrollChangedCount = 0;
            scrollViewer.ScrollChanged += (_, e) =>
            {
                if (e.VerticalChange != 0)
                {
                    scrollChangedCount++;
                }
            };

            // Act
            for (var i = 0; i < ItemCount; i++)
            {
                items.Add($"Item {i}");
            }

            WaitForIdle();

            // Assert
            Assert.Equal(1, scrollChangedCount);
            Assert.True(IsInView(listView, items[^1]));
            Assert.False(IsInView(listView, items[0]));
        }
        finally
        {
            window.Close();
        }
    }

    [StaFact]
    public void ScrollDirectionTop_ScrollsToTheFirstItem()
    {
        // Arrange
        var items = new ObservableCollection<string>();
        for (var i = 0; i < ItemCount; i++)
        {
            items.Add($"Item {i}");
        }

        var listView = new ListView { ItemsSource = items };
        var window = ShowInWindow(listView);
        try
        {
            listView.ScrollIntoView(items[^1]);
            WaitForIdle();
            Assert.False(IsInView(listView, items[0]));

            var behavior = new AutoScrollListViewBehavior
            {
                IsEnabled = true,
                ScrollDirection = ScrollDirectionType.Top,
            };
            Interaction.GetBehaviors(listView).Add(behavior);

            // Act
            items.Add("Item new");
            WaitForIdle();

            // Assert
            Assert.True(IsInView(listView, items[0]));
        }
        finally
        {
            window.Close();
        }
    }

    [StaFact]
    public void IsEnabledFalse_DoesNotScroll()
    {
        // Arrange
        var items = new ObservableCollection<string>();
        var listView = new ListView { ItemsSource = items };
        var behavior = new AutoScrollListViewBehavior { IsEnabled = false };
        var window = ShowInWindow(listView);
        try
        {
            Interaction.GetBehaviors(listView).Add(behavior);

            // Act
            for (var i = 0; i < ItemCount; i++)
            {
                items.Add($"Item {i}");
            }

            WaitForIdle();

            // Assert
            Assert.Equal(0, FindScrollViewer(listView).VerticalOffset);
            Assert.True(IsInView(listView, items[0]));
        }
        finally
        {
            window.Close();
        }
    }

    [StaFact]
    public void Detach_BeforeThePostedScrollRuns_DoesNotThrow()
    {
        // Arrange
        var items = new ObservableCollection<string>();
        var listView = new ListView { ItemsSource = items };
        var behavior = new AutoScrollListViewBehavior { IsEnabled = true };
        var window = ShowInWindow(listView);
        try
        {
            Interaction.GetBehaviors(listView).Add(behavior);
            for (var i = 0; i < ItemCount; i++)
            {
                items.Add($"Item {i}");
            }

            // Act
            var exception = Record.Exception(() =>
            {
                Interaction.GetBehaviors(listView).Remove(behavior);
                items.Add("Item after detach");
                WaitForIdle();
            });

            // Assert
            Assert.Null(exception);
            Assert.Null(((IAttachedObject)behavior).AssociatedObject);
            Assert.Equal(0, FindScrollViewer(listView).VerticalOffset);
        }
        finally
        {
            window.Close();
        }
    }

    private static Window ShowInWindow(ListView listView)
    {
        var window = new Window
        {
            Content = listView,
            Width = 200,
            Height = 200,
            Left = -10000,
            Top = -10000,
            ShowInTaskbar = false,
            ShowActivated = false,
            WindowStyle = WindowStyle.None,
        };
        window.Show();
        WaitForIdle();
        return window;
    }

    private static void WaitForIdle()
        => Dispatcher.CurrentDispatcher.Invoke(() => { }, DispatcherPriority.ApplicationIdle);

    private static ScrollViewer FindScrollViewer(DependencyObject element)
        => TryFindScrollViewer(element)
           ?? throw new InvalidOperationException("No ScrollViewer found.");

    private static ScrollViewer? TryFindScrollViewer(DependencyObject element)
    {
        if (element is ScrollViewer scrollViewer)
        {
            return scrollViewer;
        }

        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
        {
            var found = TryFindScrollViewer(VisualTreeHelper.GetChild(element, i));
            if (found is not null)
            {
                return found;
            }
        }

        return null;
    }

    private static bool IsInView(
        ListView listView,
        object item)
    {
        if (listView.ItemContainerGenerator.ContainerFromItem(item) is not FrameworkElement container ||
            !container.IsVisible)
        {
            return false;
        }

        var bounds = container
            .TransformToAncestor(listView)
            .TransformBounds(new Rect(0, 0, container.ActualWidth, container.ActualHeight));

        return bounds.Bottom > 0 && bounds.Top < listView.ActualHeight;
    }
}
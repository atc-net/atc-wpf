// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
namespace Atc.Wpf.Theming.Behaviors;

[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static class StylizedBehaviors
{
    public static readonly DependencyProperty BehaviorsProperty
        = DependencyProperty.RegisterAttached(
            "Behaviors",
            typeof(StylizedBehaviorCollection),
            typeof(StylizedBehaviors),
            new FrameworkPropertyMetadata(
                defaultValue: null,
                OnPropertyChanged));

    public static StylizedBehaviorCollection? GetBehaviors(DependencyObject d)
        => (StylizedBehaviorCollection?)d.GetValue(BehaviorsProperty);

    public static void SetBehaviors(
        DependencyObject d,
        StylizedBehaviorCollection? value)
        => d.SetValue(BehaviorsProperty, value);

    private static void OnPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not FrameworkElement frameworkElement)
        {
            return;
        }

        var newBehaviors = e.NewValue as StylizedBehaviorCollection;
        var oldBehaviors = e.OldValue as StylizedBehaviorCollection;
        if (newBehaviors == oldBehaviors)
        {
            return;
        }

        var itemBehaviors = Interaction.GetBehaviors(frameworkElement);

        frameworkElement.Unloaded -= FrameworkElementUnloaded;

        if (oldBehaviors != null)
        {
            foreach (var behavior in oldBehaviors)
            {
                var index = GetIndexOf(itemBehaviors, behavior);
                if (index >= 0)
                {
                    itemBehaviors.RemoveAt(index);
                }
            }
        }

        if (newBehaviors is not null)
        {
            foreach (var behavior in newBehaviors)
            {
                var index = GetIndexOf(itemBehaviors, behavior);
                if (index >= 0)
                {
                    continue;
                }

                var clone = (Behavior)behavior.Clone();
                SetOriginalBehavior(clone, behavior);
                itemBehaviors.Add(clone);
            }
        }

        if (itemBehaviors.Count > 0)
        {
            frameworkElement.Unloaded += FrameworkElementUnloaded;
        }
    }

    private static void FrameworkElementUnloaded(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is not FrameworkElement frameworkElement)
        {
            return;
        }

        var itemBehaviors = Interaction.GetBehaviors(frameworkElement);
        foreach (var behavior in itemBehaviors)
        {
            behavior.Detach();
        }

        frameworkElement.Loaded += FrameworkElementLoaded;
    }

    private static void FrameworkElementLoaded(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is not FrameworkElement frameworkElement)
        {
            return;
        }

        frameworkElement.Loaded -= FrameworkElementLoaded;
        var itemBehaviors = Interaction.GetBehaviors(frameworkElement);
        foreach (var behavior in itemBehaviors)
        {
            behavior.Attach(frameworkElement);
        }
    }

    private static int GetIndexOf(
        BehaviorCollection itemBehaviors,
        Behavior behavior)
    {
        var index = -1;

        var originalBehavior = GetOriginalBehavior(behavior);

        for (var i = 0; i < itemBehaviors.Count; i++)
        {
            var currentBehavior = itemBehaviors[i];
            if (currentBehavior == behavior || currentBehavior == originalBehavior)
            {
                index = i;
                break;
            }

            var currentOriginalBehavior = GetOriginalBehavior(currentBehavior);
            if (currentOriginalBehavior == behavior || currentOriginalBehavior == originalBehavior)
            {
                index = i;
                break;
            }
        }

        return index;
    }

    private static readonly DependencyProperty OriginalBehaviorProperty
        = DependencyProperty.RegisterAttached(
            "OriginalBehavior",
            typeof(Behavior),
            typeof(StylizedBehaviors),
            new UIPropertyMetadata(propertyChangedCallback: null));

    private static Behavior? GetOriginalBehavior(DependencyObject d)
        => (Behavior?)d.GetValue(OriginalBehaviorProperty);

    private static void SetOriginalBehavior(
        DependencyObject d,
        Behavior? value)
        => d.SetValue(OriginalBehaviorProperty, value);
}
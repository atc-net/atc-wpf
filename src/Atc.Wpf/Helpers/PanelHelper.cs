namespace Atc.Wpf.Helpers;

/// <summary>
/// PanelHelper is a helper/extension to all types of <see cref="Panel" /> controls like
/// <see cref="DockPanel" />, <see cref="StackPanel" />, <see cref="WrapPanel" /> and of cause <see cref="Panel" />.
/// Panels overview: https://learn.microsoft.com/en-us/dotnet/desktop/wpf/controls/panels-overview?view=netframeworkdesktop-4.8
/// Set horizontal or vertical spacing between items (framework-elements).
/// Set margin on the panel.
/// </summary>
/// <example>
/// <![CDATA[
/// <!-- Example 1
/// Set the 'ItemMargin' to 10 and the 'LastItemMargin' to 5.
/// -->
/// <WrapPanel
///    atc:PanelHelper.ItemMargin="10"
///    atc:PanelHelper.LastItemMargin="5">
/// <!-- panel content -->
/// </WrapPanel>
///
/// <!-- Example 2
/// Set the horizontal spacing to 15 and vertical spacing to 20.
/// -->
/// <WrapPanel
///    atc:PanelHelper.HorizontalSpacing="15"
///    atc:PanelHelper.VerticalSpacing="20">
/// <!-- panel content -->
/// </WrapPanel>/// ]]>
/// </example>
[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static class PanelHelper
{
    private static readonly Thickness ZeroThickness = new(0);

    /// <summary>Identifies the Spacing attached property.</summary>
    public static readonly DependencyProperty SpacingProperty = DependencyProperty.RegisterAttached(
        "Spacing",
        typeof(double),
        typeof(PanelHelper),
        new UIPropertyMetadata(
            defaultValue: 0d,
            OnSpacingChanged));

    /// <summary>Gets the value of the Spacing attached property.</summary>
    public static double GetSpacing(DependencyObject obj)
        => (double)obj.GetValue(SpacingProperty);

    /// <summary>Sets the value of the Spacing attached property.</summary>
    public static void SetSpacing(
        DependencyObject obj,
        double space)
        => obj.SetValue(SpacingProperty, space);

    /// <summary>Identifies the HorizontalSpacing attached property.</summary>
    public static readonly DependencyProperty HorizontalSpacingProperty = DependencyProperty.RegisterAttached(
        "HorizontalSpacing",
        typeof(double),
        typeof(PanelHelper),
        new UIPropertyMetadata(
            defaultValue: 0d,
            OnHorizontalSpacingChanged));

    /// <summary>Gets the value of the HorizontalSpacing attached property.</summary>
    public static double GetHorizontalSpacing(DependencyObject obj)
        => (double)obj.GetValue(HorizontalSpacingProperty);

    /// <summary>Sets the value of the HorizontalSpacing attached property.</summary>
    public static void SetHorizontalSpacing(
        DependencyObject obj,
        double space)
        => obj.SetValue(HorizontalSpacingProperty, space);

    /// <summary>Identifies the VerticalSpacing attached property.</summary>
    public static readonly DependencyProperty VerticalSpacingProperty = DependencyProperty.RegisterAttached(
        "VerticalSpacing",
        typeof(double),
        typeof(PanelHelper),
        new UIPropertyMetadata(
            defaultValue: 0d,
            OnVerticalSpacingChanged));

    /// <summary>Gets the value of the VerticalSpacing attached property.</summary>
    public static double GetVerticalSpacing(DependencyObject obj)
        => (double)obj.GetValue(VerticalSpacingProperty);

    /// <summary>Sets the value of the VerticalSpacing attached property.</summary>
    public static void SetVerticalSpacing(
        DependencyObject obj,
        double value)
        => obj.SetValue(VerticalSpacingProperty, value);

    /// <summary>Identifies the ItemMargin attached property.</summary>
    public static readonly DependencyProperty ItemMarginProperty = DependencyProperty.RegisterAttached(
        "ItemMargin",
        typeof(Thickness),
        typeof(PanelHelper),
        new UIPropertyMetadata(
            new Thickness(0),
            OnItemMarginChanged));

    /// <summary>Gets the value of the ItemMargin attached property.</summary>
    public static Thickness GetItemMargin(DependencyObject obj)
        => (Thickness)obj.GetValue(ItemMarginProperty);

    private static void SetItemMargin(
        DependencyObject obj,
        Thickness value)
        => obj.SetValue(ItemMarginProperty, value);

    /// <summary>Identifies the LastItemMargin attached property.</summary>
    public static readonly DependencyProperty LastItemMarginProperty = DependencyProperty.RegisterAttached(
        "LastItemMargin",
        typeof(Thickness),
        typeof(PanelHelper),
        new UIPropertyMetadata(
            new Thickness(0),
            OnItemMarginChanged));

    /// <summary>Gets the value of the LastItemMargin attached property.</summary>
    public static Thickness GetLastItemMargin(DependencyObject obj)
        => (Thickness)obj.GetValue(LastItemMarginProperty);

    private static void SetLastItemMargin(
        DependencyObject obj,
        Thickness value)
        => obj.SetValue(LastItemMarginProperty, value);

    private static void OnSpacingChanged(
        object sender,
        DependencyPropertyChangedEventArgs e)
    {
        if (sender is not Panel panel)
        {
            return;
        }

        var space = (double)e.NewValue;
        var obj = (DependencyObject)sender;

        var itemThickness = GetItemMargin(panel);
        itemThickness.Right = space;
        itemThickness.Bottom = space;

        SetItemMargin(obj, itemThickness);
        SetLastItemMargin(obj, ZeroThickness);
    }

    private static void OnHorizontalSpacingChanged(
        object sender,
        DependencyPropertyChangedEventArgs e)
    {
        if (sender is not Panel panel)
        {
            return;
        }

        var space = (double)e.NewValue;
        var obj = (DependencyObject)sender;

        var itemThickness = GetItemMargin(panel);
        itemThickness.Right = space;

        SetItemMargin(obj, itemThickness);
        SetLastItemMargin(obj, ZeroThickness);
    }

    private static void OnVerticalSpacingChanged(
        object sender,
        DependencyPropertyChangedEventArgs e)
    {
        if (sender is not Panel panel)
        {
            return;
        }

        var space = (double)e.NewValue;
        var obj = (DependencyObject)sender;

        var itemThickness = GetItemMargin(panel);
        itemThickness.Bottom = space;

        SetItemMargin(obj, itemThickness);
        SetLastItemMargin(obj, ZeroThickness);
    }

    private static void OnItemMarginChanged(
        object sender,
        DependencyPropertyChangedEventArgs e)
    {
        if (sender is not Panel panel)
        {
            return;
        }

        panel.Loaded -= OnPanelLoaded;
        panel.Loaded += OnPanelLoaded;
        panel.Unloaded -= OnPanelUnloaded;
        panel.Unloaded += OnPanelUnloaded;

        if (panel.IsLoaded)
        {
            OnPanelLoaded(panel, e: null);
        }
    }

    private static void OnPanelLoaded(
        object sender,
        RoutedEventArgs? e)
    {
        if (sender is not Panel panel)
        {
            return;
        }

        SubscribeLayoutUpdated(panel);
        ApplyItemMargins(panel);
    }

    private static void OnPanelUnloaded(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is not Panel panel)
        {
            return;
        }

        UnsubscribeLayoutUpdated(panel);
    }

    /// <summary>
    /// Panel exposes no children-changed event, so the panel listens to
    /// <see cref="UIElement.LayoutUpdated"/> while it is loaded (adding or removing a child
    /// always triggers a layout pass) and re-applies the margins only when the children changed.
    /// The handler is removed again on Unloaded, because the layout manager holds it strongly.
    /// </summary>
    private static void SubscribeLayoutUpdated(Panel panel)
    {
        if (panel.GetValue(LayoutUpdatedHandlerProperty) is EventHandler)
        {
            return;
        }

        EventHandler handler = (_, _) => OnPanelLayoutUpdated(panel);
        panel.SetValue(LayoutUpdatedHandlerProperty, handler);
        panel.LayoutUpdated += handler;
    }

    private static void UnsubscribeLayoutUpdated(Panel panel)
    {
        if (panel.GetValue(LayoutUpdatedHandlerProperty) is not EventHandler handler)
        {
            return;
        }

        panel.LayoutUpdated -= handler;
        panel.ClearValue(LayoutUpdatedHandlerProperty);
    }

    private static void OnPanelLayoutUpdated(Panel panel)
    {
        var lastApplied = (ChildrenSnapshot?)panel.GetValue(ChildrenSnapshotProperty);
        if (lastApplied is not null && lastApplied.Matches(panel))
        {
            return;
        }

        ApplyItemMargins(panel);
    }

    /// <summary>
    /// Applies <see cref="ItemMarginProperty"/> to every child element except the last one,
    /// which receives <see cref="LastItemMarginProperty"/> so the panel gets no trailing spacing.
    /// A child whose margin was set locally by someone other than <see cref="PanelHelper"/> keeps it.
    /// </summary>
    internal static void ApplyItemMargins(Panel panel)
    {
        panel.SetValue(ChildrenSnapshotProperty, ChildrenSnapshot.Create(panel));

        var children = panel.Children.OfType<FrameworkElement>().ToList();
        if (children.Count == 0)
        {
            return;
        }

        var itemMargin = GetItemMargin(panel);
        for (var i = 0; i < children.Count - 1; i++)
        {
            ApplyMargin(children[i], itemMargin);
        }

        ApplyMargin(children[^1], GetLastItemMargin(panel));
    }

    private static void ApplyMargin(
        FrameworkElement element,
        Thickness margin)
    {
        var appliedMargin = (Thickness?)element.GetValue(AppliedMarginProperty);
        var ownsMargin = appliedMargin.HasValue && element.Margin == appliedMargin.Value;

        if (!ownsMargin &&
            DependencyPropertyHelper.GetValueSource(element, FrameworkElement.MarginProperty).BaseValueSource is BaseValueSource.Local)
        {
            // The margin was set locally by the user (value or binding) - leave it alone.
            // Style/theme margins are still replaced, as before.
            if (appliedMargin.HasValue)
            {
                element.ClearValue(AppliedMarginProperty);
            }

            return;
        }

        if (element.Margin != margin)
        {
            element.Margin = margin;
        }

        element.SetValue(AppliedMarginProperty, margin);
    }

    /// <summary>
    /// Remembers the margin <see cref="PanelHelper"/> last applied to a child, so later
    /// spacing changes can be re-applied without overwriting a margin the user set.
    /// </summary>
    private static readonly DependencyProperty AppliedMarginProperty = DependencyProperty.RegisterAttached(
        "AppliedMargin",
        typeof(Thickness?),
        typeof(PanelHelper),
        new PropertyMetadata(defaultValue: null));

    private static readonly DependencyProperty LayoutUpdatedHandlerProperty = DependencyProperty.RegisterAttached(
        "LayoutUpdatedHandler",
        typeof(EventHandler),
        typeof(PanelHelper),
        new PropertyMetadata(defaultValue: null));

    private static readonly DependencyProperty ChildrenSnapshotProperty = DependencyProperty.RegisterAttached(
        "ChildrenSnapshot",
        typeof(ChildrenSnapshot),
        typeof(PanelHelper),
        new PropertyMetadata(defaultValue: null));

    /// <summary>
    /// A cheap fingerprint of a panel's children (count plus the last child) used to skip
    /// re-applying margins on layout passes where the children did not change.
    /// </summary>
    private sealed class ChildrenSnapshot
    {
        private readonly int count;
        private readonly WeakReference<UIElement>? lastChild;

        private ChildrenSnapshot(
            int count,
            UIElement? lastChild)
        {
            this.count = count;
            this.lastChild = lastChild is null ? null : new WeakReference<UIElement>(lastChild);
        }

        public static ChildrenSnapshot Create(Panel panel)
        {
            var children = panel.Children;
            return new ChildrenSnapshot(
                children.Count,
                children.Count > 0 ? children[^1] : null);
        }

        public bool Matches(Panel panel)
        {
            var children = panel.Children;
            if (children.Count != count)
            {
                return false;
            }

            if (count == 0)
            {
                return true;
            }

            return lastChild is not null &&
                   lastChild.TryGetTarget(out var target) &&
                   ReferenceEquals(target, children[^1]);
        }
    }
}
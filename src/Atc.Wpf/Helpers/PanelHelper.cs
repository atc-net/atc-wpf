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
    public static readonly DependencyProperty HorizontalSpacingProperty = DependencyProperty.RegisterAttached(
        "HorizontalSpacing",
        typeof(double),
        typeof(PanelHelper),
        new UIPropertyMetadata(
            defaultValue: 0d,
            OnHorizontalSpacingChanged));

    public static double GetHorizontalSpacing(
        DependencyObject obj)
        => (double)obj.GetValue(HorizontalSpacingProperty);

    public static void SetHorizontalSpacing(
        DependencyObject obj,
        double space)
        => obj.SetValue(HorizontalSpacingProperty, space);

    public static readonly DependencyProperty VerticalSpacingProperty = DependencyProperty.RegisterAttached(
        "VerticalSpacing",
        typeof(double),
        typeof(PanelHelper),
        new UIPropertyMetadata(
            defaultValue: 0d,
            OnVerticalSpacingChanged));

    public static double GetVerticalSpacing(
        DependencyObject obj)
        => (double)obj.GetValue(VerticalSpacingProperty);

    public static void SetVerticalSpacing(
        DependencyObject obj,
        double value)
        => obj.SetValue(VerticalSpacingProperty, value);

    public static readonly DependencyProperty ItemMarginProperty = DependencyProperty.RegisterAttached(
        "ItemMargin",
        typeof(Thickness),
        typeof(PanelHelper),
        new UIPropertyMetadata(
            new Thickness(0),
            OnItemMarginChanged));

    public static Thickness GetItemMargin(
        DependencyObject obj)
        => (Thickness)obj.GetValue(ItemMarginProperty);

    private static void SetItemMargin(
        DependencyObject obj,
        Thickness value)
        => obj.SetValue(ItemMarginProperty, value);

    public static readonly DependencyProperty LastItemMarginProperty = DependencyProperty.RegisterAttached(
        "LastItemMargin",
        typeof(Thickness),
        typeof(PanelHelper),
        new UIPropertyMetadata(
            new Thickness(0),
            OnItemMarginChanged));

    public static Thickness GetLastItemMargin(
        DependencyObject obj)
        => (Thickness)obj.GetValue(LastItemMarginProperty);

    private static void SetLastItemMargin(
        DependencyObject obj,
        Thickness value)
        => obj.SetValue(LastItemMarginProperty, value);

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
        SetLastItemMargin(obj, new Thickness(0));
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
        SetLastItemMargin(obj, new Thickness(0));
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

        for (var i = 0; i < panel.Children.Count; i++)
        {
            var child = panel.Children[i];
            if (child is not FrameworkElement fe)
            {
                continue;
            }

            fe.Margin = GetItemMargin(panel);
        }
    }
}
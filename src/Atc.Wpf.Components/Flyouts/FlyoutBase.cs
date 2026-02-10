namespace Atc.Wpf.Components.Flyouts;

/// <summary>
/// Provides attached properties for associating flyouts with UI elements.
/// </summary>
public static class FlyoutBase
{
    /// <summary>
    /// Identifies the AttachedFlyout attached property.
    /// </summary>
    public static readonly DependencyProperty AttachedFlyoutProperty =
        DependencyProperty.RegisterAttached(
            "AttachedFlyout",
            typeof(Flyout),
            typeof(FlyoutBase),
            new PropertyMetadata(null));

    /// <summary>
    /// Identifies the IsOpen attached property for triggering flyout open/close.
    /// </summary>
    public static readonly DependencyProperty IsOpenProperty =
        DependencyProperty.RegisterAttached(
            "IsOpen",
            typeof(bool),
            typeof(FlyoutBase),
            new PropertyMetadata(false, OnIsOpenChanged));

    /// <summary>
    /// Identifies the OpenOnClick attached property.
    /// When true, clicking the element opens the attached flyout.
    /// </summary>
    public static readonly DependencyProperty OpenOnClickProperty =
        DependencyProperty.RegisterAttached(
            "OpenOnClick",
            typeof(bool),
            typeof(FlyoutBase),
            new PropertyMetadata(false, OnOpenOnClickChanged));

    /// <summary>
    /// Gets the attached flyout for the specified element.
    /// </summary>
    /// <param name="element">The element to get the flyout from.</param>
    /// <returns>The attached flyout, or null.</returns>
    public static Flyout? GetAttachedFlyout(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (Flyout?)element.GetValue(AttachedFlyoutProperty);
    }

    /// <summary>
    /// Sets the attached flyout for the specified element.
    /// </summary>
    /// <param name="element">The element to set the flyout on.</param>
    /// <param name="value">The flyout to attach.</param>
    public static void SetAttachedFlyout(
        DependencyObject element,
        Flyout? value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValue(AttachedFlyoutProperty, value);
    }

    /// <summary>
    /// Gets whether the attached flyout is open.
    /// </summary>
    /// <param name="element">The element to check.</param>
    /// <returns>True if the attached flyout is open.</returns>
    public static bool GetIsOpen(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (bool)element.GetValue(IsOpenProperty);
    }

    /// <summary>
    /// Sets whether the attached flyout is open.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="value">True to open the flyout, false to close it.</param>
    public static void SetIsOpen(
        DependencyObject element,
        bool value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValue(IsOpenProperty, value);
    }

    /// <summary>
    /// Gets whether clicking the element opens the attached flyout.
    /// </summary>
    /// <param name="element">The element to check.</param>
    /// <returns>True if clicking opens the flyout.</returns>
    public static bool GetOpenOnClick(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (bool)element.GetValue(OpenOnClickProperty);
    }

    /// <summary>
    /// Sets whether clicking the element opens the attached flyout.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="value">True to open flyout on click.</param>
    public static void SetOpenOnClick(
        DependencyObject element,
        bool value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValue(OpenOnClickProperty, value);
    }

    /// <summary>
    /// Shows the attached flyout for the specified element.
    /// </summary>
    /// <param name="element">The element with an attached flyout.</param>
    public static void ShowAttachedFlyout(FrameworkElement element)
    {
        ArgumentNullException.ThrowIfNull(element);

        var flyout = GetAttachedFlyout(element);
        if (flyout is null)
        {
            return;
        }

        // Ensure the flyout is in the visual tree
        EnsureFlyoutInVisualTree(element, flyout);

        flyout.IsOpen = true;
    }

    /// <summary>
    /// Hides the attached flyout for the specified element.
    /// </summary>
    /// <param name="element">The element with an attached flyout.</param>
    public static void HideAttachedFlyout(FrameworkElement element)
    {
        ArgumentNullException.ThrowIfNull(element);

        var flyout = GetAttachedFlyout(element);
        if (flyout is not null)
        {
            flyout.IsOpen = false;
        }
    }

    private static void OnIsOpenChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not FrameworkElement element)
        {
            return;
        }

        var isOpen = (bool)e.NewValue;
        if (isOpen)
        {
            ShowAttachedFlyout(element);
        }
        else
        {
            HideAttachedFlyout(element);
        }
    }

    private static void OnOpenOnClickChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not UIElement element)
        {
            return;
        }

        var openOnClick = (bool)e.NewValue;
        if (openOnClick)
        {
            element.PreviewMouseLeftButtonUp += OnElementClick;
        }
        else
        {
            element.PreviewMouseLeftButtonUp -= OnElementClick;
        }
    }

    private static void OnElementClick(
        object sender,
        MouseButtonEventArgs e)
    {
        if (sender is FrameworkElement element)
        {
            ShowAttachedFlyout(element);
        }
    }

    private static void EnsureFlyoutInVisualTree(
        FrameworkElement element,
        Flyout flyout)
    {
        // If the flyout already has a parent, it's in the visual tree
        if (flyout.Parent is not null)
        {
            return;
        }

        // Find the root panel (typically a Grid at the window level)
        var parent = element;
        Panel? rootPanel = null;

        while (parent is not null)
        {
            if (parent is Window window)
            {
                rootPanel = window.Content as Panel;
                break;
            }

            if (parent is Panel panel && parent.Parent is Window)
            {
                rootPanel = panel;
                break;
            }

            parent = parent.Parent as FrameworkElement;
        }

        // Add the flyout to the root panel
        if (rootPanel is not null && !rootPanel.Children.Contains(flyout))
        {
            rootPanel.Children.Add(flyout);
        }
    }
}
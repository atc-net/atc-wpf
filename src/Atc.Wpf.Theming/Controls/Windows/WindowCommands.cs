// ReSharper disable LoopCanBeConvertedToQuery
namespace Atc.Wpf.Theming.Controls.Windows;

/// <summary>
/// A toolbar of commands hosted in the title bar of a window.
/// </summary>
[StyleTypedProperty(Property = nameof(ItemContainerStyle), StyleTargetType = typeof(WindowCommands))]
[DefaultStyleKey]
public sealed partial class WindowCommands : ToolBar
{
    /// <summary>
    /// Gets or sets the value indicating the current theme.
    /// </summary>
    [DependencyProperty(
        DefaultValueExpression = "ThemeManager.BaseColorLight",
        PropertyChangedCallback = nameof(OnThemePropertyChanged))]
    private string theme;

    /// <summary>
    /// Gets or sets the value indicating the light theme ControlTemplate.
    /// </summary>
    [DependencyProperty]
    private ControlTemplate? lightTemplate;

    /// <summary>
    /// Gets or sets the value indicating the light theme ControlTemplate.
    /// </summary>
    [DependencyProperty]
    private ControlTemplate? darkTemplate;

    /// <summary>
    /// Gets or sets the value indicating whether to show the separators or not.
    /// </summary>
    [DependencyProperty(
        DefaultValue = true,
        Flags = FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
        PropertyChangedCallback = nameof(OnShowSeparatorsPropertyChanged))]
    private bool showSeparators;

    /// <summary>
    /// Gets or sets the value indicating whether to show the last separator or not.
    /// </summary>
    [DependencyProperty(
        DefaultValue = true,
        Flags = FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
        PropertyChangedCallback = nameof(OnShowLastSeparatorPropertyChanged))]
    private bool showLastSeparator;

    [DependencyProperty(
        DefaultValue = 15d,
        Flags = FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
        GenerateClrAccessors = false)]
    private double separatorHeight;

    private static void OnThemePropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue == e.OldValue ||
            e.NewValue is not string baseColor)
        {
            return;
        }

        var windowCommands = (WindowCommands)d;

        switch (baseColor)
        {
            case ThemeManager.BaseColorLightConst:
                if (windowCommands.LightTemplate is not null)
                {
                    windowCommands.SetValue(TemplateProperty, windowCommands.LightTemplate);
                }

                break;

            case ThemeManager.BaseColorDarkConst:
                if (windowCommands.DarkTemplate is not null)
                {
                    windowCommands.SetValue(TemplateProperty, windowCommands.DarkTemplate);
                }

                break;
        }
    }

    private static void OnShowSeparatorsPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue == e.OldValue)
        {
            return;
        }

        ((WindowCommands)d).ResetSeparators();
    }

    private static void OnShowLastSeparatorPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue == e.OldValue)
        {
            return;
        }

        ((WindowCommands)d).ResetSeparators(false);
    }

    /// <summary>
    /// Gets or sets the value indicating the height of the separators.
    /// </summary>
    [TypeConverter(typeof(LengthConverter))]
    public double SeparatorHeight
    {
        get => (double)GetValue(SeparatorHeightProperty);
        set => SetValue(SeparatorHeightProperty, value);
    }

    internal static readonly DependencyPropertyKey ParentWindowPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(ParentWindow),
        typeof(Window),
        typeof(WindowCommands),
        new PropertyMetadata(propertyChangedCallback: null));

    /// <summary>Identifies the <see cref="ParentWindow"/> dependency property.</summary>
    public static readonly DependencyProperty ParentWindowProperty = ParentWindowPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets the window.
    /// </summary>
    public Window? ParentWindow
    {
        get => (Window?)GetValue(ParentWindowProperty);
        set => SetValue(ParentWindowPropertyKey, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowCommands"/> class.
    /// </summary>
    public WindowCommands()
    {
        Loaded += WindowCommandsLoaded;
    }

    /// <inheritdoc />
    protected override DependencyObject GetContainerForItemOverride()
        => new WindowCommandsItem();

    /// <inheritdoc />
    protected override bool IsItemItsOwnContainerOverride(object item)
        => item is WindowCommandsItem;

    /// <inheritdoc />
    protected override void PrepareContainerForItemOverride(
        DependencyObject element,
        object item)
    {
        base.PrepareContainerForItemOverride(
            element,
            item);

        if (element is not WindowCommandsItem windowCommandsItem)
        {
            return;
        }

        if (item is not FrameworkElement)
        {
            windowCommandsItem.ApplyTemplate();
            _ = windowCommandsItem.ContentTemplate?.LoadContent() as FrameworkElement;
        }

        AttachVisibilityHandler(
            windowCommandsItem,
            item as UIElement);
        ResetSeparators();
    }

    /// <inheritdoc />
    protected override void ClearContainerForItemOverride(
        DependencyObject element,
        object item)
    {
        base.ClearContainerForItemOverride(
            element,
            item);

        DetachVisibilityHandler(element as WindowCommandsItem);
        ResetSeparators(false);
    }

    private void AttachVisibilityHandler(
        WindowCommandsItem? container,
        UIElement? item)
    {
        if (container is null)
        {
            return;
        }

        if (item is null)
        {
            container.ApplyTemplate();
            if (container.ContentTemplate?.LoadContent() is not UIElement)
            {
                container.Visibility = Visibility.Collapsed;
            }

            return;
        }

        container.Visibility = item.Visibility;
        var isVisibilityNotifier = new PropertyChangeNotifier(
            item,
            VisibilityProperty);
        isVisibilityNotifier.ValueChanged += VisibilityPropertyChanged;
        container.VisibilityPropertyChangeNotifier = isVisibilityNotifier;
    }

    private static void DetachVisibilityHandler(WindowCommandsItem? container)
    {
        if (container is not null)
        {
            container.VisibilityPropertyChangeNotifier = null;
        }
    }

    private void VisibilityPropertyChanged(
        object? sender,
        EventArgs e)
    {
        if (sender is UIElement item)
        {
            var container = GetWindowCommandsItem(item);
            if (container is not null)
            {
                container.Visibility = item.Visibility;
                ResetSeparators();
            }
        }
    }

    /// <inheritdoc />
    protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
    {
        base.OnItemsChanged(e);

        ResetSeparators();
    }

    private void ResetSeparators(bool reset = true)
    {
        if (Items.Count == 0)
        {
            return;
        }

        var windowCommandsItems = GetWindowCommandsItems()
            .ToList();

        if (reset)
        {
            foreach (var windowCommandsItem in windowCommandsItems)
            {
                windowCommandsItem.IsSeparatorVisible = ShowSeparators;
            }
        }

        var lastContainer = windowCommandsItems.LastOrDefault(i => i.IsVisible);
        if (lastContainer is not null)
        {
            lastContainer.IsSeparatorVisible = ShowSeparators && ShowLastSeparator;
        }
    }

    private WindowCommandsItem? GetWindowCommandsItem(object? item)
    {
        if (item is WindowCommandsItem windowCommandsItem)
        {
            return windowCommandsItem;
        }

        return (WindowCommandsItem?)ItemContainerGenerator.ContainerFromItem(item);
    }

    private IEnumerable<WindowCommandsItem> GetWindowCommandsItems()
    {
        foreach (var item in Items)
        {
            var windowCommandsItem = GetWindowCommandsItem(item);
            if (windowCommandsItem is not null)
            {
                yield return windowCommandsItem;
            }
        }
    }

    private void WindowCommandsLoaded(
        object sender,
        RoutedEventArgs e)
    {
        Loaded -= WindowCommandsLoaded;

        var contentPresenter = this.TryFindParent<ContentPresenter>();
        if (contentPresenter is not null)
        {
            SetCurrentValue(DockPanel.DockProperty, contentPresenter.GetValue(DockPanel.DockProperty));
        }

        if (ParentWindow is null)
        {
            var window = this.TryFindParent<Window>();
            SetValue(ParentWindowPropertyKey, window);
        }
    }

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer()
        => new WindowCommandsAutomationPeer(this);
}
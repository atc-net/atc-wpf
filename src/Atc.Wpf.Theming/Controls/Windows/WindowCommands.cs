// ReSharper disable LoopCanBeConvertedToQuery

namespace Atc.Wpf.Theming.Controls.Windows;

[StyleTypedProperty(Property = nameof(ItemContainerStyle), StyleTargetType = typeof(WindowCommands))]
public class WindowCommands : ToolBar
{
    public static readonly DependencyProperty ThemeProperty = DependencyProperty.Register(
        nameof(Theme),
        typeof(string),
        typeof(WindowCommands),
        new PropertyMetadata(ThemeManager.BaseColorLight, OnThemePropertyChanged));

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

    /// <summary>
    /// Gets or sets the value indicating the current theme.
    /// </summary>
    public string Theme
    {
        get => (string)GetValue(ThemeProperty);
        set => SetValue(ThemeProperty, value);
    }

    public static readonly DependencyProperty LightTemplateProperty = DependencyProperty.Register(
        nameof(LightTemplate),
        typeof(ControlTemplate),
        typeof(WindowCommands),
        new PropertyMetadata(propertyChangedCallback: null));

    /// <summary>
    /// Gets or sets the value indicating the light theme ControlTemplate.
    /// </summary>
    public ControlTemplate? LightTemplate
    {
        get => (ControlTemplate?)GetValue(LightTemplateProperty);
        set => SetValue(LightTemplateProperty, value);
    }

    public static readonly DependencyProperty DarkTemplateProperty = DependencyProperty.Register(
        nameof(DarkTemplate),
        typeof(ControlTemplate),
        typeof(WindowCommands),
        new PropertyMetadata(propertyChangedCallback: null));

    /// <summary>
    /// Gets or sets the value indicating the light theme ControlTemplate.
    /// </summary>
    public ControlTemplate? DarkTemplate
    {
        get => (ControlTemplate?)GetValue(DarkTemplateProperty);
        set => SetValue(DarkTemplateProperty, value);
    }

    public static readonly DependencyProperty ShowSeparatorsProperty = DependencyProperty.Register(
        nameof(ShowSeparators),
        typeof(bool),
        typeof(WindowCommands),
        new FrameworkPropertyMetadata(
            BooleanBoxes.TrueBox,
            FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
            OnShowSeparatorsPropertyChanged));

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

    /// <summary>
    /// Gets or sets the value indicating whether to show the separators or not.
    /// </summary>
    public bool ShowSeparators
    {
        get => (bool)GetValue(ShowSeparatorsProperty);
        set => SetValue(ShowSeparatorsProperty, BooleanBoxes.Box(value));
    }

    public static readonly DependencyProperty ShowLastSeparatorProperty = DependencyProperty.Register(
        nameof(ShowLastSeparator),
        typeof(bool),
        typeof(WindowCommands),
        new FrameworkPropertyMetadata(
            BooleanBoxes.TrueBox,
            FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
            OnShowLastSeparatorPropertyChanged));

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
    /// Gets or sets the value indicating whether to show the last separator or not.
    /// </summary>
    public bool ShowLastSeparator
    {
        get => (bool)GetValue(ShowLastSeparatorProperty);
        set => SetValue(ShowLastSeparatorProperty, BooleanBoxes.Box(value));
    }

    public static readonly DependencyProperty SeparatorHeightProperty = DependencyProperty.Register(
        nameof(SeparatorHeight),
        typeof(double),
        typeof(WindowCommands),
        new FrameworkPropertyMetadata(
            15d,
            FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

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
        protected set => SetValue(ParentWindowPropertyKey, value);
    }

    static WindowCommands()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowCommands), new FrameworkPropertyMetadata(typeof(WindowCommands)));
    }

    public WindowCommands()
    {
        Loaded += WindowCommandsLoaded;
    }

    protected override DependencyObject GetContainerForItemOverride()
    {
        return new WindowCommandsItem();
    }

    protected override bool IsItemItsOwnContainerOverride(
        object item)
    {
        return item is WindowCommandsItem;
    }

    protected override void PrepareContainerForItemOverride(
        DependencyObject element,
        object item)
    {
        base.PrepareContainerForItemOverride(element, item);

        if (element is not WindowCommandsItem windowCommandsItem)
        {
            return;
        }

        // ReSharper disable once NotAccessedVariable
        var frameworkElement = item as FrameworkElement;
        if (item is not FrameworkElement)
        {
            windowCommandsItem.ApplyTemplate();

            // ReSharper disable once RedundantAssignment
            frameworkElement = windowCommandsItem.ContentTemplate?.LoadContent() as FrameworkElement;
        }

        AttachVisibilityHandler(windowCommandsItem, item as UIElement);
        ResetSeparators();
    }

    protected override void ClearContainerForItemOverride(
        DependencyObject element,
        object item)
    {
        base.ClearContainerForItemOverride(element, item);

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
        var isVisibilityNotifier = new PropertyChangeNotifier(item, VisibilityProperty);
        isVisibilityNotifier.ValueChanged += VisibilityPropertyChanged;
        container.VisibilityPropertyChangeNotifier = isVisibilityNotifier;
    }

    private static void DetachVisibilityHandler(
        WindowCommandsItem? container)
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

    protected override void OnItemsChanged(
        NotifyCollectionChangedEventArgs e)
    {
        base.OnItemsChanged(e);

        ResetSeparators();
    }

    private void ResetSeparators(
        bool reset = true)
    {
        if (Items.Count == 0)
        {
            return;
        }

        var windowCommandsItems = GetWindowCommandsItems().ToList();

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

    private WindowCommandsItem? GetWindowCommandsItem(
        object? item)
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

    protected override AutomationPeer OnCreateAutomationPeer()
        => new WindowCommandsAutomationPeer(this);
}
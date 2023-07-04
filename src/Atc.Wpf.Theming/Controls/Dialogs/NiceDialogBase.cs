// ReSharper disable InvertIf
// ReSharper disable InconsistentNaming
namespace Atc.Wpf.Theming.Controls.Dialogs;

/// <summary>
/// The base class for dialogs.
/// </summary>
[TemplatePart(Name = PART_Top, Type = typeof(ContentPresenter))]
[TemplatePart(Name = PART_Content, Type = typeof(ContentPresenter))]
[TemplatePart(Name = PART_Bottom, Type = typeof(ContentPresenter))]
public abstract class NiceDialogBase : ContentControl
{
    private const string PART_Top = "PART_Top";
    private const string PART_Content = "PART_Content";
    private const string PART_Bottom = "PART_Bottom";

    public static readonly DependencyProperty ColorSchemeProperty = DependencyProperty.Register(
        nameof(ColorScheme),
        typeof(DialogColorScheme),
        typeof(NiceDialogBase),
        new PropertyMetadata(DialogColorScheme.Theme));

    public DialogColorScheme ColorScheme
    {
        get => (DialogColorScheme)GetValue(ColorSchemeProperty);
        set => SetValue(ColorSchemeProperty, value);
    }

    public static readonly DependencyProperty DialogContentMarginProperty = DependencyProperty.Register(
        nameof(DialogContentMargin),
        typeof(GridLength),
        typeof(NiceDialogBase),
        new PropertyMetadata(
            new GridLength(25, GridUnitType.Star)));

    /// <summary>
    /// Gets or sets the left and right margin for the dialog content.
    /// </summary>
    public GridLength DialogContentMargin
    {
        get => (GridLength)GetValue(DialogContentMarginProperty);
        set => SetValue(DialogContentMarginProperty, value);
    }

    public static readonly DependencyProperty DialogContentWidthProperty = DependencyProperty.Register(
        nameof(DialogContentWidth),
        typeof(GridLength),
        typeof(NiceDialogBase),
        new PropertyMetadata(
            new GridLength(50, GridUnitType.Star)));

    /// <summary>
    /// Gets or sets the width for the dialog content.
    /// </summary>
    public GridLength DialogContentWidth
    {
        get => (GridLength)GetValue(DialogContentWidthProperty);
        set => SetValue(DialogContentWidthProperty, value);
    }

    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title),
        typeof(string),
        typeof(NiceDialogBase),
        new PropertyMetadata(default(string)));

    /// <summary>
    /// Gets or sets the title of the dialog.
    /// </summary>
    public string? Title
    {
        get => (string?)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly DependencyProperty DialogTopProperty = DependencyProperty.Register(
        nameof(DialogTop),
        typeof(object),
        typeof(NiceDialogBase),
        new PropertyMetadata(
            defaultValue: null,
            UpdateLogicalChild));

    /// <summary>
    /// Gets or sets the content above the dialog.
    /// </summary>
    public object? DialogTop
    {
        get => GetValue(DialogTopProperty);
        set => SetValue(DialogTopProperty, value);
    }

    public static readonly DependencyProperty DialogBottomProperty = DependencyProperty.Register(
        nameof(DialogBottom),
        typeof(object),
        typeof(NiceDialogBase),
        new PropertyMetadata(
            defaultValue: null,
            UpdateLogicalChild));

    /// <summary>
    /// Gets or sets the content below the dialog.
    /// </summary>
    public object? DialogBottom
    {
        get => GetValue(DialogBottomProperty);
        set => SetValue(DialogBottomProperty, value);
    }

    public static readonly DependencyProperty DialogTitleFontSizeProperty = DependencyProperty.Register(
        nameof(DialogTitleFontSize),
        typeof(double),
        typeof(NiceDialogBase),
        new PropertyMetadata(26D));

    /// <summary>
    /// Gets or sets the font size of the dialog title.
    /// </summary>
    public double DialogTitleFontSize
    {
        get => (double)GetValue(DialogTitleFontSizeProperty);
        set => SetValue(DialogTitleFontSizeProperty, value);
    }

    public static readonly DependencyProperty DialogMessageFontSizeProperty = DependencyProperty.Register(
        nameof(DialogMessageFontSize),
        typeof(double),
        typeof(NiceDialogBase),
        new PropertyMetadata(15D));

    /// <summary>
    /// Gets or sets the font size of the dialog message text.
    /// </summary>
    public double DialogMessageFontSize
    {
        get => (double)GetValue(DialogMessageFontSizeProperty);
        set => SetValue(DialogMessageFontSizeProperty, value);
    }

    public static readonly DependencyProperty DialogButtonFontSizeProperty = DependencyProperty.Register(
        nameof(DialogButtonFontSize),
        typeof(double),
        typeof(NiceDialogBase),
        new PropertyMetadata(SystemFonts.MessageFontSize));

    /// <summary>
    /// Gets or sets the font size of any dialog buttons.
    /// </summary>
    public double DialogButtonFontSize
    {
        get => (double)GetValue(DialogButtonFontSizeProperty);
        set => SetValue(DialogButtonFontSizeProperty, value);
    }

    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
        nameof(Icon),
        typeof(object),
        typeof(NiceDialogBase),
        new PropertyMetadata());

    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly DependencyProperty IconTemplateProperty = DependencyProperty.Register(
        nameof(IconTemplate),
        typeof(DataTemplate),
        typeof(NiceDialogBase));

    public DataTemplate? IconTemplate
    {
        get => (DataTemplate?)GetValue(IconTemplateProperty);
        set => SetValue(IconTemplateProperty, value);
    }

    public DialogSettings DialogSettings { get; private set; } = null!;

    internal SizeChangedEventHandler? SizeChangedHandler { get; set; }

    static NiceDialogBase()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(NiceDialogBase), new FrameworkPropertyMetadata(typeof(NiceDialogBase)));
    }

    protected NiceDialogBase(
        NiceWindow? owningWindow,
        DialogSettings? settings)
    {
        Initialize(owningWindow, settings);
    }

    protected NiceDialogBase()
        : this(owningWindow: null, new DialogSettings())
    {
    }

    private static void UpdateLogicalChild(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs e)
    {
        if (dependencyObject is not NiceDialogBase dialog)
        {
            return;
        }

        if (e.OldValue is FrameworkElement oldChild)
        {
            dialog.RemoveLogicalChild(oldChild);
        }

        if (e.NewValue is FrameworkElement newChild)
        {
            dialog.AddLogicalChild(newChild);
            newChild.DataContext = dialog.DataContext;
        }
    }

    protected override IEnumerator LogicalChildren
    {
        get
        {
            var children = new ArrayList();
            if (DialogTop is not null)
            {
                children.Add(DialogTop);
            }

            if (Content is not null)
            {
                children.Add(Content);
            }

            if (DialogBottom is not null)
            {
                children.Add(DialogBottom);
            }

            return children.GetEnumerator();
        }
    }

    protected virtual DialogSettings ConfigureSettings(
        DialogSettings settings)
    {
        return settings;
    }

    private void Initialize(
        NiceWindow? owningWindow,
        DialogSettings? settings)
    {
        AccessKeyHelper.SetIsAccessKeyScope(this, value: true);

        OwningWindow = owningWindow;
        DialogSettings = ConfigureSettings(settings ?? owningWindow?.NiceDialogOptions ?? new DialogSettings());

        if (DialogSettings.CustomResourceDictionary is not null)
        {
            Resources.MergedDictionaries.Add(DialogSettings.CustomResourceDictionary);
        }

        SetCurrentValue(ColorSchemeProperty, DialogSettings.ColorScheme);

        SetCurrentValue(IconProperty, DialogSettings.Icon);
        SetCurrentValue(IconTemplateProperty, DialogSettings.IconTemplate);

        HandleThemeChange();

        DataContextChanged += NiceDialogBaseDataContextChanged;
        Loaded += NiceDialogBaseLoaded;
        Unloaded += NiceDialogBaseUnloaded;
    }

    private void NiceDialogBaseDataContextChanged(
        object? sender,
        DependencyPropertyChangedEventArgs e)
    {
        if (DialogTop is FrameworkElement elementTop)
        {
            elementTop.DataContext = DataContext;
        }

        if (DialogBottom is FrameworkElement elementBottom)
        {
            elementBottom.DataContext = DataContext;
        }
    }

    private void NiceDialogBaseLoaded(
        object? sender,
        RoutedEventArgs e)
    {
        ThemeManager.Current.ThemeChanged -= HandleThemeManagerThemeChanged;
        ThemeManager.Current.ThemeChanged += HandleThemeManagerThemeChanged;
        OnLoaded();
    }

    private void NiceDialogBaseUnloaded(
        object? sender,
        RoutedEventArgs e)
    {
        ThemeManager.Current.ThemeChanged -= HandleThemeManagerThemeChanged;
    }

    private void HandleThemeManagerThemeChanged(
        object? sender,
        ThemeChangedEventArgs e)
    {
        this.Invoke(HandleThemeChange);
    }

    private static object? TryGetResource(
        Theme? theme,
        string key)
    {
        return theme?.Resources[key];
    }

    internal void HandleThemeChange()
    {
        var theme = DetectTheme(this);

        if (DesignerProperties.GetIsInDesignMode(this)
            || theme is null)
        {
            return;
        }

        switch (DialogSettings.ColorScheme)
        {
            case DialogColorScheme.Theme:
                ThemeManager.Current.ChangeTheme(this, Resources, theme);
                SetCurrentValue(BackgroundProperty, TryGetResource(theme, "AtcApps.Brushes.Dialog.Background"));
                SetCurrentValue(ForegroundProperty, TryGetResource(theme, "AtcApps.Brushes.Dialog.Foreground"));
                break;

            case DialogColorScheme.Inverted:
                theme = ThemeManager.Current.GetInverseTheme(theme);
                if (theme is null)
                {
                    throw new InvalidOperationException("The inverse dialog theme only works if the window theme abides the naming convention. " +
                                                        "See ThemeManager.GetInverseAppTheme for more infos");
                }

                ThemeManager.Current.ChangeTheme(this, Resources, theme);
                SetCurrentValue(BackgroundProperty, TryGetResource(theme, "AtcApps.Brushes.Dialog.Background"));
                SetCurrentValue(ForegroundProperty, TryGetResource(theme, "AtcApps.Brushes.Dialog.Foreground"));
                break;

            case DialogColorScheme.Accented:
                ThemeManager.Current.ChangeTheme(this, Resources, theme);
                SetCurrentValue(BackgroundProperty, TryGetResource(theme, "AtcApps.Brushes.Dialog.Background.Accent"));
                SetCurrentValue(ForegroundProperty, TryGetResource(theme, "AtcApps.Brushes.Dialog.Foreground.Accent"));
                break;
        }

        if (ParentDialogWindow is not null)
        {
            ParentDialogWindow.SetCurrentValue(BackgroundProperty, Background);
            var glowColor = TryGetResource(theme, "AtcApps.Colors.Accent");
            if (glowColor is not null)
            {
                ParentDialogWindow.SetCurrentValue(WindowChromeWindow.GlowColorProperty, glowColor);
            }
        }
    }

    /// <summary>
    /// This is called in the loaded event.
    /// </summary>
    protected virtual void OnLoaded()
    {
    }

    private static Theme? DetectTheme(
        NiceDialogBase? dialog)
    {
        if (dialog is null)
        {
            return null;
        }

        var window = dialog.OwningWindow ?? dialog.TryFindParent<NiceWindow>();
        var theme = window is not null ? ThemeManager.Current.DetectTheme(window) : null;
        if (theme is not null)
        {
            return theme;
        }

        if (Application.Current is not null)
        {
            theme = Application.Current.MainWindow is null
                ? ThemeManager.Current.DetectTheme(Application.Current)
                : ThemeManager.Current.DetectTheme(Application.Current.MainWindow);
            if (theme is not null)
            {
                return theme;
            }
        }

        return null;
    }

    private RoutedEventHandler? dialogOnLoaded;

    /// <summary>
    /// Waits for the dialog to become ready for interaction.
    /// </summary>
    /// <returns>A task that represents the operation and it's status.</returns>
    public Task WaitForLoadAsync()
    {
        Dispatcher.VerifyAccess();

        if (IsLoaded)
        {
            return Task.CompletedTask;
        }

        var tcs = new TaskCompletionSource<object>();

        if (!DialogSettings.AnimateShow)
        {
            SetCurrentValue(OpacityProperty, 1.0);
        }

        dialogOnLoaded = (_, _) =>
        {
            Loaded -= dialogOnLoaded;

            Focus();

            tcs.TrySetResult(null!);
        };

        Loaded += dialogOnLoaded;

        return tcs.Task;
    }

    public async Task RequestCloseAsync()
    {
        if (OnRequestClose())
        {
            if (ParentDialogWindow is null)
            {
                if (OwningWindow is null)
                {
                    Trace.TraceWarning($"{this}: Can not request async closing, because the OwningWindow is already null. This can maybe happen if the dialog was closed manually.");
                    return;
                }

                return;
            }

            await WaitForCloseAsync().ConfigureAwait(true);

            ParentDialogWindow.Close();
        }
    }

    internal void FireOnShown()
    {
        OnShown();
    }

    protected virtual void OnShown()
    {
    }

    internal void FireOnClose()
    {
        OnClose();

        ParentDialogWindow?.Close();
    }

    protected virtual void OnClose()
    {
    }

    protected virtual bool OnRequestClose()
    {
        return true;
    }

    protected internal Window? ParentDialogWindow { get; internal set; }

    protected NiceWindow? OwningWindow { get; private set; }

    public Task WaitUntilUnloadedAsync()
    {
        var tcs = new TaskCompletionSource<object>();

        Unloaded += (_, _) => { tcs.TrySetResult(null!); };

        return tcs.Task;
    }

    private EventHandler? closingStoryboardOnCompleted;

    public Task WaitForCloseAsync()
    {
        var tcs = new TaskCompletionSource<object>();

        if (DialogSettings.AnimateHide)
        {
            if (TryFindResource("AtcApps.Storyboard.Dialogs.Close") is not Storyboard closingStoryboard)
            {
                throw new InvalidOperationException("Unable to find the dialog closing storyboard. Did you forget to add NiceDialogBase.xaml to your merged dictionaries?");
            }

            closingStoryboard = closingStoryboard.Clone();

            closingStoryboardOnCompleted = (_, _) =>
            {
                closingStoryboard.Completed -= closingStoryboardOnCompleted;

                tcs.TrySetResult(null!);
            };

            closingStoryboard.Completed += closingStoryboardOnCompleted;

            closingStoryboard.Begin(this);
        }
        else
        {
            SetCurrentValue(OpacityProperty, 0.0);
            tcs.TrySetResult(null!);
        }

        return tcs.Task;
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new NiceDialogAutomationPeer(this);
    }

    protected override void OnKeyDown(
        KeyEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        if (e.Key == Key.System &&
            e.SystemKey is Key.LeftAlt or Key.RightAlt or Key.F10 &&
            ReferenceEquals(e.Source, this))
        {
            var menu = this.FindChildren<Menu>(forceUsingTheVisualTreeHelper: true).FirstOrDefault(m => m.IsMainMenu);
            if (menu is null)
            {
                e.Handled = true;
            }
        }

        base.OnKeyDown(e);
    }
}
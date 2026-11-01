namespace Atc.Wpf.Controls.Layouts.Internal;

/// <summary>
/// The themed tool window that hosts a floating <see cref="DockPane"/>. Its title bar has a Dock button, and
/// double-clicking the title bar docks the pane back.
/// </summary>
internal sealed class DockFloatingWindow : Atc.Wpf.Theming.Controls.Windows.NiceWindow
{
    /// <summary>
    /// The height a floating window adds to its pane for the title bar.
    /// </summary>
    public const double TitleBarExtent = 32;

    private readonly Button dockButton;
    private readonly MenuItem dockMenuItem;
    private readonly MenuItem closeMenuItem;
    private readonly List<ResourceDictionary> inheritedResources = [];

    public DockFloatingWindow(DockPane pane)
    {
        Pane = pane;
        ShowInTaskbar = false;
        ShowMinButton = false;
        ShowMaxRestoreButton = false;
        ShowIconOnTitleBar = false;
        ResizeMode = ResizeMode.CanResize;
        WindowStartupLocation = WindowStartupLocation.Manual;
        SetResourceReference(BackgroundProperty, "AtcApps.Brushes.ThemeBackground");
        SetBinding(TitleProperty, new Binding(nameof(DockPane.Header)) { Source = pane, Mode = BindingMode.OneWay, Converter = HeaderTextConverter.Instance, ConverterParameter = pane });
        AutomationProperties.SetAutomationId(this, "FloatingPane_" + (pane.PaneId ?? string.Empty));

        dockButton = new Button
        {
            Command = DockPanelProCommands.RedockPane,
            CommandParameter = pane,
            Content = CreateDockGlyph(),
        };
        AutomationProperties.SetAutomationId(dockButton, "RedockPane");
        var commands = new Atc.Wpf.Theming.Controls.Windows.WindowCommands();
        commands.Items.Add(dockButton);
        RightWindowCommands = commands;

        // Shift+F10 (or a right-click) inside the pane: Dock / Close by keyboard.
        dockMenuItem = new MenuItem { Command = DockPanelProCommands.RedockPane, CommandParameter = pane, CommandTarget = this };
        closeMenuItem = new MenuItem { Command = DockPanelProCommands.ClosePane, CommandParameter = pane, CommandTarget = this };
        var menu = new ContextMenu();
        menu.Items.Add(dockMenuItem);
        menu.Items.Add(closeMenuItem);
        ContextMenu = menu;

        Content = pane;
        ApplyTexts();
    }

    /// <summary>
    /// Occurs when the window is moved or resized.
    /// </summary>
    public event EventHandler? BoundsChanged;

    public DockPane Pane { get; }

    public bool IsClosed { get; private set; }

    /// <summary>
    /// Gets the bounds of the window in its normal (restored) state.
    /// </summary>
    public Rect CurrentBounds
        => WindowState == WindowState.Normal || RestoreBounds.IsEmpty
            ? new Rect(Left, Top, double.IsNaN(Width) ? ActualWidth : Width, double.IsNaN(Height) ? ActualHeight : Height)
            : RestoreBounds;

    public void ApplyTexts()
    {
        var name = string.Format(CultureInfo.CurrentCulture, Atc.Wpf.Resources.Miscellaneous.Dock1, Pane.HeaderText);
        AutomationProperties.SetName(dockButton, name);
        dockButton.ToolTip = name;
        dockMenuItem.Header = name;
        closeMenuItem.Header = Atc.Wpf.Resources.Word.Close;
    }

    /// <summary>
    /// Lets go of the pane, so the region can show it again.
    /// </summary>
    public void ReleasePane()
    {
        BindingOperations.ClearBinding(this, TitleProperty);
        Content = null;
        if (!IsClosed && IsLoaded)
        {
            UpdateLayout();
        }
    }

    /// <summary>
    /// Merges the resource dictionaries of the panel's ancestors into the window (outermost first).
    /// </summary>
    public void InheritResources(IEnumerable<ResourceDictionary> dictionaries)
    {
        foreach (var dictionary in dictionaries)
        {
            if (ReferenceEquals(dictionary, Resources) ||
                Resources.MergedDictionaries.Contains(dictionary))
            {
                continue;
            }

            Resources.MergedDictionaries.Add(dictionary);
            inheritedResources.Add(dictionary);
        }
    }

    /// <summary>
    /// Removes the merged ancestor dictionaries again, so they no longer list the window as an owner.
    /// </summary>
    public void ReleaseResources()
    {
        foreach (var dictionary in inheritedResources)
        {
            Resources.MergedDictionaries.Remove(dictionary);
        }

        inheritedResources.Clear();
    }

    /// <summary>
    /// Closes the window unless it is already closed.
    /// </summary>
    public void CloseIfOpen()
    {
        if (!IsClosed)
        {
            Close();
        }
    }

    protected override void OnClosed(EventArgs e)
    {
        IsClosed = true;
        base.OnClosed(e);
    }

    protected override void OnLocationChanged(EventArgs e)
    {
        base.OnLocationChanged(e);
        BoundsChanged?.Invoke(this, EventArgs.Empty);
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);
        BoundsChanged?.Invoke(this, EventArgs.Empty);
    }

    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        if (e.ClickCount == 2 &&
            e.GetPosition(this).Y <= TitleBarHeight &&
            DockPanelProCommands.RedockPane.CanExecute(Pane, this))
        {
            e.Handled = true;
            DockPanelProCommands.RedockPane.Execute(Pane, this);
            return;
        }

        base.OnPreviewMouseLeftButtonDown(e);
    }

    private static System.Windows.Shapes.Path CreateDockGlyph()
    {
        // A window with a filled left part: "dock into the main window".
        var path = new System.Windows.Shapes.Path
        {
            Width = 12,
            Height = 12,
            Stretch = Stretch.Uniform,
            StrokeThickness = 1.2,
            Data = Geometry.Parse("M 0.5,0.5 H 11.5 V 11.5 H 0.5 Z M 0.5,3 H 11.5 M 0.5,3 H 5 V 11.5"),
            SnapsToDevicePixels = true,
        };
        path.SetBinding(Shape.StrokeProperty, new Binding(nameof(Control.Foreground)) { RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(Button), 1) });
        return path;
    }

    /// <summary>
    /// Shows the pane header as plain text in the window title.
    /// </summary>
    private sealed class HeaderTextConverter : IValueConverter
    {
        public static readonly HeaderTextConverter Instance = new();

        public object Convert(
            object? value,
            Type targetType,
            object? parameter,
            CultureInfo culture)
            => parameter is DockPane pane
                ? pane.HeaderText
                : value?.ToString() ?? string.Empty;

        public object ConvertBack(
            object? value,
            Type targetType,
            object? parameter,
            CultureInfo culture)
            => Binding.DoNothing;
    }
}
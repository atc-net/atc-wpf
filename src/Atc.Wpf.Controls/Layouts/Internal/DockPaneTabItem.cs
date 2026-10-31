namespace Atc.Wpf.Controls.Layouts.Internal;

/// <summary>
/// The tab of a docked <see cref="DockPane"/>: shows the pane header and a close button (when the pane can close),
/// opens a Float/Close context menu (also with Shift+F10), floats the pane on double-click or when dragged out of the strip.
/// </summary>
[TemplatePart(Name = PartCloseButton, Type = typeof(ButtonBase))]
internal sealed class DockPaneTabItem : TabItem
{
    private const string PartCloseButton = "PART_CloseButton";

    private ButtonBase? closeButton;
    private Point? dragStart;

    public DockPaneTabItem()
    {
        ContextMenu = CreatePaneContextMenu(() => Pane, this);
    }

    public DockPane? Pane { get; private set; }

    /// <summary>
    /// Creates the Float / Dock / Close context menu for a pane; the texts are set each time it opens.
    /// </summary>
    public static ContextMenu CreatePaneContextMenu(
        Func<DockPane?> getPane,
        IInputElement commandTarget)
    {
        var floatItem = new MenuItem { Command = DockPanelProCommands.FloatPane, CommandTarget = commandTarget };
        var closeItem = new MenuItem { Command = DockPanelProCommands.ClosePane, CommandTarget = commandTarget };
        var menu = new ContextMenu();
        menu.Items.Add(floatItem);
        menu.Items.Add(closeItem);
        menu.Opened += (_, _) =>
        {
            var pane = getPane();
            floatItem.CommandParameter = pane;
            closeItem.CommandParameter = pane;
            floatItem.Header = Atc.Wpf.Resources.Miscellaneous.Float;
            closeItem.Header = Atc.Wpf.Resources.Word.Close;
            AutomationProperties.SetName(floatItem, Atc.Wpf.Resources.Miscellaneous.Float);
            AutomationProperties.SetName(closeItem, Atc.Wpf.Resources.Word.Close);
            CommandManager.InvalidateRequerySuggested();
        };

        return menu;
    }

    /// <summary>
    /// Gets a value indicating whether a header drag went far enough out of the tab strip to float the pane.
    /// </summary>
    public static bool IsDraggedOut(
        Point start,
        Point current,
        double stripHeight)
    {
        var threshold = System.Math.Max(stripHeight, SystemParameters.MinimumVerticalDragDistance * 4);
        return System.Math.Abs(current.Y - start.Y) > threshold;
    }

    public void Attach(DockPane pane)
    {
        Pane = pane;
        SetBinding(HeaderProperty, new Binding(nameof(DockPane.Header)) { Source = pane });
        SetBinding(HeaderTemplateProperty, new Binding(nameof(DockPane.HeaderTemplate)) { Source = pane });
        SetBinding(AutomationProperties.AutomationIdProperty, new Binding(nameof(DockPane.PaneId)) { Source = pane });
        ApplyTexts();
    }

    public void Detach()
    {
        BindingOperations.ClearBinding(this, HeaderProperty);
        BindingOperations.ClearBinding(this, HeaderTemplateProperty);
        BindingOperations.ClearBinding(this, AutomationProperties.AutomationIdProperty);
        Pane = null;
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        closeButton = GetTemplateChild(PartCloseButton) as ButtonBase;
        ApplyTexts();
    }

    public void ApplyTexts()
    {
        if (closeButton is null || Pane is null)
        {
            return;
        }

        var name = string.Format(CultureInfo.CurrentCulture, Atc.Wpf.Resources.Miscellaneous.Close1, Pane.HeaderText);
        AutomationProperties.SetName(closeButton, name);
        closeButton.ToolTip = name;
        closeButton.CommandParameter = Pane;
    }

    protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
    {
        base.OnMouseDoubleClick(e);

        if (e.ChangedButton != MouseButton.Left ||
            Pane is null ||
            !DockPanelProCommands.FloatPane.CanExecute(Pane, this))
        {
            return;
        }

        dragStart = null;
        DockPanelProCommands.FloatPane.Execute(Pane, this);
        e.Handled = true;
    }

    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnPreviewMouseLeftButtonDown(e);
        dragStart = e.ClickCount == 1 ? e.GetPosition(this) : null;
    }

    protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        base.OnPreviewMouseLeftButtonUp(e);
        dragStart = null;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (dragStart is null || Pane is null)
        {
            return;
        }

        if (e.LeftButton != MouseButtonState.Pressed)
        {
            dragStart = null;
            return;
        }

        if (!IsDraggedOut(dragStart.Value, e.GetPosition(this), ActualHeight))
        {
            return;
        }

        dragStart = null;
        Pane.Region?.OwnerPanel?.FloatPaneByDrag(Pane);
    }
}
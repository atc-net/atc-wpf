namespace Atc.Wpf.Controls.DataGrids.Internal;

/// <summary>
/// The funnel button in a filterable column header. Clicking it (or Alt+Down while it has focus) opens the column
/// filter pop-up; it shows a filled accent funnel while the column has an active filter.
/// </summary>
internal sealed class DataGridFilterButton : Button
{
    private const string OutlineFunnel = "M1,2 L15,2 L9.5,8.5 L9.5,14 L6.5,12 L6.5,8.5 Z";

    private static readonly Geometry FunnelGeometry = CreateFunnelGeometry();

    private static readonly ControlTemplate ButtonTemplate = CreateTemplate();

    private readonly System.Windows.Shapes.Path funnel;

    public DataGridFilterButton()
    {
        funnel = new System.Windows.Shapes.Path
        {
            Data = FunnelGeometry,
            Width = 12,
            Height = 12,
            Stretch = Stretch.Uniform,
            StrokeThickness = 1.2,
            SnapsToDevicePixels = true,
        };

        Template = ButtonTemplate;
        Content = funnel;
        Background = Brushes.Transparent;
        Padding = new Thickness(3);
        Margin = new Thickness(4, 0, 0, 0);
        VerticalAlignment = VerticalAlignment.Center;
        Cursor = Cursors.Hand;
        IsTabStop = true;
        UpdateFunnel();
    }

    /// <summary>
    /// Gets or sets the column whose filter the button opens.
    /// </summary>
    public DataGridColumn? Column { get; set; }

    /// <summary>
    /// Gets or sets the grid of <see cref="Column"/>.
    /// </summary>
    public DataGrid? Grid { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the column has an active filter.
    /// </summary>
    public bool IsFilterActive
    {
        get => (bool)GetValue(IsFilterActiveProperty);
        set => SetValue(IsFilterActiveProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="IsFilterActive"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsFilterActiveProperty = DependencyProperty.Register(
        nameof(IsFilterActive),
        typeof(bool),
        typeof(DataGridFilterButton),
        new PropertyMetadata(
            BooleanBoxes.FalseBox,
            static (d, _) => ((DataGridFilterButton)d).UpdateFunnel()));

    /// <summary>
    /// Gets the pop-up content while the button's pop-up is open (for UI Automation), else <see langword="null"/>.
    /// </summary>
    internal DataGridFilterPopupContent? OpenPopupContent
        => Grid is not null && Column is not null
            ? DataGridFilter.GetOpenPopupContent(Grid, Column)
            : null;

    /// <summary>
    /// Updates the automation name ("Filter {column}") and tool tip from the column header.
    /// </summary>
    internal void UpdateName()
    {
        if (Column is null)
        {
            return;
        }

        var text = string.Format(
            CultureInfo.CurrentCulture,
            Atc.Wpf.Resources.Miscellaneous.FilterColumn1,
            DataGridFilter.GetColumnTitle(Column));
        AutomationProperties.SetName(this, text);
        ToolTip = text;
    }

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer()
        => new DataGridFilterButtonAutomationPeer(this);

    /// <inheritdoc />
    protected override void OnClick()
    {
        base.OnClick();
        if (Grid is not null && Column is not null)
        {
            DataGridFilter.OpenColumnFilter(Grid, Column);
        }
    }

    /// <inheritdoc />
    protected override void OnMouseEnter(MouseEventArgs e)
    {
        base.OnMouseEnter(e);
        SetResourceReference(BackgroundProperty, "AtcApps.Brushes.Gray7");
    }

    /// <inheritdoc />
    protected override void OnMouseLeave(MouseEventArgs e)
    {
        base.OnMouseLeave(e);
        Background = Brushes.Transparent;
    }

    private static Geometry CreateFunnelGeometry()
    {
        var geometry = Geometry.Parse(OutlineFunnel);
        geometry.Freeze();
        return geometry;
    }

    private static ControlTemplate CreateTemplate()
    {
        var border = new FrameworkElementFactory(typeof(Border));
        border.SetValue(Border.CornerRadiusProperty, new CornerRadius(2));
        border.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(BackgroundProperty));
        border.SetValue(Border.PaddingProperty, new TemplateBindingExtension(PaddingProperty));
        var presenter = new FrameworkElementFactory(typeof(ContentPresenter));
        border.AppendChild(presenter);

        var template = new ControlTemplate(typeof(DataGridFilterButton)) { VisualTree = border };
        template.Seal();
        return template;
    }

    private void UpdateFunnel()
    {
        if (IsFilterActive)
        {
            funnel.SetResourceReference(Shape.FillProperty, "AtcApps.Brushes.Accent");
            funnel.SetResourceReference(Shape.StrokeProperty, "AtcApps.Brushes.Accent");
        }
        else
        {
            // The header's (themed) foreground: the button's own Foreground comes from the system button style.
            funnel.ClearValue(Shape.FillProperty);
            funnel.SetBinding(
                Shape.StrokeProperty,
                new Binding(nameof(Control.Foreground))
                {
                    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(DataGridColumnHeader), 1),
                });
        }
    }
}
namespace Atc.Wpf.Theming.Controls;

/// <summary>
/// A content control that draws a line along one side of its content.
/// </summary>
[DefaultStyleKey]
public sealed partial class Underline : ContentControl
{
    /// <summary>
    /// The name of the template part that draws the line.
    /// </summary>
    public const string UnderlineBorderPartName = "PART_UnderlineBorder";

    [TemplatePartField(UnderlineBorderPartName)]
    private Border? underlineBorder;

    /// <summary>
    /// Gets or sets the side of the content where the line is drawn.
    /// </summary>
    [DependencyProperty(DefaultValue = Dock.Left, PropertyChangedCallback = nameof(OnBorderPropertyChanged))]
    private Dock placement;

    /// <summary>
    /// Gets or sets the thickness of the line.
    /// </summary>
    [DependencyProperty(DefaultValue = 1d, PropertyChangedCallback = nameof(OnBorderPropertyChanged))]
    private double lineThickness;

    /// <summary>
    /// Gets or sets the size of the line border across the placement side
    /// (height for top/bottom, width for left/right); <see cref="double.NaN"/> sizes it automatically.
    /// </summary>
    [DependencyProperty(DefaultValue = double.NaN, PropertyChangedCallback = nameof(OnBorderPropertyChanged))]
    private double lineExtent;

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        ResolveTemplateParts();

        ApplyBorderProperties();
    }

    private static void OnBorderPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => (d as Underline)?.ApplyBorderProperties();

    private void ApplyBorderProperties()
    {
        if (underlineBorder is null)
        {
            return;
        }

        void Execute()
        {
            underlineBorder.Height = double.NaN;
            underlineBorder.Width = double.NaN;
            underlineBorder.BorderThickness = new Thickness(0);
            switch (Placement)
            {
                case Dock.Left:
                    underlineBorder.Width = LineExtent;
                    underlineBorder.BorderThickness = new Thickness(LineThickness, 0d, 0d, 0d);
                    break;
                case Dock.Top:
                    underlineBorder.Height = LineExtent;
                    underlineBorder.BorderThickness = new Thickness(0d, LineThickness, 0d, 0d);
                    break;
                case Dock.Right:
                    underlineBorder.Width = LineExtent;
                    underlineBorder.BorderThickness = new Thickness(0d, 0d, LineThickness, 0d);
                    break;
                case Dock.Bottom:
                    underlineBorder.Height = LineExtent;
                    underlineBorder.BorderThickness = new Thickness(0d, 0d, 0d, LineThickness);
                    break;
                default:
                    throw new SwitchCaseDefaultException(Placement);
            }

            InvalidateVisual();
        }

        this.ExecuteWhenLoaded(Execute);
    }
}
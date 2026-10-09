namespace Atc.Wpf.Controls.ColorEditing;

/// <summary>Base class for vertical color sliders that track the mouse with a slider adorner.</summary>
public abstract class SliderBase : UserControl
{
    private readonly SliderPickerAdorner adorner;

    /// <summary>Initializes a new instance of the <see cref="SliderBase"/> class.</summary>
    protected SliderBase()
    {
        adorner = new SliderPickerAdorner(this);
        Loaded += OnLoaded;
    }

    /// <summary>Gets or sets the color shown by the slider adorner.</summary>
    protected Color AdornerColor
    {
        get => adorner.Color;
        set => adorner.Color = value;
    }

    /// <summary>Gets or sets the vertical position of the slider adorner, as a fraction of the control height.</summary>
    protected double AdornerVerticalPercent
    {
        get => adorner.VerticalPercent;
        set => adorner.VerticalPercent = value;
    }

    /// <inheritdoc />
    protected override void OnMouseMove(MouseEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnMouseMove(e);

        if (e.LeftButton != MouseButtonState.Pressed)
        {
            return;
        }

        Mouse.Capture(this);

        var mousePosition = e
            .GetPosition(this)
            .Clamp(this);
        UpdateAdorner(mousePosition);
    }

    /// <inheritdoc />
    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnMouseUp(e);

        Mouse.Capture(element: null);

        var mousePosition = e
            .GetPosition(this)
            .Clamp(this);
        UpdateAdorner(mousePosition);
    }

    /// <summary>Called when the user moves the slider adorner; override to react to the new vertical position (a fraction of the control height).</summary>
    protected virtual void OnAdornerPositionChanged(double verticalPercent)
    {
    }

    private void UpdateAdorner(Point mousePosition)
    {
        var verticalPercent = mousePosition.Y / ActualHeight;
        adorner.VerticalPercent = verticalPercent;
        OnAdornerPositionChanged(verticalPercent);
    }

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        adorner.ElementSize = new Rect(new Size(ActualWidth, ActualHeight));
        var adornerLayer = AdornerLayer.GetAdornerLayer(this);
        adornerLayer?.Add(adorner);
    }
}
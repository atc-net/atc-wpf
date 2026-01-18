namespace Atc.Wpf.Controls.ColorEditing;

public abstract class SliderBase : UserControl
{
    private readonly SliderPickerAdorner adorner;

    protected SliderBase()
    {
        adorner = new SliderPickerAdorner(this);
        Loaded += OnLoaded;
    }

    protected Color AdornerColor
    {
        get => adorner.Color;
        set => adorner.Color = value;
    }

    protected double AdornerVerticalPercent
    {
        get => adorner.VerticalPercent;
        set => adorner.VerticalPercent = value;
    }

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
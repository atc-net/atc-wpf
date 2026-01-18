namespace Atc.Wpf.Controls.ColorEditing;

public partial class HueSlider
{
    [DependencyProperty(
        DefaultValue = 0.0,
        PropertyChangedCallback = nameof(OnHueChanged))]
    private double hue;

    public HueSlider()
    {
        InitializeComponent();
    }

    protected override void OnAdornerPositionChanged(double verticalPercent)
    {
        var color = hueGradients.GradientStops.GetColorAtOffset(verticalPercent);
        AdornerColor = color;
        Hue = color.GetHue();
    }

    private static void OnHueChanged(
        DependencyObject o,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (HueSlider)o;
        control.UpdateAdorner((double)e.NewValue);
    }

    private void UpdateAdorner(double hue)
    {
        var percent = hue / 360;

        // Make it so that the arrow doesn't jump back to the top when it goes to the bottom
        var mousePos = Mouse.GetPosition(this);
        if (percent.IsZero() && ActualHeight - mousePos.Y < 1)
        {
            percent = 1;
        }

        AdornerVerticalPercent = percent;
        AdornerColor = ColorHelper.GetColorFromHsv(hue, 1, 1);
    }
}
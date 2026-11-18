namespace Atc.Wpf.Controls.Layouts;

/// <summary>A content control that draws a grid of horizontal and vertical lines on top of its content.</summary>
public sealed partial class GridLines : ContentControl
{
    private readonly Canvas containerCanvas = new();

    /// <summary>Gets or sets the horizontal spacing between the vertical lines; a value that is not positive draws none.</summary>
    [DependencyProperty(
        DefaultValue = 20d,
        PropertyChangedCallback = nameof(OnReDrawGridLines),
        Category = "Layout",
        Description = "The horizontal step property")]
    private double horizontalStep;

    /// <summary>Gets or sets the vertical spacing between the horizontal lines; a value that is not positive draws none.</summary>
    [DependencyProperty(
        DefaultValue = 20d,
        PropertyChangedCallback = nameof(OnReDrawGridLines),
        Category = "Layout",
        Description = "The vertical step property")]
    private double verticalStep;

    /// <summary>Gets or sets the brush used to draw the grid lines.</summary>
    [DependencyProperty(
        DefaultValueExpression = "Brushes.DeepPink",
        PropertyChangedCallback = nameof(OnReDrawGridLines),
        Category = "Layout",
        Description = "The line brush property")]
    private Brush lineBrush;

    /// <summary>Initializes a new instance of the <see cref="GridLines"/> class.</summary>
    public GridLines()
    {
        IsHitTestVisible = false;
        HorizontalContentAlignment = HorizontalAlignment.Stretch;
        VerticalContentAlignment = VerticalAlignment.Stretch;
        Content = containerCanvas;

        Loaded += OnLoaded;
        SizeChanged += OnGridLinesSizeChanged;
    }

    private static void OnReDrawGridLines(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not GridLines gridLines)
        {
            return;
        }

        gridLines.ReDrawGridLines();
    }

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
        => ReDrawGridLines();

    private void OnGridLinesSizeChanged(
        object sender,
        SizeChangedEventArgs e)
        => ReDrawGridLines();

    private void ReDrawGridLines()
    {
        containerCanvas.Children.Clear();

        // One device pixel on the monitor this control is shown on (no application or main window needed).
        var thickness = 1 / VisualTreeHelper.GetDpi(this).DpiScaleX;

        if (IsValidStep(HorizontalStep))
        {
            for (double x = 0; x < ActualWidth; x += HorizontalStep)
            {
                var line = new System.Windows.Shapes.Rectangle
                {
                    Width = thickness,
                    Height = ActualHeight,
                    Fill = LineBrush,
                };

                Canvas.SetLeft(line, x);

                containerCanvas.Children.Add(line);
            }
        }

        if (IsValidStep(VerticalStep))
        {
            for (double y = 0; y < ActualHeight; y += VerticalStep)
            {
                var line = new System.Windows.Shapes.Rectangle
                {
                    Width = ActualWidth,
                    Height = thickness,
                    Fill = LineBrush,
                };

                Canvas.SetTop(line, y);

                containerCanvas.Children.Add(line);
            }
        }
    }

    /// <summary>A step that is not a positive finite number would never advance the loop, so it draws no lines.</summary>
    private static bool IsValidStep(double step)
        => double.IsFinite(step) && step > 0;
}
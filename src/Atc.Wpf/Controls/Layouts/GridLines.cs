namespace Atc.Wpf.Controls.Layouts;

public sealed partial class GridLines : ContentControl
{
    private readonly Canvas containerCanvas = new();

    [DependencyProperty(
        Category = "Layout",
        Description = "The horizontal step property",
        DefaultValue = 20,
        PropertyChangedCallback = nameof(OnReDrawGridLines))]
    private double horizontalStep;

    [DependencyProperty(
        Category = "Layout",
        Description = "The vertical step property",
        DefaultValue = 20,
        PropertyChangedCallback = nameof(OnReDrawGridLines))]
    private double verticalStep;

    [DependencyProperty(
        Category = "Layout",
        Description = "The line brush property",
        DefaultValue = "DeepPink",
        PropertyChangedCallback = nameof(OnReDrawGridLines))]
    private Brush lineBrush;

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

        var thickness = 1d;

        var currentMainWindow = Application.Current.MainWindow;
        if (currentMainWindow is not null)
        {
            var matrix = PresentationSource.FromVisual(currentMainWindow)!.CompositionTarget!.TransformToDevice;
            var dpiFactor = 1 / matrix.M11;
            thickness = 1 * dpiFactor;
        }

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
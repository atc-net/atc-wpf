namespace Atc.Wpf.Controls.Layouts;

public class GridLines : ContentControl
{
    private readonly Canvas containerCanvas = new();

    public static readonly DependencyProperty HorizontalStepProperty = DependencyProperty.Register(
        nameof(HorizontalStep),
        typeof(double),
        typeof(GridLines),
        new PropertyMetadata(
            20d,
            (sender, _) => ((GridLines)sender).ReDrawGridLines()));

    public double HorizontalStep
    {
        get => (double)GetValue(HorizontalStepProperty);
        set => SetValue(HorizontalStepProperty, value);
    }

    public static readonly DependencyProperty VerticalStepProperty = DependencyProperty.Register(
        nameof(VerticalStep),
        typeof(double),
        typeof(GridLines),
        new PropertyMetadata(
            20d,
            (sender, _) => ((GridLines)sender).ReDrawGridLines()));

    public double VerticalStep
    {
        get => (double)GetValue(VerticalStepProperty);
        set => SetValue(VerticalStepProperty, value);
    }

    public static readonly DependencyProperty LineBrushProperty = DependencyProperty.Register(
        nameof(LineBrush),
        typeof(Brush),
        typeof(GridLines),
        new PropertyMetadata(
            defaultValue: null,
            (sender, _) => ((GridLines)sender).ReDrawGridLines()));

    public Brush LineBrush
    {
        get => (Brush)GetValue(LineBrushProperty);
        set => SetValue(LineBrushProperty, value);
    }

    public GridLines()
    {
        IsHitTestVisible = false;
        HorizontalContentAlignment = HorizontalAlignment.Stretch;
        VerticalContentAlignment = VerticalAlignment.Stretch;
        Content = containerCanvas;

        Loaded += OnLoaded;
        SizeChanged += OnGridLinesSizeChanged;
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
            var matrix = PresentationSource.FromVisual(currentMainWindow)!.CompositionTarget.TransformToDevice;
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
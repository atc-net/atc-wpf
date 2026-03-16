namespace Atc.Wpf.Controls.Zoom;

public partial class ZoomScrollViewer : ScrollViewer
{
    public static readonly DependencyProperty ZoomContentProperty = DependencyProperty.Register(
        nameof(ZoomContent),
        typeof(ZoomBox),
        typeof(ZoomScrollViewer),
        new FrameworkPropertyMetadata(propertyChangedCallback: null));

    /// <summary>
    /// Gets or sets the ZoomBox content control.
    /// </summary>
    public ZoomBox? ZoomContent
    {
        get => (ZoomBox)GetValue(ZoomContentProperty);
        set => SetValue(ZoomContentProperty, value);
    }

    [DependencyProperty(DefaultValue = ZoomMinimumType.MinimumZoom)]
    private ZoomMinimumType minimumZoomType;

    [DependencyProperty]
    private Point? mousePosition;

    public static readonly DependencyProperty UseAnimationsProperty = DependencyProperty.Register(
        nameof(UseAnimations),
        typeof(bool),
        typeof(ZoomScrollViewer),
        new FrameworkPropertyMetadata(BooleanBoxes.TrueBox));

    /// <summary>
    /// Gets or sets a value indicating whether animations are enabled.
    /// </summary>
    public bool UseAnimations
    {
        get => (bool)GetValue(UseAnimationsProperty);
        set => SetValue(UseAnimationsProperty, BooleanBoxes.Box(value));
    }

    public static readonly DependencyProperty ViewportZoomProperty = DependencyProperty.Register(
        nameof(ViewportZoom),
        typeof(double),
        typeof(ZoomScrollViewer),
        new FrameworkPropertyMetadata(
            1.0,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    /// <summary>
    /// Gets or sets the current zoom level.
    /// </summary>
    public double ViewportZoom
    {
        get => (double)GetValue(ViewportZoomProperty);
        set => SetValue(ViewportZoomProperty, value);
    }

    [DependencyProperty(DefaultValue = ZoomInitialPositionType.Default)]
    private ZoomInitialPositionType zoomInitialPosition;

    static ZoomScrollViewer()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(ZoomScrollViewer),
            new FrameworkPropertyMetadata(typeof(ZoomScrollViewer)));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        ZoomContent = Template.FindName("PART_ZoomControl", this) as ZoomBox;
        OnPropertyChanged(new DependencyPropertyChangedEventArgs(ZoomContentProperty, oldValue: null, ZoomContent));
        RefreshProperties();
    }

    /// <summary>
    /// Command to implement the zoom to fill.
    /// </summary>
    public ICommand FillCommand => new RelayCommand(
        () => ZoomContent?.ZoomFillCommand.Execute(parameter: null),
        () => ZoomContent?.ZoomFillCommand.CanExecute(parameter: null) ?? true);

    /// <summary>
    /// Command to implement the zoom to fit.
    /// </summary>
    public ICommand FitCommand => new RelayCommand(
            () => ZoomContent?.ZoomFitCommand.Execute(parameter: null),
            () => ZoomContent?.ZoomFitCommand.CanExecute(parameter: null) ?? true);

    /// <summary>
    /// Command to implement the zoom to a percentage.
    /// </summary>
    public ICommand ZoomPercentCommand => new RelayCommand<double>(
            value => ZoomContent?.ZoomPercentCommand.Execute(value),
            value => ZoomContent?.ZoomPercentCommand.CanExecute(value) ?? true);

    /// <summary>
    /// Command to implement the zoom ratio from minimum.
    /// </summary>
    public ICommand ZoomRatioFromMinimumCommand => new RelayCommand<double>(
            value => ZoomContent?.ZoomRatioFromMinimumCommand.Execute(value),
            value => ZoomContent?.ZoomRatioFromMinimumCommand.CanExecute(value) ?? true);

    /// <summary>
    /// Command to implement the zoom out.
    /// </summary>
    public ICommand ZoomOutCommand => new RelayCommand(
            () => ZoomContent?.ZoomOutCommand.Execute(parameter: null),
            () => ZoomContent?.ZoomOutCommand.CanExecute(parameter: null) ?? true);

    /// <summary>
    /// Command to implement the zoom in.
    /// </summary>
    public ICommand ZoomInCommand => new RelayCommand(
            () => ZoomContent?.ZoomInCommand.Execute(parameter: null),
            () => ZoomContent?.ZoomInCommand.CanExecute(parameter: null) ?? true);

    /// <summary>
    /// Command to implement Undo.
    /// </summary>
    public ICommand UndoZoomCommand => new RelayCommand(
            () => ZoomContent?.UndoZoomCommand.Execute(parameter: null),
            () => ZoomContent?.UndoZoomCommand.CanExecute(parameter: null) ?? true);

    /// <summary>
    /// Command to implement Redo.
    /// </summary>
    public ICommand RedoZoomCommand => new RelayCommand(
            () => ZoomContent?.RedoZoomCommand.Execute(parameter: null),
            () => ZoomContent?.RedoZoomCommand.CanExecute(parameter: null) ?? true);

    private void RefreshProperties()
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FillCommand)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FitCommand)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ZoomPercentCommand)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ZoomRatioFromMinimumCommand)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ZoomInCommand)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ZoomOutCommand)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UndoZoomCommand)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RedoZoomCommand)));
    }
}
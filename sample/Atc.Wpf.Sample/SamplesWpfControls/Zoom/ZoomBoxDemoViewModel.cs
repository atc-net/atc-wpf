namespace Atc.Wpf.Sample.SamplesWpfControls.Zoom;

public partial class ZoomBoxDemoViewModel : ViewModelBase
{
    private ZoomScrollViewer? zoomScrollViewer;

    private double viewportZoom = 1.0;

    [PropertyDisplay("Viewport Zoom", "Zoom", 1)]
    [PropertyRange(0.1, 5.0, 0.1)]
    [PropertyEditorHint(EditorHint.Slider)]
    public double ViewportZoom
    {
        get => viewportZoom;
        set
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (viewportZoom == value)
            {
                return;
            }

            viewportZoom = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ZoomPercent));
        }
    }

    [PropertyDisplay("Zoom %", "Zoom", 2)]
    [PropertyRange(10, 500, 1)]
    public int ZoomPercent
    {
        get => (int)System.Math.Round(ViewportZoom * 100);
        set => ViewportZoom = value / 100.0;
    }

    [PropertyDisplay("Zoom to Fit", "Commands", 1)]
    public ICommand FitCommand => new RelayCommand(
        () => zoomScrollViewer?.FitCommand.Execute(parameter: null),
        () => zoomScrollViewer?.FitCommand.CanExecute(parameter: null) ?? false);

    [PropertyDisplay("Zoom to Fill", "Commands", 2)]
    public ICommand FillCommand => new RelayCommand(
        () => zoomScrollViewer?.FillCommand.Execute(parameter: null),
        () => zoomScrollViewer?.FillCommand.CanExecute(parameter: null) ?? false);

    [PropertyDisplay("Zoom to 100%", "Commands", 3)]
    public ICommand ZoomPercentCommand => new RelayCommand(
        () => zoomScrollViewer?.ZoomPercentCommand.Execute(100.0),
        () => zoomScrollViewer?.ZoomPercentCommand.CanExecute(100.0) ?? false);

    [PropertyDisplay("Zoom In", "Commands", 4)]
    public ICommand ZoomInCommand => new RelayCommand(
        () => zoomScrollViewer?.ZoomInCommand.Execute(parameter: null),
        () => zoomScrollViewer?.ZoomInCommand.CanExecute(parameter: null) ?? false);

    [PropertyDisplay("Zoom Out", "Commands", 5)]
    public ICommand ZoomOutCommand => new RelayCommand(
        () => zoomScrollViewer?.ZoomOutCommand.Execute(parameter: null),
        () => zoomScrollViewer?.ZoomOutCommand.CanExecute(parameter: null) ?? false);

    [PropertyDisplay("Undo Zoom", "Commands", 6)]
    public ICommand UndoZoomCommand => new RelayCommand(
        () => zoomScrollViewer?.UndoZoomCommand.Execute(parameter: null),
        () => zoomScrollViewer?.UndoZoomCommand.CanExecute(parameter: null) ?? false);

    [PropertyDisplay("Redo Zoom", "Commands", 7)]
    public ICommand RedoZoomCommand => new RelayCommand(
        () => zoomScrollViewer?.RedoZoomCommand.Execute(parameter: null),
        () => zoomScrollViewer?.RedoZoomCommand.CanExecute(parameter: null) ?? false);

    [PropertyDisplay("Use Animations", "Behavior", 1)]
    [ObservableProperty]
    private bool useAnimations = true;

    [PropertyDisplay("Initial Position", "Behavior", 2)]
    [ObservableProperty]
    private ZoomInitialPositionType zoomInitialPosition = ZoomInitialPositionType.FitScreen;

    [PropertyDisplay("Minimum Zoom Type", "Zoom Limits", 1)]
    [ObservableProperty]
    private ZoomMinimumType minimumZoomType = ZoomMinimumType.MinimumZoom;

    public void SetZoomScrollViewer(ZoomScrollViewer viewer)
    {
        zoomScrollViewer = viewer;
    }
}
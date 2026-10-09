namespace Atc.Wpf.Sample.SamplesWpfControls.Inputs;

public partial class PixelSizeBoxView
{
    public PixelSizeBoxView()
    {
        InitializeComponent();
        DataContext = new PixelSizeBoxDemoViewModel();

        InteractiveBox.ValueWidthLostFocus += LostFocusEventLog.Writer<int?>(LostFocusLog, nameof(InteractiveBox.ValueWidthLostFocus));
        InteractiveBox.ValueHeightLostFocus += LostFocusEventLog.Writer<int?>(LostFocusLog, nameof(InteractiveBox.ValueHeightLostFocus));
    }
}
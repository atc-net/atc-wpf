namespace Atc.Wpf.Sample.SamplesWpfForms.LabelControls;

public partial class LabelPixelSizeBoxView
{
    public LabelPixelSizeBoxView()
    {
        InitializeComponent();
        DataContext = new LabelControlDemoViewModel();

        InteractiveBox.ValueWidthLostFocus += LostFocusEventLog.Writer<int?>(LostFocusLog, nameof(InteractiveBox.ValueWidthLostFocus));
        InteractiveBox.ValueHeightLostFocus += LostFocusEventLog.Writer<int?>(LostFocusLog, nameof(InteractiveBox.ValueHeightLostFocus));
    }
}
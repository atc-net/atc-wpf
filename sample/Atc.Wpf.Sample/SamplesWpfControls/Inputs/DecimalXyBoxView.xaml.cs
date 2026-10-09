namespace Atc.Wpf.Sample.SamplesWpfControls.Inputs;

public partial class DecimalXyBoxView
{
    public DecimalXyBoxView()
    {
        InitializeComponent();
        DataContext = new DecimalXyBoxDemoViewModel();

        InteractiveBox.ValueXLostFocus += LostFocusEventLog.Writer<decimal?>(LostFocusLog, nameof(InteractiveBox.ValueXLostFocus));
        InteractiveBox.ValueYLostFocus += LostFocusEventLog.Writer<decimal?>(LostFocusLog, nameof(InteractiveBox.ValueYLostFocus));
    }
}
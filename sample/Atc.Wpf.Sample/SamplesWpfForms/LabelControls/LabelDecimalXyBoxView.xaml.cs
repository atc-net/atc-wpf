namespace Atc.Wpf.Sample.SamplesWpfForms.LabelControls;

public partial class LabelDecimalXyBoxView
{
    public LabelDecimalXyBoxView()
    {
        InitializeComponent();

        DataContext = new LabelControlDemoViewModel();

        InteractiveBox.ValueXLostFocus += LostFocusEventLog.Writer<decimal?>(LostFocusLog, nameof(InteractiveBox.ValueXLostFocus));
        InteractiveBox.ValueYLostFocus += LostFocusEventLog.Writer<decimal?>(LostFocusLog, nameof(InteractiveBox.ValueYLostFocus));
    }
}
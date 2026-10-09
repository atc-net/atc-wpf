namespace Atc.Wpf.Sample.SamplesWpfForms.LabelControls;

public partial class LabelIntegerXyBoxView
{
    public LabelIntegerXyBoxView()
    {
        InitializeComponent();
        DataContext = new LabelControlDemoViewModel();

        InteractiveBox.ValueXLostFocus += LostFocusEventLog.Writer<int?>(LostFocusLog, nameof(InteractiveBox.ValueXLostFocus));
        InteractiveBox.ValueYLostFocus += LostFocusEventLog.Writer<int?>(LostFocusLog, nameof(InteractiveBox.ValueYLostFocus));
    }
}
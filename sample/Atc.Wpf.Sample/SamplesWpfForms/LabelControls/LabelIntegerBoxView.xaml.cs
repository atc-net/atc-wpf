namespace Atc.Wpf.Sample.SamplesWpfForms.LabelControls;

public partial class LabelIntegerBoxView
{
    public LabelIntegerBoxView()
    {
        InitializeComponent();
        DataContext = new LabelControlDemoViewModel();

        InteractiveBox.ValueLostFocus += LostFocusEventLog.Writer<int?>(LostFocusLog, nameof(InteractiveBox.ValueLostFocus));
    }
}
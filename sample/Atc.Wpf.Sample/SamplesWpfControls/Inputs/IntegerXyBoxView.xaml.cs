namespace Atc.Wpf.Sample.SamplesWpfControls.Inputs;

public partial class IntegerXyBoxView
{
    public IntegerXyBoxView()
    {
        InitializeComponent();
        DataContext = new IntegerXyBoxDemoViewModel();

        InteractiveBox.ValueXLostFocus += LostFocusEventLog.Writer<int?>(LostFocusLog, nameof(InteractiveBox.ValueXLostFocus));
        InteractiveBox.ValueYLostFocus += LostFocusEventLog.Writer<int?>(LostFocusLog, nameof(InteractiveBox.ValueYLostFocus));
    }
}
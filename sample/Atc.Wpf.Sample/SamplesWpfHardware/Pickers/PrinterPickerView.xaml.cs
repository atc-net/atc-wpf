namespace Atc.Wpf.Sample.SamplesWpfHardware.Pickers;

public partial class PrinterPickerView
{
    public PrinterPickerView()
    {
        InitializeComponent();
        DataContext = new PrinterPickerDemoViewModel();
    }
}
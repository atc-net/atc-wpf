namespace Atc.Wpf.Sample.SamplesWpfControls.Inputs;

public partial class FontPickerView
{
    public FontPickerView()
    {
        InitializeComponent();

        DataContext = new FontPickerDemoViewModel();
    }
}
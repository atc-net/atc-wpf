namespace Atc.Wpf.Sample.SamplesWpfControls.ColorEditing;

public partial class AdvancedColorPickerView
{
    public AdvancedColorPickerView()
    {
        InitializeComponent();
        DataContext = new AdvancedColorPickerDemoViewModel();
    }
}
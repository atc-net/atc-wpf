namespace Atc.Wpf.Sample.SamplesWpfControls.FontEditing;

public partial class AdvancedFontPickerView
{
    public AdvancedFontPickerView()
    {
        InitializeComponent();

        DataContext = new AdvancedFontPickerDemoViewModel();
    }
}
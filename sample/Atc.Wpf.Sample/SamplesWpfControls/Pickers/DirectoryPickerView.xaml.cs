namespace Atc.Wpf.Sample.SamplesWpfControls.Pickers;

public partial class DirectoryPickerView
{
    public DirectoryPickerView()
    {
        InitializeComponent();
        DataContext = new PickerDemoViewModel();
    }
}
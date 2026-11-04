namespace Atc.Wpf.Sample.SamplesWpfComponents.DialogBoxes;

public partial class LongTextDialogBoxView
{
    public LongTextDialogBoxView()
    {
        InitializeComponent();
        DataContext = new LongTextDialogBoxViewModel();
    }
}
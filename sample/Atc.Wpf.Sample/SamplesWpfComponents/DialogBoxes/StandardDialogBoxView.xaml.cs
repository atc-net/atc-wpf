namespace Atc.Wpf.Sample.SamplesWpfComponents.DialogBoxes;

public partial class StandardDialogBoxView
{
    public StandardDialogBoxView()
    {
        InitializeComponent();
        DataContext = new StandardDialogBoxViewModel();
    }
}

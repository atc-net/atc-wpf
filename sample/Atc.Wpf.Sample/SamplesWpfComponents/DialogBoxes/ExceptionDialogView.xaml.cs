namespace Atc.Wpf.Sample.SamplesWpfComponents.DialogBoxes;

public partial class ExceptionDialogView
{
    public ExceptionDialogView()
    {
        InitializeComponent();
        DataContext = new ExceptionDialogViewModel();
    }
}
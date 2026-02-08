namespace Atc.Wpf.Sample.SamplesWpfControls.DragDrop;

public partial class DragDropView
{
    public DragDropView()
    {
        InitializeComponent();
        DataContext = new DragDropViewModel();
    }
}
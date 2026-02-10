namespace Atc.Wpf.Sample.SamplesWpf.DragDrop;

public partial class DragDropView
{
    public DragDropView()
    {
        InitializeComponent();
        DataContext = new DragDropViewModel();
    }
}
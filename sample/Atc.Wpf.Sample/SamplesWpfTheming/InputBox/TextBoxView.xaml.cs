namespace Atc.Wpf.Sample.SamplesWpfTheming.InputBox;

public partial class TextBoxView
{
    public TextBoxView()
    {
        InitializeComponent();

        DataContext = this;
    }

    public ICommand? ControlButtonCommand { get; }
}
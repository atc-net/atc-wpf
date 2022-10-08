namespace Atc.Wpf.Theming.Sample.Samples.InputBox;

/// <summary>
/// Interaction logic for TextBoxView.
/// </summary>
public partial class TextBoxView
{
    public TextBoxView()
    {
        InitializeComponent();

        DataContext = this;
    }

    public ICommand ControlButtonCommand { get; }
}
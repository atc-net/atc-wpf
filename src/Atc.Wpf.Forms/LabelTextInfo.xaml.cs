namespace Atc.Wpf.Forms;

public partial class LabelTextInfo : ILabelTextInfo
{
    [DependencyProperty(DefaultValue = "")]
    private string text;

    public LabelTextInfo()
    {
        InitializeComponent();
    }
}
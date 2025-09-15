namespace Atc.Wpf.Controls.LabelControls;

public partial class LabelTextInfo : ILabelTextInfo
{
    [DependencyProperty(DefaultValue = "")]
    private string text;

    public LabelTextInfo()
    {
        InitializeComponent();
    }
}
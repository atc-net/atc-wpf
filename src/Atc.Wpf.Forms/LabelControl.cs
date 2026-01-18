namespace Atc.Wpf.Forms;

public partial class LabelControl : LabelControlBase, ILabelControl
{
    [DependencyProperty(DefaultValue = true)]
    private bool showAsteriskOnMandatory;

    [DependencyProperty(DefaultValue = false)]
    private bool isMandatory;

    [DependencyProperty(DefaultValue = nameof(Colors.Red))]
    private SolidColorBrush mandatoryColor;

    [DependencyProperty(DefaultValue = "Application.Current?.Resources[\"AtcApps.Brushes.Control.Validation\"] as SolidColorBrush ?? new SolidColorBrush(Colors.Red)")]
    private SolidColorBrush validationColor;

    [DependencyProperty(DefaultValue = "")]
    private string validationText;

    public virtual bool IsValid() => true;
}
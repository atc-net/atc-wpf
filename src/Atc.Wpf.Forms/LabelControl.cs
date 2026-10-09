namespace Atc.Wpf.Forms;

/// <summary>
/// Base class for labeled form controls that add mandatory indication and validation display.
/// </summary>
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

    /// <summary>
    /// Validates the control's current value and returns whether it is valid.
    /// </summary>
    /// <returns><see langword="true"/> if the value is valid; otherwise, <see langword="false"/>. The base implementation always returns <see langword="true"/>.</returns>
    public virtual bool IsValid() => true;
}
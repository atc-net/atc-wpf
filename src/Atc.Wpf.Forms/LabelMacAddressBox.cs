namespace Atc.Wpf.Forms;

/// <summary>
/// A <see cref="LabelTextBox"/> for a MAC address, such as <c>00-11-22-33-44-55</c>.
/// </summary>
public class LabelMacAddressBox : LabelTextBox
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LabelMacAddressBox"/> class.
    /// </summary>
    public LabelMacAddressBox()
        => SetCurrentValue(ValidationFormatProperty, TextBoxValidationRuleType.MacAddress);
}
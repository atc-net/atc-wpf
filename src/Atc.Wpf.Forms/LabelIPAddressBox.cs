namespace Atc.Wpf.Forms;

/// <summary>
/// A <see cref="LabelTextBox"/> for an IPv4 or IPv6 address.
/// </summary>
public class LabelIPAddressBox : LabelTextBox
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LabelIPAddressBox"/> class.
    /// </summary>
    public LabelIPAddressBox()
        => SetCurrentValue(ValidationFormatProperty, TextBoxValidationRuleType.IPAddress);
}
namespace Atc.Wpf.Forms;

/// <summary>
/// A <see cref="LabelTextBox"/> for an IPv4 range in CIDR notation, such as <c>192.168.1.0/24</c>.
/// </summary>
public class LabelIPRangeBox : LabelTextBox
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LabelIPRangeBox"/> class.
    /// </summary>
    public LabelIPRangeBox()
        => SetCurrentValue(ValidationFormatProperty, TextBoxValidationRuleType.IPv4Cidr);
}
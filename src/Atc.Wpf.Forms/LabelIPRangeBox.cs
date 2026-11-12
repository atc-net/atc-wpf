namespace Atc.Wpf.Forms;

/// <summary>
/// A <see cref="LabelTextBox"/> for one or more IPv4 ranges in CIDR notation, such as <c>192.168.1.0/24</c> or <c>192.168.1.0/24, 10.0.0.0/24</c>.
/// </summary>
public class LabelIPRangeBox : LabelTextBox
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LabelIPRangeBox"/> class.
    /// </summary>
    public LabelIPRangeBox()
        => SetCurrentValue(ValidationFormatProperty, TextBoxValidationRuleType.IPv4Cidr);
}
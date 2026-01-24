namespace Atc.Wpf.Forms;

public partial class LabelDecimalNumberControl : LabelNumberControl, ILabelDecimalNumberControl
{
    [DependencyProperty(
        DefaultValue = 2,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)]
    private int decimalPlaces;

    [DependencyProperty(
        DefaultValue = PropertyDefaultValueConstants.MinValue,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal)]
    private decimal minimum;

    [DependencyProperty(
        DefaultValue = PropertyDefaultValueConstants.MaxValue,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal)]
    private decimal maximum;
}
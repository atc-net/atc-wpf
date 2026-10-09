namespace Atc.Wpf.Forms;

/// <summary>
/// Base class for labeled integer input controls, adding the minimum and maximum values.
/// </summary>
public partial class LabelIntegerNumberControl : LabelNumberControl, ILabelIntegerNumberControl
{
    [DependencyProperty(
        DefaultValue = PropertyDefaultValueConstants.MinValue,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal)]
    private int minimum;

    [DependencyProperty(
        DefaultValue = PropertyDefaultValueConstants.MaxValue,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal)]
    private int maximum;
}
namespace Atc.Wpf.Controls.LabelControls;

public class LabelDecimalNumberControl : LabelNumberControl, ILabelDecimalNumberControl
{
    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
        nameof(Maximum),
        typeof(decimal),
        typeof(LabelDecimalNumberControl),
        new FrameworkPropertyMetadata(
            defaultValue: decimal.MaxValue,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));

    public decimal Maximum
    {
        get => (decimal)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
        nameof(Minimum),
        typeof(decimal),
        typeof(LabelDecimalNumberControl),
        new FrameworkPropertyMetadata(
            defaultValue: decimal.MinValue,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));

    public decimal Minimum
    {
        get => (decimal)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }
}
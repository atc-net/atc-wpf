// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault
namespace Atc.Wpf.Controls.LabelControls.Internal.ValueConverters;

public class LabelControlHideLeftRightAreaMultiValueConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
        {
            return "10,*,20";
        }

        ArgumentNullException.ThrowIfNull(values);

        if (values.Length != 2)
        {
            throw new ConstraintException("Expected two values of type(bool)");
        }

        var widthLeft = 0;
        if (values[0].ToString()!.IsFalse())
        {
            widthLeft = 10;
        }

        var widthRight = 0;
        if (values[1].ToString()!.IsFalse())
        {
            widthRight = 20;
        }

        return $"{widthLeft},*,{widthRight}";
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException("This is a OneWay converter.");
    }
}
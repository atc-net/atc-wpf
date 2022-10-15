//namespace Atc.Wpf.Controls.LabelControls.Internal.ValueConverters;

//internal class LabelControlHideAreasToBoolValueConverter : IValueConverter
//{
//    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
//    {
//        if (value is not LabelControlHideAreasType currentHideAreasType)
//        {
//            throw new InvalidEnumArgumentException(nameof(value), 0, typeof(LabelControlHideAreasType));
//        }

//        if (parameter is not LabelControlHideAreasType requiredHideAreasType)
//        {
//            throw new InvalidEnumArgumentException(nameof(value), 0, typeof(LabelControlHideAreasType));
//        }

//        return currentHideAreasType == LabelControlHideAreasType.None ||
//               currentHideAreasType.HasFlag(requiredHideAreasType);
//    }

//    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
//    {
//        throw new NotSupportedException("This is a OneWay converter.");
//    }
//}
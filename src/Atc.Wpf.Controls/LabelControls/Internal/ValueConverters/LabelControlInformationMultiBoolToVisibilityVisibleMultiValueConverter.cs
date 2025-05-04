namespace Atc.Wpf.Controls.LabelControls.Internal.ValueConverters;

internal class LabelControlInformationMultiBoolToVisibilityVisibleMultiValueConverter : IMultiValueConverter
{
    public static readonly LabelControlInformationMultiBoolToVisibilityVisibleMultiValueConverter Instance = new();

    /// <inheritdoc />
    public object Convert(
        object[] values,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(values);

        if (values is not { Length: 3 })
        {
            return Visibility.Collapsed;
        }

        var hideAreasIsInformation = values[0] is bool && (bool)values[0];
        var informationTextIsNotEmpty = values[1] is bool && (bool)values[1];
        var informationContentIsNotNull = values[2] is bool && (bool)values[2];

        return hideAreasIsInformation &&
               (informationTextIsNotEmpty || informationContentIsNotNull)
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    /// <inheritdoc />
    public object[] ConvertBack(
        object value,
        Type[] targetTypes,
        object parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}
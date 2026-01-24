namespace Atc.Wpf.Controls.Inputs;

public sealed partial class DecimalBox : NumericBox
{
    private bool isUpdatingDecimalPlaces;

    [DependencyProperty(
        DefaultValue = 2,
        PropertyChangedCallback = nameof(OnDecimalPlacesPropertyChanged),
        CoerceValueCallback = nameof(CoerceDecimalPlaces))]
    private int decimalPlaces;

    public DecimalBox()
    {
        Loaded += OnLoaded;
    }

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        SyncStringFormatWithDecimalPlaces();
    }

    private void SyncStringFormatWithDecimalPlaces()
    {
        var expectedFormat = $"N{DecimalPlaces}";
        if (string.Equals(StringFormat, expectedFormat, StringComparison.Ordinal))
        {
            return;
        }

        SetCurrentValue(StringFormatProperty, expectedFormat);
    }

    private static void OnDecimalPlacesPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not DecimalBox decimalBox ||
            decimalBox.isUpdatingDecimalPlaces)
        {
            return;
        }

        decimalBox.isUpdatingDecimalPlaces = true;

        try
        {
            decimalBox.SetCurrentValue(StringFormatProperty, $"N{(int)e.NewValue}");
        }
        finally
        {
            decimalBox.isUpdatingDecimalPlaces = false;
        }
    }

    private static object CoerceDecimalPlaces(
        DependencyObject d,
        object value)
    {
        var places = (int)value;
        return places < 0
            ? 0
            : places > 15
                ? 15
                : places;
    }
}
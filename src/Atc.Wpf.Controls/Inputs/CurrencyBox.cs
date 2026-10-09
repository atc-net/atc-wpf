namespace Atc.Wpf.Controls.Inputs;

/// <summary>A locale-aware currency input box with currency formatting and a configurable number of decimal places.</summary>
public sealed partial class CurrencyBox : NumericBox
{
    private bool isUpdatingDecimalPlaces;

    [DependencyProperty(
        DefaultValue = 2,
        PropertyChangedCallback = nameof(OnDecimalPlacesPropertyChanged),
        CoerceValueCallback = nameof(CoerceDecimalPlaces))]
    private int decimalPlaces;

    /// <summary>Initializes a new instance of the <see cref="CurrencyBox"/> class.</summary>
    public CurrencyBox()
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
        var expectedFormat = $"C{DecimalPlaces}";
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
        if (d is not CurrencyBox currencyBox ||
            currencyBox.isUpdatingDecimalPlaces)
        {
            return;
        }

        currencyBox.isUpdatingDecimalPlaces = true;

        try
        {
            currencyBox.SetCurrentValue(StringFormatProperty, $"C{(int)e.NewValue}");
        }
        finally
        {
            currencyBox.isUpdatingDecimalPlaces = false;
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
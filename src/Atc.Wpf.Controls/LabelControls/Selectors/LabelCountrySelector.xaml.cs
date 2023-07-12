// ReSharper disable CheckNamespace
namespace Atc.Wpf.Controls.LabelControls;

/// <summary>
/// Interaction logic for LabelCountrySelector.
/// </summary>
public partial class LabelCountrySelector : ILabelCountrySelector
{
    public static readonly DependencyProperty DropDownFirstItemTypeProperty = DependencyProperty.Register(
        nameof(DropDownFirstItemType),
        typeof(DropDownFirstItemType),
        typeof(LabelCountrySelector),
        new PropertyMetadata(DropDownFirstItemType.None));

    public DropDownFirstItemType DropDownFirstItemType
    {
        get => (DropDownFirstItemType)GetValue(DropDownFirstItemTypeProperty);
        set => SetValue(DropDownFirstItemTypeProperty, value);
    }

    public static readonly DependencyProperty RenderFlagIndicatorTypeTypeProperty = DependencyProperty.Register(
        nameof(RenderFlagIndicatorType),
        typeof(RenderFlagIndicatorType),
        typeof(LabelCountrySelector),
        new PropertyMetadata(RenderFlagIndicatorType.Flat16));

    public RenderFlagIndicatorType RenderFlagIndicatorType
    {
        get => (RenderFlagIndicatorType)GetValue(RenderFlagIndicatorTypeTypeProperty);
        set => SetValue(RenderFlagIndicatorTypeTypeProperty, value);
    }

    public static readonly DependencyProperty UseOnlySupportedCountriesProperty = DependencyProperty.Register(
        nameof(UseOnlySupportedCountries),
        typeof(bool),
        typeof(LabelCountrySelector),
        new PropertyMetadata(defaultValue: true));

    public bool UseOnlySupportedCountries
    {
        get => (bool)GetValue(UseOnlySupportedCountriesProperty);
        set => SetValue(UseOnlySupportedCountriesProperty, value);
    }

    public static readonly DependencyProperty DefaultCultureLcidProperty = DependencyProperty.Register(
        nameof(DefaultCultureLcid),
        typeof(int?),
        typeof(LabelCountrySelector),
        new PropertyMetadata(default(int?)));

    public static readonly DependencyProperty SelectedKeyProperty = DependencyProperty.Register(
        nameof(SelectedKey),
        typeof(string),
        typeof(LabelCountrySelector),
        new PropertyMetadata(string.Empty));

    public string SelectedKey
    {
        get => (string)GetValue(SelectedKeyProperty);
        set => SetValue(SelectedKeyProperty, value);
    }

    public int? DefaultCultureLcid
    {
        get => (int?)GetValue(DefaultCultureLcidProperty);
        set => SetValue(DefaultCultureLcidProperty, value);
    }

    public LabelCountrySelector()
    {
        InitializeComponent();
    }
}
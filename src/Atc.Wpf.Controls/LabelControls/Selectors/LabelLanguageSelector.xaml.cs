// ReSharper disable CheckNamespace
namespace Atc.Wpf.Controls.LabelControls;

/// <summary>
/// Interaction logic for LabelLanguageSelector.
/// </summary>
public partial class LabelLanguageSelector : ILabelLanguageSelector
{
    public static readonly DependencyProperty DropDownFirstItemTypeProperty = DependencyProperty.Register(
        nameof(DropDownFirstItemType),
        typeof(DropDownFirstItemType),
        typeof(LabelLanguageSelector),
        new PropertyMetadata(DropDownFirstItemType.None));

    public DropDownFirstItemType DropDownFirstItemType
    {
        get => (DropDownFirstItemType)GetValue(DropDownFirstItemTypeProperty);
        set => SetValue(DropDownFirstItemTypeProperty, value);
    }

    public static readonly DependencyProperty RenderFlagIndicatorTypeTypeProperty = DependencyProperty.Register(
        nameof(RenderFlagIndicatorType),
        typeof(RenderFlagIndicatorType),
        typeof(LabelLanguageSelector),
        new PropertyMetadata(RenderFlagIndicatorType.Flat16));

    public RenderFlagIndicatorType RenderFlagIndicatorType
    {
        get => (RenderFlagIndicatorType)GetValue(RenderFlagIndicatorTypeTypeProperty);
        set => SetValue(RenderFlagIndicatorTypeTypeProperty, value);
    }

    public static readonly DependencyProperty UseOnlySupportedLanguagesProperty = DependencyProperty.Register(
        nameof(UseOnlySupportedLanguages),
        typeof(bool),
        typeof(LabelLanguageSelector),
        new PropertyMetadata(defaultValue: true));

    public bool UseOnlySupportedLanguages
    {
        get => (bool)GetValue(UseOnlySupportedLanguagesProperty);
        set => SetValue(UseOnlySupportedLanguagesProperty, value);
    }

    public static readonly DependencyProperty DefaultCultureIdentifierProperty = DependencyProperty.Register(
        nameof(DefaultCultureIdentifier),
        typeof(string),
        typeof(LabelLanguageSelector),
        new PropertyMetadata(default));

    public string? DefaultCultureIdentifier
    {
        get => (string?)GetValue(DefaultCultureIdentifierProperty);
        set => SetValue(DefaultCultureIdentifierProperty, value);
    }

    public static readonly DependencyProperty SelectedKeyProperty = DependencyProperty.Register(
        nameof(SelectedKey),
        typeof(string),
        typeof(LabelLanguageSelector),
        new PropertyMetadata(string.Empty));

    public string SelectedKey
    {
        get => (string)GetValue(SelectedKeyProperty);
        set => SetValue(SelectedKeyProperty, value);
    }

    public static readonly DependencyProperty UpdateUiCultureOnChangeEventProperty = DependencyProperty.Register(
        nameof(UpdateUiCultureOnChangeEvent),
        typeof(bool),
        typeof(LabelLanguageSelector),
        new PropertyMetadata(defaultValue: true));

    public bool UpdateUiCultureOnChangeEvent
    {
        get => (bool)GetValue(UpdateUiCultureOnChangeEventProperty);
        set => SetValue(UpdateUiCultureOnChangeEventProperty, value);
    }

    public LabelLanguageSelector()
    {
        InitializeComponent();
    }
}
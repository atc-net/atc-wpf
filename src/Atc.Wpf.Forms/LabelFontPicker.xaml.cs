namespace Atc.Wpf.Forms;

public partial class LabelFontPicker : ILabelFontPicker
{
    [DependencyProperty(DefaultValue = "Segoe UI")]
    private FontFamily? selectedFontFamily;

    [DependencyProperty(DefaultValue = FontDescription.DefaultSize)]
    private double selectedFontSize;

    [DependencyProperty]
    private FontWeight selectedFontWeight;

    [DependencyProperty]
    private FontStyle selectedFontStyle;

    [DependencyProperty]
    private FontStretch selectedFontStretch;

    [DependencyProperty(DefaultValue = true)]
    private bool showFontFamily;

    [DependencyProperty(DefaultValue = true)]
    private bool showFontSize;

    [DependencyProperty(DefaultValue = true)]
    private bool showFontWeight;

    [DependencyProperty(DefaultValue = true)]
    private bool showFontStyle;

    [DependencyProperty(DefaultValue = true)]
    private bool showFontStretch;

    [DependencyProperty(DefaultValue = true)]
    private bool showForegroundColor;

    [DependencyProperty(DefaultValue = true)]
    private bool showBackgroundColor;

    [DependencyProperty(DefaultValue = true)]
    private bool showTextDecorations;

    [DependencyProperty(DefaultValue = true)]
    private bool showQuickToggles;

    [DependencyProperty(DefaultValue = true)]
    private bool showPreview;

    [DependencyProperty(DefaultValue = true)]
    private bool isFontFamilyEnabled;

    [DependencyProperty(DefaultValue = true)]
    private bool isFontSizeEnabled;

    [DependencyProperty(DefaultValue = true)]
    private bool isFontWeightEnabled;

    [DependencyProperty(DefaultValue = true)]
    private bool isFontStyleEnabled;

    [DependencyProperty(DefaultValue = true)]
    private bool isFontStretchEnabled;

    [DependencyProperty(DefaultValue = true)]
    private bool isForegroundColorEnabled;

    [DependencyProperty(DefaultValue = true)]
    private bool isBackgroundColorEnabled;

    [DependencyProperty(DefaultValue = true)]
    private bool isTextDecorationsEnabled;

    [DependencyProperty(DefaultValue = FontColorEditorMode.WellKnownColorSelector)]
    private FontColorEditorMode colorEditorMode;

    [DependencyProperty(DefaultValue = "Black")]
    private SolidColorBrush? selectedForegroundBrush;

    [DependencyProperty(DefaultValue = "Transparent")]
    private SolidColorBrush? selectedBackgroundBrush;

    [DependencyProperty]
    private TextDecorationCollection? selectedTextDecorations;

    [DependencyProperty(
        DefaultValue = "The quick brown fox jumps over the lazy dog 0123456789")]
    private string previewText;

    public event EventHandler<ValueChangedEventArgs<FontDescription>>? FontChanged;

    public LabelFontPicker()
    {
        SetCurrentValue(SelectedFontWeightProperty, FontWeights.Normal);
        SetCurrentValue(SelectedFontStyleProperty, FontStyles.Normal);
        SetCurrentValue(SelectedFontStretchProperty, FontStretches.Normal);

        InitializeComponent();

        var fontPickers = this
            .FindChildren<FontPicker>()
            .ToList();
        if (fontPickers.Count != 1)
        {
            throw new UnexpectedTypeException($"{nameof(LabelFontPicker)} should only contains one {nameof(FontPicker)}");
        }

        fontPickers[0].FontChanged += OnFontChanged;

        Loaded += OnLoaded;
    }

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
        => ThemeBrushHelper.ApplyDefaults(
            this,
            SelectedForegroundBrushProperty,
            SelectedBackgroundBrushProperty);

    private void OnFontChanged(
        object? sender,
        ValueChangedEventArgs<FontDescription> e)
    {
        ArgumentNullException.ThrowIfNull(e);

        FontChanged?.Invoke(
            this,
            new ValueChangedEventArgs<FontDescription>(
                ControlHelper.GetIdentifier(this),
                oldValue: e.OldValue,
                newValue: e.NewValue));
    }
}